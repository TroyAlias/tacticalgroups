using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TacticalGroups
{
    public static class HarmonyPatches_CaravanSorting
    {
        private static List<Pawn> PawnsInCurrentSections;
        public static bool AddPawnsSections(TransferableOneWayWidget widget, List<TransferableOneWay> transferables)
        {
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
                    if (sectionPawns.Any(pawn => pawn.IsFreeNonSlaveColonist))
                    {
                        widget.AddSection(pawnGroup.curGroupName, from pawn in sectionPawns
                                                                  where pawn.IsFreeNonSlaveColonist
                                                                  select pawnTransferable[pawn]);
                    }

                    // Add a new section containing all slave pawns within the group
                    if (ModsConfig.IdeologyActive && sectionPawns.Any(pawn => pawn.IsSlave))
                    {
                        widget.AddSection("TG.GroupSlavesSection".Translate(pawnGroup.curGroupName), from pawn in sectionPawns
                                                                                                where pawn.IsSlave
                                                                                                select pawnTransferable[pawn]);
                    }
                }
            }

            // Create a section containing all the ungrouped pawns
            if (ungroupedPawns.Any(pawn => pawn.IsFreeNonSlaveColonist))
            {
                widget.AddSection("TG.UngroupedColonistsSection".Translate(), from pawn in ungroupedPawns
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
                widget.AddSection("TG.UngroupedSlavesSection".Translate(), from pawn in ungroupedPawns
                                                                           where pawn.IsSlave
                                                                           select pawnTransferable[pawn]);
            }


            // We then return to vanilla code
            /*
            widget.AddSection("ColonistsSection".Translate(), from pawn in pawns
                                                              where pawn.IsFreeNonSlaveColonist
                                                              select pawnTransferable[pawn]);

            if (ModsConfig.IdeologyActive)
            {
                widget.AddSection("SlavesSection".Translate(), from pawn in pawns
                                                               where pawn.IsSlave
                                                               select pawnTransferable[pawn]);
            }
            */

            widget.AddSection("PrisonersSection".Translate(), from pawn in pawns
                                                              where pawn.IsPrisoner
                                                              select pawnTransferable[pawn]);

            widget.AddSection("CaptureSection".Translate(), from pawn in pawns
                                                            where pawn.Downed && CaravanUtility.ShouldAutoCapture(pawn, Faction.OfPlayer)
                                                            select pawnTransferable[pawn]);

            widget.AddSection("AnimalsSection".Translate(), from pawn in pawns
                                                            where pawn.RaceProps.Animal
                                                            select pawnTransferable[pawn]);
            return false;
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
