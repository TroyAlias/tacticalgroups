using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	public static class ModCompatibility
	{
		public static bool PawnBadgesIsActive;
		public static bool RimworldOfMagicIsActive;

		public static MethodInfo pawnBadgesDrawMethod;
		public static MethodInfo rimworldOfMagicDrawMethod;

		public static bool AlteredCarbonIsActive;
		public static MethodInfo alteredCarbonHandleClicks_PatchMethod;
		public static MethodInfo alteredCarbonDrawColonist_PatchMethod;
		static ModCompatibility()
        {
			PawnBadgesIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "saucypigeon.pawnbadge").Any();
			if (PawnBadgesIsActive)
            {
				pawnBadgesDrawMethod = AccessTools.Method(AccessTools.TypeByName("RR_PawnBadge.RimWorld_ColonistBarColonistDrawer_DrawColonist"), "Postfix");
			}
			RimworldOfMagicIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "torann.arimworldofmagic").Any();
			if (RimworldOfMagicIsActive)
			{
				rimworldOfMagicDrawMethod = AccessTools.Method(AccessTools.TypeByName("TorannMagic.TorannMagicMod+ColonistBarColonistDrawer_Patch"), "Postfix");
			}

			AlteredCarbonIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "hlx.rimworldalteredcarbon").Any();
			if (AlteredCarbonIsActive)
			{
				alteredCarbonHandleClicks_PatchMethod = AccessTools.Method(AccessTools.TypeByName("AlteredCarbon.HandleClicks_Patch"), "Prefix");
				alteredCarbonDrawColonist_PatchMethod = AccessTools.Method(AccessTools.TypeByName("AlteredCarbon.DrawColonist_Patch"), "Prefix");
			}
		}
	}
}
