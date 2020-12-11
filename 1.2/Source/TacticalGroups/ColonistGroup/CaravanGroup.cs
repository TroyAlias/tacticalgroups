using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
    public class CaravanGroup : ColonistGroup
	{
        public override void Init()
        {
            base.Init();
			this.groupIcon = Textures.Caravan_GroupIcon;
			this.groupIconFolder = "CaravanGroup";
			this.groupIconName = "Caravan_GroupIcon";
			this.defaultGroupName = Strings.Caravan;
			this.updateIcon = true;
		}
		public CaravanGroup()
		{
			this.Init();
		}

		public CaravanGroup(List<Pawn> pawns)
        {
			this.Init();
			this.pawns = pawns;
			foreach (var pawn in pawns)
            {
				this.pawnIcons[pawn] = new PawnIcon(pawn);
			}
			this.groupID = TacticUtils.TacticalGroups.caravanGroups.Count + 1;
		}

		public CaravanGroup(Pawn pawn)
        {
			this.Init();
			this.pawns = new List<Pawn> { pawn } ;
			this.pawnIcons = new Dictionary<Pawn, PawnIcon> { { pawn, new PawnIcon(pawn) } };
			this.groupID = TacticUtils.TacticalGroups.caravanGroups.Count + 1;
		}

		public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
