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
	public class JobDriver_ExecuteDownedRaiders : JobDriver
	{
		protected Pawn Victim => (Pawn)job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch).FailOn(() => !Victim.Downed);
			Toil execute = new Toil();
			execute.initAction = delegate
			{
				ExecutionUtility.DoExecutionByCut(execute.actor, Victim);
			};
			execute.defaultCompleteMode = ToilCompleteMode.Instant;
			execute.activeSkill = (() => SkillDefOf.Melee);
			yield return execute;
		}
	}
}
