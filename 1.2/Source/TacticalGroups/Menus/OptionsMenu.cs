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
		protected override Vector2 InitialPositionShift => new Vector2(-45f, 35f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);
		public OptionsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Text.Anchor = TextAnchor.LowerLeft;
			Text.Font = GameFont.Small;
			Vector2 topLeftHalf = new Vector2(rect.x + ((rect.width / 2f) - 50f), rect.y + 25f);

			var showAllColonistsRect = new Rect(rect.x + 10, topLeftHalf.y, Textures.MenuButton.width, 25f);
			GUI.DrawTexture(showAllColonistsRect, Textures.MenuButton);
			if (Mouse.IsOver(showAllColonistsRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticUtils.ShowAllColonists();
					Event.current.Use();
				}
			}
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.ShowAllColonists);
			
			topLeftHalf.y += 25f;
			Widgets.Checkbox(topLeftHalf, ref TacticalGroupsSettings.HidePawnsWhenOffMap);
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.HidePawnsWhenOffMap);
			
			topLeftHalf.y += 25f;
			Widgets.Checkbox(topLeftHalf, ref TacticalGroupsSettings.HideGroups);
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.HideGroups);
			
			topLeftHalf.y += 45f;
			Widgets.Checkbox(topLeftHalf, ref TacticalGroupsSettings.DisplayFood);
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.DisplayFood);
			
			topLeftHalf.y += 25f;
			Widgets.Checkbox(topLeftHalf, ref TacticalGroupsSettings.DisplayRest);
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.DisplayRest);
			
			topLeftHalf.y += 25f;
			Widgets.Checkbox(topLeftHalf, ref TacticalGroupsSettings.DisplayHealth);
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.DisplayHealth);

			topLeftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.PawnNeedsSize + ": " + TacticalGroupsSettings.PawnNeedsWidth.ToString());
			topLeftHalf.y += 25f;
			TacticalGroupsSettings.PawnNeedsWidth = (int)Widgets.HorizontalSlider(new Rect(rect.x + 20, topLeftHalf.y, (rect.width / 2f) - 40, 25f), TacticalGroupsSettings.PawnNeedsWidth, 1f, 25f);

			topLeftHalf.y += 30f;
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.ColonistBarPositionY + ": " + (int)TacticalGroupsSettings.ColonistBarPositionY);
			topLeftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarPositionY = Widgets.HorizontalSlider(new Rect(rect.x + 20, topLeftHalf.y, (rect.width / 2f) - 40, 25f), TacticalGroupsSettings.ColonistBarPositionY, 0f, 100f);
			
			topLeftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.ColonistBarPositionX + ": " + (int)TacticalGroupsSettings.ColonistBarPositionX);
			topLeftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarPositionX = Widgets.HorizontalSlider(new Rect(rect.x + 20, topLeftHalf.y, (rect.width / 2f) - 40, 25f), 
				TacticalGroupsSettings.ColonistBarPositionX, -400f, 400f);

			topLeftHalf.y += 25f;
			Widgets.Label(new Rect(rect.x + 20, topLeftHalf.y, rect.width - 55f, 25f), Strings.ColonistBarSpacing + ": " + (int)TacticalGroupsSettings.ColonistBarSpacing);
			topLeftHalf.y += 25f;
			TacticalGroupsSettings.ColonistBarSpacing = Widgets.HorizontalSlider(new Rect(rect.x + 20, topLeftHalf.y, (rect.width / 2f) - 40, 25f), TacticalGroupsSettings.ColonistBarSpacing, 0f, 100f);


			Vector2 topRightHalf = new Vector2(topLeftHalf.x + ((rect.width / 2f) - 20f), rect.y + 25f);
			float xRightHalfPos = rect.x + 230;
			Widgets.Checkbox(topRightHalf, ref TacticalGroupsSettings.HideCreateGroup);
			Widgets.Label(new Rect(xRightHalfPos, topRightHalf.y, rect.width - 55f, 25f), Strings.HideCreateGroup);

			topRightHalf.y += 25f;
			Widgets.Label(new Rect(xRightHalfPos, topRightHalf.y, rect.width - 55f, 25f), Strings.PawnScale + ": " + TacticalGroupsSettings.PawnScale.ToStringDecimalIfSmall());
			topRightHalf.y += 25f;
			TacticalGroupsSettings.PawnScale = Widgets.HorizontalSlider(new Rect(xRightHalfPos, topRightHalf.y, (rect.width / 2f) - 40, 25f), TacticalGroupsSettings.PawnScale, 0.5f, 5f);


			topRightHalf.y += 45f;
			Widgets.Checkbox(topRightHalf, ref TacticalGroupsSettings.DisplayColorBars);
			Widgets.Label(new Rect(xRightHalfPos, topRightHalf.y, rect.width - 55f, 25f), Strings.DisplayColorBars);

			topRightHalf.y += 25f;

			if (Widgets.RadioButtonLabeled(new Rect(xRightHalfPos, topRightHalf.y, 80f, 30f), Strings.ColorBarModeDefault, TacticalGroupsSettings.ColorBarMode == ColorBarMode.Default))
			{
				TacticalGroupsSettings.ColorBarMode = ColorBarMode.Default;
			}
			else if (Widgets.RadioButtonLabeled(new Rect(xRightHalfPos + 90f, topRightHalf.y, 100f, 30f), Strings.ColorBarModeExtended, TacticalGroupsSettings.ColorBarMode != ColorBarMode.Default))
			{
				TacticalGroupsSettings.ColorBarMode = ColorBarMode.Extended;
			}

			topRightHalf.y += 25f;
			Widgets.Checkbox(topRightHalf, ref TacticalGroupsSettings.DisplayWeapons);
			Widgets.Label(new Rect(xRightHalfPos, topRightHalf.y, rect.width - 55f, 25f), Strings.DisplayWeapons);
			
			topRightHalf.y += 25f;
			TacticalGroupsSettings.WeaponPlacementOffset = (int)Widgets.HorizontalSlider(new Rect(xRightHalfPos, topRightHalf.y, (rect.width / 2f) - 40, 25f), TacticalGroupsSettings.WeaponPlacementOffset, 0, 100);
			topRightHalf.y += 15f;

			if (Widgets.RadioButtonLabeled(new Rect(xRightHalfPos, topRightHalf.y, 80f, 30f), Strings.WeaponModeShowDrafted, TacticalGroupsSettings.WeaponShowMode == WeaponShowMode.Drafted))
			{
				TacticalGroupsSettings.WeaponShowMode = WeaponShowMode.Drafted;
			}
			else if (Widgets.RadioButtonLabeled(new Rect(xRightHalfPos + 90f, topRightHalf.y, 100f, 30f), Strings.WeaponModeShowAlways, TacticalGroupsSettings.WeaponShowMode != WeaponShowMode.Drafted))
			{
				TacticalGroupsSettings.WeaponShowMode = WeaponShowMode.Always;
			}

			topRightHalf.y += 40f;

			Widgets.Label(new Rect(xRightHalfPos, topRightHalf.y, rect.width - 55f, 25f), Strings.GroupRowCount + ": " + TacticalGroupsSettings.GroupRowCount.ToString());
			topRightHalf.y += 25f;
			TacticalGroupsSettings.GroupRowCount = (int)Widgets.HorizontalSlider(new Rect(xRightHalfPos, topRightHalf.y, (rect.width / 2f) - 40, 25f), TacticalGroupsSettings.GroupRowCount, 1f, 12f);
			topRightHalf.y += 25f;
			Widgets.Label(new Rect(xRightHalfPos, topRightHalf.y, rect.width - 55f, 25f), Strings.GroupScale + ": " + TacticalGroupsSettings.GroupScale.ToStringDecimalIfSmall());
			topRightHalf.y += 25f;
			TacticalGroupsSettings.GroupScale = Widgets.HorizontalSlider(new Rect(xRightHalfPos, topRightHalf.y, (rect.width / 2f) - 40, 25f), TacticalGroupsSettings.GroupScale, 0.5f, 2f);

			topRightHalf.y += 25f;
			Widgets.Checkbox(topRightHalf, ref TacticalGroupsSettings.DisableLabelBackground);
			Text.Font = GameFont.Tiny;
			Widgets.Label(new Rect(xRightHalfPos, topRightHalf.y, (rect.width / 3f) + 10, 35f), Strings.DisableLabelBackground);

			TacticUtils.TacticalColonistBar?.UpdateSizes();
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;

			TacticalGroupsMod.instance.WriteSettings();
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}
	}
}
