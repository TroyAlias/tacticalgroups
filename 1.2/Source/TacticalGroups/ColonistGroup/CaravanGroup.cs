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
		private Caravan caravan;
        public override void Init()
        {
            base.Init();
			this.groupIcon = Textures.Caravan_GroupIcon;
			this.groupIconFolder = "CaravanGroup";
			this.groupIconName = "Caravan_GroupIcon";
			this.defaultGroupName = Strings.Caravan;
			this.pawnRowCount = 3;
			this.pawnDocRowCount = 8;
			this.entireGroupIsVisible = true;
			this.updateIcon = true;
		}
		public CaravanGroup()
		{
			this.Init();
		}

		public CaravanGroup(Caravan caravan)
        {
			this.Init();
			this.pawns = new List<Pawn>();
			foreach (var pawn in caravan.PawnsListForReading)
			{
				this.pawns.Add(pawn);
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

        public override void Disband(Pawn pawn)
        {
            base.Disband(pawn);
			if (this.pawns.Count == 0)
            {
				TacticUtils.TacticalGroups.caravanGroups.Remove(caravan);
			}
		}
        public override void Disband()
        {
            base.Disband();
			TacticUtils.TacticalGroups.caravanGroups.Remove(caravan);
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_References.Look(ref caravan, "caravan");
        }
    }
}
