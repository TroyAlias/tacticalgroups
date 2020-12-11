using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System;
using Verse.Sound;

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


            harmony.Patch(AccessTools.Method(typeof(Caravan), "Notify_PawnAdded", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), "Notify_PawnRemoved", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), "PostAdd", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Caravan), "PostRemove", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Game), "AddMap", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn), "SetFaction", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "EntriesDirty", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Window), "Notify_ResolutionChanged", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "IsPlayingDirty_Postfix", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Game), "DeinitAndRemoveMap", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "IsPlayingDirty_Postfix", null), null, null);

            harmony.Patch(AccessTools.Method(typeof(Pawn), "SpawnSetup", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "Pawn_SpawnSetup_Postfix", null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn), "Destroy", null, null), prefix: new HarmonyMethod(typeof(HarmonyPatches), "Pawn_Destroy_Prefix", null), null, null);
            
            //harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "Notify_Resurrected", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "Pawn_Resurrected_Postfix", null), null, null);
            //harmony.Patch(AccessTools.Method(typeof(WorldCameraDriver), "JumpTo", new Type[]
            //{
            //    typeof(Vector3)
            //}, null), new HarmonyMethod(typeof(HarmonyPatches), "StopFollow_Prefix", null), null, null, null);
            //harmony.Patch(AccessTools.Method(typeof(ThingSelectionUtility), "SelectNextColonist", null, null), new HarmonyMethod(typeof(HarmonyPatches), "StartFollowSelectedColonist1", null), new HarmonyMethod(typeof(HarmonyPatches), "StartFollowSelectedColonist2", null), null, null);
            //harmony.Patch(AccessTools.Method(typeof(ThingSelectionUtility), "SelectPreviousColonist", null, null), new HarmonyMethod(typeof(HarmonyPatches), "StartFollowSelectedColonist1", null), new HarmonyMethod(typeof(HarmonyPatches), "StartFollowSelectedColonist2", null), null, null);
            //harmony.Patch(AccessTools.Method(typeof(CameraDriver), "JumpToCurrentMapLoc", new Type[]
            //{
            //    typeof(Vector3)
            //}, null), new HarmonyMethod(typeof(HarmonyPatches), "StopFollow_Prefix_Vector3", null), null, null, null);
            //harmony.Patch(AccessTools.Method(typeof(Pawn), "PostApplyDamage", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "Pawn_PostApplyDamage_Postfix", null), null, null);
            //harmony.Patch(AccessTools.Method(typeof(Corpse), "NotifyColonistBar", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "NotifyColonistBar_Postfix", null), null, null);
            //harmony.Patch(AccessTools.Method(typeof(MapPawns), "DoListChangedNotifications", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "IsColonistBarNull_Postfix", null), null, null);
            //harmony.Patch(AccessTools.Method(typeof(ThingOwner), "NotifyColonistBarIfColonistCorpse", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "NotifyColonistBarIfColonistCorpse_Postfix", null), null, null);
            //harmony.Patch(AccessTools.Method(typeof(Thing), "DeSpawn", null, null), null, new HarmonyMethod(typeof(HarmonyPatches), "DeSpawn_Postfix", null), null, null);
            //harmony.Patch(AccessTools.Method(typeof(PlaySettings), "DoPlaySettingsGlobalControls", null, null), new HarmonyMethod(typeof(HarmonyPatches), "PlaySettingsDirty_Prefix", null), new HarmonyMethod(typeof(HarmonyPatches), "PlaySettingsDirty_Postfix", null), null, null);
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

        private static void Pawn_SpawnSetup_Postfix(Pawn __instance)
        {
            if (__instance.Spawned && __instance.FactionOrExtraMiniOrHomeFaction == Faction.OfPlayer && __instance.RaceProps.Humanlike)
            {
                foreach (var group in TacticUtils.TacticalGroups.colonyGroups.Values)
                {
                    if (group.Map == __instance.Map)
                    {
                        group.Add(__instance);
                    }
                }
            }
        }
        private static void Pawn_Destroy_Prefix(Pawn __instance)
        {
            foreach (var group in TacticUtils.TacticalGroups.Groups)
            {
                group.pawnIcons.Remove(__instance);
                group.pawns.Remove(__instance);
            }
            foreach (var group in TacticUtils.TacticalGroups.caravanGroups.Values)
            {
                group.pawnIcons.Remove(__instance);
                group.pawns.Remove(__instance);
            }
            foreach (var group in TacticUtils.TacticalGroups.colonyGroups.Values)
            {
                group.pawnIcons.Remove(__instance);
                group.pawns.Remove(__instance);
            }
        }
    }
}