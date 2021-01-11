using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public class WorkMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(25f, 30f);

		public Dictionary<Texture2D, WorkType> workIconStates = new Dictionary<Texture2D, WorkType>();
		public Dictionary<Texture2D, BreakType> breakIconStates = new Dictionary<Texture2D, BreakType>();
		public Dictionary<Texture2D, string> tooltips = new Dictionary<Texture2D, string>();

		public WorkMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();

			var lookBusy = new TieredFloatMenuOption(Strings.LookBusy, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, null, TextAnchor.MiddleCenter, 
				MenuOptionPriority.High, 0f, Textures.LookBusyButton.width - 2f, Strings.LookBusyTooltip);
			lookBusy.action = delegate
			{
				TacticDefOf.TG_WorkSFX.PlayOneShotOnCamera();
				WorkSearchUtility.SearchForWork(WorkType.None, this.colonistGroup.ActivePawns);
			};
			lookBusy.bottomIndent = 400f;
			options.Add(lookBusy);

			var takeFive = new TieredFloatMenuOption(Strings.TakeFive, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, null, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f,
				Textures.LookBusyButton.width - 2f, Strings.TakeFiveTooltip);
			takeFive.action = delegate
			{
				TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
				WorkSearchUtility.TakeABreak(BreakType.None, this.colonistGroup.ActivePawns);
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

			workIconStates[Textures.TailorButton] = WorkType.Tailor;
			workIconStates[Textures.SmithButton] = WorkType.Smith;
			workIconStates[Textures.FireExtinguishButton] = WorkType.FireExtinguish;
			workIconStates[Textures.ArtButton] = WorkType.Art;

			workIconStates[Textures.FarmButton] = WorkType.Plants;
			workIconStates[Textures.HandleButton] = WorkType.Handle;
			workIconStates[Textures.WardenButton] = WorkType.Warden;
			workIconStates[Textures.ClearSnowButton] = WorkType.ClearSnow;

			breakIconStates[Textures.ChowHallButton] = BreakType.ChowHall;
			breakIconStates[Textures.LightsOutButton] = BreakType.LightsOut;

			tooltips[Textures.ConstructButton] = Strings.WorkTaskTooltipConstruction;
			tooltips[Textures.CraftButton] = Strings.WorkTaskTooltipCraft;
			tooltips[Textures.HaulButton] = Strings.WorkTaskTooltipHaul;
			tooltips[Textures.CleanButton] = Strings.WorkTaskTooltipClean;
			tooltips[Textures.HuntButton] = Strings.WorkTaskTooltipHunt;
			tooltips[Textures.CookButton] = Strings.WorkTaskTooltipCook;
			tooltips[Textures.MineButton] = Strings.WorkTaskTooltipMine;
			tooltips[Textures.ChopWoodButton] = Strings.WorkTaskTooltipChopWood;
			tooltips[Textures.FarmButton] = Strings.WorkTaskTooltipFarm;
			tooltips[Textures.ClearSnowButton] = Strings.WorkTaskTooltipClearSnow;
			tooltips[Textures.WardenButton] = Strings.WorkTaskTooltipWarden;

			tooltips[Textures.TailorButton] = Strings.WorkTaskTooltipTailor;
			tooltips[Textures.SmithButton] = Strings.WorkTaskTooltipSmith;
			tooltips[Textures.HandleButton] = Strings.WorkTaskTooltipHandle;
			tooltips[Textures.FireExtinguishButton] = Strings.WorkTaskTooltipFireExtinguish;
			tooltips[Textures.ArtButton] = Strings.WorkTaskTooltipArt;

			tooltips[Textures.ChowHallButton] = Strings.ChowHallToolTip;
			tooltips[Textures.LightsOutButton] = Strings.SleepTooltip;
			
			
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

			var rect3 = new Rect(rect.x + zero.x, rect.y + 75f, rect.width, rect.height);
			var iconRows = GetWorkIconRows(2);
			for (var i = 0; i < iconRows.Count; i++)
			{
				for (var j = 0; j < iconRows[i].Count; j++)
				{
					Rect iconRect = new Rect(rect3.x + (j * iconRows[i][j].width) + j * 10, rect3.y + (i * iconRows[i][j].height) + i * 7,
						iconRows[i][j].width, iconRows[i][j].height);
					GUI.DrawTexture(iconRect, iconRows[i][j]);
					if (this.colonistGroup.activeWorkTypes.TryGetValue(workIconStates[iconRows[i][j]], out WorkState state2))
                    {
						if (state2 == WorkState.Active)
                        {
							GUI.DrawTexture(iconRect, Textures.Clock);
                        }
						else if (state2 == WorkState.ForcedLabor)
                        {
							GUI.DrawTexture(iconRect, Textures.ClockSlave);
						}
					}
					TooltipHandler.TipRegion(iconRect, tooltips[iconRows[i][j]] + "\n" + Strings.ForcedLaborTooltip);
					if (Mouse.IsOver(iconRect))
					{
						GUI.DrawTexture(iconRect, Textures.WorkButtonHover);
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
						{
							this.colonistGroup.RemoveWorkState(workIconStates[iconRows[i][j]]);
							TacticDefOf.TG_WorkSFX.PlayOneShotOnCamera();
							WorkSearchUtility.SearchForWork(workIconStates[iconRows[i][j]], this.colonistGroup.ActivePawns);
							Event.current.Use();
						}
						else if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.clickCount == 1)
                        {
							this.colonistGroup.ChangeWorkState(workIconStates[iconRows[i][j]]);
							if (this.colonistGroup.activeWorkTypes[workIconStates[iconRows[i][j]]] == WorkState.ForcedLabor)
                            {
								TacticDefOf.TG_SlaveLaborSFX.PlayOneShotOnCamera();
							}
							else
                            {
								TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                            }
							WorkSearchUtility.SearchForWork(workIconStates[iconRows[i][j]], this.colonistGroup.ActivePawns);
							Event.current.Use();
						}
					}
				}
			}

			var rect4 = new Rect(rect.x + zero.x, rect.y + 475f, rect.width, rect.height);
			var iconRows2 = GetBreakIconRows(2);
			for (var i = 0; i < iconRows2.Count; i++)
			{
				for (var j = 0; j < iconRows2[i].Count; j++)
				{
					Rect iconRect = new Rect(rect4.x + (j * iconRows2[i][j].width) + j * 10, rect4.y + (i * iconRows2[i][j].height) + i * 7,
						iconRows2[i][j].width, iconRows2[i][j].height);
					GUI.DrawTexture(iconRect, iconRows2[i][j]);
					TooltipHandler.TipRegion(iconRect, tooltips[iconRows2[i][j]]);

					if (Mouse.IsOver(iconRect))
					{
						GUI.DrawTexture(iconRect, Textures.WorkButtonHover);
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
						{
							if (breakIconStates[iconRows2[i][j]] == BreakType.LightsOut)
                            {
								TacticDefOf.TG_LightsOutSFX.PlayOneShotOnCamera();
							}
							else if (breakIconStates[iconRows2[i][j]] == BreakType.ChowHall)
                            {
								TacticDefOf.TG_ChowTimeSFX.PlayOneShotOnCamera();
							}
							WorkSearchUtility.TakeABreak(breakIconStates[iconRows2[i][j]], this.colonistGroup.ActivePawns);
							Event.current.Use();
						}
					}
				}
			}

			var workRows = GetWorkTypeRows(2);
			var initialRect = new Rect((rect.x + rect.width) - 245, rect.y + 75, 240, rect.height - 95);
			var workMenuRect = new Rect(initialRect);
			workMenuRect.height -= 103;

			DoManualPrioritiesCheckbox(new Rect(workMenuRect.x, rect.y + 30, workMenuRect.width, 40));
			float listHeight = workRows.Count * 33 + (workRows.Count * 2);
			Rect rect5 = new Rect(0f, 0f, workMenuRect.width - 16f, listHeight);
			Widgets.BeginScrollView(workMenuRect, ref scrollPosition, rect5);

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


			var caravanButtonRect = new Rect(initialRect.x + 155, rect.y + 23, Textures.CaravanButton.width, Textures.CaravanButton.height);
			GUI.DrawTexture(caravanButtonRect, Textures.CaravanButton);
			if (Mouse.IsOver(caravanButtonRect))
			{
				GUI.DrawTexture(caravanButtonRect, Textures.CaravanHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_WorkSFX.PlayOneShotOnCamera();

					var floatMenu = new CaravanMenu(this, this.colonistGroup, this.windowRect, Textures.CaravanMenu);
					this.OpenNewMenu(floatMenu);
					Event.current.Use();
				}
			}


			var researchWorkRect = new Rect(rect.x + 253, rect.y + 459, Textures.ResearchWorkButton.width, Textures.ResearchWorkButton.height);
			GUI.DrawTexture(researchWorkRect, Textures.ResearchWorkButton);
			if (this.colonistGroup.activeWorkTypes.TryGetValue(WorkType.Research, out WorkState state))
			{
				if (state == WorkState.Active)
                {
					GUI.DrawTexture(researchWorkRect, Textures.Clock, ScaleMode.ScaleToFit);
                }
				else if (state == WorkState.ForcedLabor)
                {
					GUI.DrawTexture(researchWorkRect, Textures.ClockSlave, ScaleMode.ScaleToFit);
				}
			}

			Text.Anchor = TextAnchor.MiddleCenter;
			var researchLabel = new Rect(researchWorkRect.x + 30, researchWorkRect.y - 25, 80, 24);
			Widgets.Label(researchLabel, Strings.WorkTaskTooltipResearch);
			Text.Anchor = TextAnchor.UpperLeft;

			TooltipHandler.TipRegion(researchWorkRect, Strings.WorkTaskTooltipResearch + "\n" + Strings.ForcedLaborTooltip);
			if (Mouse.IsOver(researchWorkRect))
			{
				GUI.DrawTexture(researchWorkRect, Textures.ResearchHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ResearchSFX.PlayOneShotOnCamera();
					this.colonistGroup.RemoveWorkState(WorkType.Research);
					WorkSearchUtility.SearchForWork(WorkType.Research, this.colonistGroup.ActivePawns);
					Event.current.Use();
				}
				else if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					this.colonistGroup.ChangeWorkState(WorkType.Research);
					WorkSearchUtility.SearchForWork(WorkType.Research, this.colonistGroup.ActivePawns);
					Event.current.Use();
				}
			}
			var researchMenuRect = new Rect(researchWorkRect.x + 89, researchWorkRect.y, Textures.ResearchMenuButton.width, Textures.ResearchMenuButton.height);
			GUI.DrawTexture(researchMenuRect, Textures.ResearchMenuButton);
			if (Mouse.IsOver(researchMenuRect))
			{
				GUI.DrawTexture(researchMenuRect, Textures.ResearchHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					Find.MainTabsRoot.ToggleTab(MainButtonDefOf.Research);
					Event.current.Use();
				}
			}
			TooltipHandler.TipRegion(researchMenuRect, Strings.WorkTaskTooltipResearchMenu);
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
			List<Pawn> pawnsListForReading = colonistGroup.ActivePawns; ;
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
	}
}
