using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TacticalGroups
{
    public class OptionsSlideMenu : TieredFloatMenu
    {
        protected override Vector2 InitialPositionShift => new Vector2(-304f, 165f);
        protected override Vector2 InitialFloatOptionPositionShift => new Vector2(backgroundTexture.width / 10, 25f);
        public OptionsSlideMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
                : base(parentWindow, colonistGroup, originRect, backgroundTexture)
        {

        }

        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);
            Rect colonyHideButtonRect = new Rect(rect.x + 13, rect.y + 13, Textures.ColonyHideButton.width, Textures.ColonyHideButton.height);
            if (colonistGroup.isColonyGroup || colonistGroup.isTaskForce)
            {
                GUI.DrawTexture(colonyHideButtonRect, Textures.ColonyHideButton);
                TooltipHandler.TipRegion(colonyHideButtonRect, Strings.GroupHideOptionsTooltip);
                if (colonistGroup.hideGroupIcon)
                {
                    GUI.DrawTexture(colonyHideButtonRect, Textures.ManageOptionsX);
                }
                if (Mouse.IsOver(colonyHideButtonRect))
                {
                    GUI.DrawTexture(colonyHideButtonRect, Textures.RescueTendHover);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                    {
                        bool value = !colonistGroup.hideGroupIcon;
                        colonistGroup.hideGroupIcon = value;
                        if (colonistGroup is ColonyGroup colonyGroup)
                        {
                            foreach (PawnGroup subGroup in TacticUtils.GetAllSubGroupFor(colonyGroup))
                            {
                                subGroup.hideGroupIcon = value;
                            }
                        }
                        TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    }
                }
            }
            else if (colonistGroup.isPawnGroup && colonistGroup is PawnGroup pawnGroup)
            {
                Rect subgroupButton = new Rect(colonyHideButtonRect);
                GUI.DrawTexture(subgroupButton, Textures.SubGroupButton);
                if (colonistGroup.isSubGroup)
                {
                    GUI.DrawTexture(subgroupButton, Textures.ManageOptionsX);
                }
                TooltipHandler.TipRegion(subgroupButton, Strings.HidePawnGroupOptionsTooltip);
                if (Mouse.IsOver(subgroupButton))
                {
                    GUI.DrawTexture(subgroupButton, Textures.RescueTendHover);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                    {
                        if (pawnGroup.isSubGroup)
                        {
                            pawnGroup.ConvertToPawnGroup();
                        }
                        else
                        {
                            pawnGroup.ConvertToSubGroup();
                        }
                        pawnGroup.ResetDrawOptions();
                        CloseAllWindows();
                        TacticUtils.TacticalColonistBar.MarkColonistsDirty();
                        TacticDefOf.TG_SubGroupSFX.PlayOneShotOnCamera();
                    }
                }
            }

            Rect hidePawnDotsRect = new Rect(colonyHideButtonRect.xMax + 10, colonyHideButtonRect.y, Textures.PawnDotsButton.width, Textures.PawnDotsButton.height);
            GUI.DrawTexture(hidePawnDotsRect, Textures.PawnDotsButton);
            TooltipHandler.TipRegion(hidePawnDotsRect, Strings.HideGroupPawnDotsOptionsTooltip);
            if (colonistGroup.hidePawnDots)
            {
                GUI.DrawTexture(hidePawnDotsRect, Textures.ManageOptionsX);
            }
            if (Mouse.IsOver(hidePawnDotsRect))
            {
                GUI.DrawTexture(hidePawnDotsRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    colonistGroup.hidePawnDots = !colonistGroup.hidePawnDots;
                }
            }

            Rect hideLifeOverlayRect = new Rect(colonyHideButtonRect.x, colonyHideButtonRect.yMax + 14, Textures.GroupOverlayButton.width, Textures.GroupOverlayButton.height);
            GUI.DrawTexture(hideLifeOverlayRect, Textures.GroupOverlayButton);
            if (colonistGroup.hideLifeOverlay)
            {
                GUI.DrawTexture(hideLifeOverlayRect, Textures.ManageOptionsX);
            }
            TooltipHandler.TipRegion(hideLifeOverlayRect, Strings.HideGroupHealthAlertOverlayOptionsTooltip);
            if (Mouse.IsOver(hideLifeOverlayRect))
            {
                GUI.DrawTexture(hideLifeOverlayRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    colonistGroup.hideLifeOverlay = !colonistGroup.hideLifeOverlay;
                }
            }

            Rect hideWeaponOverlayRect = new Rect(colonyHideButtonRect.xMax + 10, hideLifeOverlayRect.y, Textures.ShowWeaponButton.width, Textures.ShowWeaponButton.height);
            GUI.DrawTexture(hideWeaponOverlayRect, Textures.ShowWeaponButton);
            if (colonistGroup.hideWeaponOverlay)
            {
                GUI.DrawTexture(hideWeaponOverlayRect, Textures.ManageOptionsX);
            }
            TooltipHandler.TipRegion(hideWeaponOverlayRect, Strings.HideWeaponOverlayOptionsTooltip);
            if (Mouse.IsOver(hideWeaponOverlayRect))
            {
                GUI.DrawTexture(hideWeaponOverlayRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    colonistGroup.hideWeaponOverlay = !colonistGroup.hideWeaponOverlay;
                }
            }

            Rect presetButtonRect = new Rect(hideLifeOverlayRect.x + 5, hideLifeOverlayRect.yMax + 17, Textures.PresetButton.width, Textures.PresetButton.height);
            GUI.DrawTexture(presetButtonRect, Textures.PresetButton);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(presetButtonRect, Strings.PresetLabel);
            Text.Anchor = TextAnchor.UpperLeft;

            TooltipHandler.TipRegion(presetButtonRect, Strings.PresetMenuOverlayOptionsTooltip);
            if (Mouse.IsOver(presetButtonRect))
            {
                GUI.DrawTexture(presetButtonRect, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
                    TieredFloatMenu presetMenu = childWindows?.FirstOrDefault(x => x is PresetMenu);
                    if (presetMenu != null)
                    {
                        presetMenu.Close();
                        childWindows.Remove(presetMenu);
                    }
                    else
                    {
                        TieredFloatMenu floatMenu = new PresetMenu(this, colonistGroup, windowRect, Textures.PresetMenu);
                        OpenNewMenu(floatMenu);
                    }
                }
            }

            //var banishPawnRect = new Rect(hideWeaponOverlayRect.x, hideWeaponOverlayRect.yMax + 14, Textures.BanishPawnButton.width, Textures.BanishPawnButton.height);
            //if (this.colonistGroup.isColonyGroup || this.colonistGroup.isTaskForce)
            //{
            //      GUI.DrawTexture(banishPawnRect, Textures.BanishPawnButton);
            //      TooltipHandler.TipRegion(banishPawnRect, Strings.BanishPawnTooltip);
            //      if (Mouse.IsOver(banishPawnRect))
            //      {
            //              GUI.DrawTexture(banishPawnRect, Textures.RescueTendHover);
            //              if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
            //              {
            //                      foreach (var pawn in Find.Selector.SelectedPawns)
            //            {
            //                              this.colonistGroup.Disband(pawn);
            //            }
            //                      TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
            //              }
            //      }
            //}
        }
    }
}