using System.Reflection;
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
using System.Reflection.Emit;
using System.Diagnostics;

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
            
            
            harmony.Patch(AccessTools.Method(typeof(SettleInExistingMapUtility), nameof(SettleInExistingMapUtility.Settle)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Settle)));
            
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
            
            harmony.Patch(AccessTools.Method(typeof(MainTabsRoot), "ToggleTab", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(MainButtonWorker_ToggleWorld), "Activate", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(CameraJumper), "TryHideWorld", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            
            harmony.Patch(AccessTools.Method(typeof(Pawn_JobTracker), "EndCurrentJob", null, null),
                new HarmonyMethod(typeof(HarmonyPatches), "EndCurrentJobPrefix", null),
                new HarmonyMethod(typeof(HarmonyPatches), "EndCurrentJobPostfix", null), null, null);
            
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "PawnTableOnGUI", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(WorldObject), "Destroy", null, null), new HarmonyMethod(typeof(HarmonyPatches), "Caravan_Destroy_Prefix", null), null, null, null);
            
            harmony.Patch(AccessTools.Method(typeof(MainButtonsRoot), "HandleLowPriorityShortcuts", null, null), null, null, new HarmonyMethod(typeof(HarmonyPatches), "HandleLowPriorityShortcuts_Transpiler"));

            harmony.Patch(
                original: AccessTools.Method(typeof(CaravanUIUtility), "AddPawnsSections"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), "AddPawnsSections")
            );

            if (ModCompatibility.GiddyUpCaravanIsActive)
            {
                harmony.Patch(
                    original: AccessTools.Method(AccessTools.TypeByName("GiddyUpCaravan.Harmony.TransferableOneWayWidget_DoRow"), "handleAnimal"),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), "HandleAnimal")
                );
            }

            harmony.Patch(
                original: AccessTools.Method(typeof(Messages), "MessagesDoGUI"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), "MessagesDoGUI")
            );
        }

        private static IEnumerable<CodeInstruction> HandleLowPriorityShortcuts_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo wantedModeInfo = AccessTools.Field(typeof(WorldRenderer), "wantedMode");
            foreach (var ins in instructions)
            {
                if (ins.OperandIs(wantedModeInfo))
                {
                    yield return ins;
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyPatches), "HandleLowPriorityShortcuts", null, null));
                }
                else
                {
                    yield return ins;
                }
            }
            yield break;
        }

        public static void HandleLowPriorityShortcuts()
        {
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
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
            if (__result is null)
            {
                var mainFloatMenu = Find.WindowStack.WindowOfType<MainFloatMenu>();
                if (mainFloatMenu != null)
                {
                    var window = Find.WindowStack.GetWindowAt(UI.MousePositionOnUIInverted);
                    if (!(window is TieredFloatMenu))
                    {
                        mainFloatMenu.CloseAllWindows();
                    }
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
                else if (__instance.TryGetGroups(out HashSet<ColonistGroup> groups))
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
        private static void Settle(Map map)
        {
            if (TacticUtils.TacticalGroups.colonyGroups.TryGetValue(map, out ColonyGroup group))
            {
                if (group.isTaskForce)
                {
                    group.ConvertToColonyGroup();
                }
            }
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

        private static Dictionary<Pawn, int> pawnsLastTick = new Dictionary<Pawn, int>();

        private static bool ModIncompatibilityCheck(Pawn ___pawn, JobCondition condition)
        {
            if (ModCompatibility.SmarterDeconstructionIsActive && condition == JobCondition.InterruptForced)
            {
                var curJobDef = ___pawn.CurJobDef;
                if (curJobDef == JobDefOf.Deconstruct || curJobDef == JobDefOf.Mine)
                {
                    return true;
                }
            }
            return false;
        }
        private static void EndCurrentJobPrefix(Pawn_JobTracker __instance, Pawn ___pawn, JobCondition condition, ref bool startNewJob, out Dictionary<WorkType, WorkState> __state, bool canReturnToPool = true)
        {
            __state = new Dictionary<WorkType, WorkState>();
            if (___pawn.jobs.jobQueue.Any() || pawnsLastTick.TryGetValue(___pawn, out int lastTick) && lastTick == Find.TickManager.TicksGame || ModIncompatibilityCheck(___pawn, condition))
            {
                return; // to prevent infinite recursion in some cases
            }
            pawnsLastTick[___pawn] = Find.TickManager.TicksGame;
            if (___pawn.RaceProps.Humanlike && ___pawn.Faction == Faction.OfPlayer && !___pawn.Drafted)
            {
                if (___pawn.TryGetGroups(out HashSet<ColonistGroup> groups))
                {
                    foreach (var group in groups)
                    {
                        if (group.temporaryWorkers.TryGetValue(___pawn, out WorkType workType))
                        {
                            if (CanWork(___pawn) && CanWorkActive(___pawn))
                            {
                                __state = new Dictionary<WorkType, WorkState> { { workType, WorkState.Temporary } };
                                startNewJob = false;
                                return;
                            }
                        }

                        if (group.activeWorkTypes?.Count > 0)
                        {
                            foreach (var data in group.activeWorkTypes)
                            {
                                if (data.Value != WorkState.Inactive || data.Value == WorkState.Active && CanWork(___pawn) && CanWorkActive(___pawn))
                                {
                                    __state[data.Key] = data.Value;
                                }
                            }
                        }
                    }

                    if (__state.Count > 0)
                    {
                        startNewJob = false;
                    }
                }
            }
        }

        private static void EndCurrentJobPostfix(Pawn ___pawn, Dictionary<WorkType, WorkState> __state)
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
                        else if (state.Value == WorkState.Temporary && ___pawn.TryGetGroups(out HashSet<ColonistGroup> groups))
                        {
                            foreach (var group in groups)
                            {
                                group.temporaryWorkers.Remove(___pawn);
                            }
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
            if (pawn.needs.food?.CurLevel < 0.10f)
            {
                return false;
            }
            if (pawn.needs.rest?.CurLevel < 0.10f)
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
            if (pawn.IsDownedOrIncapable() || pawn.IsSick() || pawn.IsBleeding())
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

        private static List<Pawn> PawnsInCurrentSections; // For Giddy-up! Caravan compatibility
        public static bool AddPawnsSections(TransferableOneWayWidget widget, List<TransferableOneWay> transferables)
        {
            PawnsInCurrentSections = new List<Pawn>(); // For Giddy-up! Caravan compatibility

            // Get all pawns from the list of transferables
            List<Pawn> pawns = new List<Pawn>();
            Dictionary<Pawn, TransferableOneWay> pawnThing = new Dictionary<Pawn, TransferableOneWay>();
            foreach (TransferableOneWay thing in transferables)
            {
                if (thing.ThingDef.category == ThingCategory.Pawn)
                {
                    Pawn pawn = (Pawn)thing.AnyThing;
                    pawns.Add(pawn);
                    pawnThing[pawn] = thing;
                }
            }

            List<PawnGroup> pawnGroups = TacticUtils.AllPawnGroups;
            pawnGroups.Reverse(); // Reverse the list because it iterates in the wrong direction

            // Create a HashSet to sort out all the ungrouped pawns
            HashSet<Pawn> ungroupedPawns = new HashSet<Pawn>(pawns);

            foreach (PawnGroup pawnGroup in pawnGroups)
            {
                if (pawnGroup.pawns != null && pawnGroup.pawns.Count > 0)
                {
                    // Remove grouped pawns from the ungroupedPawns HashSet
                    ungroupedPawns.ExceptWith(from pawn in pawnGroup.pawns select pawn);

                    // Get only the group's pawns that are in the current list of transferables
                    List<TransferableOneWay> sectionTranferables = new List<TransferableOneWay>();
                    foreach (Pawn pawn in pawnGroup.pawns)
                    {
                        if (pawns.Contains(pawn) && pawnThing.ContainsKey(pawn) && pawn.IsFreeColonist)
                        {
                            sectionTranferables.Add(pawnThing[pawn]);
                            if (!PawnsInCurrentSections.Contains(pawn))
                            {
                                PawnsInCurrentSections.Add(pawn); // For Giddy-up! Caravan compatibility
                            }
                        }
                    }

                    if (sectionTranferables.Count > 0)
                    {
                        // Add a new section containing all pawns within the group
                        widget.AddSection(pawnGroup.curGroupName ?? "", from pawn in sectionTranferables select pawn);
                    }
                }
            }

            if (ungroupedPawns.Count > 0)
            {
                // Create a section containing all the ungrouped pawns
                widget.AddSection("Ungrouped " + "ColonistsSection".Translate().ToLower(), from pawn in ungroupedPawns
                                                                                           where pawn.IsFreeColonist
                                                                                           select pawnThing[pawn]);
                foreach (Pawn pawn in ungroupedPawns)
                {
                    if (pawn.IsFreeColonist && !PawnsInCurrentSections.Contains(pawn))
                    {
                        PawnsInCurrentSections.Add(pawn); // For Giddy-up! Caravan compatibility
                    }
                }
            }

            // We then return to vanilla code (slightly tweaked), commenting out the Colonists section of course
            /*
            widget.AddSection("ColonistsSection".Translate(), from pawn in pawns
                                                              where pawn.IsFreeColonist
                                                              select pawnThing[pawn]);
            */

            widget.AddSection("PrisonersSection".Translate(), from pawn in pawns
                                                              where pawn.IsPrisoner
                                                              select pawnThing[pawn]);

            widget.AddSection("CaptureSection".Translate(), from pawn in pawns
                                                            where pawn.Downed && CaravanUtility.ShouldAutoCapture(pawn, Faction.OfPlayer)
                                                            select pawnThing[pawn]);

            widget.AddSection("AnimalsSection".Translate(), from pawn in pawns
                                                            where pawn.RaceProps.Animal
                                                            select pawnThing[pawn]);
            return false;
        }

        public static bool HandleAnimal(float num, Rect buttonRect, Pawn animal, ref List<Pawn> pawns, TransferableOneWay trad)
        {
            if (PawnsInCurrentSections != null)
            {
                pawns = PawnsInCurrentSections; // Fixes Giddy-up! Caravan rider selection functionality
            }
            return true;
        }

        public static bool MessagesDoGUI(List<Message> ___liveMessages)
        {
            Text.Font = GameFont.Small;

            int xOffsetStandard = 12;
            int yOffsetStandard = 12;

            int xOffset = (int)Messages.MessagesTopLeftStandard.x;
            int yOffset = (int)Messages.MessagesTopLeftStandard.y;

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
            List<Rect> drawLocs = null;
            try { drawLocs = TacticUtils.TacticalColonistBar.DrawLocs; } catch { }
            if (drawLocs != null)
            {
                foreach (Rect rect in drawLocs)
                {
                    checkRect(rect);
                }
            }

            // Colonist groups
            List<ColonistGroup> colonistGroups = null;
            try { colonistGroups = TacticUtils.AllGroups; } catch { }
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
    }
}