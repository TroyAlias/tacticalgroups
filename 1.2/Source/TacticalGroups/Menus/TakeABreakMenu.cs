using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public enum BreakType
    {
		None,
		Socialize,
		Entertainment,
		ChowHall,
		LightsOut, 
	}
	public class TakeABreakMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);

		public Dictionary<Texture2D, BreakType> breakIconStates = new Dictionary<Texture2D, BreakType>();
		public TakeABreakMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			var option = new TieredFloatMenuOption(Strings.TakeFive, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, null, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				TakeABreak(BreakType.None);
			};
			options.Add(option);

			breakIconStates[Textures.SocializeButton] = BreakType.Socialize;
			breakIconStates[Textures.EntertainmentButton] = BreakType.Entertainment;
			breakIconStates[Textures.ChowHallButton] = BreakType.ChowHall;
			breakIconStates[Textures.LightsOutButton] = BreakType.LightsOut;
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}
		public List<List<Texture2D>> GetIconRows(int columnCount)
		{
			int num = 0;
			List<List<Texture2D>> iconRows = new List<List<Texture2D>>();
			List<Texture2D> row = new List<Texture2D>();
			foreach (var icon in breakIconStates.Keys)
			{
				if (num == columnCount)
				{
					iconRows.Add(row.ListFullCopy());
					row = new List<Texture2D>();
					num = 0;
				}
				num++;
				row.Add(icon);
			}
			if (row.Any())
			{
				iconRows.Add(row);
			}
			return iconRows;
		}


		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Vector2 zero = Vector2.zero;
			zero += InitialFloatOptionPositionShift;
			for (int i = 0; i < options.Count; i++)
			{
				TieredFloatMenuOption floatMenuOption = options[i];
				Rect rect2 = new Rect(zero.x, zero.y, (this.backgroundTexture.width - InitialFloatOptionPositionShift.x) / 1.2f, floatMenuOption.curIcon.height);
				if (floatMenuOption.DoGUI(rect2, this))
				{
					Find.WindowStack.TryRemove(this);
					break;
				}
				zero.y += floatMenuOption.curIcon.height + 7f;
			}

			var rect3 = new Rect(rect.x + zero.x, rect.y + zero.y, rect.width, rect.height);
			var iconRows = GetIconRows(2);
			for (var i = 0; i < iconRows.Count; i++)
			{
				for (var j = 0; j < iconRows[i].Count; j++)
				{
					Rect iconRect = new Rect(rect3.x + (j * iconRows[i][j].width) + j * 10, rect3.y + (i * iconRows[i][j].height) + i * 7,
						iconRows[i][j].width, iconRows[i][j].height);
					GUI.DrawTexture(iconRect, iconRows[i][j]);

					if (Mouse.IsOver(iconRect))
					{
						GUI.DrawTexture(iconRect, Textures.WorkButtonHover);
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
						{
							TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
							TakeABreak(breakIconStates[iconRows[i][j]]);
							Event.current.Use();
						}
					}
				}
			}

			DrawExtraGui(rect);
			GUI.color = Color.white;
		}


		public void TakeABreak(BreakType breakType)
		{
			switch (breakType)
			{
				case BreakType.None: TakeFive(); break;
				case BreakType.Socialize: SearchForSocialRelax(); break;
				case BreakType.Entertainment: TakeFive(); break;
				case BreakType.ChowHall: ChowHall(); break;
				case BreakType.LightsOut: LightsOut(); break;
				default: return;
			}

		}

		public void TakeFive()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (!pawn.mindState.IsIdle && pawn.mindState.lastJobTag != JobTag.SatisfyingNeeds)
				{
					ThinkResult result = ThinkResult.NoJob;
					try
					{
						var joyGiver = new JobGiver_GetJoy();
						joyGiver.ResolveReferences();
						result = joyGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result.Job != null && result.Job.def != JobDefOf.GotoWander)
					{
						Log.Message(pawn + " should get " + result.Job);
						pawn.jobs.TryTakeOrderedJob(result.Job);
					}
				}
				else
				{
					Log.Message(pawn + " doesnt search for job: " + pawn.mindState.lastJobTag);
				}
			}
		}

		public void SearchForSocialRelax()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (!pawn.mindState.IsIdle && pawn.CurJobDef != JobDefOf.SocialRelax)
				{
					Job result = null;
					try
					{
						var joyGiver = new JoyGiver_SocialRelax();
						result = joyGiver.TryGiveJob(pawn);
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result != null && result.def != JobDefOf.GotoWander)
					{
						Log.Message(pawn + " should get " + result);
						pawn.jobs.TryTakeOrderedJob(result);
					}
				}
				else
				{
					Log.Message(pawn + " doesnt search for job: " + pawn.mindState.lastJobTag);
				}
			}
		}

		public void ChowHall()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (!pawn.mindState.IsIdle && pawn.CurJobDef != JobDefOf.Ingest)
				{
					ThinkResult result = ThinkResult.NoJob;
					try
					{
						var joyGiver = new JobGiver_GetFood();
						joyGiver.ResolveReferences();
						result = joyGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result.Job != null && result.Job.def != JobDefOf.GotoWander)
					{
						Log.Message(pawn + " should get " + result.Job);
						pawn.jobs.TryTakeOrderedJob(result.Job);
					}
				}
				else
				{
					Log.Message(pawn + " doesnt search for job: " + pawn.mindState.lastJobTag);
				}
			}
		}
		public void LightsOut()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (!pawn.mindState.IsIdle && pawn.CurJobDef != JobDefOf.LayDown)
				{
					ThinkResult result = ThinkResult.NoJob;
					try
					{
						var joyGiver = new JobGiver_GetRest();
						joyGiver.ResolveReferences();
						result = joyGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result.Job != null && result.Job.def != JobDefOf.GotoWander)
					{
						Log.Message(pawn + " should get " + result.Job);
						pawn.jobs.TryTakeOrderedJob(result.Job);
					}
				}
				else
				{
					Log.Message(pawn + " doesnt search for job: " + pawn.mindState.lastJobTag);
				}
			}
		}
	}
}
