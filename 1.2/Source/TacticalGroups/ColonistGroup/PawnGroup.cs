using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
    public class PawnGroup : ColonistGroup
	{
        public override void Init()
        {
            base.Init();
			this.groupIcon = Textures.GroupIcon_Default;
			this.pawnRowCount = 3;
			this.pawnRowXPosShift = 2f;
			this.defaultGroupName = Strings.Group;
			this.updateIcon = true;
		}
		public PawnGroup()
		{
			this.Init();
		}

		public PawnGroup(List<Pawn> pawns)
        {
			this.Init();
			this.pawns = new List<Pawn>();
			foreach (var pawn in pawns)
            {
				this.pawns.Add(pawn);
				this.pawnIcons[pawn] = new PawnIcon(pawn);
			}
			this.groupID = TacticUtils.TacticalGroups.pawnGroups.Count + 1;
		}

		public PawnGroup(Pawn pawn)
        {
			this.Init();
			this.pawns = new List<Pawn> { pawn } ;
			this.pawnIcons = new Dictionary<Pawn, PawnIcon> { { pawn, new PawnIcon(pawn) } };
			this.groupID = TacticUtils.TacticalGroups.pawnGroups.Count + 1;
		}

        public override void Disband()
        {
            base.Disband();
			TacticUtils.TacticalGroups.pawnGroups.Remove(this);
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
