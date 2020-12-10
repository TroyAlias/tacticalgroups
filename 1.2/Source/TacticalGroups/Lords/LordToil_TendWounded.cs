using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace TacticalGroups
{
	public class LordToil_TendWounded : LordToil
	{
		public const int UpdateIntervalTicks = 300;
		public override bool ForceHighStoryDanger => true;
		public override bool AllowSatisfyLongNeeds => false;

		public LordToil_TendWounded()
		{
		}
		public override void Notify_ReachedDutyLocation(Pawn pawn)
		{
			UpdateAllDuties();
		}

		public override void UpdateAllDuties()
		{
			for (int i = 0; i < lord.ownedPawns.Count; i++)
			{
				PawnDuty duty = lord.ownedPawns[i].mindState.duty;
				if (duty == null || duty.def != TacticDefOf.TG_TendWounded)
				{
					lord.ownedPawns[i].mindState.duty = new PawnDuty(TacticDefOf.TG_TendWounded);
				}
			}
		}

		public override void LordToilTick()
		{
			if (lord.ticksInToil % 300 == 0)
			{
				UpdateAllDuties();
			}
		}
	}
}
