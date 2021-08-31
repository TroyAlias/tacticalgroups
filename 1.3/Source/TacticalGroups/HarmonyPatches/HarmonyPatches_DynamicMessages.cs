using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public static class HarmonyPatches_DynamicMessages
    {
        public static bool MessagesDoGUI(List<Message> ___liveMessages)
        {
            if (___liveMessages.Any())
            {
                int xOffset = (int)Messages.MessagesTopLeftStandard.x;
                int yOffset = (int)Messages.MessagesTopLeftStandard.y;
                Text.Font = GameFont.Small;

                int xOffsetStandard = 12;
                int yOffsetStandard = 12;

                if (Current.Game != null && Find.ActiveLesson.ActiveLessonVisible)
                {
                    yOffset += (int)Find.ActiveLesson.Current.MessagesYOffset;
                }

                // Getting the largest X of all the messages, for determining whether to move messages downwards or not
                float largestRectX = xOffsetStandard;
                for (int i = ___liveMessages.Count - 1; i >= 0; i--)
                {
                    Rect messageRect = ___liveMessages[i].CalculateRect(xOffset, yOffset);
                    largestRectX = Math.Max(largestRectX, xOffsetStandard + messageRect.x + messageRect.width);
                }

                // Function that checks whether a rect should be used, and if so, uses it
                void checkRect(Rect rect)
                {
                    if (largestRectX > rect.x)
                    {
                        yOffset = (int)Math.Max(yOffset, yOffsetStandard + rect.y + rect.height);
                    }
                }

                // Pawn draw locs
                List<Rect> drawLocs = TacticUtils.TacticalColonistBar?.DrawLocs;
                if (drawLocs != null)
                {
                    foreach (Rect rect in drawLocs)
                    {
                        checkRect(rect);
                    }
                }

                // Colonist groups
                List<ColonistGroup> colonistGroups = TacticUtils.AllGroups;
                if (colonistGroups != null)
                {
                    foreach (ColonistGroup colonistGroup in colonistGroups)
                    {
                        // Colonist group
                        Rect curRect = colonistGroup.curRect;
                        if (colonistGroup.isSubGroup)
                        {
                            curRect.width /= 2f;
                            curRect.height /= 2f;
                        }
                        checkRect(curRect);

                        // Colonist group name
                        if (!colonistGroup.isSubGroup && !colonistGroup.bannerModeEnabled && !colonistGroup.hideGroupIcon)
                        {
                            float groupNameHeight = Text.CalcHeight(colonistGroup.curGroupName, (float)colonistGroup.groupBanner.width);
                            checkRect(new Rect(curRect.x, curRect.y + curRect.height, curRect.width, groupNameHeight));
                        }

                        // Colonist group pawn dots
                        if (!colonistGroup.hidePawnDots)
                        {
                            List<PawnDot> pawnDots = colonistGroup.GetPawnDots(curRect);
                            if (pawnDots.Count > 0)
                            {
                                foreach (PawnDot pawnDot in pawnDots)
                                {
                                    checkRect(pawnDot.rect);
                                }
                            }
                        }

                        // Colonist group pawn rows
                        if (colonistGroup.pawnWindowIsActive || colonistGroup.showPawnIconsRightClickMenu || colonistGroup.ShowExpanded)
                        {
                            foreach (KeyValuePair<Pawn, Rect> pawnRect in colonistGroup.pawnRects)
                            {
                                checkRect(pawnRect.Value);
                            }
                        }
                    }
                }

                void checkWindow(Window window)
                {
                    if (window != null)
                    {
                        checkRect(window.windowRect);
                    }
                }

                checkWindow(Find.WindowStack.WindowOfType<MainFloatMenu>()); // Colonist group right click menu
                checkWindow(Find.WindowStack.WindowOfType<WorkMenu>()); // Colonist group [right click > work] menu
                checkWindow(Find.WindowStack.WindowOfType<OrderMenu>()); // Colonist group [right click > orders] menu
                checkWindow(Find.WindowStack.WindowOfType<ManageMenu>()); // Colonist group [right click > manage] menu
                checkWindow(Find.WindowStack.WindowOfType<OptionsSlideMenu>()); // Colonist group [right click > manage] options slide menu
                checkWindow(Find.WindowStack.WindowOfType<IconMenu>()); // Colonist group [right click > manage > icon] menu
                checkWindow(Find.WindowStack.WindowOfType<SortMenu>()); // Colonist group [right click > manage > sort] menu
                checkWindow(Find.WindowStack.WindowOfType<ManagementMenu>()); // Colonist group [right click > manage > management] menu
                checkWindow(Find.WindowStack.WindowOfType<PrisonerMenu>()); // Colonist group [right click > manage > prisoner menu] menu
                checkWindow(Find.WindowStack.WindowOfType<AnimalMenu>()); // Colonist group [right click > manage > animal menu] menu
                checkWindow(Find.WindowStack.WindowOfType<GuestMenu>()); // Colonist group [right click > manage > guest menu] menu
                checkWindow(Find.WindowStack.WindowOfType<PresetMenu>()); // Colonist group [right click > manage > preset] menu
                                                                          // Display the messages like normal
                for (int i = ___liveMessages.Count - 1; i >= 0; i--)
                {
                    ___liveMessages[i].Draw(xOffset, yOffset);
                    yOffset += 26;
                }

                return false;
            }
            return true;
        }
    }
}
