using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	public class JobDriver_ExecuteDownedRaiders : JobDriver
	{
		protected Pawn Victim => (Pawn)job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
		}
		public override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch).FailOn(() => !Victim.Downed);
			Toil execute = new Toil();
			execute.initAction = delegate
			{
				ExecutionUtility.DoExecutionByCut(execute.actor, Victim);
			};
			execute.defaultCompleteMode = ToilCompleteMode.Instant;
			execute.activeSkill = () => SkillDefOf.Melee;
			yield return execute;
		}
	}
}
