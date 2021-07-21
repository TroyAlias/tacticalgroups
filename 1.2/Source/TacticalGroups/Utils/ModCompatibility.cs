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
		public static MethodInfo pawnBadgesDrawMethod;

		public static bool RimworldOfMagicIsActive;
		public static MethodInfo rimworldOfMagicDrawMethod;

		public static bool AlteredCarbonIsActive;
		public static MethodInfo alteredCarbonHandleClicks_PatchMethod;
		public static MethodInfo alteredCarbonDrawColonist_PatchMethod;

		public static bool CombatExtendedIsActive;
		public static MethodInfo combatExtendedHasAmmo_Method;

		public static bool JobInBarIsActive;
		public static MethodInfo jobInBarDrawMethod;

		public static bool BetterPawnControlIsActive;
		public static MethodInfo workManagerSaveCurrentStateMethod;
		public static MethodInfo assignManagerSaveCurrentStateMethod;
		public static MethodInfo restrictManagerSaveCurrentStateMethod;

		public static bool SmarterDeconstructionIsActive;

		public static bool GiddyUpCaravanIsActive;

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
			CombatExtendedIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "ceteam.combatextended").Any();
			if (CombatExtendedIsActive)
			{
				combatExtendedHasAmmo_Method = AccessTools.Method(AccessTools.TypeByName("CombatExtended.CE_Utility"), "HasAmmo");
			}

			JobInBarIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.Name == "Job In Bar").Any();
			if (JobInBarIsActive)
			{
				jobInBarDrawMethod = AccessTools.Method(AccessTools.TypeByName("JobInBar.LabelPatch"), "Postfix");
			}
			BetterPawnControlIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower().Contains("voult.betterpawncontrol")).Any();
			if (BetterPawnControlIsActive)
			{
				workManagerSaveCurrentStateMethod = AccessTools.Method(AccessTools.TypeByName("BetterPawnControl.WorkManager"), "SaveCurrentState");
				assignManagerSaveCurrentStateMethod = AccessTools.Method(AccessTools.TypeByName("BetterPawnControl.AssignManager"), "SaveCurrentState");
				restrictManagerSaveCurrentStateMethod = AccessTools.Method(AccessTools.TypeByName("BetterPawnControl.RestrictManager"), "SaveCurrentState");
			}
			SmarterDeconstructionIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower().Contains("legodude17.smartdecon")).Any();
			GiddyUpCaravanIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "roolo.giddyupcaravan").Any();
		}
	}
}
