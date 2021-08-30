using System.Linq;
using UnityEngine;
using Verse;
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
            Rect prisonerButtonRect = new Rect(rect.x, rect.yMax - Textures.PrisonerButton.height, Textures.PrisonerButton.width, Textures.PrisonerButton.height);
            GUI.DrawTexture(prisonerButtonRect, Textures.PrisonerButton);
            TooltipHandler.TipRegion(prisonerButtonRect, Strings.PrisonerMenuTooltip);
            if (Mouse.IsOver(prisonerButtonRect))
            {
                GUI.DrawTexture(prisonerButtonRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    if (childWindows?.Any() ?? false)
                    {
                        for (int num = childWindows.Count - 1; num >= 0; num--)
                        {
                            if (childWindows[num] is PrisonerMenu)
                            {
                                childWindows[num].Close();
                            }
                            else
                            {
                                childWindows[num].Close();
                                PrisonerMenu floatMenu = new PrisonerMenu(this, colonistGroup, windowRect, Textures.PrisonerMenu, Strings.Prisoners);
                                OpenNewMenu(floatMenu);
                            }
                        }
                    }
                    else
                    {
                        PrisonerMenu floatMenu = new PrisonerMenu(this, colonistGroup, windowRect, Textures.PrisonerMenu, Strings.Prisoners);
                        OpenNewMenu(floatMenu);
                    }
                }
            }

            Rect slaveButtonRect = new Rect(prisonerButtonRect.xMax + 5, rect.yMax - Textures.SlaveButton.height, Textures.SlaveButton.width, Textures.SlaveButton.height);
            GUI.DrawTexture(slaveButtonRect, Textures.SlaveButton);
            TooltipHandler.TipRegion(slaveButtonRect, Strings.SlaveMenuTooltip);
            if (Mouse.IsOver(slaveButtonRect))
            {
                GUI.DrawTexture(slaveButtonRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    if (childWindows?.Any() ?? false)
                    {
                        for (int num = childWindows.Count - 1; num >= 0; num--)
                        {
                            if (childWindows[num] is SlaveMenu)
                            {
                                childWindows[num].Close();
                            }
                            else
                            {
                                childWindows[num].Close();
                                SlaveMenu floatMenu = new SlaveMenu(this, colonistGroup, windowRect, Textures.SlaveMenu, Strings.Slaves);
                                OpenNewMenu(floatMenu);
                            }
                        }
                    }
                    else
                    {
                        SlaveMenu floatMenu = new SlaveMenu(this, colonistGroup, windowRect, Textures.SlaveMenu, Strings.Slaves);
                        OpenNewMenu(floatMenu);
                    }
                }
            }

            Rect animalButtonRect = new Rect(slaveButtonRect.xMax + 5, rect.yMax - Textures.AnimalButton.height, Textures.AnimalButton.width, Textures.AnimalButton.height);
            GUI.DrawTexture(animalButtonRect, Textures.AnimalButton);
            TooltipHandler.TipRegion(animalButtonRect, Strings.AnimalMenuTooltip);
            if (Mouse.IsOver(animalButtonRect))
            {
                GUI.DrawTexture(animalButtonRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    if (childWindows?.Any() ?? false)
                    {
                        for (int num = childWindows.Count - 1; num >= 0; num--)
                        {
                            if (childWindows[num] is AnimalMenu)
                            {
                                childWindows[num].Close();
                            }
                            else
                            {
                                childWindows[num].Close();
                                AnimalMenu floatMenu = new AnimalMenu(this, colonistGroup, windowRect, Textures.AnimalMenu, Strings.Animals);
                                OpenNewMenu(floatMenu);
                            }
                        }
                    }
                    else
                    {
                        AnimalMenu floatMenu = new AnimalMenu(this, colonistGroup, windowRect, Textures.AnimalMenu, Strings.Animals);
                        OpenNewMenu(floatMenu);
                    }
                }
            }

            Rect guestsButtonRect = new Rect(animalButtonRect.xMax + 5, rect.yMax - Textures.GuestButton.height, Textures.GuestButton.width, Textures.GuestButton.height);
            GUI.DrawTexture(guestsButtonRect, Textures.GuestButton);
            TooltipHandler.TipRegion(guestsButtonRect, Strings.GuestMenuTooltip);
            if (Mouse.IsOver(guestsButtonRect))
            {
                GUI.DrawTexture(guestsButtonRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    if (childWindows?.Any() ?? false)
                    {
                        for (int num = childWindows.Count - 1; num >= 0; num--)
                        {
                            if (childWindows[num] is GuestMenu)
                            {
                                childWindows[num].Close();
                            }
                            else
                            {
                                childWindows[num].Close();
                                GuestMenu floatMenu = new GuestMenu(this, colonistGroup, windowRect, Textures.GuestMenu, Strings.Guests);
                                OpenNewMenu(floatMenu);
                            }
                        }
                    }
                    else
                    {
                        GuestMenu floatMenu = new GuestMenu(this, colonistGroup, windowRect, Textures.GuestMenu, Strings.Guests);
                        OpenNewMenu(floatMenu);
                    }
                }
            }
        }
    }
}
