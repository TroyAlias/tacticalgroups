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
	public enum WorkType
    {
		None,
		Construction,
		Crafting,
		Hauling,
		Cleaning, 
		Hunting, 
		Cooking, 
		Mining,
		WoodChopping,
		Plants,
		ClearSnow,
		Doctor,
		Warden
	}
	public class GetToWorkMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);

		public Dictionary<Texture2D, WorkType> workIconStates = new Dictionary<Texture2D, WorkType>();
		public GetToWorkMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			var option = new TieredFloatMenuOption(Strings.LookBusy, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, null, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				SearchForWork(WorkType.None);
			};
			options.Add(option);

			workIconStates[Textures.ConstructButton] = WorkType.Construction;
			workIconStates[Textures.CraftButton] = WorkType.Crafting;
			workIconStates[Textures.HaulButton] = WorkType.Hauling;
			workIconStates[Textures.CleanButton] = WorkType.Cleaning;
			workIconStates[Textures.HuntButton] = WorkType.Hunting;
			workIconStates[Textures.CookButton] = WorkType.Cooking;
			workIconStates[Textures.MineButton] = WorkType.Mining;
			workIconStates[Textures.ChopWoodButton] = WorkType.WoodChopping;
			workIconStates[Textures.FarmButton] = WorkType.Plants;
			workIconStates[Textures.ClearSnowButton] = WorkType.ClearSnow;
			workIconStates[Textures.DoctorButton] = WorkType.Doctor;
			workIconStates[Textures.WardenButton] = WorkType.Warden;

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
			foreach (var icon in workIconStates.Keys)
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
				if (floatMenuOption.DoGUI(rect2, givesColonistOrders, this))
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
							SearchForWork(workIconStates[iconRows[i][j]]);
							Event.current.Use();
						}
					}
				}
			}

            DrawExtraGui(rect);
            if (Event.current.type == EventType.MouseDown)
            {
                Event.current.Use();
                Close();
            }
            GUI.color = Color.white;
        }


		public void SearchForWork(WorkType workType)
        {
			switch (workType)
            {
				case WorkType.None: SearchForWorkGeneral(); break;
				default: return;
			}

        }

		public void SearchForWorkGeneral()
        {
			foreach (var pawn in this.colonistGroup.pawns)
            {
				if (pawn.mindState.IsIdle)
                {
					ThinkResult result = ThinkResult.NoJob;
					try
					{
						result = pawn.thinker.MainThinkNodeRoot.TryIssueJobPackage(pawn, default(JobIssueParams));
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
			}
        }
	}
}
