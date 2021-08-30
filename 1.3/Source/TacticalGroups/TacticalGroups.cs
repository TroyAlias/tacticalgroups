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
                if (Find.CurrentMap != null && colonyGroups.TryGetValue(Find.CurrentMap, out ColonyGroup colonyGroup))
                {
                    if (Find.WindowStack.WindowOfType<SlaveMenu>() is null)
                    {
                        Vector2 initialSize = new Vector2(Textures.SlaveMenu.width, Textures.SlaveMenu.height);
                        Rect windowRect = new Rect((UI.screenWidth - initialSize.x) / 2f, (UI.screenHeight - initialSize.y) / 2f, initialSize.x, initialSize.y);
                        windowRect = windowRect.Rounded();

                        SlaveMenu floatMenu = new SlaveMenu(null, colonyGroup, windowRect, Textures.SlaveMenu, Strings.Slaves);
                        Find.WindowStack.Add(floatMenu);
                    }
                }
            }
        }
        public void PreInit()
        {
            if (pawnGroups is null)
            {
                pawnGroups = new List<PawnGroup>();
            }

            if (colonyGroups is null)
            {
                colonyGroups = new Dictionary<Map, ColonyGroup>();
            }

            if (caravanGroups is null)
            {
                caravanGroups = new Dictionary<Caravan, CaravanGroup>();
            }

            if (visiblePawns is null)
            {
                visiblePawns = new HashSet<Pawn>();
            }
        }

        public void AddGroup(List<Pawn> pawns)
        {
            pawnGroups.Insert(0, new PawnGroup(pawns));
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public CaravanGroup AddCaravanGroup(Caravan caravan)
        {
            caravanGroups[caravan] = new CaravanGroup(caravan);
            for (int num = colonyGroups.Values.Count - 1; num >= 0; num--)
            {
                ColonyGroup colonyGroup = colonyGroups.Values.ElementAt(num);
                foreach (Pawn pawn in caravan.pawns.InnerListForReading)
                {
                    colonyGroup.Disband(pawn);
                }
            }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            return caravanGroups[caravan];
        }

        public void RemoveCaravanGroup(Caravan caravan)
        {
            if (caravanGroups.ContainsKey(caravan))
            {
                foreach (KeyValuePair<Pawn, FormerGroup> formerGroup in caravanGroups[caravan].formerGroups)
                {
                    foreach (PawnGroup pawnGroup in formerGroup.Value.pawnGroups)
                    {
                        PawnGroup group2 = pawnGroups.Where(x => x.groupID == pawnGroup.groupID && x.groupName == pawnGroup.groupName).FirstOrDefault();
                        if (group2 == null)
                        {
                            pawnGroups.Add(pawnGroup);
                            pawnGroup.Add(formerGroup.Key);
                        }
                        else
                        {
                            group2.Add(formerGroup.Key);
                        }
                    }
                }
                caravanGroups.Remove(caravan);
                TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            }
        }

        public ColonyGroup CreateOrJoinColony(List<Pawn> pawns, Map map)
        {
            if (colonyGroups is null)
            {
                colonyGroups = new Dictionary<Map, ColonyGroup>();
            }
            if (pawnGroups is null)
            {
                pawnGroups = new List<PawnGroup>();
            }
            if (colonyGroups.TryGetValue(map, out ColonyGroup colonyGroup) && colonyGroup != null)
            {
                colonyGroup.Add(pawns);
            }
            else
            {
                ColonyGroup newColony = new ColonyGroup(pawns);
                colonyGroups[map] = newColony;
                if (!map.IsPlayerHome || map.ParentFaction != Faction.OfPlayer)
                {
                    newColony.ConvertToTaskForce();
                }
            }

            RemovePawnsFromOtherColonies(colonyGroups[map], pawns);
            RecheckCaravanGroups();
            foreach (PawnGroup pawnGroup in pawnGroups.Where(x => x.Map == map))
            {
                foreach (Pawn pawn in pawns)
                {
                    if (pawnGroup.formerPawns != null && pawnGroup.formerPawns.Contains(pawn))
                    {
                        pawnGroup.Add(pawn);
                    }
                }
            }
            colonyGroups[map].UpdateData();
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            return colonyGroups[map];
        }

        public void RecheckCaravanGroups()
        {
            List<Caravan> caravanKeysToRemove = new List<Caravan>();
            foreach (KeyValuePair<Caravan, CaravanGroup> group in caravanGroups)
            {
                if (!group.Key.PawnsListForReading.Any(x => group.Value.pawns.Contains(x)))
                {
                    caravanKeysToRemove.Add(group.Key);
                }
            }

            foreach (Caravan key in caravanKeysToRemove)
            {
                caravanGroups.Remove(key);
            }
        }
        public void RemovePawnsFromOtherColonies(ColonyGroup mainGroup, List<Pawn> pawns)
        {
            List<Map> colonyKeysToRemove = new List<Map>();
            foreach (KeyValuePair<Map, ColonyGroup> group in colonyGroups)
            {
                if (group.Value != mainGroup)
                {
                    colonyKeysToRemove.Add(group.Key);
                }
            }

            foreach (Map key in colonyKeysToRemove)
            {
                if (colonyGroups.TryGetValue(key, out ColonyGroup colonyGroup))
                {
                    foreach (Pawn pawn in pawns)
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
                foreach (ColonyGroup colonyGroup in colonyGroups.Values)
                {
                    colonyGroup.UpdateData();
                }
                foreach (PawnGroup pawnGroup in pawnGroups)
                {
                    pawnGroup.UpdateData();
                }
                foreach (CaravanGroup caravanGroup in caravanGroups.Values)
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
                        PawnGroup group = pawnGroups[num];
                        if (group.pawns != null)
                        {
                            for (int num2 = group.pawns.Count - 1; num2 >= 0; num2--)
                            {
                                Pawn pawn = group.pawns[num2];
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
                List<Caravan> caravanKeysToRemove = new List<Caravan>();
                foreach (KeyValuePair<Caravan, CaravanGroup> group in caravanGroups)
                {
                    if (group.Value.pawns != null)
                    {
                        for (int num = group.Value.pawns.Count - 1; num >= 0; num--)
                        {
                            Pawn pawn = group.Value.pawns[num];
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

                foreach (Caravan key in caravanKeysToRemove)
                {
                    caravanGroups.Remove(key);
                }
            }
            catch { }
            try
            {
                List<Map> colonyKeysToRemove = new List<Map>();
                foreach (KeyValuePair<Map, ColonyGroup> group in colonyGroups)
                {
                    if (group.Value?.pawns != null)
                    {
                        for (int num = group.Value.pawns.Count - 1; num >= 0; num--)
                        {
                            Pawn pawn = group.Value.pawns[num];
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

                foreach (Map key in colonyKeysToRemove)
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
