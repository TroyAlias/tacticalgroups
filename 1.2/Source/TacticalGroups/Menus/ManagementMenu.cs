using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public class ManagementMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 55f);
		public ManagementMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public List<List<WorkTypeDef>> GetWorkTypeRows(int columnCount)
		{
			int num = 0;
			List<List<WorkTypeDef>> workTypeRows = new List<List<WorkTypeDef>>();
			List<WorkTypeDef> row = new List<WorkTypeDef>();
			foreach (WorkTypeDef workType in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.Where((WorkTypeDef d) => d.visible).Reverse())
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
			var hostilityResponseRect = new Rect(rect.x + 20, rect.y + 25, 24, 24);
			HostilityResponseModeUtilityGroup.DrawResponseButton(hostilityResponseRect, this.colonistGroup, true);

			var medicalCareRect = new Rect(rect.x + 50, rect.y + 25, 24, 24);
			MedicalCareUtilityGroup.MedicalCareSelectButton(medicalCareRect, this.colonistGroup);

			var timeAssignmentSelectorGridRect = new Rect(rect.x + 100, rect.y + 20, 191f, 65f);
			TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(timeAssignmentSelectorGridRect);
			var timeTableHeaderRect = new Rect(rect.x + 10, rect.y + 85f, rect.width * 0.675f, 20f);
			DoTimeTableHeader(timeTableHeaderRect);
			var timeTableRect = new Rect(rect.x + 10, rect.y + 105, rect.width * 0.675f, 30f);
			DoTimeTableCell(timeTableRect);

			var policyButtonWidth = rect.width * 0.325f;
			var areaHeaderRect = new Rect(rect.x + 10, rect.y + 195f, policyButtonWidth, 20f);
			DoAreaHeader(areaHeaderRect);
			var areaRect = new Rect(rect.x + 10, rect.y + 205, policyButtonWidth, 30f);
			DoAreaCell(areaRect);

			var outfitHeaderRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 195f, policyButtonWidth, 20f);
			DoOutfitHeader(outfitHeaderRect);
			var	outfitRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 205, policyButtonWidth, 30f);
			DoOutfitCell(outfitRect);

			var drugPolicyHeaderRect = new Rect(rect.x + 10, rect.y + 305f, policyButtonWidth, 20f);
			DoDrugPolicyHeader(drugPolicyHeaderRect);
			var drugPolicyRect = new Rect(rect.x + 10, rect.y + 315, policyButtonWidth, 30f);
			DoDrugPolicyCell(drugPolicyRect);

			var foodHeaderRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 305f, policyButtonWidth, 20f);
			DoFoodHeader(foodHeaderRect);
			var foodRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 315, policyButtonWidth, 30f);
			DoFoodCell(foodRect);

			DoManualPrioritiesCheckbox();
			var workRows = GetWorkTypeRows(2);
			var initialRect = new Rect(rect);
			initialRect.y += 25f;
			initialRect.x += initialRect.width - 206;
			initialRect.height -= 45f;
			float listHeight = workRows.Count * 30 + (workRows.Count * 2);
			Rect rect2 = new Rect(0f, 0f, initialRect.width - 16f, listHeight);
			Widgets.BeginScrollView(initialRect, ref scrollPosition, rect2);
			for (var i = 0; i < workRows.Count; i++)
			{
				for (var j = 0; j < workRows[i].Count; j++)
				{
					Rect workRect = new Rect((rect2.x - 20) + (j * 20) + j * 100, rect2.y + 50 + (i * 20) + i * 20, 24, 24);
					this.DoWorkCell(workRect, workRows[i][j]);
					this.DoWorkHeader(workRect, workRows[i][j]);
				}
			}

			Widgets.EndScrollView();
			GUI.color = Color.white;
		}

		private void DoManualPrioritiesCheckbox()
		{
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect = new Rect(105f, 5f, 140f, 30f);
			bool useWorkPriorities = Current.Game.playSettings.useWorkPriorities;
			Widgets.CheckboxLabeled(rect, "ManualPriorities".Translate(), ref Current.Game.playSettings.useWorkPriorities);
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

		private Vector2 scrollPosition;
		public void DoTimeTableCell(Rect rect)
		{
			float num = rect.x;
			float num2 = rect.width / 24f;
			for (int i = 0; i < 24; i++)
			{
				Rect rect2 = new Rect(num, rect.y, num2, rect.height);
				DoTimeAssignment(rect2, i);
				num += num2;
			}
			GUI.color = Color.white;
			if (TimeAssignmentSelector.selectedAssignment != null)
			{
				UIHighlighter.HighlightOpportunity(rect, "TimeAssignmentTableRow-If" + TimeAssignmentSelector.selectedAssignment.defName + "Selected");
			}
		}

		public void DoTimeTableHeader(Rect rect)
		{
			float num = rect.x;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.LowerCenter;
			float num2 = rect.width / 24f;
			for (int i = 0; i < 24; i++)
			{
				Widgets.Label(new Rect(num, rect.y, num2, rect.height + 3f), i.ToString());
				num += num2;
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		private void DoTimeAssignment(Rect rect, int hour)
		{
			rect = rect.ContractedBy(1f);
			bool mouseButton = Input.GetMouseButton(0);
			TimeAssignmentDef assignment = this.colonistGroup.pawns.First().timetable.GetAssignment(hour);
			GUI.DrawTexture(rect, assignment.ColorTexture);
			if (!mouseButton)
			{
				MouseoverSounds.DoRegion(rect);
			}
			if (!Mouse.IsOver(rect))
			{
				return;
			}
			Widgets.DrawBox(rect, 2);
			if (mouseButton && assignment != TimeAssignmentSelector.selectedAssignment && TimeAssignmentSelector.selectedAssignment != null)
			{
				SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
				foreach (var p in this.colonistGroup.pawns)
                {
					p.timetable.SetAssignment(hour, TimeAssignmentSelector.selectedAssignment);
				}
			}
		}

		public void DoAreaCell(Rect rect)
		{
			AreaAllowedGUIGroup.DoAllowedAreaSelectors(rect, this.colonistGroup);
		}

		public void DoAreaHeader(Rect rect)
		{
			if (Widgets.ButtonText(new Rect(rect.x, rect.y + (rect.height - 65f), Mathf.Min(rect.width, 360f), 32f), "ManageAreas".Translate()))
			{
				var window = new Dialog_ManageAreas(this.colonistGroup.Map);
				Find.WindowStack.Add(window);
				window.windowRect.y += 220f;
			}
		}

		public void DoOutfitHeader(Rect rect)
		{
			MouseoverSounds.DoRegion(rect);
			Rect rect2 = new Rect(rect.x, rect.y + (rect.height - 65f), Mathf.Min(rect.width, 360f), 32f);
			if (Widgets.ButtonText(rect2, "ManageOutfits".Translate()))
			{
				var window = new Dialog_ManageOutfits(null);
				Find.WindowStack.Add(window);
				window.windowRect.y += 350f;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Outfits, KnowledgeAmount.Total);
			}
			UIHighlighter.HighlightOpportunity(rect2, "ManageOutfits");
		}

		public void DoOutfitCell(Rect rect)
		{

			int num = Mathf.FloorToInt((rect.width - 4f) * 0.714285731f);
			int num2 = Mathf.FloorToInt((rect.width - 4f) * 0.2857143f);
			float x = rect.x;
			bool somethingIsForced = this.colonistGroup.pawns.First().outfits.forcedHandler.SomethingIsForced;
			Rect rect2 = new Rect(x, rect.y + 2f, num, rect.height - 4f);
			if (somethingIsForced)
			{
				rect2.width -= 4f + (float)num2;
			}
			Widgets.Dropdown(rect2, this.colonistGroup, (ColonistGroup group) => group.pawns.First().outfits.CurrentOutfit, Button_GenerateMenu, 
				this.colonistGroup.pawns.First().outfits.CurrentOutfit.label.Truncate(rect2.width), null,
				this.colonistGroup.pawns.First().outfits.CurrentOutfit.label, null, null, paintable: true);
			x += rect2.width;
			x += 4f;
			Rect rect3 = new Rect(x, rect.y + 2f, num2, rect.height - 4f);
			if (somethingIsForced)
			{
				if (Widgets.ButtonText(rect3, "ClearForcedApparel".Translate()))
				{
					foreach (var pawn in this.colonistGroup.pawns)
                    {
						pawn.outfits.forcedHandler.Reset();
					}
				}
				x += (float)num2;
				x += 4f;
			}
			Rect rect4 = new Rect(x, rect.y + 2f, num2, rect.height - 4f);
			if (Widgets.ButtonText(rect4, "AssignTabEdit".Translate()))
			{
				var window = new Dialog_ManageOutfits(colonistGroup.pawns.First().outfits.CurrentOutfit);
				Find.WindowStack.Add(window);
				window.windowRect.y += 350f;
			}
			x += (float)num2;
		}

		private IEnumerable<Widgets.DropdownMenuElement<Outfit>> Button_GenerateMenu(ColonistGroup group)
		{
			foreach (Outfit outfit in Current.Game.outfitDatabase.AllOutfits)
			{
				yield return new Widgets.DropdownMenuElement<Outfit>
				{
					option = new FloatMenuOption(outfit.label, delegate
					{
						foreach (var pawn in group.pawns)
                        {
							pawn.outfits.CurrentOutfit = outfit;
						}
					}),
					payload = outfit
				};
			}
		}

		public void DoDrugPolicyHeader(Rect rect)
		{
			MouseoverSounds.DoRegion(rect);
			Rect rect2 = new Rect(rect.x, rect.y + (rect.height - 65f), Mathf.Min(rect.width, 360f), 32f);
			if (Widgets.ButtonText(rect2, "ManageDrugPolicies".Translate()))
			{
				var window = new Dialog_ManageDrugPolicies(null);
				Find.WindowStack.Add(window);
				window.windowRect.y += 350f;
			}
			UIHighlighter.HighlightOpportunity(rect2, "ManageDrugPolicies");
			UIHighlighter.HighlightOpportunity(rect2, "ButtonAssignDrugs");
		}

		public void DoDrugPolicyCell(Rect rect)
		{
			DrugPolicyUIUtilityGroup.DoAssignDrugPolicyButtons(rect, this.colonistGroup);
		}

		public void DoFoodHeader(Rect rect)
		{
			MouseoverSounds.DoRegion(rect);
			if (Widgets.ButtonText(new Rect(rect.x, rect.y + (rect.height - 65f), Mathf.Min(rect.width, 360f), 32f), "ManageFoodRestrictions".Translate()))
			{
				var window = new Dialog_ManageFoodRestrictions(null);
				Find.WindowStack.Add(window);
				window.windowRect.y += 350f;
			}
		}

		public void DoFoodCell(Rect rect)
		{
			DoAssignFoodRestrictionButtons(rect, this.colonistGroup);
		}

		private IEnumerable<Widgets.DropdownMenuElement<FoodRestriction>> Button_GenerateFoodMenu(ColonistGroup group)
		{
			foreach (FoodRestriction foodRestriction in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
			{
				yield return new Widgets.DropdownMenuElement<FoodRestriction>
				{
					option = new FloatMenuOption(foodRestriction.label, delegate
					{
						foreach (var pawn in group.pawns)
                        {
							pawn.foodRestriction.CurrentFoodRestriction = foodRestriction;
						}
					}),
					payload = foodRestriction
				};
			}
		}

		private void DoAssignFoodRestrictionButtons(Rect rect, ColonistGroup group)
		{
			int num = Mathf.FloorToInt((rect.width - 4f) * 0.714285731f);
			int num2 = Mathf.FloorToInt((rect.width - 4f) * 0.2857143f);
			float x = rect.x;
			Rect rect2 = new Rect(x, rect.y + 2f, num, rect.height - 4f);
			Widgets.Dropdown(rect2, group, (ColonistGroup g) => g.pawns.First().foodRestriction.CurrentFoodRestriction, Button_GenerateFoodMenu, 
				group.pawns.First().foodRestriction.CurrentFoodRestriction.label.Truncate(rect2.width), null,
				group.pawns.First().foodRestriction.CurrentFoodRestriction.label, null, null, paintable: true);
			x += (float)num;
			x += 4f;
			if (Widgets.ButtonText(new Rect(x, rect.y + 2f, num2, rect.height - 4f), "AssignTabEdit".Translate()))
			{
				var window = new Dialog_ManageFoodRestrictions(group.pawns.First().foodRestriction.CurrentFoodRestriction);
				Find.WindowStack.Add(window);
				window.windowRect.y += 350f;
			}
			x += (float)num2;
		}

		private Vector2 cachedWorkLabelSize;

		public void DoWorkCell(Rect rect, WorkTypeDef workType)
		{
			Widgets.DrawBox(rect);
			//Text.Font = GameFont.Medium;
			//float x = rect.x + (rect.width - 25f) / 2f;
			//float y = rect.y + 2.5f;
			//WidgetsWorkGroup.DrawWorkBoxFor(x, y, workType, this.colonistGroup);
			//Text.Font = GameFont.Small;
		}

		public void DoWorkHeader(Rect rect, WorkTypeDef workType)
		{
			//Text.Font = GameFont.Small;
			//if (cachedWorkLabelSize == default(Vector2))
			//{
			//	cachedWorkLabelSize = Text.CalcSize(workType.labelShort);
			//}
			//Rect labelRect = GetLabelRect(rect);
			//MouseoverSounds.DoRegion(labelRect);
			//Text.Anchor = TextAnchor.MiddleCenter;
			//Widgets.Label(labelRect, workType.labelShort);
			//GUI.color = new Color(1f, 1f, 1f, 0.3f);
			//Widgets.DrawLineVertical(labelRect.center.x, labelRect.yMax - 3f, rect.y + 50f - labelRect.yMax + 3f);
			//Widgets.DrawLineVertical(labelRect.center.x + 1f, labelRect.yMax - 3f, rect.y + 50f - labelRect.yMax + 3f);
			//GUI.color = Color.white;
			//Text.Anchor = TextAnchor.UpperLeft;
		}

		public int GetMinHeaderHeight(PawnTable table)
		{
			return 50;
		}

		public int GetMinWidth(PawnTable table)
		{
			return Mathf.Max(32);
		}

		public int GetOptimalWidth(PawnTable table)
		{
			return 39;
		}

		public int GetMaxWidth(PawnTable table)
		{
			return Mathf.Min(80);
		}

		private bool IsIncapableOfWholeWorkType(Pawn p, WorkTypeDef work)
		{
			for (int i = 0; i < work.workGiversByPriority.Count; i++)
			{
				bool flag = true;
				for (int j = 0; j < work.workGiversByPriority[i].requiredCapacities.Count; j++)
				{
					PawnCapacityDef capacity = work.workGiversByPriority[i].requiredCapacities[j];
					if (!p.health.capacities.CapableOf(capacity))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		protected Rect GetInteractableHeaderRect(Rect headerRect, PawnTable table)
		{
			return GetLabelRect(headerRect);
		}

		private Rect GetLabelRect(Rect headerRect)
		{
			float x = headerRect.center.x;
			Rect result = new Rect(x - cachedWorkLabelSize.x / 2f, headerRect.y, cachedWorkLabelSize.x, cachedWorkLabelSize.y);
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
	}
}
