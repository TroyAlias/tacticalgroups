using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

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
    public class CaravanGroup : ColonistGroup
	{
        public override Map Map => null;
        private Caravan caravan;

		public Dictionary<Pawn, FormerGroup> formerGroups;
        public override void Init()
        {
            base.Init();
			this.groupBannerFolder = "CaravanGroup";
			this.groupIcon = Textures.CaravanGroupIcon_Default;
			this.groupBanner = Textures.PawnGroupBanner_Default;
			this.groupIconName = "Caravan_GroupIcon";
			this.defaultGroupName = Strings.Caravan;
            this.pawnRowCount = 3;
			this.pawnDocRowCount = 8;
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
            this.formerGroups = new Dictionary<Pawn, FormerGroup>();
            foreach (var pawn in caravan.PawnsListForReading)
            {
                foreach (var pawnGroup in TacticUtils.AllPawnGroups)
                {
                    if (pawnGroup.pawns.Contains(pawn))
                    {
                        if (this.formerGroups.ContainsKey(pawn))
                        {
                            this.formerGroups[pawn].pawnGroups.Add(pawnGroup);
                        }
                        else
                        {
                            this.formerGroups[pawn] = new FormerGroup(pawn, new List<PawnGroup> { pawnGroup });
                        }
                    }
                }
                this.Add(pawn);
            }
            this.groupID = TacticUtils.TacticalGroups.caravanGroups.Count + 1;
            this.curGroupName = this.defaultGroupName + " " + this.groupID;
        }

        public override List<Pawn> ActivePawns => this.pawns;
        public override List<Pawn> VisiblePawns => this.pawns;

        public CaravanGroup(Pawn pawn)
        {
			this.Init();
            this.Add(pawn);
			this.groupID = TacticUtils.TacticalGroups.caravanGroups.Count + 1;
            this.curGroupName = this.defaultGroupName + " " + this.groupID;
        }

        public override void Disband(Pawn pawn)
        {
            base.Disband(pawn);
			if (this.pawns.Count == 0)
            {
				TacticUtils.TacticalGroups.caravanGroups.Remove(caravan);
			}
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}
        public override void Disband()
        {
			TacticUtils.TacticalGroups.caravanGroups.Remove(caravan);
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_References.Look(ref caravan, "caravan");
			Scribe_Collections.Look(ref formerGroups, "formerGroups", LookMode.Reference, LookMode.Deep, ref pawnKeys, ref formerGroupValues);
        }

		private List<Pawn> pawnKeys;
		private List<FormerGroup> formerGroupValues;
    }
}
