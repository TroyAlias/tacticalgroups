using RimWorld.Planet;
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
    public class CaravanGroup : ColonistGroup
    {
        public override Map Map => null;
        private Caravan caravan;

        public Dictionary<Pawn, FormerGroup> formerGroups;
        public override void Init()
        {
            base.Init();
            groupBannerFolder = "CaravanGroup";
            groupIcon = Textures.CaravanGroupIcon_Default;
            groupBanner = Textures.PawnGroupBanner_Default;
            groupIconName = "Caravan_GroupIcon";
            defaultGroupName = Strings.Caravan;
            pawnRowCount = 3;
            pawnDocRowCount = 8;
            updateIcon = true;
        }
        public CaravanGroup()
        {
            Init();
        }

        public CaravanGroup(Caravan caravan)
        {
            Init();
            pawns = new List<Pawn>();
            formerGroups = new Dictionary<Pawn, FormerGroup>();
            foreach (Pawn pawn in caravan.PawnsListForReading)
            {
                foreach (PawnGroup pawnGroup in TacticUtils.AllPawnGroups)
                {
                    if (pawnGroup.pawns.Contains(pawn))
                    {
                        if (formerGroups.ContainsKey(pawn))
                        {
                            formerGroups[pawn].pawnGroups.Add(pawnGroup);
                        }
                        else
                        {
                            formerGroups[pawn] = new FormerGroup(pawn, new List<PawnGroup> { pawnGroup });
                        }
                    }
                }
                Add(pawn);
            }
            groupID = TacticUtils.TacticalGroups.caravanGroups.Count + 1;
            curGroupName = defaultGroupName + " " + groupID;
        }

        public override List<Pawn> ActivePawns => pawns;
        public override List<Pawn> VisiblePawns => pawns;

        public CaravanGroup(Pawn pawn)
        {
            Init();
            Add(pawn);
            groupID = TacticUtils.TacticalGroups.caravanGroups.Count + 1;
            curGroupName = defaultGroupName + " " + groupID;
        }

        public override void Disband(Pawn pawn)
        {
            base.Disband(pawn);
            if (pawns.Count == 0)
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
