using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using static Verse.CameraJumper;

namespace TacticalGroups
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("Troy_Alias.TacticalGroups");

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

            harmony.Patch(AccessTools.Method(typeof(Caravan), nameof(Caravan.Notify_PawnAdded), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), nameof(Caravan.Notify_PawnRemoved), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), nameof(Caravan.PostAdd), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), nameof(Caravan.PostRemove), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.AddMap), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.SetFaction), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "SetFaction_Postfix", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Window), nameof(Window.Notify_ResolutionChanged), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "IsPlayingDirty_Postfix", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.DeinitAndRemoveMap), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "IsPlayingDirty_Postfix", null), null, null);

            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.SpawnSetup), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "Pawn_SpawnSetup_Postfix", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.Destroy), null, null), prefix: new HarmonyMethod(typeof(HarmonyPatches), "Pawn_Destroy_Prefix", null), null, null);

            harmony.Patch(AccessTools.PropertySetter(typeof(Game), nameof(Game.CurrentMap)), null, postfix: new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null);

            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.Notify_Resurrected), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(CameraJumper), nameof(CameraJumper.TryJumpInternal), new Type[]
            {
                typeof(Thing), typeof(MovementMode)
            }, null), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.EntriesDirty), null), null, null);
            harmony.Patch(AccessTools.Method(typeof(WorldCameraDriver), nameof(WorldCameraDriver.JumpTo), new Type[]
            {
                typeof(Vector3)
            }, null), new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.EntriesDirty), null), null, null, null);

            harmony.Patch(AccessTools.Method(typeof(MainTabsRoot), nameof(MainTabsRoot.ToggleTab), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(MainButtonWorker_ToggleWorld), nameof(MainButtonWorker_ToggleWorld.Activate), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(CameraJumper), nameof(CameraJumper.TryHideWorld), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);

            harmony.Patch(AccessTools.Method(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.EndCurrentJob), null, null),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.EndCurrentJobPrefix), null),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.EndCurrentJobPostfix), null), null, null);

            harmony.Patch(AccessTools.Method(typeof(PawnTable), nameof(PawnTable.PawnTableOnGUI), null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "PawnTableOnGUI", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(WorldObject), nameof(WorldObject.Destroy), null, null), new HarmonyMethod(typeof(HarmonyPatches), "Caravan_Destroy_Prefix", null), null, null, null);

            harmony.Patch(AccessTools.Method(typeof(MainButtonsRoot), nameof(MainButtonsRoot.HandleLowPriorityShortcuts), null, null), null, null, new HarmonyMethod(typeof(HarmonyPatches), "HandleLowPriorityShortcuts_Transpiler"));

            if (ModsConfig.IdeologyActive)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(JobGiver_OptimizeApparel), nameof(JobGiver_OptimizeApparel.TryCreateRecolorJob)),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.TryCreateRecolorJobPrefix)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(JobGiver_DyeHair), nameof(JobGiver_DyeHair.TryGiveJob)),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.JobGiver_DyeHair_TryGiveJobPrefix)));

            }

            #region HarmonyPatches_CaravanSorting
            harmony.Patch(
                original: AccessTools.Method(typeof(CaravanUIUtility), nameof(CaravanUIUtility.AddPawnsSections)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches_CaravanSorting), nameof(HarmonyPatches_CaravanSorting.AddPawnsSectionsPrefix)));

            if (ModCompatibility.GiddyUpCaravanIsActive)
            {
                harmony.Patch(
                    original: AccessTools.Method(AccessTools.TypeByName("GiddyUpCaravan.Harmony.TransferableOneWayWidget_DoRow"), "handleAnimal"),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches_CaravanSorting), "HandleAnimal")
                );
            }
            #endregion

            #region HarmonyPatches_DynamicMessages
            harmony.Patch(
                original: AccessTools.Method(typeof(Messages), "MessagesDoGUI"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches_DynamicMessages), "MessagesDoGUI")
            );
            #endregion

            #region HarmonyPatches_GroupBills
            harmony.Patch(
                original: AccessTools.Method(typeof(Dialog_BillConfig), "DoWindowContents"),
                transpiler: new HarmonyMethod(typeof(HarmonyPatches_GroupBills), "DoWindowContents_Transpiler")
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Dialog_BillConfig), "GeneratePawnRestrictionOptions"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches_GroupBills), "GeneratePawnRestrictionOptions")
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Bill), "PawnAllowedToStartAnew"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches_GroupBills), "PawnAllowedToStartAnew")
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Bill), "SetPawnRestriction"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches_GroupBills), "SetPawnRestriction")
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Bill), "SetAnySlaveRestriction"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches_GroupBills), "SetPawnRestriction")
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Bill), "SetAnyPawnRestriction"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches_GroupBills), "SetPawnRestriction")
            );
            #endregion
        }

        private static IEnumerable<CodeInstruction> HandleLowPriorityShortcuts_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo wantedModeInfo = AccessTools.Field(typeof(WorldRenderer), "wantedMode");
            foreach (CodeInstruction ins in instructions)
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
                MainFloatMenu mainFloatMenu = Find.WindowStack.WindowOfType<MainFloatMenu>();
                if (mainFloatMenu != null)
                {
                    Window window = Find.WindowStack.GetWindowAt(UI.MousePositionOnUIInverted);
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
                if (__instance.Faction == Faction.OfPlayer)
                {
                    TacticUtils.TacticalGroups.CreateOrJoinColony(new List<Pawn> { __instance }, __instance.Map);
                }
                else if (__instance.TryGetGroups(out HashSet<ColonistGroup> groups))
                {
                    List<ColonistGroup> groupsList = groups.ToList();
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
            if (__instance.Spawned && __instance.Faction == Faction.OfPlayer && __instance.RaceProps.Humanlike)
            {
                TacticUtils.TacticalGroups.CreateOrJoinColony(new List<Pawn> { __instance }, __instance.Map);
            }
        }

        private static void Pawn_Destroy_Prefix(Pawn __instance)
        {
            if (TacticUtils.TacticalGroups?.pawnGroups != null)
            {
                for (int num = TacticUtils.TacticalGroups.pawnGroups.Count - 1; num >= 0; num--)
                {
                    PawnGroup group = TacticUtils.TacticalGroups.pawnGroups[num];
                    group.Disband(__instance);
                }
            }

            if (TacticUtils.TacticalGroups?.caravanGroups != null)
            {
                List<Caravan> caravanKeysToRemove = new List<Caravan>();
                foreach (KeyValuePair<Caravan, CaravanGroup> group in TacticUtils.TacticalGroups.caravanGroups)
                {
                    group.Value.pawnIcons.Remove(__instance);
                    group.Value.pawns.Remove(__instance);
                    if (group.Value.pawns.Count == 0)
                    {
                        caravanKeysToRemove.Add(group.Key);
                    }
                }

                foreach (Caravan key in caravanKeysToRemove)
                {
                    TacticUtils.TacticalGroups.caravanGroups.Remove(key);
                }
            }

            if (TacticUtils.TacticalGroups?.colonyGroups != null)
            {
                List<Map> colonyKeysToRemove = new List<Map>();
                foreach (KeyValuePair<Map, ColonyGroup> group in TacticUtils.TacticalGroups.colonyGroups)
                {
                    group.Value.pawnIcons.Remove(__instance);
                    group.Value.pawns.Remove(__instance);
                    if (group.Value.pawns.Count == 0)
                    {
                        colonyKeysToRemove.Add(group.Key);
                    }
                }

                foreach (Map key in colonyKeysToRemove)
                {
                    TacticUtils.TacticalGroups.colonyGroups.Remove(key);
                }
            }

            if (TacticUtils.TacticalColonistBar != null)
            {
                TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            }
        }

        private static readonly Dictionary<Pawn, int> pawnsLastTick = new Dictionary<Pawn, int>();

        private static bool ModIncompatibilityCheck(Pawn ___pawn, JobCondition condition)
        {
            if (ModCompatibility.SmarterDeconstructionIsActive && condition == JobCondition.InterruptForced)
            {
                JobDef curJobDef = ___pawn.CurJobDef;
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
            if (___pawn.jobs.jobQueue.Any() || (pawnsLastTick.TryGetValue(___pawn, out int lastTick) && lastTick == Find.TickManager.TicksGame) || ModIncompatibilityCheck(___pawn, condition))
            {
                return; // to prevent infinite recursion in some cases
            }
            pawnsLastTick[___pawn] = Find.TickManager.TicksGame;
            if (___pawn.RaceProps.Humanlike && ___pawn.Faction == Faction.OfPlayer && !___pawn.Drafted && ___pawn.GetLord() is null && ___pawn.mindState.duty is null)
            {
                if (___pawn.TryGetGroups(out HashSet<ColonistGroup> groups))
                {
                    foreach (ColonistGroup group in groups)
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
                            foreach (KeyValuePair<WorkType, WorkState> data in group.activeWorkTypes.OrderByDescending(x => x.Value))
                            {
                                if (data.Value != WorkState.Inactive || (data.Value == WorkState.Active && CanWork(___pawn) && CanWorkActive(___pawn)))
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
                foreach (KeyValuePair<WorkType, WorkState> state in __state.InRandomOrder())
                {
                    if (state.Value != WorkState.Inactive && CanWork(___pawn))
                    {
                        if (state.Value == WorkState.Active && !CanWorkActive(___pawn))
                        {
                            continue;
                        }
                        Job curJob = ___pawn.CurJob;
                        WorkSearchUtility.SearchForWork(state.Key, new List<Pawn> { ___pawn });
                        if (curJob != ___pawn.CurJob)
                        {
                            break;
                        }
                        else if (state.Value == WorkState.Temporary && ___pawn.TryGetGroups(out HashSet<ColonistGroup> groups))
                        {
                            foreach (ColonistGroup group in groups)
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
            return pawn.needs.mood.CurLevel > pawn.mindState.mentalBreaker.BreakThresholdMinor;
        }
        private static bool CanWork(Pawn pawn)
        {
            return (pawn.needs.food?.CurLevel) >= 0.10f
&& (pawn.needs.rest?.CurLevel) >= 0.10f
&& pawn.MentalState == null
&& pawn.mindState.lastJobTag != JobTag.SatisfyingNeeds
&& !pawn.IsDownedOrIncapable() && !pawn.IsSick() && !pawn.IsBleeding();
        }

        private static readonly Dictionary<Apparel, CompColorable> cachedComps = new Dictionary<Apparel, CompColorable>();
        public static void TryCreateRecolorJobPrefix(Pawn pawn)
        {
            if (pawn.apparel?.WornApparel != null)
            {
                foreach (Apparel apparel in pawn.apparel.WornApparel)
                {
                    if (!cachedComps.TryGetValue(apparel, out CompColorable comp))
                    {
                        cachedComps[apparel] = apparel.TryGetComp<CompColorable>();
                    }
                    if (comp != null)
                    {
                        if (!comp.DesiredColor.HasValue)
                        {
                            Color? desiredColor = ColorUtils.GetDesiredColor(pawn, apparel);
                            if (desiredColor.HasValue && comp.Color != desiredColor.Value)
                            {
                                comp.DesiredColor = desiredColor;
                            }
                        }
                        else if (comp.DesiredColor.HasValue && comp.Color == comp.DesiredColor.Value)
                        {
                            comp.DesiredColor = null;
                        }
                    }
                }
            }
        }

        public static void JobGiver_DyeHair_TryGiveJobPrefix(Pawn pawn)
        {
            Color? desiredColor = ColorUtils.GetDesiredHairColor(pawn);
            if (desiredColor.HasValue)
            {
                if (pawn.story.hairColor != desiredColor.Value)
                {
                    if (!pawn.style.nextHairColor.HasValue || pawn.style.nextHairColor != desiredColor.Value)
                    {
                        pawn.style.nextHairColor = desiredColor.Value;
                    }
                }
                else if (pawn.style.nextHairColor.HasValue)
                {
                    pawn.style.nextHairColor = null;
                }
            }
        }
        public static void PawnTableOnGUI(Vector2 position, PawnTableDef ___def, List<float> ___cachedColumnWidths, Vector2 ___cachedSize, float ___cachedHeaderHeight, float ___cachedHeightNoScrollbar)
        {
            if (___def == PawnTableDefOf.Assign)
            {
                Rect outRect = new Rect((int)position.x, (int)position.y + (int)___cachedHeaderHeight, (int)___cachedSize.x, (int)___cachedSize.y - (int)___cachedHeaderHeight);
                Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, (int)___cachedHeightNoScrollbar - (int)___cachedHeaderHeight);

                Rect createGroupRect = new Rect(viewRect.x + 10, outRect.y - 50, Textures.CreateGroupIcon.width, Textures.CreateGroupIcon.height);
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