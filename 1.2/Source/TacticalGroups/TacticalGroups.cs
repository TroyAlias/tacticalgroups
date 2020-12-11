using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class TacticalGroups : WorldComponent
    {
        public TacticalGroups(World world) : base(world)
        {

        }

        private TacticalColonistBar colonistBar;
        public TacticalColonistBar TacticalColonistBar
        {
            get
            {
                if (colonistBar is null)
                {
                    colonistBar = new TacticalColonistBar();
                }
                return colonistBar;
            }
        }

        public List<PawnGroup> pawnGroups;

        public Dictionary<Caravan, CaravanGroup> caravanGroups;

        public Dictionary<Map, ColonyGroup> colonyGroups;

        public void PreInit()
        {
            if (pawnGroups is null) pawnGroups = new List<PawnGroup>();
            if (colonyGroups is null) colonyGroups = new Dictionary<Map, ColonyGroup>();
            if (caravanGroups is null) caravanGroups = new Dictionary<Caravan, CaravanGroup>();
        }

        public void AddGroup(List<Pawn> pawns)
        {
            this.pawnGroups.Insert(0, new PawnGroup(pawns));
        }

        public void AddCaravanGroup(Caravan caravan)
        {
            this.caravanGroups[caravan] = new CaravanGroup(caravan);
            foreach (var colonyGroup in colonyGroups.Values)
            {
                colonyGroup.pawns.RemoveAll(x => caravan.pawns.InnerListForReading.Contains(x));
            }
        }
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            TacticUtils.ResetTacticGroups();
            PreInit();
            MedicalCareUtilityGroup.Reset();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pawnGroups, "pawnGroups", LookMode.Deep);
            Scribe_Collections.Look(ref caravanGroups, "caravanGroups", LookMode.Reference, LookMode.Deep, ref caravanKeys, ref caravanValues);
            Scribe_Collections.Look(ref colonyGroups, "colonyGroups", LookMode.Reference, LookMode.Deep, ref mapKeys, ref groupValues);
        }

        private List<Map> mapKeys;
        private List<ColonyGroup> groupValues;

        private List<Caravan> caravanKeys;
        private List<CaravanGroup> caravanValues;
    }
}
