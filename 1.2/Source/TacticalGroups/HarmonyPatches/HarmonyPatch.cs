using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace TacticalGroups 
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit 
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Troy_Alias.TacticalGroups");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(ColonistBar), "ColonistBarOnGUI")]
    public static class ColonistBarOnGUIPatch
    {
        public static bool Prefix()
        {
            TacticalGroups.ColonistBar.ColonistBarOnGUI();
            return false;
        }
    }

    [HarmonyPatch(typeof(ColonistBar), "MarkColonistsDirty")]
    public static class MarkColonistsDirtyPatch
    {
        public static void Postfix()
        {
            TacticalGroups.ColonistBar.MarkColonistsDirty();
        }
    }
}