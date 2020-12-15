using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public enum BreakType
	{
		None,
		Socialize,
		Entertainment,
		ChowHall,
		LightsOut,
	}
	public class ActionsMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(25f, 25f);

		public Dictionary<Texture2D, WorkType> workIconStates = new Dictionary<Texture2D, WorkType>();
		public Dictionary<Texture2D, BreakType> breakIconStates = new Dictionary<Texture2D, BreakType>();

		public ActionsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();

			var lookBusy = new TieredFloatMenuOption(Strings.LookBusy, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, null, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f, 
				Textures.LookBusyButton.width - 2f);
			lookBusy.action = delegate
			{
				SearchForWork(WorkType.None);
			};
			lookBusy.bottomIndent = 310f;
			options.Add(lookBusy);

			var takeFive = new TieredFloatMenuOption(Strings.TakeFive, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, null, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f,
				Textures.LookBusyButton.width - 2f);
			takeFive.action = delegate
			{
				TakeABreak(BreakType.None);
			};
			options.Add(takeFive);

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

			breakIconStates[Textures.SocializeButton] = BreakType.Socialize;
			breakIconStates[Textures.EntertainmentButton] = BreakType.Entertainment;
			breakIconStates[Textures.ChowHallButton] = BreakType.ChowHall;
			breakIconStates[Textures.LightsOutButton] = BreakType.LightsOut;

			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}
		public List<List<Texture2D>> GetWorkIconRows(int columnCount)
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

		public List<List<Texture2D>> GetBreakIconRows(int columnCount)
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

		public List<List<WorkTypeDef>> GetWorkTypeRows(int columnCount)
		{
			int num = 0;
			List<List<WorkTypeDef>> workTypeRows = new List<List<WorkTypeDef>>();
			List<WorkTypeDef> row = new List<WorkTypeDef>();
			foreach (WorkTypeDef workType in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.Where((WorkTypeDef d) => d.visible))
			{
				if (num == columnCount)
				{
					workTypeRows.Add(row.ListFullCopy());
					row = new List<WorkTypeDef>();
					num = 0;
				}
				num++;
				row.Add(workType);
			}
			if (row.Any())
			{
				workTypeRows.Add(row);
			}
			return workTypeRows;
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

			var rect3 = new Rect(rect.x + zero.x, rect.y + 70f, rect.width, rect.height);
			var iconRows = GetWorkIconRows(2);
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
							SearchForWork(workIconStates[iconRows[i][j]]);
							Event.current.Use();
						}
					}
				}
			}


			var rect4 = new Rect(rect.x + zero.x, rect.y + 380f, rect.width, rect.height);
			var iconRows2 = GetBreakIconRows(2);
			for (var i = 0; i < iconRows2.Count; i++)
			{
				for (var j = 0; j < iconRows2[i].Count; j++)
				{
					Rect iconRect = new Rect(rect4.x + (j * iconRows2[i][j].width) + j * 10, rect4.y + (i * iconRows2[i][j].height) + i * 7,
						iconRows2[i][j].width, iconRows2[i][j].height);
					GUI.DrawTexture(iconRect, iconRows2[i][j]);
			
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

			var workRows = GetWorkTypeRows(2);
			var initialRect = new Rect((rect.x + rect.width) - 245, rect.y + 75, 240, rect.height - 95);
			DoManualPrioritiesCheckbox(new Rect(initialRect.x, rect.y + 30, initialRect.width, 40));
			float listHeight = workRows.Count * 33 + (workRows.Count * 2);
			Rect rect5 = new Rect(0f, 0f, initialRect.width - 16f, listHeight);
			Widgets.BeginScrollView(initialRect, ref scrollPosition, rect5);

			for (var i = 0; i < workRows.Count; i++)
			{
				for (var j = 0; j < workRows[i].Count; j++)
				{
					Rect workRect = new Rect(rect5.x + (j * 20) + j * 91.5f, rect5.y + (i * 17) + i * 17, 24, 24);
					this.DoWorkCell(workRect, workRows[i][j]);
					this.DoWorkHeader(workRect, workRows[i][j]);
				}
			}
			Widgets.EndScrollView();
			DrawExtraGui(rect);
			GUI.color = Color.white;
		}

		private Vector2 scrollPosition;
		private void DoManualPrioritiesCheckbox(Rect rect)
		{
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect2 = new Rect(rect.x + 5, rect.y, 140f, 30f);
			bool useWorkPriorities = Current.Game.playSettings.useWorkPriorities;
			Widgets.CheckboxLabeled(rect2, "ManualPriorities".Translate(), ref Current.Game.playSettings.useWorkPriorities);
			if (useWorkPriorities != Current.Game.playSettings.useWorkPriorities)
			{
				foreach (Pawn item in PawnsFinder.AllMapsWorldAndTemporary_Alive)
				{
					if (item.Faction == Faction.OfPlayer && item.workSettings != null)
					{
						item.workSettings.Notify_UseWorkPrioritiesChanged();
					}
				}
			}
		}
		public void DoWorkCell(Rect rect, WorkTypeDef workType)
		{
			Text.Font = GameFont.Medium;
			float x = rect.x + 5 + (rect.width - 25f) / 2f;
			float y = rect.y + 2.5f;
			WidgetsWorkGroup.DrawWorkBoxFor(x, y, workType, this.colonistGroup);
			Text.Font = GameFont.Small;
		}

		public void DoWorkHeader(Rect rect, WorkTypeDef workType)
		{
			Text.Font = GameFont.Small;
			Rect labelRect = GetLabelRect(rect);
			MouseoverSounds.DoRegion(labelRect);
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(labelRect, workType.labelShort);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		private Rect GetLabelRect(Rect headerRect)
		{
			Rect result = new Rect(headerRect.x + 33f, headerRect.y + 3, 80, 25);
			return result;
		}

		protected string GetHeaderTip(WorkTypeDef workType)
		{
			string str = workType.gerundLabel.CapitalizeFirst() + "\n\n" + workType.description + "\n\n" + SpecificWorkListString(workType);
			str += "\n";
			str += "\n" + "ClickToSortByThisColumn".Translate();
			if (Find.PlaySettings.useWorkPriorities)
			{
				return str + ("\n" + "WorkPriorityShiftClickTip".Translate());
			}
			return str + ("\n" + "WorkPriorityShiftClickEnableDisableTip".Translate());
		}

		private static string SpecificWorkListString(WorkTypeDef def)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < def.workGiversByPriority.Count; i++)
			{
				stringBuilder.Append(def.workGiversByPriority[i].LabelCap);
				if (def.workGiversByPriority[i].emergency)
				{
					stringBuilder.Append(" (" + "EmergencyWorkMarker".Translate() + ")");
				}
				if (i < def.workGiversByPriority.Count - 1)
				{
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		protected void HeaderClicked(WorkTypeDef workType)
		{
			if (!Event.current.shift)
			{
				return;
			}
			List<Pawn> pawnsListForReading = colonistGroup.pawns; ;
			for (int i = 0; i < pawnsListForReading.Count; i++)
			{
				Pawn pawn = pawnsListForReading[i];
				if (pawn.workSettings == null || !pawn.workSettings.EverWork || pawn.WorkTypeIsDisabled(workType))
				{
					continue;
				}
				if (Find.PlaySettings.useWorkPriorities)
				{
					int priority = pawn.workSettings.GetPriority(workType);
					if (Event.current.button == 0 && priority != 1)
					{
						int num = priority - 1;
						if (num < 0)
						{
							num = 4;
						}
						pawn.workSettings.SetPriority(workType, num);
					}
					if (Event.current.button == 1 && priority != 0)
					{
						int num2 = priority + 1;
						if (num2 > 4)
						{
							num2 = 0;
						}
						pawn.workSettings.SetPriority(workType, num2);
					}
				}
				else if (pawn.workSettings.GetPriority(workType) > 0)
				{
					if (Event.current.button == 1)
					{
						pawn.workSettings.SetPriority(workType, 0);
					}
				}
				else if (Event.current.button == 0)
				{
					pawn.workSettings.SetPriority(workType, 3);
				}
			}
			if (Find.PlaySettings.useWorkPriorities)
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}
			else if (Event.current.button == 0)
			{
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			}
			else if (Event.current.button == 1)
			{
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
			}
		}


		public void SearchForWork(WorkType workType)
		{
			switch (workType)
			{
				case WorkType.None: SearchForWorkGeneral(); break;
				case WorkType.Construction: SearchForWorkConstruction(); break;
				case WorkType.Cleaning: SearchForWorkCleaning(); break;
				case WorkType.Cooking: SearchForWorkCooking(); break;
				case WorkType.Crafting: SearchForWorkCrafting(); break;

				case WorkType.ClearSnow: SearchForWorkClearSnow(); break;
				case WorkType.Doctor: SearchForWorkDoctor(); break;
				case WorkType.Hauling: SearchForWorkHauling(); break;
				case WorkType.Hunting: SearchForWorkHunting(); break;
				case WorkType.Mining: SearchForWorkMining(); break;
				case WorkType.Plants: SearchForWorkPlants(); break;
				case WorkType.Warden: SearchForWorkWarden(); break;
				case WorkType.WoodChopping: SearchForWorkWoodChopping(); break;

				default: return;
			}

		}

		public void SearchForWorkGeneral()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.mindState.IsIdle || pawn.mindState.lastJobTag == JobTag.SatisfyingNeeds)
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
				else
				{
					Log.Message(pawn + " doesnt search for job: " + pawn.mindState.lastJobTag);
				}
			}
		}

		public void SearchForWorkConstruction()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.FinishFrame)
				{
					if (TryGetJobForAllItems(WorkTypeDefOf.Construction.workGiversByPriority, pawn))
                    {
						TryGetJobForAllCells(WorkTypeDefOf.Construction.workGiversByPriority, pawn);
                    }
				}
			}
		}

		public void SearchForWorkCleaning()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.Clean)
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("Cleaning");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
                    {
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}

		public void SearchForWorkCooking()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.DoBill && pawn.CurJob.workGiverDef != DefDatabase<WorkGiverDef>.GetNamed("DoBillsCook"))
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("Cooking");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
					{
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}

		public void SearchForWorkCrafting()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.DoBill)
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("Crafting");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
					{
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}

		public void SearchForWorkClearSnow()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.ClearSnow)
				{
					var workgivers = new List<WorkGiverDef> { DefDatabase<WorkGiverDef>.GetNamed("CleanClearSnow") };
					if (!TryGetJobForAllItems(workgivers, pawn))
					{
						TryGetJobForAllCells(workgivers, pawn);
					}
				}
			}
		}

		public void SearchForWorkDoctor()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.TendPatient)
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("Doctor");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
					{
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}

		public void SearchForWorkHauling()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.HaulToCell && pawn.CurJobDef != JobDefOf.HaulToContainer && pawn.CurJobDef != JobDefOf.HaulToTransporter)
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("Hauling");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
					{
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}

		public void SearchForWorkHunting()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.Hunt)
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("Hunting");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
					{
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}
		public void SearchForWorkMining()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.Mine)
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("Mining");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
					{
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}
		public void SearchForWorkPlants()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.CutPlantDesignated && pawn.CurJobDef != JobDefOf.Sow)
				{
					var growingWorkType = DefDatabase<WorkTypeDef>.GetNamed("Growing");
					var plantCuttingWorkType = DefDatabase<WorkTypeDef>.GetNamed("PlantCutting");

					var workgivers = new List<WorkGiverDef>();
					workgivers.AddRange(growingWorkType.workGiversByPriority);
					workgivers.AddRange(plantCuttingWorkType.workGiversByPriority);
					if (!TryGetJobForAllItems(workgivers, pawn))
					{
						TryGetJobForAllCells(workgivers, pawn);
					}
				}
			}
		}

		public void SearchForWorkWarden()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.jobs?.curJob?.workGiverDef?.workType != WorkTypeDefOf.Warden)
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("Warden");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
					{
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}

		public void SearchForWorkWoodChopping()
		{
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.CurJobDef != JobDefOf.CutPlantDesignated)
				{
					var workType = DefDatabase<WorkTypeDef>.GetNamed("PlantCutting");
					if (!TryGetJobForAllItems(workType.workGiversByPriority, pawn))
					{
						TryGetJobForAllCells(workType.workGiversByPriority, pawn);
					}
				}
			}
		}
		public bool TryGetJobForAllItems(List<WorkGiverDef> workGiverDefs, Pawn pawn)
        {
			if (pawn.thinker.TryGetMainTreeThinkNode<JobGiver_Work>() != null)
			{
				foreach (Thing item in pawn.Map.listerThings.AllThings)
				{
					foreach (var workGiver in workGiverDefs)
					{
						WorkGiver_Scanner workGiver_Scanner = workGiver.Worker as WorkGiver_Scanner;
						if (workGiver_Scanner != null && workGiver_Scanner.def.directOrderable)
						{
							if ((workGiver_Scanner.PotentialWorkThingRequest.Accepts(item) || (workGiver_Scanner.PotentialWorkThingsGlobal(pawn) != null && workGiver_Scanner.PotentialWorkThingsGlobal(pawn).Contains(item))) && !workGiver_Scanner.ShouldSkip(pawn, forced: true))
							{
								if (workGiver_Scanner.MissingRequiredCapacity(pawn) == null)
								{
									Job job = workGiver_Scanner.HasJobOnThing(pawn, item, forced: true) ? workGiver_Scanner.JobOnThing(pawn, item, forced: true) : null;
									if (job != null)
									{
										WorkTypeDef workType2 = workGiver_Scanner.def.workType;
										if (pawn.WorkTagIsDisabled(workGiver_Scanner.def.workTags))
										{
										}
										else if (pawn.jobs.curJob != null && pawn.jobs.curJob.JobIsSameAs(job))
										{
										}
										else if (pawn.workSettings.GetPriority(workType2) == 0)
										{
										}
										else if (job.def == JobDefOf.Research && item is Building_ResearchBench)
										{
										}
										else if (item.IsForbidden(pawn))
										{
										}
										else if (!pawn.CanReach(item, workGiver_Scanner.PathEndMode, Danger.Deadly))
										{
										}
										else
										{
											WorkGiver_Scanner localScanner2 = workGiver_Scanner;
											job.workGiverDef = workGiver_Scanner.def;
											pawn.jobs.TryTakeOrderedJobPrioritizedWork(job, localScanner2, pawn.Position);
											return true;
										}
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		public bool TryGetJobForAllCells(List<WorkGiverDef> workGiverDefs, Pawn pawn)
		{
			if (pawn.thinker.TryGetMainTreeThinkNode<JobGiver_Work>() != null)
			{
				foreach (IntVec3 cell in pawn.Map.AllCells)
				{
					foreach (var workGiver in workGiverDefs)
					{
						if (!pawn.Drafted || workGiver.canBeDoneWhileDrafted)
						{
							WorkGiver_Scanner workGiver_Scanner2 = workGiver.Worker as WorkGiver_Scanner;
							if (workGiver_Scanner2 != null && workGiver_Scanner2.def.directOrderable)
							{
								if (workGiver_Scanner2.PotentialWorkCellsGlobal(pawn).Contains(cell) && !workGiver_Scanner2.ShouldSkip(pawn, forced: true))
								{
									if (workGiver_Scanner2.MissingRequiredCapacity(pawn) != null)
									{
										Job job2 = workGiver_Scanner2.HasJobOnCell(pawn, cell, forced: true) ? workGiver_Scanner2.JobOnCell(pawn, cell, forced: true) : null;
										if (job2 != null)
										{
											WorkTypeDef workType2 = workGiver_Scanner2.def.workType;
											if (pawn.jobs.curJob != null && pawn.jobs.curJob.JobIsSameAs(job2))
											{
											}
											else if (pawn.workSettings.GetPriority(workType2) == 0)
											{
											}
											else if (cell.IsForbidden(pawn))
											{
											}
											else if (!pawn.CanReach(cell, PathEndMode.Touch, Danger.Deadly))
											{
											}
											else
											{
												WorkGiver_Scanner localScanner = workGiver_Scanner2;
												job2.workGiverDef = workGiver_Scanner2.def;
												pawn.jobs.TryTakeOrderedJobPrioritizedWork(job2, localScanner, cell);
												return true;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return false;
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
