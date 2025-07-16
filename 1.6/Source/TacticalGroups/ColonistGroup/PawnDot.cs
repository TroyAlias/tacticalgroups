using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public class PawnDot
	{
		public Pawn pawn;
		public Rect rect;
		public PawnState state;
		public PawnDot(Pawn pawn, Rect rect, PawnState state)
		{
			this.pawn = pawn;
			this.rect = rect;
			this.state = state;
		}
	}
}
