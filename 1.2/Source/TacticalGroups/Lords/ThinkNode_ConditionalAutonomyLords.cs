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
                Log.Message(pawn + " - Satisfied - var lordJob = lord.LordJob; - 3", true);
                var lordJob = lord.LordJob;
                Log.Message(pawn + " - Satisfied - if (lordJob is LordJob_AssaultThingsStrongest || lordJob is LordJob_AssaultThingsWeakest || lordJob is LordJob_AssaultThingsPursueFleeing) - 4", true);
                if (lordJob is LordJob_AssaultThingsStrongest || lordJob is LordJob_AssaultThingsWeakest || lordJob is LordJob_AssaultThingsPursueFleeing)
                {
                    Log.Message(pawn + " - Satisfied - if (pawn.Drafted) - 5", true);
                    if (pawn.Drafted)
                    {
                        Log.Message(pawn + " - Satisfied - return true; - 6", true);
                        return true;
                    }
                    lord.ownedPawns.Remove(pawn);
                    Log.Message(pawn + " - Satisfied - pawn.mindState.duty = null; - 8", true);
                    pawn.mindState.duty = null;
                }
                else if (lordJob is LordJob_RescueFallen || lordJob is LordJob_TendWounded)
                {
                    Log.Message(pawn + " - Satisfied - return true; - 10", true);
                    return true;
                }
            }
            Log.Message(pawn + " - Satisfied - return null; - 10", true);
            return false;
        }
	}
}
