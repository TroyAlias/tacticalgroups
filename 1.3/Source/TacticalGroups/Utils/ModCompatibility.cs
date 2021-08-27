using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using CompatUtils;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	public static class ModCompatibility
	{
		public static bool SmarterDeconstructionIsActive;
		public static bool GiddyUpCaravanIsActive;

		public static MethodInfo pawnBadgesDrawMethod;

		public static MethodInfo rimworldOfMagicDrawMethod;

		public static MethodInfo alteredCarbonHandleClicks_PatchMethod;
		public static MethodInfo alteredCarbonDrawColonist_PatchMethod;

		public static MethodInfo combatExtendedHasAmmo_Method;

		public static MethodInfo jobInBarDrawMethod;

		public static MethodInfo workManagerSaveCurrentStateMethod;
		public static MethodInfo assignManagerSaveCurrentStateMethod;

		private static MethodInfo getIntelligenceMethod;
		public static Intelligence GetIntelligence(this Pawn pawn)
		{
			if (!(getIntelligenceMethod is null))
            {
				return (Intelligence)getIntelligenceMethod.Invoke(null, new object[] { pawn });
            }
			return pawn.RaceProps.intelligence;
		}

		static ModCompatibility()
        {
			SmarterDeconstructionIsActive = Compatibility.IsModActive("legodude17.smartdecon");
			GiddyUpCaravanIsActive = Compatibility.IsModActive("roolo.giddyupcaravan");

			pawnBadgesDrawMethod = Compatibility.GetConsistentMethod("saucypigeon.pawnbadge", "RR_PawnBadge.RimWorld_ColonistBarColonistDrawer_DrawColonist", "Postfix", new Type[] {
				typeof(Rect), typeof(Pawn), typeof(Map), typeof(bool), typeof(bool)
			}, logError: true);

			rimworldOfMagicDrawMethod = Compatibility.GetConsistentMethod("torann.arimworldofmagic", "TorannMagic.TorannMagicMod+ColonistBarColonistDrawer_Patch", "Postfix", new Type[] {
				typeof(RimWorld.ColonistBarColonistDrawer), typeof(Rect), typeof(Pawn)
			}, logError: true);

			alteredCarbonHandleClicks_PatchMethod = Compatibility.GetConsistentMethod("hlx.ultratechalteredcarbon", "AlteredCarbon.HandleClicks_Patch", "Prefix", new Type[] {
				typeof(Rect), typeof(Pawn), typeof(int), typeof(bool)
			}, logError: true);
			alteredCarbonDrawColonist_PatchMethod = Compatibility.GetConsistentMethod("hlx.ultratechalteredcarbon", "AlteredCarbon.DrawColonist_Patch", "Prefix", new Type[] {
				typeof(Rect), typeof(Pawn), typeof(Map), typeof(bool), typeof(bool), typeof(Dictionary<string, string>), typeof(Vector2), typeof(Texture2D), typeof(Vector2[])
			}, logError: true);

			combatExtendedHasAmmo_Method = Compatibility.GetConsistentMethod("ceteam.combatextended", "CombatExtended.CE_Utility", "HasAmmo", new Type[] {
				typeof(ThingWithComps)
			}, logError: true);

			jobInBarDrawMethod = Compatibility.GetConsistentMethod("dark.jobinbar", "JobInBar.ColonistBarColonistDrawer_DrawColonist_Patch", "Postfix", new Type[] {
				typeof(Rect), typeof(Pawn), typeof(Map), typeof(bool), typeof(bool)
			}, logError: true);

			workManagerSaveCurrentStateMethod = Compatibility.GetConsistentMethod("voult.betterpawncontrol", "BetterPawnControl.WorkManager", "SaveCurrentState", new Type[] {
				typeof(List<Pawn>)
			}, logError: true);
			assignManagerSaveCurrentStateMethod = Compatibility.GetConsistentMethod("voult.betterpawncontrol", "BetterPawnControl.AssignManager", "SaveCurrentState", new Type[] {
				typeof(List<Pawn>)
			}, logError: true);

			getIntelligenceMethod = Compatibility.GetConsistentMethod("tachyonite.pawnmorpherpublic", "Pawnmorph.FormerHumanUtilities:GetIntelligence", new Type[] {
				typeof(Pawn)
			}, logError: true);
		}
	}
}
