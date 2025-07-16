using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TacticalGroups
{
    public static class HarmonyPatches_CaravanSorting
    {
        private static List<Pawn> PawnsInCurrentSections;
        private static List<TransferableOneWay> addedTransferables;
        public static void AddPawnsSectionsPrefix(TransferableOneWayWidget widget, ref List<TransferableOneWay> transferables)
        {
            addedTransferables = new List<TransferableOneWay>();
            if (ModCompatibility.GiddyUpCaravanIsActive)
            {
                PawnsInCurrentSections = new List<Pawn>();
            }

            // Get all pawns from the list of transferables
            List<Pawn> pawns = new List<Pawn>();
            Dictionary<Pawn, TransferableOneWay> pawnTransferable = new Dictionary<Pawn, TransferableOneWay>();
            foreach (TransferableOneWay transferable in transferables)
            {
                if (transferable.ThingDef.category == ThingCategory.Pawn)
                {
                    Pawn pawn = (Pawn)transferable.AnyThing;
                    pawns.Add(pawn);
                    pawnTransferable[pawn] = transferable;
                }
            }

            List<PawnGroup> pawnGroups = TacticUtils.AllPawnGroups;
            pawnGroups.Reverse(); // Reverse the list because it iterates in the wrong direction

            // Create a HashSet to sort out all the ungrouped pawns
            HashSet<Pawn> ungroupedPawns = new HashSet<Pawn>(pawns);

            foreach (PawnGroup pawnGroup in pawnGroups)
            {
                if (!(pawnGroup.pawns is null) && pawnGroup.pawns.Count > 0)
                {
                    // Remove grouped pawns from the ungroupedPawns HashSet
                    ungroupedPawns.ExceptWith(from pawn in pawnGroup.pawns select pawn);

                    // Get only the group's pawns that are in the current list of transferables
                    List<Pawn> sectionPawns = new List<Pawn>();
                    sectionPawns.AddRange(from pawn in pawnGroup.pawns
                                          where pawns.Contains(pawn)
                                          select pawn);

                    if (ModCompatibility.GiddyUpCaravanIsActive)
                    {
                        PawnsInCurrentSections.AddRange(from pawn in sectionPawns.Except(PawnsInCurrentSections)
                                                        where pawn.IsFreeNonSlaveColonist
                                                        select pawn);
                    }

                    // Add a new section containing all pawns within the group
                    if (sectionPawns.Any(pawn => pawn.IsFreeNonSlaveColonist || pawn.IsPlayerMechanoid()))
                    {
                        widget.AddSectionAndRemovePawnsFromOtherSections(pawnGroup.curGroupName, from pawn in sectionPawns
                                                                                                 where pawn.IsFreeNonSlaveColonist || pawn.IsPlayerMechanoid()
                                                                                                 select pawnTransferable[pawn]);
                    }

                    // Add a new section containing all slave pawns within the group
                    if (ModsConfig.IdeologyActive && sectionPawns.Any(pawn => pawn.IsSlave))
                    {
                        widget.AddSectionAndRemovePawnsFromOtherSections("TG.GroupSlavesSection".Translate(pawnGroup.curGroupName), from pawn in sectionPawns
                                                                                                                                    where pawn.IsSlave
                                                                                                                                    select pawnTransferable[pawn]);
                    }
                }
            }

            // Create a section containing all the ungrouped pawns
            if (ungroupedPawns.Any(pawn => pawn.IsFreeNonSlaveColonist))
            {
                widget.AddSectionAndRemovePawnsFromOtherSections("TG.UngroupedColonistsSection".Translate(), from pawn in ungroupedPawns
                                                                                                             where pawn.IsFreeNonSlaveColonist
                                                                                                             select pawnTransferable[pawn]);

                if (ModCompatibility.GiddyUpCaravanIsActive)
                {
                    PawnsInCurrentSections.AddRange(from pawn in ungroupedPawns.Except(PawnsInCurrentSections)
                                                    where pawn.IsFreeNonSlaveColonist
                                                    select pawn);
                }
            }

            // Create a section containing all the ungrouped slave pawns
            if (ModsConfig.IdeologyActive && ungroupedPawns.Any(pawn => pawn.IsSlave))
            {
                widget.AddSectionAndRemovePawnsFromOtherSections("TG.UngroupedSlavesSection".Translate(), from pawn in ungroupedPawns
                                                                                                          where pawn.IsSlave
                                                                                                          select pawnTransferable[pawn]);
            }
            transferables = transferables.Where(x => addedTransferables.Contains(x) is false).ToList();
        }

        public static void AddSectionAndRemovePawnsFromOtherSections(this TransferableOneWayWidget widget, string title, IEnumerable<TransferableOneWay> transferables)
        {
            addedTransferables.AddRange(transferables);
            widget.AddSection(title, transferables);
        }

        public static bool HandleAnimal(ref List<Pawn> pawns)
        {
            if (!(PawnsInCurrentSections is null))
            {
                pawns = PawnsInCurrentSections; // Fixes Giddy-up! Caravan rider selection functionality
            }
            return true;
        }
    }
}
