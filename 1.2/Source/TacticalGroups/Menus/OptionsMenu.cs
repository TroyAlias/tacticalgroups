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
	public class OptionsMenu : TieredFloatMenu
	{
		//protected override Vector2 InitialPositionShift => new Vector2(-45f, 35f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);
		public OptionsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}

		protected override void SetInitialSizeAndPosition()
		{
			windowRect = new Rect(((float)UI.screenWidth - InitialSize.x) / 2f, ((float)UI.screenHeight - InitialSize.y) / 2f, InitialSize.x, InitialSize.y);
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
			Vector2 leftHalf = new Vector2(rect.x + checkBoxesWidth, rect.y + 25f);

			var showAllColonistsRect = new Rect(rect.x + 10, leftHalf.y, Textures.MenuButton.width, 25f);
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
			
			leftHalf.y += 25f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.DisplayRest);
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.DisplayRest);
			
			leftHalf.y += 25f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.DisplayHealth);
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.DisplayHealth);

			leftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.PawnNeedsSize + ": " + TacticalGroupsSettings.PawnNeedsWidth.ToString());
			leftHalf.y += 25f;
			TacticalGroupsSettings.PawnNeedsWidth = (int)Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnNeedsWidth, 1f, 25f);

			leftHalf.y += 30f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.ColonistBarPositionY + ": " + (int)TacticalGroupsSettings.ColonistBarPositionY);
			leftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarPositionY = Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.ColonistBarPositionY, 0f, 100f);
			
			leftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.ColonistBarPositionX + ": " + (int)TacticalGroupsSettings.ColonistBarPositionX);
			leftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarPositionX = Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), 
				TacticalGroupsSettings.ColonistBarPositionX, -400f, 400f);

			leftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), Strings.ColonistBarSpacing + ": " + (int)TacticalGroupsSettings.ColonistBarSpacing);
			leftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarSpacing = Widgets.HorizontalSlider(new Rect(rect.x + 20, leftHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.ColonistBarSpacing, 0f, 100f);

			leftHalf.y += 45f;
			Widgets.Checkbox(leftHalf, ref TacticalGroupsSettings.DisableLabelBackground);
			var disableLabelBackgroundRect = new Rect(rect.x + 20, leftHalf.y, textFieldWidth - 30, 45f);
			Text.Font = GameFont.Tiny;
			Widgets.Label(disableLabelBackgroundRect, Strings.DisableLabelBackground);
			Text.Font = GameFont.Small;

			Vector2 middle = new Vector2(leftHalf.x + checkBoxesWidth, rect.y + 25f);
			float xMiddlePos = rect.x + 230;
			Widgets.Checkbox(middle, ref TacticalGroupsSettings.HideCreateGroup);
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.HideCreateGroup);
			
			middle.y += 95f;
			Widgets.Checkbox(middle, ref TacticalGroupsSettings.DisplayColorBars);
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.DisplayColorBars);
			
			middle.y += 25f;
			var colorBarDefaultRect = new Rect(xMiddlePos, middle.y, 80f, 30f);
			var colorBarExtendedRect = new Rect(xMiddlePos + 90f, middle.y, 100f, 30f);
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

			middle.y += 35f;
			Widgets.Checkbox(middle, ref TacticalGroupsSettings.DisplayBreakRiskOverlay);
			Widgets.Label(new Rect(xMiddlePos, middle.y, textFieldWidth, 25f), Strings.DisplayBreakRiskOverlay);


			middle.y += 70f;
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
			
			middle.y += 50f;
			var resetButtonRect = new Rect(xMiddlePos, middle.y, Textures.MenuButton.width, 25f);
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
			var resetPawnViewRect = new Rect(xRightPos, rightHalf.y, Textures.MenuButton.width, 25f);
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
			TacticalGroupsSettings.PawnCameraOffsetZ = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnCameraOffsetZ, -1f, 1f);
			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnScale = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnScale, 0f, 5f);

			rightHalf.y += 15f;
			Widgets.Label(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), Strings.Box);
			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnBoxHeight = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnBoxHeight, 0f, 300f);
			rightHalf.y += 25f;
			TacticalGroupsSettings.PawnBoxWidth = Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.PawnBoxWidth, 0f, 300f);

			Text.Anchor = TextAnchor.UpperLeft;
			rightHalf.y += 50f;
			Widgets.Checkbox(rightHalf, ref TacticalGroupsSettings.DisplayWeapons);
			Widgets.Label(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), Strings.DisplayWeapons);

			rightHalf.y += 25f;
			TacticalGroupsSettings.WeaponPlacementOffset = (int)Widgets.HorizontalSlider(new Rect(xRightPos, rightHalf.y, textFieldWidth, 25f), TacticalGroupsSettings.WeaponPlacementOffset, 0, 100);
			rightHalf.y += 15f;
			
			if (Widgets.RadioButtonLabeled(new Rect(xRightPos, rightHalf.y, 80f, 30f), Strings.WeaponModeShowDrafted, TacticalGroupsSettings.WeaponShowMode == WeaponShowMode.Drafted))
			{
				TacticalGroupsSettings.WeaponShowMode = WeaponShowMode.Drafted;
			}
			else if (Widgets.RadioButtonLabeled(new Rect(xRightPos + 90f, rightHalf.y, 100f, 30f), Strings.WeaponModeShowAlways, TacticalGroupsSettings.WeaponShowMode != WeaponShowMode.Drafted))
			{
				TacticalGroupsSettings.WeaponShowMode = WeaponShowMode.Always;
			}

			TacticUtils.TacticalColonistBar?.UpdateSizes();
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;

			TacticalGroupsMod.instance.WriteSettings();
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}
	}
}
