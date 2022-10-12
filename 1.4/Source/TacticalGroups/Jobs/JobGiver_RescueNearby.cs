using RimWorld;
using Verse;
using Verse.AI;

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

        public override Job TryGiveJob(Pawn pawn)
        {
            bool validator(Thing t)
            {
                Pawn pawn3 = (Pawn)t;
                return (pawn3.Downed && pawn3.Faction == pawn.Faction
                && !pawn3.InBed() && pawn.CanReserve(pawn3) && !pawn3.IsForbidden(pawn)
                && !GenAI.EnemyIsNear(pawn3, 25f));
            }
            Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn),
                    radius, validator);
            if (pawn2 == null)
            {
                return null;
            }
            Building_Bed building_Bed = RestUtility.FindBedFor(pawn2, pawn, checkSocialProperness: false);
            if (building_Bed == null || !pawn2.CanReserve(building_Bed))
            {
                return null;
            }
            Job job = JobMaker.MakeJob(JobDefOf.Rescue, pawn2, building_Bed);
            job.count = 1;
            return job;
        }

    }
}
