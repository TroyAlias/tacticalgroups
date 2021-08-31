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
	internal class ThinkNode_ConditionalAutonomyLords : ThinkNode_Conditional
	{
        protected override bool Satisfied(Pawn pawn)
        {
            var lord = pawn.GetLord();
            if (lord != null)
            {
                var lordJob = lord.LordJob;
                if (lordJob is LordJob_AssaultThingsStrongest || lordJob is LordJob_AssaultThingsWeakest || lordJob is LordJob_AssaultThingsPursueFleeing)
                {
                    if (pawn.Drafted)
                    {
                        return true;
                    }
                    lord.ownedPawns.Remove(pawn);
                    pawn.mindState.duty = null;
                }
            }
            return false;
        }
	}
}
