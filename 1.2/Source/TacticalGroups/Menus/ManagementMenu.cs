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
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			var hostilityResponseRect = new Rect(rect.x + 20, rect.y + 25, 24, 24);
			HostilityResponseModeUtilityGroup.DrawResponseButton(hostilityResponseRect, this.colonistGroup, true);

			var medicalCareRect = new Rect(rect.x + 50, rect.y + 25, 24, 24);
			MedicalCareUtilityGroup.MedicalCareSelectButton(medicalCareRect, this.colonistGroup);

			var timeAssignmentSelectorGridRect = new Rect(rect.x + 80, rect.y + 20, 191f, 65f);
			TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(timeAssignmentSelectorGridRect);
			var timeTableHeaderRect = new Rect(rect.x + 10, rect.y + 85f, rect.width - 20f, 20f);
			DoTimeTableHeader(timeTableHeaderRect);
			var timeTableRect = new Rect(rect.x + 10, rect.y + 105, rect.width - 20f, 30f);
			DoTimeTableCell(timeTableRect);

			var policyButtonWidth = rect.width * 0.45f;
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

			Text.Anchor = TextAnchor.MiddleCenter;
			var moodTexture = GetMoodTexture(out string moodLabel);
			var moodRect = new Rect(rect.x + policyButtonWidth + 135f, rect.y + 25, moodTexture.width, moodTexture.height);
			GUI.DrawTexture(moodRect, moodTexture);
			var moodLabelRect = new Rect(moodRect.x, moodRect.y + moodTexture.height, 45, 24);
			Widgets.Label(moodLabelRect, moodLabel);

			var healthTexture = GetHealthTexture(out string healthPercent);
			var healthRect = new Rect(moodRect.x + 45f, moodRect.y, healthTexture.width, healthTexture.height);
			GUI.DrawTexture(healthRect, healthTexture);
			var healthLabelRect = new Rect(healthRect.x, healthRect.y + healthRect.height, 40, 24);
			Widgets.Label(healthLabelRect, healthPercent);

			var restTexture = GetRestTexture(out string restPercent);
			var restRect = new Rect(healthRect.x + 45f, healthRect.y, restTexture.width, restTexture.height);
			GUI.DrawTexture(restRect, restTexture);
			var restLabelRect = new Rect(restRect.x, restRect.y + restRect.height, 40, 24);
			Widgets.Label(restLabelRect, restPercent);

			var foodTexture = GetFoodTexture(out string foodPercent);
			var foodStatRect = new Rect(restRect.x + 45f, restRect.y, foodTexture.width, foodTexture.height);
			GUI.DrawTexture(foodStatRect, foodTexture);
			var foodLabelRect = new Rect(foodStatRect.x, foodStatRect.y + foodStatRect.height, 40, 24);
			Widgets.Label(foodLabelRect, foodPercent);

			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
		}

		public Texture2D GetMoodTexture(out string moodString)
        {
			var moodAverage = new List<float>();
			foreach (var pawn in this.colonistGroup.pawns)
            {
				if (pawn.needs?.mood != null)
                {
					moodAverage.Add(pawn.needs.mood.CurLevelPercentage);
				}
            }
			var averageValue = moodAverage.Average();
			if (averageValue < 0.33)
            {
				moodString = Strings.Sad;
				return Textures.SadIcon;
            }
			else if (averageValue < 0.66)
            {
				moodString = Strings.Okay;
				return Textures.OkayIcon;
            }
			moodString = Strings.Happy;
			return Textures.HappyIcon;
        }

		public Texture2D GetHealthTexture(out string healthPercent)
		{
			var healthAverage = new List<float>();
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.health?.summaryHealth != null)
				{
					healthAverage.Add(pawn.health.summaryHealth.SummaryHealthPercent);
				}
			}
			var averageValue = healthAverage.Average() * 100f;
			healthPercent = averageValue.ToStringDecimalIfSmall() + "%";
			if (averageValue < 0.33)
			{
				return Textures.HurtIcon;
			}
			else if (averageValue < 0.66)
			{
				return Textures.AliveIcon;
			}
			return Textures.HealthyIcon;
		}

		public Texture2D GetRestTexture(out string restPercent)
		{
			var restAverage = new List<float>();
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.needs?.rest != null)
				{
					restAverage.Add(pawn.needs.rest.CurLevelPercentage);
				}
			}
			var averageValue = restAverage.Average() * 100f;
			restPercent = averageValue.ToStringDecimalIfSmall() + "%";
			if (averageValue < 0.33)
			{
				return Textures.TiredIcon;
			}
			else if (averageValue < 0.66)
			{
				return Textures.AwakeIcon;
			}
			return Textures.RestedIcon;
		}

		public Texture2D GetFoodTexture(out string foodPercent)
		{
			var foodAverage = new List<float>();
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.needs?.food != null)
				{
					foodAverage.Add(pawn.needs.food.CurLevelPercentage);
				}
			}
			var averageValue = foodAverage.Average() * 100f;
			foodPercent = averageValue.ToStringDecimalIfSmall() + "%";
			if (averageValue < 0.33)
			{
				return Textures.StarvingIcon;
			}
			else if (averageValue < 0.66)
			{
				return Textures.HungryIcon;
			}
			return Textures.FullIcon;
		}

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
			}
			x += (float)num2;
		}
	}
}
