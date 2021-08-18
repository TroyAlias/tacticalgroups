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

		public static bool SmarterDeconstructionIsActive;
		public static bool GiddyUpCaravanIsActive;
		public static bool PawnMorpherIsActive;

		private static readonly Func<Pawn, Intelligence> pawnMorpherInteligence;
		public static Intelligence GetIntelligence(this Pawn pawn)
		{
			if (PawnMorpherIsActive)
            {
				return pawnMorpherInteligence(pawn);
            }
			return pawn.RaceProps.intelligence;
		}
		static ModCompatibility()
        {
			PawnBadgesIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "saucypigeon.pawnbadge").Any();
			if (PawnBadgesIsActive)
            {
				pawnBadgesDrawMethod = AccessTools.Method(AccessTools.TypeByName("RR_PawnBadge.RimWorld_ColonistBarColonistDrawer_DrawColonist"), "Postfix");
				if (pawnBadgesDrawMethod is null)
                {
					Log.Error("Colony Groups failed to support Pawn Badges. Report about it.");
                }
			}
			RimworldOfMagicIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "torann.arimworldofmagic").Any();
			if (RimworldOfMagicIsActive)
			{
				rimworldOfMagicDrawMethod = AccessTools.Method(AccessTools.TypeByName("TorannMagic.TorannMagicMod+ColonistBarColonistDrawer_Patch"), "Postfix");
				if (rimworldOfMagicDrawMethod is null)
                {
					Log.Error("Colony Groups failed to support Rimworld Of Magic. Report about it.");
                }
			}

			AlteredCarbonIsActive = ModLister.AllInstalledMods.Any(x => x.Active && x.PackageId.ToLower() == "hlx.ultratechalteredcarbon");
			if (AlteredCarbonIsActive)
			{
				alteredCarbonHandleClicks_PatchMethod = AccessTools.Method(AccessTools.TypeByName("AlteredCarbon.HandleClicks_Patch"), "Prefix");
				alteredCarbonDrawColonist_PatchMethod = AccessTools.Method(AccessTools.TypeByName("AlteredCarbon.DrawColonist_Patch"), "Prefix");
				if (alteredCarbonHandleClicks_PatchMethod is null || alteredCarbonDrawColonist_PatchMethod is null)
                {
					Log.Error("Colony Groups failed to support Altered Carbon. Report about it.");
				}
			}
			CombatExtendedIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "ceteam.combatextended").Any();
			if (CombatExtendedIsActive)
			{
				combatExtendedHasAmmo_Method = AccessTools.Method(AccessTools.TypeByName("CombatExtended.CE_Utility"), "HasAmmo");
				if (combatExtendedHasAmmo_Method is null)
                {
					Log.Error("Colony Groups failed to support Combat Extended. Report about it.");
				}
			}

			JobInBarIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.Name == "Job In Bar").Any();
			if (JobInBarIsActive)
			{
				jobInBarDrawMethod = AccessTools.Method(AccessTools.TypeByName("JobInBar.ColonistBarColonistDrawer_DrawColonist_Patch"), "Postfix");
				if (jobInBarDrawMethod is null)
                {
					Log.Error("Colony Groups failed to support Job in Bar. Report about it.");
				}
			}
			BetterPawnControlIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower().Contains("voult.betterpawncontrol")).Any();
			if (BetterPawnControlIsActive)
			{
				workManagerSaveCurrentStateMethod = AccessTools.Method(AccessTools.TypeByName("BetterPawnControl.WorkManager"), "SaveCurrentState");
				assignManagerSaveCurrentStateMethod = AccessTools.Method(AccessTools.TypeByName("BetterPawnControl.AssignManager"), "SaveCurrentState");
				if (workManagerSaveCurrentStateMethod is null || assignManagerSaveCurrentStateMethod is null)
                {
					Log.Error("Colony Groups failed to support Better Pawn Control. Report about it.");
				}
			}
			SmarterDeconstructionIsActive = ModLister.AllInstalledMods.Any(x => x.Active && x.PackageId.ToLower().Contains("legodude17.smartdecon"));
			GiddyUpCaravanIsActive = ModLister.AllInstalledMods.Any(x => x.Active && x.PackageId.ToLower() == "roolo.giddyupcaravan");
			PawnMorpherIsActive = ModLister.AllInstalledMods.Any(x => x.Active && x.PackageId.ToLower() == "tachyonite.pawnmorpherpublic");
			if (PawnMorpherIsActive)
            {
				var getIntelligenceMethod = AccessTools.Method("Pawnmorph.FormerHumanUtilities:GetIntelligence");
				pawnMorpherInteligence = (Func<Pawn, Intelligence>)Delegate.CreateDelegate(typeof(Func<Pawn, Intelligence>), getIntelligenceMethod);
				if (pawnMorpherInteligence is null)
                {
					Log.Error("Colony Groups failed to support PawnMorpher. Report about it.");
				}
			}
		}
	}
}
