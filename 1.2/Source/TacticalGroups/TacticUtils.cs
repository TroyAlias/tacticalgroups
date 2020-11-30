using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	internal static class TacticUtils
	{
		public static TacticalGroups TacticalGroups
		{
			get
			{
				if (tacticalGroups == null)
				{
					tacticalGroups = Current.Game.GetComponent<TacticalGroups>();
					return tacticalGroups;
				}
				return tacticalGroups;
			}
		}
		public static void ResetTacticGroups()
		{
			tacticalGroups = Current.Game.GetComponent<TacticalGroups>();
		}

		public static bool IsDownedOrIncapable(this Pawn pawn)
		{
			if (pawn.Downed)
			{
				return true;
			}
			PawnCapacitiesHandler pawnCapacitiesHandler;
			if (pawn == null)
			{
				pawnCapacitiesHandler = null;
			}
			else
			{
				Pawn_HealthTracker health = pawn.health;
				pawnCapacitiesHandler = ((health != null) ? health.capacities : null);
			}
			PawnCapacitiesHandler pawnCapacitiesHandler2 = pawnCapacitiesHandler;
			if (pawnCapacitiesHandler2 == null)
			{
				return true;
			}
			if (pawnCapacitiesHandler2.GetLevel(PawnCapacityDefOf.Consciousness) < 0.1f)
			{
				return true;
			}
			if (pawnCapacitiesHandler2.GetLevel(PawnCapacityDefOf.Moving) < 0.1f)
			{
				return true;
			}
			return false;
		}

		public static bool IsShotOrBleeding(this Pawn pawn)
        {
			float num = 0f;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury = hediffs[i] as Hediff_Injury;
				if (hediff_Injury != null && (hediff_Injury.CanHealFromTending() || hediff_Injury.CanHealNaturally() || hediff_Injury.Bleeding))
				{
					num += hediff_Injury.Severity;
				}
			}
			return num > 8f * pawn.RaceProps.baseHealthScale;
		}
		public static bool IsSick(this Pawn pawn)
		{
			if (pawn.health.hediffSet.HasImmunizableNotImmuneHediff())
			{
				return true;
			}
			return false;
		}

		private static TacticalGroups tacticalGroups;
		public static TacticalColonistBar TacticalColonistBar => TacticalGroups.TacticalColonistBar;
		public static List<ColonistGroup> Groups => TacticalGroups.Groups;

	}
}
