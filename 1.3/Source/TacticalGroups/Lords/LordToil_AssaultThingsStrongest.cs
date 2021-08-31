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
	public class LordToil_AssaultThingsStrongest : LordToil
	{
		private List<Thing> things;

		public const int UpdateIntervalTicks = 300;

		public override bool ForceHighStoryDanger => true;

		public override bool AllowSatisfyLongNeeds => false;

		public LordToil_AssaultThingsStrongest(IEnumerable<Thing> things)
		{
			this.things = new List<Thing>(things);
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
                if (duty == null || duty.def != TacticDefOf.TG_AssaultThingsStrongest || duty.focus.ThingDestroyed)
                {
                    if (things.Where((Thing t) => t?.Spawned ?? false).TryRandomElement(out Thing result))
                    {
                        lord.ownedPawns[i].mindState.duty = new PawnDuty(TacticDefOf.TG_AssaultThingsStrongest, result);
                    }
                    else
                    {
                        lord.ownedPawns[i].mindState.duty = new PawnDuty(TacticDefOf.TG_AssaultThingsStrongest);
                    }
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
