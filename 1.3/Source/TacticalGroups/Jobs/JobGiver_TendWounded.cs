using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public class JobGiver_TendWounded : ThinkNode_JobGiver
	{
		private float radius = 30f;

		private const float MinDistFromEnemy = 25f;

		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_TendWounded obj = (JobGiver_TendWounded)base.DeepCopy(resolve);
			obj.radius = radius;
			return obj;
		}

		public bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			if (pawn2 == null || pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) || !GoodLayingStatusForTend(pawn2, pawn) 
				|| !HealthAIUtility.ShouldBeTendedNowByPlayer(pawn2) || !pawn.CanReserve(pawn2, 1, -1, null, forced))
			{
				return false;
			}
			return true;
		}

		public static bool GoodLayingStatusForTend(Pawn patient, Pawn doctor)
		{
			if (patient == doctor)
			{
				return true;
			}
			if (patient.RaceProps.Humanlike)
			{
				return patient.InBed();
			}
			return patient.GetPosture() != PawnPosture.Standing;
		}
		protected override Job TryGiveJob(Pawn pawn)
		{
			Predicate <Thing> validator = delegate (Thing t)
			{
				Pawn patient = (Pawn)t;
				return HasJobOnThing(pawn, patient);
			};
			Pawn pawn2 = (Pawn)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.mapPawns.SpawnedPawnsWithAnyHediff, PathEndMode.OnCell, TraverseParms.For(pawn), radius, validator);

			if (pawn2 != null)
            {
				Thing thing = HealthAIUtility.FindBestMedicine(pawn, pawn2);
				if (thing != null)
				{
					return JobMaker.MakeJob(JobDefOf.TendPatient, pawn2, thing);
				}
				return JobMaker.MakeJob(JobDefOf.TendPatient, pawn2);
			}
			else
            {
				return null;
            }
		}
	}
}
