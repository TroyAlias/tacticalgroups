using RimWorld;
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
                    colonistBar.UpdateSizes();
                }
                return colonistBar;
            }
        }

        public List<PawnGroup> pawnGroups;

        public Dictionary<Caravan, CaravanGroup> caravanGroups;

        public Dictionary<Map, ColonyGroup> colonyGroups;

        public HashSet<Pawn> visiblePawns;

        public void PreInit()
        {
            if (pawnGroups is null) pawnGroups = new List<PawnGroup>();
            if (colonyGroups is null) colonyGroups = new Dictionary<Map, ColonyGroup>();
            if (caravanGroups is null) caravanGroups = new Dictionary<Caravan, CaravanGroup>();
            if (visiblePawns is null) visiblePawns = new HashSet<Pawn>();
        }

        public void AddGroup(List<Pawn> pawns)
        {
            this.pawnGroups.Insert(0, new PawnGroup(pawns));
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public CaravanGroup AddCaravanGroup(Caravan caravan)
        {
            this.caravanGroups[caravan] = new CaravanGroup(caravan);
            for (int num = colonyGroups.Values.Count - 1; num >= 0; num--)
            {
                colonyGroups.Values.ElementAt(num).Disband(caravan.pawns.InnerListForReading);
            }
            for (int num = pawnGroups.Count - 1; num >= 0; num--)
            {
                pawnGroups[num].Disband(caravan.pawns.InnerListForReading);
            }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            return this.caravanGroups[caravan];
        }

        public void RemoveCaravanGroup(Caravan caravan)
        {
            if (caravanGroups.ContainsKey(caravan))
            {
                foreach (var formerGroup in caravanGroups[caravan].formerGroups)
                {
                    foreach (var pawnGroup in formerGroup.Value.pawnGroups)
                    {
                        var group2 = this.pawnGroups.Where(x => x.groupID == pawnGroup.groupID && x.groupName == pawnGroup.groupName).FirstOrDefault();
                        if (group2 == null)
                        {
                            this.pawnGroups.Add(pawnGroup);
                            pawnGroup.Add(formerGroup.Key);
                        }
                        else
                        {
                            group2.Add(formerGroup.Key);
                        }
                    }
                }
                this.caravanGroups.Remove(caravan);
                TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            }
        }

        public ColonyGroup CreateOrJoinColony(List<Pawn> pawns, Map map)
        {
            if (this.colonyGroups.ContainsKey(map))
            {
                this.colonyGroups[map].Add(pawns);
            }
            else
            {
                var newColony = new ColonyGroup(pawns);
                this.colonyGroups[map] = newColony;
                if (!map.IsPlayerHome || map.ParentFaction != Faction.OfPlayer)
                {
                    newColony.ConvertToTaskForce();
                }
            }

            RemovePawnsFromOtherColonies(this.colonyGroups[map], pawns);
            foreach (var pawnGroup in this.pawnGroups.Where(x => x.Map == map))
            {
                foreach (var pawn in pawns)
                {
                    if (pawnGroup.formerPawns.Contains(pawn))
                    {
                        pawnGroup.Add(pawn);
                    }
                }
            }
            this.colonyGroups[map].UpdateData();
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
                if (colonyGroups.ContainsKey(key))
                {
                    colonyGroups[key].Disband(pawns);
                    if (colonyGroups.ContainsKey(key)
                        && colonyGroups[key].pawns.Count == 0)
                    {
                        colonyGroups.Remove(key);
                    }
                }
            }
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                foreach (var colonyGroup in this.colonyGroups.Values)
                {
                    colonyGroup.UpdateData();
                }
                foreach (var pawnGroup in this.pawnGroups)
                {
                    pawnGroup.UpdateData();
                }
                foreach (var caravanGroup in this.caravanGroups.Values)
                {
                    caravanGroup.UpdateData();
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
            Scribe_Collections.Look(ref visiblePawns, "visiblePawns", LookMode.Reference);
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
                            TacticUtils.pawnsWithGroups[pawn] = new HashSet<ColonistGroup> { group };
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
