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

			var check = new Vector2(areaHeaderRect.xMax - 25f, areaHeaderRect.yMax - 34);
			Widgets.Checkbox(check, ref this.colonistGroup.groupAreaEnabled);
			var drawBoxRect = new Rect(check, new Vector2(24f, 24f));
			Widgets.DrawBox(drawBoxRect);
			TooltipHandler.TipRegion(drawBoxRect, Strings.GroupAreaTooltip);

			var areaRect = new Rect(rect.x + 10, rect.y + 205, policyButtonWidth, 30f);
			DoAreaCell(areaRect);

			var outfitHeaderRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 195f, policyButtonWidth, 20f);
			DoOutfitHeader(outfitHeaderRect);

			check = new Vector2(outfitHeaderRect.xMax - 26f, outfitHeaderRect.yMax - 33);
			Widgets.Checkbox(check, ref this.colonistGroup.groupOutfitEnabled);
			drawBoxRect = new Rect(check, new Vector2(24f, 24f));
			Widgets.DrawBox(drawBoxRect);
			TooltipHandler.TipRegion(drawBoxRect, Strings.GroupFoodTooltip);

			var outfitRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 205, policyButtonWidth, 30f);
			DoOutfitCell(outfitRect);

			var drugPolicyHeaderRect = new Rect(rect.x + 10, rect.y + 305f, policyButtonWidth, 20f);
			DoDrugPolicyHeader(drugPolicyHeaderRect);

			check = new Vector2(drugPolicyHeaderRect.xMax - 25f, drugPolicyHeaderRect.yMax - 33);
			Widgets.Checkbox(check, ref this.colonistGroup.groupDrugPolicyEnabled);
			drawBoxRect = new Rect(check, new Vector2(24f, 24f));
			Widgets.DrawBox(drawBoxRect);
			TooltipHandler.TipRegion(drawBoxRect, Strings.GroupDrugsTooltip);

			var drugPolicyRect = new Rect(rect.x + 10, rect.y + 315, policyButtonWidth, 30f);
			DoDrugPolicyCell(drugPolicyRect);

			var foodHeaderRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 305f, policyButtonWidth, 20f);
			DoFoodHeader(foodHeaderRect);

			check = new Vector2(foodHeaderRect.xMax - 26f, foodHeaderRect.yMax - 33);
			Widgets.Checkbox(check, ref this.colonistGroup.groupFoodRestrictionEnabled);
			drawBoxRect = new Rect(check, new Vector2(24f, 24f));
			Widgets.DrawBox(drawBoxRect);
			TooltipHandler.TipRegion(drawBoxRect, Strings.GroupFoodTooltip);

			var foodRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 315, policyButtonWidth, 30f);
			DoFoodCell(foodRect);

			if (ModCompatibility.BetterPawnControlIsActive)
            {
				ModCompatibility.restrictManagerSaveCurrentStateMethod.Invoke(null, new object[] { this.colonistGroup.pawns });
            }
			Text.Anchor = TextAnchor.MiddleCenter;
			var moodTexture = GetMoodTexture(out string moodLabel);
			var moodRect = new Rect(rect.x + policyButtonWidth + 135f, rect.y + 25, moodTexture.width, moodTexture.height);
			GUI.DrawTexture(moodRect, moodTexture);
			var moodLabelRect = new Rect(moodRect.x, moodRect.y + moodTexture.height, 45, 24);
			Widgets.Label(moodLabelRect, moodLabel);
			TooltipHandler.TipRegion(moodRect, Strings.MoodIconTooltip);

			var healthTexture = GetHealthTexture(out string healthPercent);
			var healthRect = new Rect(moodRect.x + 45f, moodRect.y, healthTexture.width, healthTexture.height);
			GUI.DrawTexture(healthRect, healthTexture);
			var healthLabelRect = new Rect(healthRect.x, healthRect.y + healthRect.height, 40, 24);
			Widgets.Label(healthLabelRect, healthPercent);
			TooltipHandler.TipRegion(healthRect, Strings.HealthIconTooltip);

			var restTexture = GetRestTexture(out string restPercent);
			var restRect = new Rect(healthRect.x + 45f, healthRect.y, restTexture.width, restTexture.height);
			GUI.DrawTexture(restRect, restTexture);
			var restLabelRect = new Rect(restRect.x, restRect.y + restRect.height, 40, 24);
			Widgets.Label(restLabelRect, restPercent);
			TooltipHandler.TipRegion(restRect, Strings.RestIconTooltip);

			var foodTexture = GetFoodTexture(out string foodPercent);
			var foodStatRect = new Rect(restRect.x + 45f, restRect.y, foodTexture.width, foodTexture.height);
			GUI.DrawTexture(foodStatRect, foodTexture);
			var foodLabelRect = new Rect(foodStatRect.x, foodStatRect.y + foodStatRect.height, 40, 24);
			Widgets.Label(foodLabelRect, foodPercent);
			TooltipHandler.TipRegion(foodStatRect, Strings.HungerIconTooltip);

			var pawnRowRect = new Rect(rect.x + 15, rect.y + (rect.height - 110f), rect.width - 30f, TacticalColonistBar.DefaultBaseSize.y + 42f);
			var pawnMargin = 20f;
			float listWidth = this.colonistGroup.pawns.Count * (TacticalColonistBar.DefaultBaseSize.x + pawnMargin);
			Rect rect1 = new Rect(pawnRowRect.x, pawnRowRect.y, listWidth, pawnRowRect.height - 16f);
			Widgets.BeginScrollView(pawnRowRect, ref scrollPosition, rect1);

			for (var i = 0; i < this.colonistGroup.pawns.Count; i++ )
            {
				var pawnRect = new Rect(pawnRowRect.x + 13f + (i * (TacticalColonistBar.DefaultBaseSize.x + pawnMargin)), pawnRowRect.y + 17, TacticalColonistBar.DefaultBaseSize.x, TacticalColonistBar.DefaultBaseSize.y);
				DrawColonist(pawnRect, this.colonistGroup.pawns[i], this.colonistGroup.pawns[i].Map, false, false);
				HandleClicks(pawnRect, this.colonistGroup.pawns[i]);
			}

			for (var i = 0; i < this.colonistGroup.pawns.Count; i++)
			{
				var pawnRect = new Rect(pawnRowRect.x + 13f + (i * (TacticalColonistBar.DefaultBaseSize.x + pawnMargin)), pawnRowRect.y + 17, TacticalColonistBar.DefaultBaseSize.x, TacticalColonistBar.DefaultBaseSize.y);
				DrawPawnArrows(pawnRect, this.colonistGroup.pawns[i]);
			}

			Widgets.EndScrollView();
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
		}

		public static void DrawColonist(Rect rect, Pawn colonist, Map pawnMap, bool highlight, bool reordering)
		{
			float alpha = TacticUtils.TacticalColonistBar.GetEntryRectAlpha(rect);
			TacticUtils.TacticalColonistBar.drawer.ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref alpha);
			if (reordering)
			{
				alpha *= 0.5f;
			}
			Color color2 = GUI.color = new Color(1f, 1f, 1f, alpha);
			GUI.DrawTexture(rect, TacticalColonistBar.BGTex);
			if (colonist.needs != null && colonist.needs.mood != null)
			{
				Rect position = rect.ContractedBy(2f);
				float num = position.height * colonist.needs.mood.CurLevelPercentage;
				position.yMin = position.yMax - num;
				position.height = num;
				if (TacticalGroupsSettings.DisplayColorBars)
				{
					GUI.DrawTexture(position, ColonistBarColonistDrawer.GetMoodBarTexture(colonist));
				}
				else
				{
					GUI.DrawTexture(position, ColonistBarColonistDrawer.MoodBGTex);
				}
			}

			if (highlight)
			{
				int thickness = (rect.width <= 22f) ? 2 : 3;
				GUI.color = Color.white;
				Widgets.DrawBox(rect, thickness);
				GUI.color = color2;
			}
			Rect rect2 = rect.ContractedBy(-2f * TacticUtils.TacticalColonistBar.Scale);
			if ((colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist.Corpse) : Find.Selector.SelectedObjects.Contains(colonist)) && !WorldRendererUtility.WorldRenderedNow)
			{
				TacticUtils.TacticalColonistBar.drawer.DrawSelectionOverlayOnGUI(colonist, rect2);
			}
			else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
			{
				TacticUtils.TacticalColonistBar.drawer.DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
			}
			GUI.DrawTexture(GetPawnTextureRect(rect.position), PortraitsCache.Get(colonist, ColonistBarColonistDrawer.DefaultPawnTextureSize, ColonistBarColonistDrawer.PawnTextureCameraOffset, 1.28205f));
			if (colonist.Drafted)
			{
				GUI.DrawTexture(rect, Textures.PawnDrafted);
			}
			GUI.color = new Color(1f, 1f, 1f, alpha * 0.8f);
			TacticUtils.TacticalColonistBar.drawer.DrawIcons(rect, colonist);
			GUI.color = color2;
			if (colonist.Dead)
			{
				GUI.DrawTexture(rect, ColonistBarColonistDrawer.DeadColonistTex);
			}
			float num2 = 4f * TacticUtils.TacticalColonistBar.Scale;
			Vector2 pos = new Vector2(rect.center.x, rect.yMax - num2);
			GenMapUI.DrawPawnLabel(colonist, pos, alpha, rect.width + TacticUtils.TacticalColonistBar.SpaceBetweenColonistsHorizontal - 2f, TacticUtils.TacticalColonistBar.drawer.pawnLabelsCache);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;

			ColonistBarColonistDrawer.DrawHealthBar(colonist, rect);
			ColonistBarColonistDrawer.DrawRestAndFoodBars(colonist, rect, Textures.RestFood.width);
			//ColonistBarColonistDrawer.ShowDrafteesWeapon(rect, colonist, 10);
		}

		public static Rect GetPawnTextureRect(Vector2 pos)
		{
			float x = pos.x;
			float y = pos.y;
			Vector2 vector = ColonistBarColonistDrawer.DefaultPawnTextureSize;
			return new Rect(x + 1f, y - (vector.y - TacticalColonistBar.DefaultBaseSize.y) - 1f, vector.x, vector.y).ContractedBy(1f);
		}

		public void HandleClicks(Rect rect, Pawn colonist)
		{
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 2 && Mouse.IsOver(rect))
			{
				Event.current.Use();
				CameraJumper.TryJump(colonist);
			}
			else if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(rect))
			{
				Event.current.Use();
				Find.Selector.ClearSelection();
				if (!colonist.IsWorldPawn())
                {
					Find.Selector.Select(colonist);
                }
				AddPawnInfoWindow(colonist);
			}
		}

		public void AddPawnInfoWindow(Pawn pawn)
		{
			TieredFloatMenu floatMenu = new PawnInfoMenu(pawn, this, colonistGroup, windowRect, Textures.PawnInfoMenu);
			OpenNewMenu(floatMenu);
		}

		public Dictionary<Pawn, bool> pawnReorderingMode = new Dictionary<Pawn, bool>();
		public void DrawPawnArrows(Rect rect, Pawn pawn)
		{
			bool reset = true;
			if (Mouse.IsOver(rect))
			{
				reset = false;
				if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.clickCount == 1)
				{
					Event.current.Use();
					pawnReorderingMode[pawn] = true;
				}
			}

			if (pawnReorderingMode.TryGetValue(pawn, out bool value) && value)
			{
				var rightPawnArrowRect = new Rect(rect.x + rect.width, rect.y, Textures.PawnArrowRight.width, Textures.PawnArrowRight.height);
				if (Mouse.IsOver(rightPawnArrowRect.ExpandedBy(3f)))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(rightPawnArrowRect))
					{
						var indexOf = this.colonistGroup.pawns.IndexOf(pawn);
						if (this.colonistGroup.pawns.Count > indexOf + 1)
						{
							this.colonistGroup.pawns.RemoveAt(indexOf);
							this.colonistGroup.pawns.Insert(indexOf + 1, pawn);
						}
						else if (indexOf != 0)
						{
							this.colonistGroup.pawns.RemoveAt(indexOf);
							this.colonistGroup.pawns.Insert(0, pawn);
						}
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					reset = false;
				}
				GUI.DrawTexture(rightPawnArrowRect, Textures.PawnArrowRight);

				var leftPawnArrowRect = new Rect(rect.x - Textures.PawnArrowLeft.width, rect.y, Textures.PawnArrowLeft.width, Textures.PawnArrowLeft.height);
				if (Mouse.IsOver(leftPawnArrowRect.ExpandedBy(3f)))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(leftPawnArrowRect))
					{
						var indexOf = this.colonistGroup.pawns.IndexOf(pawn);
						if (indexOf > 0)
						{
							this.colonistGroup.pawns.RemoveAt(indexOf);
							this.colonistGroup.pawns.Insert(indexOf - 1, pawn);
						}
						else if (indexOf != this.colonistGroup.pawns.Count)
						{
							this.colonistGroup.pawns.RemoveAt(indexOf);
							this.colonistGroup.pawns.Insert(this.colonistGroup.pawns.Count, pawn);
						}
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					reset = false;
				}
				GUI.DrawTexture(leftPawnArrowRect, Textures.PawnArrowLeft);
			}

			if (reset)
			{
				pawnReorderingMode[pawn] = false;
			}
		}

		private Vector2 scrollPosition;
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
			moodString = Strings.Happy;
			if (moodAverage.Count > 0)
            {
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
			}
			else
            {
				moodString = "NoMood".Translate();
            }
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
			var averageValue = healthAverage.Average();
			healthPercent = (averageValue * 100f).ToStringDecimalIfSmall() + "%";
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
			if (restAverage.Count() > 0)
            {
				var averageValue = restAverage.Average();
				restPercent = (averageValue * 100f).ToStringDecimalIfSmall() + "%";
				if (averageValue < 0.33)
				{
					return Textures.TiredIcon;
				}
				else if (averageValue < 0.66)
				{
					return Textures.AwakeIcon;
				}
			}
			else
            {
				restPercent = "NoRest".Translate();
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
			if (foodAverage.Count > 0)
            {
				var averageValue = foodAverage.Average();
				foodPercent = (averageValue * 100f).ToStringDecimalIfSmall() + "%";
				if (averageValue < 0.33f)
				{
					return Textures.StarvingIcon;
				}
				else if (averageValue < 0.66f)
				{
					return Textures.HungryIcon;
				}
			}
			else
            {
				foodPercent = "NoFood".Translate();
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
						if (group.groupOutfitEnabled)
                        {
							group.groupOutfit = outfit;
                        }
						if (ModCompatibility.BetterPawnControlIsActive)
						{
							ModCompatibility.assignManagerSaveCurrentStateMethod.Invoke(null, new object[]
							{
								group.pawns
							});
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
						if (group.groupFoodRestrictionEnabled)
                        {
							group.groupFoodRestriction = foodRestriction;
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
