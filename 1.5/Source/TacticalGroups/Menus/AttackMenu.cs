using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace TacticalGroups
{
	public class AttackMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 30f);
		public AttackMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddFireAtWillButton();
			AddStrongestButton();
			AddWeakestButton();
			AddPursueFleeingButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public void AddFireAtWillButton()
		{
			var option = new TieredFloatMenuOption(Strings.FireAtWill, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter,
				MenuOptionPriority.High, 0f, -1f, Strings.FireAtWillTooltip);
			option.action = delegate
			{
				this.colonistGroup.Draft();
				foreach (var pawn in this.colonistGroup.ActivePawns)
				{
					if (pawn.drafter != null)
					{
						pawn.drafter.FireAtWill = true;
					}
				}
				this.colonistGroup.SelectAll();
				this.colonistGroup.RemoveOldLord();
				TacticDefOf.TG_AttackOrdersSFX.PlayOneShotOnCamera();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 30;
			options.Add(option);
		}

		public void AddStrongestButton()
		{
			var option = new TieredFloatMenuOption(Strings.Strongest, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter,
				MenuOptionPriority.High, 0f, -1f, Strings.StrongestTooltip);
			option.action = delegate
			{
				TacticDefOf.TG_AttackOrdersSFX.PlayOneShotOnCamera();
				this.colonistGroup.Draft();
				this.colonistGroup.SelectAll();
				this.colonistGroup.SwitchToAttackMode();
				this.colonistGroup.RemoveOldLord();
				var targets = this.colonistGroup.Map.attackTargetsCache.GetPotentialTargetsFor(this.colonistGroup.ActivePawns.First()).Where(x => !x.Thing.Fogged()).Select(x => x.Thing).ToList();
				LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_AssaultThingsStrongest(Faction.OfPlayer, targets, 0, true), this.colonistGroup.Map, this.colonistGroup.ActivePawns);
				this.colonistGroup.SearchForJob();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 10;
			options.Add(option);
		}

		public void AddWeakestButton()
		{
			var option = new TieredFloatMenuOption(Strings.Weakest, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter,
				MenuOptionPriority.High, 0f, -1f, Strings.WeakestTooltip);
			option.action = delegate
			{
				TacticDefOf.TG_AttackOrdersSFX.PlayOneShotOnCamera();
				this.colonistGroup.Draft();
				this.colonistGroup.SelectAll();
				this.colonistGroup.SwitchToAttackMode();
				this.colonistGroup.RemoveOldLord();
				var targets = this.colonistGroup.Map.attackTargetsCache.GetPotentialTargetsFor(this.colonistGroup.ActivePawns.First()).Where(x => !x.Thing.Fogged()).Select(x => x.Thing).ToList();
				LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_AssaultThingsWeakest(Faction.OfPlayer, targets, 0, true), this.colonistGroup.Map, this.colonistGroup.ActivePawns);
				this.colonistGroup.SearchForJob();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 35;
			options.Add(option);
		}

		public void AddPursueFleeingButton()
		{
			var option = new TieredFloatMenuOption(Strings.PursueFleeing, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter,
				MenuOptionPriority.High, 0f, -1f, Strings.PursueFleeingTooltip);
			option.action = delegate
			{
				TacticDefOf.TG_AttackOrdersSFX.PlayOneShotOnCamera();
				this.colonistGroup.Draft();
				this.colonistGroup.SelectAll();
				this.colonistGroup.SwitchToAttackMode();
				this.colonistGroup.RemoveOldLord();
				var targets = this.colonistGroup.Map.attackTargetsCache.GetPotentialTargetsFor(this.colonistGroup.ActivePawns.First()).Where(x => !x.Thing.Fogged()).Select(x => x.Thing).ToList();
				LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_AssaultThingsPursueFleeing(Faction.OfPlayer, targets, 0, true), this.colonistGroup.Map, this.colonistGroup.ActivePawns);
				this.colonistGroup.SearchForJob();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height;
			options.Add(option);
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
				zero.y += floatMenuOption.bottomIndent;
			}
			DrawExtraGui(rect);

			var captureRect = new Rect(rect.x + 27, zero.y + 5, Textures.Arrest.width, Textures.Arrest.height);
			GUI.DrawTexture(captureRect, Textures.Arrest);
			TooltipHandler.TipRegion(captureRect, Strings.CaptureTooltip);
			if (Mouse.IsOver(captureRect))
			{
				GUI.DrawTexture(captureRect, Textures.RescueTendHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					var victims = this.colonistGroup.Map.mapPawns.AllPawns.Where(x => x.RaceProps.Humanlike).ToList(); ;
					foreach (var pawn in this.colonistGroup.ActivePawns)
					{
						if (!pawn.Dead && !pawn.Downed && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
						{
							for (int num = victims.Count - 1; num >= 0; num--)
							{
								var victim = victims[num];
								if (!victim.InBed() && !victim.mindState.WillJoinColonyIfRescued && pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
								{
									if (victim.InMentalState || victim.Faction != Faction.OfPlayer || (victim.Downed && (victim.guilt.IsGuilty || victim.IsPrisonerOfColony)))
									{
										var designation = victim.Map.designationManager.DesignationOn(victim);
										if (designation is null || designation.def.defName != "FinishOffDesignation")
                                        {
											Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, false, false, GuestStatus.Prisoner);
											if (building_Bed == null)
											{
												building_Bed = RestUtility.FindBedFor(victim, pawn, checkSocialProperness: false, ignoreOtherReservations: true, GuestStatus.Prisoner);
											}
											if (building_Bed != null)
											{
												Job job = JobMaker.MakeJob(JobDefOf.Capture, victim, building_Bed);
												job.count = 1;
												pawn.jobs.TryTakeOrderedJob(job);
												PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Capturing, KnowledgeAmount.Total);
												if (victim.Faction != null && victim.Faction != Faction.OfPlayer && !victim.Faction.Hidden && !victim.Faction.HostileTo(Faction.OfPlayer) && !victim.IsPrisonerOfColony)
												{
													Messages.Message("MessageCapturingWillAngerFaction".Translate(victim.Named("PAWN")).AdjustedFor(victim), victim, MessageTypeDefOf.CautionInput, historical: false);
												}
												victims.RemoveAt(num);
												break;
											}
										}
									}
								}
							}
						}
					}
					TacticDefOf.TG_ArrestSFX.PlayOneShotOnCamera();
				}
			}

			var executeRect = new Rect(captureRect.xMax + 15, captureRect.y, Textures.Execute.width, Textures.Execute.height);
			GUI.DrawTexture(executeRect, Textures.Execute);
			TooltipHandler.TipRegion(executeRect, Strings.ExecuteTooltip);
			if (Mouse.IsOver(executeRect))
			{
				GUI.DrawTexture(executeRect, Textures.RescueTendHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					var victims = this.colonistGroup.Map.mapPawns.AllPawns.Where(x => x.RaceProps.Humanlike && x.Downed && x.HostileTo(Faction.OfPlayer)).ToList();
					foreach (var pawn in this.colonistGroup.ActivePawns)
					{
						if (!pawn.Dead && !pawn.Downed && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
						{
							for (int num = victims.Count - 1; num >= 0; num--)
							{
								var victim = victims[num];
								if (pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
								{
									Job job = JobMaker.MakeJob(TacticDefOf.TG_ExecuteDownedRaiders, victim);
									job.count = 1;
									pawn.jobs.TryTakeOrderedJob(job);
									victims.RemoveAt(num);
									break;
								}
							}
						}
					}
					TacticDefOf.TG_ExecuteSFX.PlayOneShotOnCamera();
				}
				GUI.color = Color.white;
			}
		}
	}
}
