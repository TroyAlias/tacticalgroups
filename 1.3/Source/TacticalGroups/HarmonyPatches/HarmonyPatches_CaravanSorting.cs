using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TacticalGroups
{
    public static class HarmonyPatches_CaravanSorting
    {
        private static List<Pawn> PawnsInCurrentSections; // For Giddy-up! Caravan compatibility
        public static bool AddPawnsSections(TransferableOneWayWidget widget, List<TransferableOneWay> transferables)
        {
            PawnsInCurrentSections = new List<Pawn>(); // For Giddy-up! Caravan compatibility

            // Get all pawns from the list of transferables
            List<Pawn> pawns = new List<Pawn>();
            Dictionary<Pawn, TransferableOneWay> pawnThing = new Dictionary<Pawn, TransferableOneWay>();
            foreach (TransferableOneWay thing in transferables)
            {
                if (thing.ThingDef.category == ThingCategory.Pawn)
                {
                    Pawn pawn = (Pawn)thing.AnyThing;
                    pawns.Add(pawn);
                    pawnThing[pawn] = thing;
                }
            }

            List<PawnGroup> pawnGroups = TacticUtils.AllPawnGroups;
            pawnGroups.Reverse(); // Reverse the list because it iterates in the wrong direction

            // Create a HashSet to sort out all the ungrouped pawns
            HashSet<Pawn> ungroupedPawns = new HashSet<Pawn>(pawns);

            foreach (PawnGroup pawnGroup in pawnGroups)
            {
                if (pawnGroup.pawns != null && pawnGroup.pawns.Count > 0)
                {
                    // Remove grouped pawns from the ungroupedPawns HashSet
                    ungroupedPawns.ExceptWith(from pawn in pawnGroup.pawns select pawn);

                    // Get only the group's pawns that are in the current list of transferables
                    List<TransferableOneWay> sectionTranferables = new List<TransferableOneWay>();
                    foreach (Pawn pawn in pawnGroup.pawns)
                    {
                        if (pawns.Contains(pawn) && pawnThing.ContainsKey(pawn) && pawn.IsFreeColonist)
                        {
                            sectionTranferables.Add(pawnThing[pawn]);
                            if (!PawnsInCurrentSections.Contains(pawn))
                            {
                                PawnsInCurrentSections.Add(pawn); // For Giddy-up! Caravan compatibility
                            }
                        }
                    }

                    if (sectionTranferables.Count > 0)
                    {
                        // Add a new section containing all pawns within the group
                        widget.AddSection(pawnGroup.curGroupName ?? "", from pawn in sectionTranferables select pawn);
                    }
                }
            }

            if (ungroupedPawns.Count > 0)
            {
                // Create a section containing all the ungrouped pawns
                widget.AddSection("Ungrouped " + "ColonistsSection".Translate().ToLower(), from pawn in ungroupedPawns
                                                                                           where pawn.IsFreeColonist
                                                                                           select pawnThing[pawn]);
                foreach (Pawn pawn in ungroupedPawns)
                {
                    if (pawn.IsFreeColonist && !PawnsInCurrentSections.Contains(pawn))
                    {
                        PawnsInCurrentSections.Add(pawn); // For Giddy-up! Caravan compatibility
                    }
                }
            }

            // We then return to vanilla code (slightly tweaked), commenting out the Colonists section of course
            /*
            widget.AddSection("ColonistsSection".Translate(), from pawn in pawns
                                                              where pawn.IsFreeColonist
                                                              select pawnThing[pawn]);
            */

            widget.AddSection("PrisonersSection".Translate(), from pawn in pawns
                                                              where pawn.IsPrisoner
                                                              select pawnThing[pawn]);

            widget.AddSection("CaptureSection".Translate(), from pawn in pawns
                                                            where pawn.Downed && CaravanUtility.ShouldAutoCapture(pawn, Faction.OfPlayer)
                                                            select pawnThing[pawn]);

            widget.AddSection("AnimalsSection".Translate(), from pawn in pawns
                                                            where pawn.RaceProps.Animal
                                                            select pawnThing[pawn]);
            return false;
        }

        public static bool HandleAnimal(ref List<Pawn> pawns)
        {
            if (PawnsInCurrentSections != null)
            {
                pawns = PawnsInCurrentSections; // Fixes Giddy-up! Caravan rider selection functionality
            }
            return true;
        }
    }
}
