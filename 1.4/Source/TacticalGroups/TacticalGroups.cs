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

        public List<PawnGroup> pawnGroups;

        public Dictionary<Caravan, CaravanGroup> caravanGroups;

        public Dictionary<Map, ColonyGroup> colonyGroups;

        public HashSet<Pawn> visiblePawns;

        public override void WorldComponentUpdate()
        {
            base.WorldComponentUpdate();
            if (Event.current.keyCode == TacticDefOf.TG_SlaveMenu.defaultKeyCodeB && Event.current.control)
            {
                if (Find.CurrentMap != null && colonyGroups.TryGetValue(Find.CurrentMap, out var colonyGroup))
                {
                    if (Find.WindowStack.WindowOfType<SlaveMenu>() is null)
                    {
                        Vector2 initialSize = new Vector2(Textures.SlaveMenu.width, Textures.SlaveMenu.height);
                        var windowRect = new Rect(((float)UI.screenWidth - initialSize.x) / 2f, ((float)UI.screenHeight - initialSize.y) / 2f, initialSize.x, initialSize.y);
                        windowRect = windowRect.Rounded();

                        SlaveMenu floatMenu = new SlaveMenu(null, colonyGroup, windowRect, Textures.SlaveMenu, Strings.Slaves);
                        Find.WindowStack.Add(floatMenu);
                    }
                }
            }
        }
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
                var colonyGroup = colonyGroups.Values.ElementAt(num);
                foreach (var pawn in caravan.pawns.InnerListForReading)
                {
                    colonyGroup.Disband(pawn);
                }
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
            if (this.colonyGroups is null)
            {
                this.colonyGroups = new Dictionary<Map, ColonyGroup>();
            }
            if (this.pawnGroups is null)
            {
                this.pawnGroups = new List<PawnGroup>();
            }
            if (this.colonyGroups.TryGetValue(map, out ColonyGroup colonyGroup) && colonyGroup != null)
            {
                colonyGroup.Add(pawns);
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
            RecheckCaravanGroups();
            foreach (var pawnGroup in this.pawnGroups.Where(x => x.Map == map))
            {
                foreach (var pawn in pawns)
                {
                    if (pawnGroup.formerPawns != null && pawnGroup.formerPawns.Contains(pawn))
                    {
                        pawnGroup.Add(pawn);
                    }
                }
            }
            this.colonyGroups[map].UpdateData();
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            return this.colonyGroups[map];
        }

        public void RecheckCaravanGroups()
        {
            var caravanKeysToRemove = new List<Caravan>();
            foreach (var group in this.caravanGroups)
            {
                if (!group.Key.PawnsListForReading.Any(x => group.Value.pawns.Contains(x)))
                {
                    caravanKeysToRemove.Add(group.Key);
                }
            }

            foreach (var key in caravanKeysToRemove)
            {
                caravanGroups.Remove(key);
            }
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
                if (colonyGroups.TryGetValue(key, out var colonyGroup))
                {
                    foreach (var pawn in pawns)
                    {
                        if (colonyGroup.pawns?.Contains(pawn) ?? false)
                        {
                            colonyGroup.Disband(pawn);
                        }
                    }
                    if (colonyGroup.pawns is null || colonyGroup.pawns.Count == 0)
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
            TacticalGroupsSettings.InitColorBars();
        }
        public void RemoveAllNullPawns()
        {
            try
            {
                if (pawnGroups != null)
                {
                    for (int num = pawnGroups.Count - 1; num >= 0; num--)
                    {
                        var group = pawnGroups[num];
                        if (group.pawns != null)
                        {
                            for (int num2 = group.pawns.Count - 1; num2 >= 0; num2--)
                            {
                                var pawn = group.pawns[num2];
                                if (pawn == null)
                                {
                                    group.pawns.RemoveAt(num2);
                                }
                            }
                            if (group.pawns.Count == 0 && group.autoDisbandWithoutPawns)
                            {
                                pawnGroups.RemoveAt(num);
                            }
                        }
                        else if (group.autoDisbandWithoutPawns)
                        {
                            pawnGroups.RemoveAt(num);
                        }
                    }
                }
            }
            catch { }

            try
            {
                var caravanKeysToRemove = new List<Caravan>();
                foreach (var group in caravanGroups)
                {
                    if (group.Value.pawns != null)
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
                    else
                    {
                        caravanKeysToRemove.Add(group.Key);
                    }
                }

                foreach (var key in caravanKeysToRemove)
                {
                    caravanGroups.Remove(key);
                }
            }
            catch { }
            try
            {
                var colonyKeysToRemove = new List<Map>();
                foreach (var group in colonyGroups)
                {
                    if (group.Value?.pawns != null)
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
                    else
                    {
                        colonyKeysToRemove.Add(group.Key);
                    }
                }

                foreach (var key in colonyKeysToRemove)
                {
                    colonyGroups.Remove(key);
                }
            }
            catch { }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }


        public override void ExposeData()
        {
            base.ExposeData();
            try
            {
                Scribe_Collections.Look(ref pawnGroups, "pawnGroups", LookMode.Deep);
            }
            catch { }
            try
            {
                Scribe_Collections.Look(ref caravanGroups, "caravanGroups", LookMode.Reference, LookMode.Deep, ref caravanKeys, ref caravanValues);
            }
            catch { }
            try
            {
                Scribe_Collections.Look(ref colonyGroups, "colonyGroups", LookMode.Reference, LookMode.Deep, ref mapKeys, ref groupValues);
            }
            catch { }
            try
            {
                Scribe_Collections.Look(ref visiblePawns, "visiblePawns", LookMode.Reference);
            }
            catch { }
            HarmonyPatches_GroupBills.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                try
                {
                    RemoveAllNullPawns();
                }
                catch { }
            }
        }

        private List<Map> mapKeys;
        private List<ColonyGroup> groupValues;

        private List<Caravan> caravanKeys;
        private List<CaravanGroup> caravanValues;
    }
}
