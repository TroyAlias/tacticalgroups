using ColourPicker;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class OptionsMenu : TieredFloatMenu
	{
		//protected override Vector2 InitialPositionShift => new Vector2(-45f, 35f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(backgroundTexture.width / 10, 25f);
		public OptionsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			closeOnClickedOutside = true;
		}

		public override void SetInitialSizeAndPosition()
		{
			windowRect = new Rect((UI.screenWidth - InitialSize.x) / 2f, (UI.screenHeight - InitialSize.y) / 2f, InitialSize.x, InitialSize.y);
			windowRect = windowRect.Rounded();
		}

		protected override void UpdateBaseColor()
		{

		}


		[TweakValue("0ColonyGroups", 0, 1000)] public static float textFieldWidth = 190f;
		[TweakValue("0ColonyGroups", 0, 1000)] public static float checkBoxesWidth = 180f;
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Text.Font = GameFont.Small;
			Vector2 colorPickerPos = new Vector2(windowRect.x + 60, windowRect.y + 195);

			Vector2 leftHalf = new Vector2(rect.x + checkBoxesWidth, rect.y + 25f);

			Rect showAllColonistsRect = new Rect(rect.x + 10, leftHalf.y, Textures.MenuButton.width, 25f);
			GUI.DrawTexture(showAllColonistsRect, Textures.MenuButton);
			if (Mouse.IsOver(showAllColonistsRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticUtils.ShowAllColonists();
					Event.current.Use();
				}
			}
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.ShowAllColonists);
			leftHalf.y += 25f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.HidePawnsWhenOffMap);
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.HidePawnsWhenOffMap);

			leftHalf.y += 25f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.HideGroups);
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.HideGroups);

			leftHalf.y += 45f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.DisplayFood);
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.DisplayFood);

			Rect displayFoodColor = new Rect(leftHalf.x - 24, leftHalf.y + 5, 12, 12);
			Widgets.DrawBoxSolidWithOutline(displayFoodColor, TacticalGroupsSettings.NeedFoodBarColor, Color.white);
			if (Mouse.IsOver(displayFoodColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.NeedFoodBarColor, (Color x) =>
				{
					TacticalGroupsSettings.NeedFoodBarColor = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}


			leftHalf.y += 25f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.DisplayRest);
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.DisplayRest);

			Rect displayRestColor = new Rect(displayFoodColor.x, leftHalf.y + 5, 12, 12);
			Widgets.DrawBoxSolidWithOutline(displayRestColor, TacticalGroupsSettings.NeedRestBarColor, Color.white);
			if (Mouse.IsOver(displayRestColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.NeedRestBarColor, (Color x) =>
				{
					TacticalGroupsSettings.NeedRestBarColor = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			leftHalf.y += 25f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.DisplayHealth);
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.DisplayHealth);

			Rect displayHealthColor = new Rect(displayFoodColor.x, leftHalf.y + 5, 12, 12);
			Widgets.DrawBoxSolidWithOutline(displayHealthColor, TacticalGroupsSettings.NeedHealthBarColor, Color.white);
			if (Mouse.IsOver(displayHealthColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.NeedHealthBarColor, (Color x) =>
				{
					TacticalGroupsSettings.NeedHealthBarColor = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			leftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.PawnNeedsSize + ": " + TacticalGroupsSettings.PawnNeedsWidth.ToString());
			leftHalf.y += 25f;
			TacticalGroupsSettings.PawnNeedsWidth = (int)Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnNeedsWidth, 1f, 25f);

			leftHalf.y += 35f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.ColonistBarPositionY + ": " + (int)TacticalGroupsSettings.ColonistBarPositionY);
			leftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarPositionY = Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.ColonistBarPositionY, 0f, 500f);

			leftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.ColonistBarPositionX + ": " + (int)TacticalGroupsSettings.ColonistBarPositionX);
			leftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarPositionX = Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f),
				TacticalGroupsSettings.ColonistBarPositionX, -400f, 400f);

			leftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.ColonistBarSpacingX + ": " + (int)TacticalGroupsSettings.ColonistBarSpacingX);
			leftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarSpacingX = Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.ColonistBarSpacingX, 1f, 500f);

			leftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.ColonistBarSpacingY + ": " + (int)TacticalGroupsSettings.ColonistBarSpacingY);
			leftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarSpacingY = Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.ColonistBarSpacingY, 1f, 500f);


			leftHalf.y += 45f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.DisableLabelBackground);
			Rect disableLabelBackgroundRect = new Rect(rect.x + 20, leftHalf.y, textFieldWidth - 30, 45f);
			Text.Font = GameFont.Tiny;
			Widgets.Label(disableLabelBackgroundRect, Strings.DisableLabelBackground);
			Text.Font = GameFont.Small;

			Vector2 middle = new Vector2(leftHalf.x + checkBoxesWidth, rect.y + 25f);
			float xMiddlePos = rect.x + 230;
			Widgets.Checkbox(middle, ref TacticalGroupsSettings.HideCreateGroup);
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.HideCreateGroup);

			middle.y += 25f;
			Widgets.Checkbox(middle, ref TacticalGroupsSettings.DisplayBreakRiskOverlay);
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.DisplayBreakRiskOverlay);

			middle.y += 70f;
			Widgets.Checkbox(middle, ref TacticalGroupsSettings.DisplayColorBars);
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.DisplayColorBars);

			Rect resetColorBars = new Rect(middle.x + 37, middle.y, 24, 24);
			GUI.DrawTexture(resetColorBars, Textures.ResetIcon);
			if (Mouse.IsOver(resetColorBars) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				TacticalGroupsSettings.ResetColorBars();
				TacticalGroupsSettings.InitColorBars();
			}

			middle.y += 25f;
			Rect colorBarDefaultRect = new Rect(xMiddlePos, middle.y, 80f, 30f);
			Rect colorBarExtendedRect = new Rect(xMiddlePos + 90f, middle.y, 100f, 30f);
			TooltipHandler.TipRegion(colorBarDefaultRect, Strings.ColorBarModeDefaultTooltip);
			TooltipHandler.TipRegion(colorBarExtendedRect, Strings.ColorBarModeExtendedTooltip);

			if (Widgets.RadioButtonLabeled(colorBarDefaultRect, Strings.ColorBarModeDefault, TacticalGroupsSettings.ColorBarMode == ColorBarMode.Default))
			{
				TacticalGroupsSettings.ColorBarMode = ColorBarMode.Default;
			}
			else if (Widgets.RadioButtonLabeled(colorBarExtendedRect, Strings.ColorBarModeExtended, TacticalGroupsSettings.ColorBarMode != ColorBarMode.Default))
			{
				TacticalGroupsSettings.ColorBarMode = ColorBarMode.Extended;
			}

			Rect defaultBottomColor = new Rect(colorBarDefaultRect.x + 3, colorBarDefaultRect.yMax - 3, 12, 12);
			Widgets.DrawBoxSolidWithOutline(defaultBottomColor, TacticalGroupsSettings.DefaultMoodBarLower, Color.white);
			if (Mouse.IsOver(defaultBottomColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.DefaultMoodBarLower, (Color x) =>
				{
					TacticalGroupsSettings.DefaultMoodBarLower = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			Rect defaultMiddleColor = new Rect(defaultBottomColor.xMax, defaultBottomColor.y, 12, 12);
			Widgets.DrawBoxSolidWithOutline(defaultMiddleColor, TacticalGroupsSettings.DefaultMoodBarMiddle, Color.white);
			if (Mouse.IsOver(defaultMiddleColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.DefaultMoodBarMiddle, (Color x) =>
				{
					TacticalGroupsSettings.DefaultMoodBarMiddle = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			Rect defaultUpperColor = new Rect(defaultMiddleColor.xMax, defaultBottomColor.y, 12, 12);
			Widgets.DrawBoxSolidWithOutline(defaultUpperColor, TacticalGroupsSettings.DefaultMoodBarUpper, Color.white);
			if (Mouse.IsOver(defaultUpperColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.DefaultMoodBarUpper, (Color x) =>
				{
					TacticalGroupsSettings.DefaultMoodBarUpper = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			Rect extendedBottomIIColor = new Rect(defaultUpperColor.xMax + 51, defaultBottomColor.y, 12, 12);
			Widgets.DrawBoxSolidWithOutline(extendedBottomIIColor, TacticalGroupsSettings.ExtendedMoodBarLowerII, Color.white);
			if (Mouse.IsOver(extendedBottomIIColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.ExtendedMoodBarLowerII, (Color x) =>
				{
					TacticalGroupsSettings.ExtendedMoodBarLowerII = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			Rect extendedBottomColor = new Rect(extendedBottomIIColor.xMax, defaultBottomColor.y, 12, 12);
			Widgets.DrawBoxSolidWithOutline(extendedBottomColor, TacticalGroupsSettings.ExtendedMoodBarLower, Color.white);
			if (Mouse.IsOver(extendedBottomColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.ExtendedMoodBarLower, (Color x) =>
				{
					TacticalGroupsSettings.ExtendedMoodBarLower = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			Rect extendedMiddleColor = new Rect(extendedBottomColor.xMax, defaultBottomColor.y, 12, 12);
			Widgets.DrawBoxSolidWithOutline(extendedMiddleColor, TacticalGroupsSettings.ExtendedMoodBarMiddle, Color.white);
			if (Mouse.IsOver(extendedMiddleColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.ExtendedMoodBarMiddle, (Color x) =>
				{
					TacticalGroupsSettings.ExtendedMoodBarMiddle = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			Rect extendedUpperColor = new Rect(extendedMiddleColor.xMax, defaultBottomColor.y, 12, 12);
			Widgets.DrawBoxSolidWithOutline(extendedUpperColor, TacticalGroupsSettings.ExtendedMoodBarUpper, Color.white);
			if (Mouse.IsOver(extendedUpperColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.ExtendedMoodBarUpper, (Color x) =>
				{
					TacticalGroupsSettings.ExtendedMoodBarUpper = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			Rect extendedUpperIIColor = new Rect(extendedUpperColor.xMax, defaultBottomColor.y, 12, 12);
			Widgets.DrawBoxSolidWithOutline(extendedUpperIIColor, TacticalGroupsSettings.ExtendedMoodBarUpperII, Color.white);
			if (Mouse.IsOver(extendedUpperIIColor) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(TacticalGroupsSettings.ExtendedMoodBarUpperII, (Color x) =>
				{
					TacticalGroupsSettings.ExtendedMoodBarUpperII = x;
					TacticalGroupsSettings.InitColorBars();
				}, colorPickerPos));
			}

			middle.y += 50f;
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.HealthBarSize + ": " + TacticalGroupsSettings.HealthBarWidth.ToString());
			middle.y += 25f;
			TacticalGroupsSettings.HealthBarWidth = (int)Widgets.HorizontalSlider(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), TacticalGroupsSettings.HealthBarWidth, 1f, 25f);

			middle.y += 35f;
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.GroupRowCount + ": " + TacticalGroupsSettings.GroupRowCount.ToString());
			middle.y += 25f;
			TacticalGroupsSettings.GroupRowCount = (int)Widgets.HorizontalSlider(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), TacticalGroupsSettings.GroupRowCount, 1f, 12f);
			middle.y += 25f;
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.ColonyGroupScale + ": " + TacticalGroupsSettings.ColonyGroupScale.ToStringDecimalIfSmall());
			middle.y += 25f;
			TacticalGroupsSettings.ColonyGroupScale = Widgets.HorizontalSlider(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), TacticalGroupsSettings.ColonyGroupScale, 0.5f, 2f);
			middle.y += 25f;
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.GroupScale + ": " + TacticalGroupsSettings.GroupScale.ToStringDecimalIfSmall());
			middle.y += 25f;
			TacticalGroupsSettings.GroupScale = Widgets.HorizontalSlider(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), TacticalGroupsSettings.GroupScale, 0.5f, 2f);

			middle.y += 100f;
			Rect resetButtonRect = new Rect(xMiddlePos, middle.y, Textures.MenuButton.width, 25f);
			GUI.DrawTexture(resetButtonRect, Textures.MenuButton);

			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(resetButtonRect, Strings.ResetToDefault);
			if (Mouse.IsOver(resetButtonRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticalGroupsSettings.DoReset();
					Event.current.Use();
				}
			}

			Vector2 rightHalf = new Vector2(middle.x + checkBoxesWidth + 70, rect.y + 25f);
			float xRightPos = xMiddlePos + checkBoxesWidth + 50;

			rightHalf.y += 10f;
			Rect resetPawnViewRect = new Rect(xRightPos, rightHalf.y, Textures.MenuButton.width, 25f);
			GUI.DrawTexture(resetPawnViewRect, Textures.MenuButton);
			Widgets.Label(new Rect(xRightPos + 10, resetPawnViewRect.y, Textures.MenuButton.width - 10, 25f), Strings.ResetPawnView);
			if (Mouse.IsOver(resetPawnViewRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticalGroupsSettings.DoPawnViewReset();
					Event.current.Use();
				}
			}

			rightHalf.y += 50f;
			Widgets.Label(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), Strings.Pawn);

			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnCameraOffsetX = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnCameraOffsetX, -1f, 1f);
			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnCameraOffsetZ = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnCameraOffsetZ, -1f, 3f);
			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnScale = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnScale, 0.1f, 5f);

			rightHalf.y += 15f;
			Widgets.Label(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), Strings.Box);
			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnBoxHeight = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnBoxHeight, 1f, 300f);
			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnBoxWidth = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnBoxWidth, 5f, 300f);

			rightHalf.y += 25f;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(new Rect(xRightPos, rightHalf.y, 150, 25f), Strings.PawnRowCount + TacticalGroupsSettings.PawnRowCount);
			Vector2 pawnRowCountCheckBox = new Vector2(xRightPos + 150, rightHalf.y);
			Widgets.Checkbox(pawnRowCountCheckBox, ref TacticalGroupsSettings.OverridePawnRowCount);
			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnRowCount = (int)Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnRowCount, 1, 10);

			Text.Anchor = TextAnchor.UpperLeft;
			rightHalf.y += 50f;
			Widgets.Checkbox(rightHalf, ref TacticalGroupsSettings.DisplayWeapons);
			Widgets.Label(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), Strings.DisplayWeapons);

			rightHalf.y += 25f;
			TacticalGroupsSettings.WeaponPlacementOffset = (int)Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f),
				TacticalGroupsSettings.WeaponPlacementOffset, -200, 200);
			rightHalf.y += 15f;

			if (Widgets.RadioButtonLabeled(new Rect(xRightPos, rightHalf.y, 80f, 30f), Strings.WeaponModeShowDrafted, TacticalGroupsSettings.WeaponShowMode == WeaponShowMode.Drafted))
			{
				TacticalGroupsSettings.WeaponShowMode = WeaponShowMode.Drafted;
			}
			else if (Widgets.RadioButtonLabeled(new Rect(xRightPos + 90f, rightHalf.y, 100f, 30f), Strings.WeaponModeShowAlways, TacticalGroupsSettings.WeaponShowMode != WeaponShowMode.Drafted))
			{
				TacticalGroupsSettings.WeaponShowMode = WeaponShowMode.Always;
			}

			rightHalf.y += 35f;
			Widgets.Label(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), Strings.WeaponSizeScale);
			rightHalf.y += 25f;
			TacticalGroupsSettings.WeaponShowScale = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.WeaponShowScale, 0.1f, 5f);

			TacticUtils.TacticalColonistBar?.UpdateSizes();
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;

			TacticalGroupsMod.instance.WriteSettings();
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}
	}
}
