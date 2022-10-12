using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TacticalGroups
{
    internal class ThinkNode_ConditionalAutonomyLords : ThinkNode_Conditional
    {
        public override bool Satisfied(Pawn pawn)
        {
            Lord lord = pawn.GetLord();
            if (lord != null)
            {
                LordJob lordJob = lord.LordJob;
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
