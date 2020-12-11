using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
    public class ColonyGroup : ColonistGroup
	{
        public override void Init()
        {
            base.Init();
			this.groupIcon = Textures.Default_ColonyIcon;
			this.pawnRowCount = 4;
			this.groupIconFolder = "ColonyBlue";
			this.groupIconName = "Default_ColonyIcon";
			this.updateIcon = true;
		}
		public ColonyGroup()
		{
			base.Init();
		}
		public ColonyGroup(List<Pawn> pawns)
        {
			this.Init();
			this.pawns = pawns;
			foreach (var pawn in pawns)
            {
				this.pawnIcons[pawn] = new PawnIcon(pawn);
			}
		}
		public ColonyGroup(Pawn pawn)
        {
			this.Init();
			this.pawns = new List<Pawn> { pawn } ;
			this.pawnIcons = new Dictionary<Pawn, PawnIcon> { { pawn, new PawnIcon(pawn) } };
		}
        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
