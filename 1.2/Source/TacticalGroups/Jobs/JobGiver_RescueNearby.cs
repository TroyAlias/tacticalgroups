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
	public class JobGiver_RescueNearby : ThinkNode_JobGiver
	{
		private float radius = 30f;

		private const float MinDistFromEnemy = 25f;
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_RescueNearby obj = (JobGiver_RescueNearby)base.DeepCopy(resolve);
			obj.radius = radius;
			return obj;
		}

        protected override Job TryGiveJob(Pawn pawn)
        {
            Predicate<Thing> validator = delegate (Thing t)
            {
                Pawn pawn3 = (Pawn)t;
                return (pawn3.Downed && pawn3.Faction == pawn.Faction
                && !pawn3.InBed() && pawn.CanReserve(pawn3) && !pawn3.IsForbidden(pawn)
                && !GenAI.EnemyIsNear(pawn3, 25f)) ? true : false;
            };
            Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn),
                    radius, validator);
            if (pawn2 == null)
            {
                Log.Message(" - TryGiveJob - return null; - 6", true);
                return null;
            }
            Building_Bed building_Bed = RestUtility.FindBedFor(pawn2, pawn, pawn2.HostFaction == pawn.Faction, checkSocialProperness: false);
            Log.Message(" - TryGiveJob - if (building_Bed == null || !pawn2.CanReserve(building_Bed)) - 8", true);
            if (building_Bed == null || !pawn2.CanReserve(building_Bed))
            {
                Log.Message(" - TryGiveJob - return null; - 9", true);
                return null;
            }
            Job job = JobMaker.MakeJob(JobDefOf.Rescue, pawn2, building_Bed);
            Log.Message(" - TryGiveJob - job.count = 1; - 11", true);
            job.count = 1;
            Log.Message(pawn + " got " + job + " for " + pawn2 + " - " + building_Bed);
            Log.Message(" - TryGiveJob - return job; - 13", true);
            return job;
        }

	}
}
