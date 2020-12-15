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
			this.pawnRowCount = 4;
			this.pawnDocRowCount = 11;
			this.groupIcon = Textures.Default_ColonyIcon;
			this.groupIconFolder = "ColonyBlue";
			this.defaultIconFolder = "ColonyBlue";
			this.colorFolder = "Colony";
			this.groupIconName = "Default_ColonyIcon";
			this.defaultGroupName = Strings.Colony;
			this.entireGroupIsVisible = true;
			this.updateIcon = true;
		}
		public ColonyGroup()
		{
			this.Init();
		}
		public ColonyGroup(List<Pawn> pawns)
        {
			this.Init();
			this.pawns = new List<Pawn>();
			foreach (var pawn in pawns)
            {
				if (this.Map == null)
				{
					this.Map = pawn.Map;
				}
				this.pawns.Add(pawn);
				this.pawnIcons[pawn] = new PawnIcon(pawn);
			}
			this.groupID = TacticUtils.TacticalGroups.colonyGroups.Count + 1;

		}
		public ColonyGroup(Pawn pawn)
        {
			this.Init();
			if (this.Map == null)
			{
				this.Map = pawn.Map;
			}
			this.pawns = new List<Pawn> { pawn } ;
			this.pawnIcons = new Dictionary<Pawn, PawnIcon> { { pawn, new PawnIcon(pawn) } };
			this.groupID = TacticUtils.TacticalGroups.colonyGroups.Count + 1;
		}

		public override void Disband()
		{
			base.Disband();
			TacticUtils.TacticalGroups.colonyGroups.Remove(this.Map);
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}
		public override void ExposeData()
        {
            base.ExposeData();
        }

        public override string ToString()
        {
			return GetGroupName() + " - " + this.pawns.Count;
		}
	}
}
