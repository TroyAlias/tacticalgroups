using System.Collections.Generic;
using Verse;

namespace TacticalGroups
{
    public class FormerGroup : IExposable
    {
		public Pawn pawn;
		public List<PawnGroup> pawnGroups;
		public FormerGroup()
        {

        }

		public FormerGroup(Pawn pawn, List<PawnGroup> pawnGroups)
		{
			this.pawn = pawn;
			this.pawnGroups = pawnGroups;
		}
		public void ExposeData()
        {
			Scribe_References.Look(ref pawn, "pawn");
			Scribe_Collections.Look(ref pawnGroups, "pawnGroups", LookMode.Deep);
		}
	}
}
