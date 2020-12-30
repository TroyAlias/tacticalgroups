using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	public class PawnIcon : IExposable
    {
		public Pawn pawn;
		public bool isVisibleOnColonistBar;
		public PawnIcon()
        {

        }
		public PawnIcon(Pawn pawn, bool isVisibleOnColonistBar = true)
        {
			this.pawn = pawn;
			this.isVisibleOnColonistBar = isVisibleOnColonistBar;
        }

        public void ExposeData()
        {
			Scribe_References.Look(ref pawn, "pawn");
			Scribe_Values.Look(ref isVisibleOnColonistBar, "visible");
		}
	}
}
