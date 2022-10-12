using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TacticalGroups
{
	public class ManagementMenu : TieredFloatMenu
	{
		public static readonly Vector2 PawnTextureSize = new Vector2(TacticalColonistBar.DefaultBaseSize.x - 2f, 75f);

		public static readonly Vector3 PawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);

		public static float PawnTextureCameraZoom = 1.28205f;
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(backgroundTexture.width / 10, 55f);
		public ManagementMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			options = new List<TieredFloatMenuOption>();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Rect hostilityResponseRect = new Rect(rect.x + 20, rect.y + 25, 24, 24);
			HostilityResponseModeUtilityGroup.DrawResponseButton(hostilityResponseRect, colonistGroup, true);

			Rect medicalCareRect = new Rect(rect.x + 50, rect.y + 25, 24, 24);
			MedicalCareUtilityGroup.MedicalCareSelectButton(medicalCareRect, colonistGroup);

			if (ModsConfig.IdeologyActive)
			{
				Rect groupColorRect = new Rect(hostilityResponseRect.x, hostilityResponseRect.yMax + 7, 24, 24);
				GUI.DrawTexture(groupColorRect.ExpandedBy(5), ContentFinder<Texture2D>.Get("Things/Item/Dye/Dye_a"));
				if (Mouse.IsOver(groupColorRect))
				{
					TooltipHandler.TipRegion(groupColorRect, Strings.GroupColorTooltip);
					Widgets.DrawHighlight(groupColorRect);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						Dialog_ColorPicker colorPicker = new Dialog_ColorPicker(this, colonistGroup, windowRect, Textures.DyeMenu);
						Find.WindowStack.Add(colorPicker);
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
				}
			}
			Rect timeAssignmentSelectorGridRect = new Rect(rect.x + 80, rect.y + 20, 191f, 65f);
			TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(timeAssignmentSelectorGridRect);
			Rect timeTableHeaderRect = new Rect(rect.x + 10, rect.y + 85f, rect.width - 20f, 20f);
			DoTimeTableHeader(timeTableHeaderRect);
			Rect timeTableRect = new Rect(rect.x + 10, rect.y + 105, rect.width - 20f, 30f);
			DoTimeTableCell(timeTableRect);


			float policyButtonWidth = rect.width * 0.45f;
			Rect areaHeaderRect = new Rect(rect.x + 10, rect.y + 195f, policyButtonWidth, 20f);
			DoAreaHeader(areaHeaderRect);

			Vector2 check = new Vector2(areaHeaderRect.xMax - 25f, areaHeaderRect.yMax - 34);
			Widgets.Checkbox(check, ref colonistGroup.groupAreaEnabled);
			Rect drawBoxRect = new Rect(check, new Vector2(24f, 24f));
			Widgets.DrawBox(drawBoxRect);
			TooltipHandler.TipRegion(drawBoxRect, Strings.GroupAreaTooltip);

			Rect areaRect = new Rect(rect.x + 10, rect.y + 205, policyButtonWidth, 30f);
			DoAreaCell(areaRect);

			Rect outfitHeaderRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 195f, policyButtonWidth, 20f);
			DoOutfitHeader(outfitHeaderRect);

			check = new Vector2(outfitHeaderRect.xMax - 26f, outfitHeaderRect.yMax - 33);
			Widgets.Checkbox(check, ref colonistGroup.groupOutfitEnabled);
			drawBoxRect = new Rect(check, new Vector2(24f, 24f));
			Widgets.DrawBox(drawBoxRect);
			TooltipHandler.TipRegion(drawBoxRect, Strings.GroupFoodTooltip);

			Rect outfitRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 205, policyButtonWidth, 30f);
			DoOutfitCell(outfitRect);

			Rect drugPolicyHeaderRect = new Rect(rect.x + 10, rect.y + 305f, policyButtonWidth, 20f);
			DoDrugPolicyHeader(drugPolicyHeaderRect);

			check = new Vector2(drugPolicyHeaderRect.xMax - 25f, drugPolicyHeaderRect.yMax - 33);
			Widgets.Checkbox(check, ref colonistGroup.groupDrugPolicyEnabled);
			drawBoxRect = new Rect(check, new Vector2(24f, 24f));
			Widgets.DrawBox(drawBoxRect);
			TooltipHandler.TipRegion(drawBoxRect, Strings.GroupDrugsTooltip);

			Rect drugPolicyRect = new Rect(rect.x + 10, rect.y + 315, policyButtonWidth, 30f);
			DoDrugPolicyCell(drugPolicyRect);

			Rect foodHeaderRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 305f, policyButtonWidth, 20f);
			DoFoodHeader(foodHeaderRect);

			check = new Vector2(foodHeaderRect.xMax - 26f, foodHeaderRect.yMax - 33);
			Widgets.Checkbox(check, ref colonistGroup.groupFoodRestrictionEnabled);
			drawBoxRect = new Rect(check, new Vector2(24f, 24f));
			Widgets.DrawBox(drawBoxRect);
			TooltipHandler.TipRegion(drawBoxRect, Strings.GroupFoodTooltip);

			Rect foodRect = new Rect(rect.x + policyButtonWidth + 30, rect.y + 315, policyButtonWidth, 30f);
			DoFoodCell(foodRect);

			if (!(ModCompatibility.assignManagerSaveCurrentStateMethod is null))
			{
				ModCompatibility.assignManagerSaveCurrentStateMethod.Invoke(null, new object[] { colonistGroup.pawns });
			}
			Text.Anchor = TextAnchor.MiddleCenter;
			Texture2D moodTexture = GetMoodTexture(out string moodLabel);
			Rect moodRect = new Rect(rect.x + policyButtonWidth + 135f, rect.y + 25, moodTexture.width, moodTexture.height);
			GUI.DrawTexture(moodRect, moodTexture);
			Rect moodLabelRect = new Rect(moodRect.x, moodRect.y + moodTexture.height, 45, 24);
			Widgets.Label(moodLabelRect, moodLabel);
			TooltipHandler.TipRegion(moodRect, Strings.MoodIconTooltip);

			Texture2D healthTexture = GetHealthTexture(out string healthPercent);
			Rect healthRect = new Rect(moodRect.x + 45f, moodRect.y, healthTexture.width, healthTexture.height);
			GUI.DrawTexture(healthRect, healthTexture);
			Rect healthLabelRect = new Rect(healthRect.x, healthRect.y + healthRect.height, 40, 24);
			Widgets.Label(healthLabelRect, healthPercent);
			TooltipHandler.TipRegion(healthRect, Strings.HealthIconTooltip);

			Texture2D restTexture = GetRestTexture(out string restPercent);
			Rect restRect = new Rect(healthRect.x + 45f, healthRect.y, restTexture.width, restTexture.height);
			GUI.DrawTexture(restRect, restTexture);
			Rect restLabelRect = new Rect(restRect.x, restRect.y + restRect.height, 40, 24);
			Widgets.Label(restLabelRect, restPercent);
			TooltipHandler.TipRegion(restRect, Strings.RestIconTooltip);

			Texture2D foodTexture = GetFoodTexture(out string foodPercent);
			Rect foodStatRect = new Rect(restRect.x + 45f, restRect.y, foodTexture.width, foodTexture.height);
			GUI.DrawTexture(foodStatRect, foodTexture);
			Rect foodLabelRect = new Rect(foodStatRect.x, foodStatRect.y + foodStatRect.height, 40, 24);
			Widgets.Label(foodLabelRect, foodPercent);
			TooltipHandler.TipRegion(foodStatRect, Strings.HungerIconTooltip);

			Rect pawnRowRect = new Rect(rect.x + 15, rect.y + (rect.height - 110f), rect.width - 30f, TacticalColonistBar.DefaultBaseSize.y + 42f);
			float pawnMargin = 20f;
			float listWidth = colonistGroup.pawns.Count * (TacticalColonistBar.DefaultBaseSize.x + pawnMargin);
			Rect rect1 = new Rect(pawnRowRect.x, pawnRowRect.y, listWidth, pawnRowRect.height - 16f);
			Widgets.BeginScrollView(pawnRowRect, ref scrollPosition, rect1);

			for (int i = 0; i < colonistGroup.pawns.Count; i++)
			{
				Rect pawnRect = new Rect(pawnRowRect.x + 13f + (i * (TacticalColonistBar.DefaultBaseSize.x + pawnMargin)), pawnRowRect.y + 17, TacticalColonistBar.DefaultBaseSize.x, TacticalColonistBar.DefaultBaseSize.y);
				DrawColonist(pawnRect, colonistGroup.pawns[i], colonistGroup.pawns[i].Map, false, false);
				HandleClicks(pawnRect, colonistGroup.pawns[i]);
			}

			for (int i = 0; i < colonistGroup.pawns.Count; i++)
			{
				Rect pawnRect = new Rect(pawnRowRect.x + 13f + (i * (TacticalColonistBar.DefaultBaseSize.x + pawnMargin)), pawnRowRect.y + 17, TacticalColonistBar.DefaultBaseSize.x, TacticalColonistBar.DefaultBaseSize.y);
				DrawPawnArrows(pawnRect, colonistGroup.pawns[i]);
			}

			Widgets.EndScrollView();
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
		}

		public static void DrawColonist(Rect rect, Pawn colonist, Map pawnMap, bool highlight, bool reordering, bool showSlaveSuppresion = false)
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
			GUI.DrawTexture(GetPawnTextureRect(rect.position), PortraitsCache.Get(colonist, PawnTextureSize, Rot4.South, PawnTextureCameraOffset, PawnTextureCameraZoom));
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

			if (showSlaveSuppresion && colonist.needs.TryGetNeed<Need_Suppression>() is Need_Suppression need_Suppression)
			{
				Rect suppressionBar = new Rect(rect.x + rect.width, rect.y, Textures.RestFood.width, rect.height);
				float num = Mathf.Clamp(need_Suppression.CurLevelPercentage, 0f, 1f);
				Rect rect3 = GenUI.ContractedBy(suppressionBar, 1f);
				float num5 = rect3.height * num;
				rect3.yMin = rect3.yMax - num5;
				rect3.height = num5;
				GUI.DrawTexture(rect3, Textures.SlaveSuppressionBar, ScaleMode.ScaleAndCrop);
				GUI.DrawTexture(suppressionBar, Textures.RestFood, ScaleMode.StretchToFill);
			}
			else
			{
				ColonistBarColonistDrawer.DrawHealthBar(rect, colonist, Textures.HealthBar.width);
				ColonistBarColonistDrawer.DrawRestAndFoodBars(rect, colonist, Textures.RestFood.width);
			}
		}

		public static Rect GetPawnTextureRect(Vector2 pos)
		{
			float x = pos.x;
			float y = pos.y;
			Vector2 vector = PawnTextureSize;
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
				if (!colonist.IsWorldPawn() && !colonist.InContainerEnclosed)
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
				Rect rightPawnArrowRect = new Rect(rect.x + rect.width, rect.y, Textures.PawnArrowRight.width, Textures.PawnArrowRight.height);
				if (Mouse.IsOver(rightPawnArrowRect.ExpandedBy(3f)))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(rightPawnArrowRect))
					{
						int indexOf = colonistGroup.pawns.IndexOf(pawn);
						if (colonistGroup.pawns.Count > indexOf + 1)
						{
							colonistGroup.pawns.RemoveAt(indexOf);
							colonistGroup.pawns.Insert(indexOf + 1, pawn);
						}
						else if (indexOf != 0)
						{
							colonistGroup.pawns.RemoveAt(indexOf);
							colonistGroup.pawns.Insert(0, pawn);
						}
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					reset = false;
				}
				GUI.DrawTexture(rightPawnArrowRect, Textures.PawnArrowRight);

				Rect leftPawnArrowRect = new Rect(rect.x - Textures.PawnArrowLeft.width, rect.y, Textures.PawnArrowLeft.width, Textures.PawnArrowLeft.height);
				if (Mouse.IsOver(leftPawnArrowRect.ExpandedBy(3f)))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(leftPawnArrowRect))
					{
						int indexOf = colonistGroup.pawns.IndexOf(pawn);
						if (indexOf > 0)
						{
							colonistGroup.pawns.RemoveAt(indexOf);
							colonistGroup.pawns.Insert(indexOf - 1, pawn);
						}
						else if (indexOf != colonistGroup.pawns.Count)
						{
							colonistGroup.pawns.RemoveAt(indexOf);
							colonistGroup.pawns.Insert(colonistGroup.pawns.Count, pawn);
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
			List<float> moodAverage = new List<float>();
			foreach (Pawn pawn in colonistGroup.pawns)
			{
				if (pawn.needs?.mood != null)
				{
					moodAverage.Add(pawn.needs.mood.CurLevelPercentage);
				}
			}
			moodString = Strings.Happy;
			if (moodAverage.Count > 0)
			{
				float averageValue = moodAverage.Average();
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
			List<float> healthAverage = new List<float>();
			foreach (Pawn pawn in colonistGroup.pawns)
			{
				if (pawn.health?.summaryHealth != null)
				{
					healthAverage.Add(pawn.health.summaryHealth.SummaryHealthPercent);
				}
			}
			float averageValue = healthAverage.Average();
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
			List<float> restAverage = new List<float>();
			foreach (Pawn pawn in colonistGroup.pawns)
			{
				if (pawn.needs?.rest != null)
				{
					restAverage.Add(pawn.needs.rest.CurLevelPercentage);
				}
			}
			if (restAverage.Count() > 0)
			{
				float averageValue = restAverage.Average();
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
			List<float> foodAverage = new List<float>();
			foreach (Pawn pawn in colonistGroup.pawns)
			{
				if (pawn.needs?.food != null)
				{
					foodAverage.Add(pawn.needs.food.CurLevelPercentage);
				}
			}
			if (foodAverage.Count > 0)
			{
				float averageValue = foodAverage.Average();
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
			TimeAssignmentDef assignment = colonistGroup.pawns.First().timetable.GetAssignment(hour);
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
				foreach (Pawn p in colonistGroup.pawns)
				{
					p.timetable.SetAssignment(hour, TimeAssignmentSelector.selectedAssignment);
				}
			}
		}

		public void DoAreaCell(Rect rect)
		{
			AreaAllowedGUIGroup.DoAllowedAreaSelectors(rect, colonistGroup);
		}

		public void DoAreaHeader(Rect rect)
		{
			if (Widgets.ButtonText(new Rect(rect.x, rect.y + (rect.height - 65f), Mathf.Min(rect.width, 360f), 32f), "ManageAreas".Translate()))
			{
				Dialog_ManageAreas window = new Dialog_ManageAreas(colonistGroup.Map);
				Find.WindowStack.Add(window);
			}
		}

		public void DoOutfitHeader(Rect rect)
		{
			MouseoverSounds.DoRegion(rect);
			Rect rect2 = new Rect(rect.x, rect.y + (rect.height - 65f), Mathf.Min(rect.width, 360f), 32f);
			if (Widgets.ButtonText(rect2, "ManageOutfits".Translate()))
			{
				Dialog_ManageOutfits window = new Dialog_ManageOutfits(null);
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
			bool somethingIsForced = colonistGroup.pawns.First().outfits.forcedHandler.SomethingIsForced;
			Rect rect2 = new Rect(x, rect.y + 2f, num, rect.height - 4f);
			if (somethingIsForced)
			{
				rect2.width -= 4f + num2;
			}
			Widgets.Dropdown(rect2, colonistGroup, (ColonistGroup group) => group.pawns.First().outfits.CurrentOutfit, Button_GenerateMenu,
				colonistGroup.pawns.First().outfits.CurrentOutfit.label.Truncate(rect2.width), null,
				colonistGroup.pawns.First().outfits.CurrentOutfit.label, null, null, paintable: true);
			x += rect2.width;
			x += 4f;
			Rect rect3 = new Rect(x, rect.y + 2f, num2, rect.height - 4f);
			if (somethingIsForced)
			{
				if (Widgets.ButtonText(rect3, "ClearForcedApparel".Translate()))
				{
					foreach (Pawn pawn in colonistGroup.pawns)
					{
						pawn.outfits.forcedHandler.Reset();
					}
				}
				x += num2;
				x += 4f;
			}
			Rect rect4 = new Rect(x, rect.y + 2f, num2, rect.height - 4f);
			if (Widgets.ButtonText(rect4, "AssignTabEdit".Translate()))
			{
				Dialog_ManageOutfits window = new Dialog_ManageOutfits(colonistGroup.pawns.First().outfits.CurrentOutfit);
				Find.WindowStack.Add(window);
			}
			x += num2;
		}

		private IEnumerable<Widgets.DropdownMenuElement<Outfit>> Button_GenerateMenu(ColonistGroup group)
		{
			foreach (Outfit outfit in Current.Game.outfitDatabase.AllOutfits)
			{
				yield return new Widgets.DropdownMenuElement<Outfit>
				{
					option = new FloatMenuOption(outfit.label, delegate
					{
						foreach (Pawn pawn in group.pawns)
						{
							pawn.outfits.CurrentOutfit = outfit;
						}
						if (group.groupOutfitEnabled)
						{
							group.groupOutfit = outfit;
						}
						if (!(ModCompatibility.assignManagerSaveCurrentStateMethod is null))
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
				Dialog_ManageDrugPolicies window = new Dialog_ManageDrugPolicies(null);
				Find.WindowStack.Add(window);
			}
			UIHighlighter.HighlightOpportunity(rect2, "ManageDrugPolicies");
			UIHighlighter.HighlightOpportunity(rect2, "ButtonAssignDrugs");
		}

		public void DoDrugPolicyCell(Rect rect)
		{
			DrugPolicyUIUtilityGroup.DoAssignDrugPolicyButtons(rect, colonistGroup);
		}

		public void DoFoodHeader(Rect rect)
		{
			MouseoverSounds.DoRegion(rect);
			if (Widgets.ButtonText(new Rect(rect.x, rect.y + (rect.height - 65f), Mathf.Min(rect.width, 360f), 32f), "ManageFoodRestrictions".Translate()))
			{
				Dialog_ManageFoodRestrictions window = new Dialog_ManageFoodRestrictions(null);
				Find.WindowStack.Add(window);
			}
		}

		public void DoFoodCell(Rect rect)
		{
			DoAssignFoodRestrictionButtons(rect, colonistGroup);
		}

		private IEnumerable<Widgets.DropdownMenuElement<FoodRestriction>> Button_GenerateFoodMenu(ColonistGroup group)
		{
			foreach (FoodRestriction foodRestriction in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
			{
				yield return new Widgets.DropdownMenuElement<FoodRestriction>
				{
					option = new FloatMenuOption(foodRestriction.label, delegate
					{
						foreach (Pawn pawn in group.pawns)
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
			x += num;
			x += 4f;
			if (Widgets.ButtonText(new Rect(x, rect.y + 2f, num2, rect.height - 4f), "AssignTabEdit".Translate()))
			{
				Dialog_ManageFoodRestrictions window = new Dialog_ManageFoodRestrictions(group.pawns.First().foodRestriction.CurrentFoodRestriction);
				Find.WindowStack.Add(window);
			}
			x += num2;
		}
	}
}
