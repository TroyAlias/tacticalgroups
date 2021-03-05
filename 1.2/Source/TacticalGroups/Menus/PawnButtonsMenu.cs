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
	public class PawnButtonsMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(-(originRect.width - 2), -31);
		public PawnButtonsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
            var prisonerButtonRect = new Rect(rect.x, rect.yMax - Textures.PrisonerButton.height, Textures.PrisonerButton.width, Textures.PrisonerButton.height);
            GUI.DrawTexture(prisonerButtonRect, Textures.PrisonerButton);
            TooltipHandler.TipRegion(prisonerButtonRect, Strings.PrisonerMenuTooltip);
            if (Mouse.IsOver(prisonerButtonRect))
            {
                GUI.DrawTexture(prisonerButtonRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    if (this.childWindows?.Any() ?? false)
                    {
                        for (int num = this.childWindows.Count - 1; num >= 0; num--)
                        {
                            if (this.childWindows[num] is PrisonerMenu)
                            {
                                this.childWindows[num].Close();
                            }
                            else
                            {
                                this.childWindows[num].Close();
                                TieredFloatMenu floatMenu = new PrisonerMenu(this, this.colonistGroup, windowRect, Textures.PrisonerMenu, Strings.Prisoners);
                                OpenNewMenu(floatMenu);
                            }
                        }
                    }
                    else
                    {
                        TieredFloatMenu floatMenu = new PrisonerMenu(this, this.colonistGroup, windowRect, Textures.PrisonerMenu, Strings.Prisoners);
                        OpenNewMenu(floatMenu);
                    }
                }
            }

            var animalButtonRect = new Rect(prisonerButtonRect.xMax + 5, rect.yMax - Textures.AnimalButton.height, Textures.AnimalButton.width, Textures.AnimalButton.height);
            GUI.DrawTexture(animalButtonRect, Textures.AnimalButton);
            TooltipHandler.TipRegion(animalButtonRect, Strings.AnimalMenuTooltip);
            if (Mouse.IsOver(animalButtonRect))
            {
                GUI.DrawTexture(animalButtonRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    if (this.childWindows?.Any() ?? false)
                    {
                        for (int num = this.childWindows.Count - 1; num >= 0; num--)
                        {
                            if (this.childWindows[num] is AnimalMenu)
                            {
                                this.childWindows[num].Close();
                            }
                            else
                            {
                                this.childWindows[num].Close();
                                TieredFloatMenu floatMenu = new AnimalMenu(this, this.colonistGroup, windowRect, Textures.AnimalMenu, Strings.Animals);
                                OpenNewMenu(floatMenu);
                            }
                        }
                    }
                    else
                    {
                        TieredFloatMenu floatMenu = new AnimalMenu(this, this.colonistGroup, windowRect, Textures.AnimalMenu, Strings.Animals);
                        OpenNewMenu(floatMenu);
                    }
                }
            }

            var guestsButtonRect = new Rect(animalButtonRect.xMax + 5, rect.yMax - Textures.GuestButton.height, Textures.GuestButton.width, Textures.GuestButton.height);
            GUI.DrawTexture(guestsButtonRect, Textures.GuestButton);
            TooltipHandler.TipRegion(guestsButtonRect, Strings.GuestMenuTooltip);
            if (Mouse.IsOver(guestsButtonRect))
            {
                GUI.DrawTexture(guestsButtonRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    if (this.childWindows?.Any() ?? false)
                    {
                        for (int num = this.childWindows.Count - 1; num >= 0; num--)
                        {
                            if (this.childWindows[num] is GuestMenu)
                            {
                                this.childWindows[num].Close();
                            }
                            else
                            {
                                this.childWindows[num].Close();
                                TieredFloatMenu floatMenu = new GuestMenu(this, this.colonistGroup, windowRect, Textures.GuestMenu, Strings.Guests);
                                OpenNewMenu(floatMenu);
                            }
                        }
                    }
                    else
                    {
                        TieredFloatMenu floatMenu = new GuestMenu(this, this.colonistGroup, windowRect, Textures.GuestMenu, Strings.Guests);
                        OpenNewMenu(floatMenu);
                    }
                }
            }
        }
	}
}
