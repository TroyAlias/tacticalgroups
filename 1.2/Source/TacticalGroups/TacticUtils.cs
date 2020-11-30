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

		private static TacticalGroups tacticalGroups;
		public static TacticalColonistBar TacticalColonistBar => TacticalGroups.TacticalColonistBar;
		public static List<ColonistGroup> Groups => TacticalGroups.Groups;

	}
}
