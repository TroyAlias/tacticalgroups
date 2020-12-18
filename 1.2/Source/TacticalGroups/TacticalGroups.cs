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
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public void AddCaravanGroup(Caravan caravan)
        {
            this.caravanGroups[caravan] = new CaravanGroup(caravan);
            foreach (var colonyGroup in colonyGroups.Values)
            {
                colonyGroup.Disband(caravan.pawns.InnerListForReading);
            }
            foreach (var pawnGroup in pawnGroups)
            {
                pawnGroup.Disband(caravan.pawns.InnerListForReading);
            }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public void RemoveCaravanGroup(Caravan caravan)
        {
            this.caravanGroups.Remove(caravan);
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public ColonyGroup CreateOrJoinColony(List<Pawn> pawns, Map map)
        {
            if (this.colonyGroups.ContainsKey(map))
            {
                this.colonyGroups[map].Add(pawns);
            }
            else
            {
                this.colonyGroups[map] = new ColonyGroup(pawns);
            }
            RemovePawnsFromOtherColonies(this.colonyGroups[map], pawns);
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            return this.colonyGroups[map];
        }

        public ColonyGroup CreateOrJoinColony(Pawn pawn, Map map)
        {
            if (this.colonyGroups.ContainsKey(map))
            {
                this.colonyGroups[map].Add(pawn);
            }
            else
            {
                this.colonyGroups[map] = new ColonyGroup(pawn);
            }
            RemovePawnsFromOtherColonies(this.colonyGroups[map], new List<Pawn> { pawn });
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            return this.colonyGroups[map];
        }

        public void RemovePawnsFromOtherColonies(ColonyGroup mainGroup, List<Pawn> pawns)
        {
            var colonyKeysToRemove = new List<Map>();
            foreach (var group in this.colonyGroups)
            {
                if (group.Value != mainGroup)
                {
                    colonyKeysToRemove.Add(group.Key);
                }
            }

            foreach (var key in colonyKeysToRemove)
            {
                colonyGroups[key].Disband(pawns);
                if (colonyGroups[key].pawns.Count == 0)
                {
                    colonyGroups.Remove(key);
                }
            }
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            TacticUtils.ResetTacticGroups();
            PreInit();
            MedicalCareUtilityGroup.Reset();
        }
        public void RemoveAllNullPawns()
        {
            for (int num = pawnGroups.Count - 1; num >= 0; num--)
            {
                var group = pawnGroups[num];
                for (int num2 = pawnGroups[num].pawns.Count - 1; num2 >= 0; num2--)
                {
                    var pawn = pawnGroups[num].pawns[num2];
                    if (pawn == null)
                    {
                        Log.Message("Remove 4");
            
                        group.pawns.RemoveAt(num2);
                    }
                }
                if (group.pawns.Count == 0)
                {
                    pawnGroups.RemoveAt(num);
                }
            }
            
            var caravanKeysToRemove = new List<Caravan>();
            foreach (var group in caravanGroups)
            {
                for (int num = group.Value.pawns.Count - 1; num >= 0; num--)
                {
                    var pawn = group.Value.pawns[num];
                    if (pawn == null)
                    {
                        Log.Message("Remove 5");
            
                        group.Value.pawns.RemoveAt(num);
                    }
                }
                if (group.Value.pawns.Count == 0)
                {
                    caravanKeysToRemove.Add(group.Key);
                }
            }
            
            foreach (var key in caravanKeysToRemove)
            {
                caravanGroups.Remove(key);
            }
            
            var colonyKeysToRemove = new List<Map>();
            foreach (var group in colonyGroups)
            {
                for (int num = group.Value.pawns.Count - 1; num >= 0; num--)
                {
                    var pawn = group.Value.pawns[num];
                    if (pawn == null)
                    {
                        Log.Message("Remove 6");
            
                        group.Value.pawns.RemoveAt(num);
                    }
                }
                if (group.Value.pawns.Count == 0)
                {
                    colonyKeysToRemove.Add(group.Key);
                }
            }
            
            foreach (var key in colonyKeysToRemove)
            {
                colonyGroups.Remove(key);
            }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pawnGroups, "pawnGroups", LookMode.Deep);
            Scribe_Collections.Look(ref caravanGroups, "caravanGroups", LookMode.Reference, LookMode.Deep, ref caravanKeys, ref caravanValues);
            Scribe_Collections.Look(ref colonyGroups, "colonyGroups", LookMode.Reference, LookMode.Deep, ref mapKeys, ref groupValues);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                RemoveAllNullPawns();
                foreach (var group in TacticUtils.AllGroups)
                {
                    foreach (var pawn in group.pawns)
                    {
                        if (TacticUtils.pawnsWithGroups.ContainsKey(pawn))
                        {
                            TacticUtils.pawnsWithGroups[pawn].Add(group);
                        }
                        else
                        {
                            TacticUtils.pawnsWithGroups[pawn] = new List<ColonistGroup> { group };
                        }
                    }
                }
            }
        }

        private List<Map> mapKeys;
        private List<ColonyGroup> groupValues;

        private List<Caravan> caravanKeys;
        private List<CaravanGroup> caravanValues;
    }
}
