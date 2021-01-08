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
	public class OptionsSlideMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(-102f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);
		public OptionsSlideMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			var colonyHideButtonRect = new Rect(rect.x + 13, rect.y + 13, Textures.ColonyHideButton.width, Textures.ColonyHideButton.height);
			if (this.colonistGroup.isColonyGroup)
            {
				GUI.DrawTexture(colonyHideButtonRect, Textures.ColonyHideButton);
				TooltipHandler.TipRegion(colonyHideButtonRect, Strings.GroupHideOptionsTooltip);
				if (this.colonistGroup.hideGroupIcon)
                {
					GUI.DrawTexture(colonyHideButtonRect, Textures.ManageOptionsX);
                }
				if (Mouse.IsOver(colonyHideButtonRect))
				{
					GUI.DrawTexture(colonyHideButtonRect, Textures.RescueTendHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						this.colonistGroup.hideGroupIcon = !this.colonistGroup.hideGroupIcon;
					}
				}
			}


			var hidePawnDotsRect = new Rect(colonyHideButtonRect.xMax + 10, colonyHideButtonRect.y, Textures.PawnDotsButton.width, Textures.PawnDotsButton.height);
			GUI.DrawTexture(hidePawnDotsRect, Textures.PawnDotsButton);
			TooltipHandler.TipRegion(hidePawnDotsRect, Strings.HideGroupPawnDotsOptionsTooltip);
			if (this.colonistGroup.hidePawnDots)
			{
				GUI.DrawTexture(hidePawnDotsRect, Textures.ManageOptionsX);
			}
			if (Mouse.IsOver(hidePawnDotsRect))
			{
				GUI.DrawTexture(hidePawnDotsRect, Textures.RescueTendHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					this.colonistGroup.hidePawnDots = !this.colonistGroup.hidePawnDots;
				}
			}

			var hideLifeOverlayRect = new Rect(hidePawnDotsRect.x, hidePawnDotsRect.yMax + 5, Textures.GroupOverlayButton.width, Textures.GroupOverlayButton.height);
			GUI.DrawTexture(hideLifeOverlayRect, Textures.GroupOverlayButton);
			if (this.colonistGroup.hideLifeOverlay)
			{
				GUI.DrawTexture(hideLifeOverlayRect, Textures.ManageOptionsX);
			}
			TooltipHandler.TipRegion(hideLifeOverlayRect, Strings.HideGroupHealthAlertOverlayOptionsTooltip);
			if (Mouse.IsOver(hideLifeOverlayRect))
			{
				GUI.DrawTexture(hideLifeOverlayRect, Textures.RescueTendHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					this.colonistGroup.hideLifeOverlay = !this.colonistGroup.hideLifeOverlay;
				}
			}


			var hideWeaponOverlayRect = new Rect(rect.x + 13, rect.y + 13, Textures.ShowWeaponButton.width, Textures.ShowWeaponButton.height);
			if (!this.colonistGroup.isColonyGroup)
            {
				GUI.DrawTexture(hideWeaponOverlayRect, Textures.ShowWeaponButton);
				if (this.colonistGroup.hideWeaponOverlay)
				{
					GUI.DrawTexture(hideWeaponOverlayRect, Textures.ManageOptionsX);
				}
				TooltipHandler.TipRegion(hideWeaponOverlayRect, Strings.HideWeaponOverlayOptionsTooltip);
				if (Mouse.IsOver(hideWeaponOverlayRect))
				{
					GUI.DrawTexture(hideWeaponOverlayRect, Textures.RescueTendHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						this.colonistGroup.hideWeaponOverlay = !this.colonistGroup.hideWeaponOverlay;
					}
				}
			}
		}
	}
}
