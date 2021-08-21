using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

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

		public static bool IsMethodConsistent(string modName, MethodInfo methodInfo, Type[] correctMethodTypes)
        {
			Type[] currentMethodTypes = methodInfo.GetParameters().Select(pi => pi.ParameterType.IsByRef ? pi.ParameterType.GetElementType() : pi.ParameterType).ToArray();
			if (currentMethodTypes.Length != correctMethodTypes.Length)
			{
				Log.Error($"Colony Groups failed to support {modName}: " +
					$"Inconsistent number of parameters for method '{methodInfo.ReflectedType.FullName + "." + methodInfo.Name}'");
				return false;
			}
			for (int i = 0; i < currentMethodTypes.Length; i++)
			{
				if (currentMethodTypes[i] != correctMethodTypes[i])
				{
					Log.Error($"Colony Groups failed to support {modName}: " +
						$"Inconsistent parameter {i + 1} for method '{methodInfo.ReflectedType.FullName + "." + methodInfo.Name}'" +
						"\n    " + currentMethodTypes[i] + " != " + correctMethodTypes[i]);
					return false;
				}
			}
			return true;
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
					PawnBadgesIsActive = false;
				}
				else
				{
					PawnBadgesIsActive = IsMethodConsistent("Pawn Badges", pawnBadgesDrawMethod, new Type[] { typeof(Rect), typeof(Pawn), typeof(Map), typeof(bool), typeof(bool) });
				}
			}
			RimworldOfMagicIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "torann.arimworldofmagic").Any();
			if (RimworldOfMagicIsActive)
			{
				rimworldOfMagicDrawMethod = AccessTools.Method(AccessTools.TypeByName("TorannMagic.TorannMagicMod+ColonistBarColonistDrawer_Patch"), "Postfix");
				if (rimworldOfMagicDrawMethod is null)
                {
					Log.Error("Colony Groups failed to support Rimworld Of Magic. Report about it.");
					RimworldOfMagicIsActive = false;
				}
				else
				{
					RimworldOfMagicIsActive = IsMethodConsistent("Rimworld Of Magic", rimworldOfMagicDrawMethod, new Type[] { typeof(RimWorld.ColonistBarColonistDrawer), typeof(Rect), typeof(Pawn) });
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
					AlteredCarbonIsActive = false;
				}
				else
				{
					AlteredCarbonIsActive = IsMethodConsistent("Altered Carbon", alteredCarbonHandleClicks_PatchMethod, new Type[] { typeof(Rect), typeof(Pawn), typeof(int), typeof(bool) });
					if (AlteredCarbonIsActive)
                    {
						AlteredCarbonIsActive = IsMethodConsistent("Altered Carbon", alteredCarbonDrawColonist_PatchMethod, new Type[] { typeof(Rect), typeof(Pawn), typeof(Map), typeof(bool), typeof(bool), typeof(Dictionary<string, string>), typeof(Vector2), typeof(Texture2D), typeof(Vector2[]) });
					}
				}
			}
			CombatExtendedIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.PackageId.ToLower() == "ceteam.combatextended").Any();
			if (CombatExtendedIsActive)
			{
				combatExtendedHasAmmo_Method = AccessTools.Method(AccessTools.TypeByName("CombatExtended.CE_Utility"), "HasAmmo");
				if (combatExtendedHasAmmo_Method is null)
                {
					Log.Error("Colony Groups failed to support Combat Extended. Report about it.");
					CombatExtendedIsActive = false;
				}
				else
				{
					CombatExtendedIsActive = IsMethodConsistent("Combat Extended", combatExtendedHasAmmo_Method, new Type[] { typeof(ThingWithComps) });
				}
			}

			JobInBarIsActive = ModLister.AllInstalledMods.Where(x => x.Active && x.Name == "Job In Bar").Any();
			if (JobInBarIsActive)
			{
				jobInBarDrawMethod = AccessTools.Method(AccessTools.TypeByName("JobInBar.ColonistBarColonistDrawer_DrawColonist_Patch"), "Postfix");
				if (jobInBarDrawMethod is null)
				{
					Log.Error("Colony Groups failed to support Job in Bar. Report about it.");
					JobInBarIsActive = false;
				}
				else
                {
					JobInBarIsActive = IsMethodConsistent("Job in Bar", jobInBarDrawMethod, new Type[] { typeof(Rect), typeof(Pawn), typeof(Map), typeof(bool), typeof(bool) });
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
					BetterPawnControlIsActive = false;
				}
				else
				{
					BetterPawnControlIsActive = IsMethodConsistent("Better Pawn Control", workManagerSaveCurrentStateMethod, new Type[] { typeof(List<Pawn>) });
					if (BetterPawnControlIsActive)
					{
						BetterPawnControlIsActive = IsMethodConsistent("Better Pawn Control", assignManagerSaveCurrentStateMethod, new Type[] { typeof(List<Pawn>) });
					}
				}
			}
			SmarterDeconstructionIsActive = ModLister.AllInstalledMods.Any(x => x.Active && x.PackageId.ToLower().Contains("legodude17.smartdecon"));
			GiddyUpCaravanIsActive = ModLister.AllInstalledMods.Any(x => x.Active && x.PackageId.ToLower() == "roolo.giddyupcaravan");
			PawnMorpherIsActive = ModLister.AllInstalledMods.Any(x => x.Active && x.PackageId.ToLower() == "tachyonite.pawnmorpherpublic");
			if (PawnMorpherIsActive)
            {
				var getIntelligenceMethod = AccessTools.Method("Pawnmorph.FormerHumanUtilities:GetIntelligence");
				if (getIntelligenceMethod is null)
                {
					Log.Error("Colony Groups failed to support PawnMorpher. Report about it.");
					PawnMorpherIsActive = false;
				}
				else
				{
					PawnMorpherIsActive = IsMethodConsistent("PawnMorpher", getIntelligenceMethod, new Type[] { typeof(Pawn) });
					if (PawnMorpherIsActive)
                    {
						pawnMorpherInteligence = (Func<Pawn, Intelligence>)Delegate.CreateDelegate(typeof(Func<Pawn, Intelligence>), getIntelligenceMethod);
						if (pawnMorpherInteligence is null)
						{
							Log.Error("Colony Groups failed to support PawnMorpher: Failed delegation. Report about it.");
							PawnMorpherIsActive = false;
						}
					}
				}
			}
		}
	}
}
