using RimWorld;
using Verse;
using Verse.AI;

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
			return t is Pawn pawn2 && !pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) && GoodLayingStatusForTend(pawn2, pawn)
				&& HealthAIUtility.ShouldBeTendedNowByPlayer(pawn2) && pawn.CanReserve(pawn2, 1, -1, null, forced);
		}

		public static bool GoodLayingStatusForTend(Pawn patient, Pawn doctor)
		{
			if (patient == doctor)
			{
				return true;
			}
			return patient.RaceProps.Humanlike ? patient.InBed() : patient.GetPosture() != PawnPosture.Standing;
		}
		public override Job TryGiveJob(Pawn pawn)
		{
			bool validator(Thing t)
			{
				Pawn patient = (Pawn)t;
				return HasJobOnThing(pawn, patient);
			}
			Pawn pawn2 = (Pawn)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.mapPawns.SpawnedPawnsWithAnyHediff, PathEndMode.OnCell, TraverseParms.For(pawn), radius, validator);

			if (pawn2 != null)
			{
				Thing thing = HealthAIUtility.FindBestMedicine(pawn, pawn2);
				return thing != null ? JobMaker.MakeJob(JobDefOf.TendPatient, pawn2, thing) : JobMaker.MakeJob(JobDefOf.TendPatient, pawn2);
			}
			else
			{
				return null;
			}
		}
	}
}
