﻿using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System;
using Verse.Sound;
using Verse.AI;
using System.Linq;

namespace TacticalGroups 
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("Troy_Alias.TacticalGroups");

            harmony.Patch(AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.ColonistBarOnGUI), null, null), 
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.ColonistBarOnGUI)));
            
            harmony.Patch(AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.MarkColonistsDirty), null, null), 
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.MarkColonistsDirty)));
            
            harmony.Patch(AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.GetColonistsInOrder), null, null), 
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.GetColonistsInOrder)));
            
            harmony.Patch(AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.MapColonistsOrCorpsesInScreenRect), null, null), 
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.MapColonistsOrCorpsesInScreenRect)));
            
            harmony.Patch(AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.CaravanMembersCaravansInScreenRect), null, null), 
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.CaravanMembersCaravansInScreenRect)));
            
            harmony.Patch(AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.ColonistOrCorpseAt), null, null), 
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.ColonistOrCorpseAt)));
            
            harmony.Patch(AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.CaravanMemberCaravanAt), null, null), 
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.CaravanMemberCaravanAt)));
            
            harmony.Patch(AccessTools.Method(typeof(ColonistBar), nameof(ColonistBar.Highlight), null, null), 
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Highlight)));
            
            harmony.Patch(AccessTools.Method(typeof(ReorderableWidget), nameof(ReorderableWidget.ReorderableWidgetOnGUI_AfterWindowStack), null, null),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.ReorderableWidgetOnGUI_AfterWindowStack)));

            harmony.Patch(AccessTools.Method(typeof(CaravanExitMapUtility), nameof(CaravanExitMapUtility.ExitMapAndCreateCaravan), parameters: new Type[]
            {
                typeof(IEnumerable<Pawn>),
                typeof(Faction),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(bool)
            }), postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.ExitMapAndCreateCaravan)));

            harmony.Patch(AccessTools.Method(typeof(CaravanEnterMapUtility), nameof(CaravanEnterMapUtility.Enter), parameters: new Type[]
            {
                typeof(Caravan),
                typeof(Map),
                typeof(Func<Pawn, IntVec3>),
                typeof(CaravanDropInventoryMode),
                typeof(bool)
            }), prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.CaravanEnter)));

            harmony.Patch(AccessTools.Method(typeof(Caravan), "Notify_PawnAdded", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), "Notify_PawnRemoved", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), "PostAdd", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), "PostRemove", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Game), "AddMap", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn), "SetFaction", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "SetFaction_Postfix", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Window), "Notify_ResolutionChanged", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "IsPlayingDirty_Postfix", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Game), "DeinitAndRemoveMap", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "IsPlayingDirty_Postfix", null), null, null);
            
            harmony.Patch(AccessTools.Method(typeof(Pawn), "SpawnSetup", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "Pawn_SpawnSetup_Postfix", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn), "Destroy", null, null), prefix: new HarmonyMethod(typeof(HarmonyPatches), "Pawn_Destroy_Prefix", null), null, null);

            harmony.Patch(AccessTools.PropertySetter(typeof(Game), "CurrentMap"), null, postfix: new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null);

            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "Notify_Resurrected", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(CameraJumper), "TryJumpInternal", new Type[]
            {
                typeof(Thing)
            }, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(WorldCameraDriver), "JumpTo", new Type[]
            {
                typeof(Vector3)
            }, null), new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null, null);

            harmony.Patch(AccessTools.Method(typeof(MainButtonWorker_ToggleWorld), "Activate", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(CameraJumper), "TryHideWorld", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);

            harmony.Patch(AccessTools.Method(typeof(Pawn_JobTracker), "EndCurrentJob", null, null), 
                new HarmonyMethod(typeof(HarmonyPatches),  "EndCurrentJobPrefix", null), 
                new HarmonyMethod(typeof(HarmonyPatches),  "EndCurrentJobPostfix", null), null, null);

            harmony.Patch(AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "PawnTableOnGUI", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(WorldObject), "Destroy", null, null), new HarmonyMethod(typeof(HarmonyPatches), "Caravan_Destroy_Prefix", null), null, null, null);

        }
        public static Pawn curClickedColonist;
        public static void ReorderableWidgetOnGUI_AfterWindowStack(bool ___released, bool ___dragBegun, int ___draggingReorderable)
        {
            if (___released && ___dragBegun && curClickedColonist != null)
            {
                TacticUtils.TacticalColonistBar.TryDropColonist(curClickedColonist);
                curClickedColonist = null;
            }
        }
        public static bool ColonistBarOnGUI()
        {
            TacticUtils.TacticalColonistBar.ColonistBarOnGUI();
            return false;
        }
        public static void MarkColonistsDirty()
        {
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public static bool MapColonistsOrCorpsesInScreenRect(ref List<Thing> __result, Rect rect)
        {
            __result = TacticUtils.TacticalColonistBar.MapColonistsOrCorpsesInScreenRect(rect);
            return false;
        }

        public static bool CaravanMembersCaravansInScreenRect(ref List<Caravan> __result, Rect rect)
        {
            __result = TacticUtils.TacticalColonistBar.CaravanMembersCaravansInScreenRect(rect);
            return false;
        }

        public static bool ColonistOrCorpseAt(ref Thing __result, Vector2 pos)
        {
            __result = TacticUtils.TacticalColonistBar.ColonistOrCorpseAt(pos);
            var mainFloatMenu = Find.WindowStack.WindowOfType<MainFloatMenu>();
            if (mainFloatMenu != null)
            {
                var window = Find.WindowStack.GetWindowAt(UI.MousePositionOnUIInverted);
                if (!(window is TieredFloatMenu))
                {
                    mainFloatMenu.CloseAllWindows();
                }
            }
            return false;
        }

        public static bool CaravanMemberCaravanAt(ref Caravan __result, Vector2 at)
        {
            __result = TacticUtils.TacticalColonistBar.CaravanMemberCaravanAt(at);
            return false;
        }

        public static bool GetColonistsInOrder(ref List<Pawn> __result)
        {
            __result = TacticUtils.TacticalColonistBar.GetColonistsInOrder();
            return false;
        }

        public static bool Highlight(Pawn pawn)
        {
            TacticUtils.TacticalColonistBar.Highlight(pawn);
            return false;
        }

        private static void SetFaction_Postfix(Pawn __instance)
        {
            if (__instance.Spawned && __instance.RaceProps.Humanlike) 
            {
                if (__instance.FactionOrExtraMiniOrHomeFaction == Faction.OfPlayer)
                {
                    TacticUtils.TacticalGroups.CreateOrJoinColony(new List<Pawn> { __instance }, __instance.Map);
                }
                else if(__instance.TryGetGroups(out HashSet<ColonistGroup> groups))
                {
                    var groupsList = groups.ToList();
                    for (int num = groupsList.Count - 1; num >= 0; num--)
                    {
                        if (groupsList[num].pawns.Contains(__instance))
                        {
                            groupsList[num].Disband(__instance);
                        }
                    }
                }
            }

            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        private static void EntriesDirty()
        {
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        private static void IsPlayingDirty_Postfix()
        {
            if (Current.ProgramState == ProgramState.Playing)
            {
                TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            }
        }

        private static void ExitMapAndCreateCaravan(Caravan __result, IEnumerable<Pawn> pawns, Faction faction, int exitFromTile, int directionTile, int destinationTile, bool sendMessage = true)
        {
            TacticUtils.TacticalGroups.AddCaravanGroup(__result);
        }

        private static void Caravan_Destroy_Prefix(WorldObject __instance)
        {
            if (__instance is Caravan caravan)
            {
                TacticUtils.TacticalGroups.RemoveCaravanGroup(caravan);
                if (caravan.PawnsListForReading.Count > 0 && caravan.PawnsListForReading.First().Map != null)
                {
                    TacticUtils.TacticalGroups.CreateOrJoinColony(caravan.PawnsListForReading, caravan.PawnsListForReading.First().Map);
                }
            }
        }

        private static void CaravanEnter(Caravan caravan, Map map, Func<Pawn, IntVec3> spawnCellGetter, CaravanDropInventoryMode dropInventoryMode = CaravanDropInventoryMode.DoNotDrop, 
            bool draftColonists = false)
        {
            TacticUtils.TacticalGroups.RemoveCaravanGroup(caravan);
            TacticUtils.TacticalGroups.CreateOrJoinColony(caravan.PawnsListForReading, map);
        }

        private static void Pawn_SpawnSetup_Postfix(Pawn __instance)
        {
            if (__instance.Spawned && __instance.FactionOrExtraMiniOrHomeFaction == Faction.OfPlayer && __instance.RaceProps.Humanlike)
            {
                TacticUtils.TacticalGroups.CreateOrJoinColony(new List<Pawn> { __instance }, __instance.Map);
            }
        }

        private static void Pawn_Destroy_Prefix(Pawn __instance)
        {
            for (int num = TacticUtils.TacticalGroups.pawnGroups.Count - 1; num >= 0; num--)
            {
                var group = TacticUtils.TacticalGroups.pawnGroups[num];
                group.Disband(__instance);
            }

            var caravanKeysToRemove = new List<Caravan>();
            foreach (var group in TacticUtils.TacticalGroups.caravanGroups)
            {
                group.Value.pawnIcons.Remove(__instance);
                group.Value.pawns.Remove(__instance);
                if (group.Value.pawns.Count == 0)
                {
                    caravanKeysToRemove.Add(group.Key);
                }
            }
            
            foreach (var key in caravanKeysToRemove)
            {
                TacticUtils.TacticalGroups.caravanGroups.Remove(key);
            }
            
            var colonyKeysToRemove = new List<Map>();
            foreach (var group in TacticUtils.TacticalGroups.colonyGroups)
            {
                group.Value.pawnIcons.Remove(__instance);
                group.Value.pawns.Remove(__instance);
                if (group.Value.pawns.Count == 0)
                {
                    colonyKeysToRemove.Add(group.Key);
                }
            }

            foreach (var key in colonyKeysToRemove)
            {
                TacticUtils.TacticalGroups.colonyGroups.Remove(key);
            }

            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }
        private static void EndCurrentJobPrefix(Pawn_JobTracker __instance, Pawn ___pawn, JobCondition condition, ref bool startNewJob, out Dictionary<WorkType, WorkState> __state, bool canReturnToPool = true)
        {
            __state = null;
            if (___pawn.RaceProps.Humanlike && ___pawn.Faction == Faction.OfPlayer && !___pawn.Drafted)
            {
                if (___pawn.TryGetGroups(out HashSet<ColonistGroup> groups))
                {
                    foreach (var group in groups)
                    {
                        if (group.activeWorkTypes?.Count > 0)
                        {
                            if (group.activeWorkTypes.Where(x => x.Value == WorkState.Active).Count() == group.activeWorkTypes.Count && (!CanWork(___pawn) || !CanWorkActive(___pawn)))
                            {
                                return;
                            }
                            __state = group.activeWorkTypes;
                            startNewJob = false;
                            return;
                        }
                    }
                }
            }
        }

        private static void EndCurrentJobPostfix(Pawn_JobTracker __instance, Pawn ___pawn, JobCondition condition, ref bool startNewJob, Dictionary<WorkType, WorkState> __state, bool canReturnToPool = true)
        {
            if (__state?.Count > 0)
            {
                foreach (var state in __state.InRandomOrder())
                {
                    if (state.Value != WorkState.Inactive && CanWork(___pawn))
                    {
                        if (state.Value == WorkState.Active && !CanWorkActive(___pawn))
                        {
                            continue;
                        }
                        var curJob = ___pawn.CurJob;
                        WorkSearchUtility.SearchForWork(state.Key, new List<Pawn> { ___pawn });
                        if (curJob != ___pawn.CurJob)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static bool CanWorkActive(Pawn pawn)
        {
            if (pawn.needs.mood.CurLevel > pawn.mindState.mentalBreaker.BreakThresholdMinor)
            {
                return true;
            }
            return false;
        }
        private static bool CanWork(Pawn pawn)
        {
            if (pawn.needs.food?.CurLevel < 0.10)
            {
                return false;
            }
            if (pawn.needs.rest?.CurLevel < 0.10)
            {
                return false;
            }
            if (pawn.MentalState != null)
            {
                return false;
            }
            if (pawn.mindState.lastJobTag == JobTag.SatisfyingNeeds)
            {
                return false;
            }
            if (pawn.IsDownedOrIncapable() || pawn.IsSick() || pawn.IsShotOrBleeding())
            {
                return false;
            }
            return true;
        }

        public static void PawnTableOnGUI(Vector2 position, PawnTableDef ___def, List<float> ___cachedColumnWidths, Vector2 ___cachedSize, float ___cachedHeaderHeight, float ___cachedHeightNoScrollbar)
        {
            if (___def == PawnTableDefOf.Assign)
            {
                Rect outRect = new Rect((int)position.x, (int)position.y + (int)___cachedHeaderHeight, (int)___cachedSize.x, (int)___cachedSize.y - (int)___cachedHeaderHeight);
                Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, (int)___cachedHeightNoScrollbar - (int)___cachedHeaderHeight);
                
                var createGroupRect = new Rect(viewRect.x + 10, (outRect.y + outRect.height + 5), Textures.CreateGroupIcon.width, Textures.CreateGroupIcon.height);
                if (Mouse.IsOver(createGroupRect))
                {
                    GUI.DrawTexture(createGroupRect, Textures.CreateGroupIconHover);
                }
                else
                {
                    GUI.DrawTexture(createGroupRect, Textures.CreateGroupIcon);
                }
                TooltipHandler.TipRegion(createGroupRect, Strings.CreateGroupTooltip);
                TacticalColonistBar.HandleGroupingClicks(createGroupRect);
                Rect optionsGearRect = new Rect(createGroupRect.x + createGroupRect.width + 10f, createGroupRect.y + 5, Textures.OptionsGear.width, Textures.OptionsGear.height);
                if (Mouse.IsOver(optionsGearRect))
                {
                    GUI.DrawTexture(optionsGearRect, Textures.OptionsGearHover);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        TieredFloatMenu floatMenu = new OptionsMenu(null, null, optionsGearRect, Textures.OptionsMenu);
                        Find.WindowStack.Add(floatMenu);
                        floatMenu.windowRect.y = UI.screenHeight - (floatMenu.windowRect.height + 100);
                    }
                }
                else
                {
                    GUI.DrawTexture(optionsGearRect, Textures.OptionsGear);
                }
                TooltipHandler.TipRegion(optionsGearRect, Strings.OptionsGearTooltip);
            }

        }
    }
}