using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	public class JobGiver_AIFightEnemiesWeakest : JobGiver_AIFightEnemy
	{
		private new readonly bool needLOSToAcquireNonPawnTargets;
		private new readonly float targetAcquireRadius = 56f;
		public override Thing FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedReachableIfCantHitFromMyPos | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
			if (needLOSToAcquireNonPawnTargets)
			{
				targetScanFlags |= TargetScanFlags.NeedLOSToNonPawns;
			}
			if (PrimaryVerbIsIncendiary(pawn))
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}
			return (Thing)TacticAttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, (Thing x) => ExtraTargetValidator(pawn, x), 0f, targetAcquireRadius, GetFlagPosition(pawn),
				GetFlagRadius(pawn), combatSearchMode: CombatSearchMode.Weakest);
		}

		private new bool PrimaryVerbIsIncendiary(Pawn pawn)
		{
			if (pawn.equipment != null && pawn.equipment.Primary != null)
			{
				List<Verb> allVerbs = pawn.equipment.Primary.GetComp<CompEquippable>().AllVerbs;
				for (int i = 0; i < allVerbs.Count; i++)
				{
					if (allVerbs[i].verbProps.isPrimary)
					{
						return allVerbs[i].IsIncendiary_Ranged();
					}
				}
			}
			return false;
		}

		public override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest, Verb verbToUse = null)
		{
			Thing enemyTarget = pawn.mindState.enemyTarget;
			bool allowManualCastWeapons = !pawn.IsColonist;
			Verb verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
			if (verb == null)
			{
				dest = IntVec3.Invalid;
				return false;
			}
			CastPositionRequest newReq = default;
			newReq.caster = pawn;
			newReq.target = enemyTarget;
			newReq.verb = verb;
			newReq.maxRangeFromTarget = verb.verbProps.range;
			newReq.wantCoverFromTarget = verb.verbProps.range > 5f;
			return CastPositionFinder.TryFindCastPosition(newReq, out dest);
		}
	}
}
