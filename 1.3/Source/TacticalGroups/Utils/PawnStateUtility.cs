using System.Collections.Generic;
using Verse;

namespace TacticalGroups
{
    public class PawnStateCache
    {
        public PawnStateCache()
        {

        }
        public HashSet<PawnState> pawnStates;
        public int updateCount;
    }

    public static class PawnStateUtility
    {
        private static Dictionary<Pawn, PawnStateCache> pawnStates = new Dictionary<Pawn, PawnStateCache>();
        public static HashSet<PawnState> GetAllPawnStatesCache(Pawn pawn)
        {
            if (pawnStates.TryGetValue(pawn, out PawnStateCache pawnStateCache))
            {
                if (pawnStateCache.updateCount == 0)
                {
                    pawnStateCache.pawnStates = GetAllPawnStates(pawn);
                    pawnStateCache.updateCount = 60;
                }
                pawnStateCache.updateCount--;
                return pawnStateCache.pawnStates;
            }
            else
            {
                pawnStateCache = new PawnStateCache
                {
                    pawnStates = GetAllPawnStates(pawn)
                };
                pawnStates[pawn] = pawnStateCache;
                return pawnStateCache.pawnStates;
            }
        }

        public static PawnState GetPawnState(Pawn pawn)
        {
            if (pawn.MentalState != null)
            {
                return PawnState.MentalState;
            }
            else if (pawn.IsDownedOrIncapable())
            {
                return PawnState.IsDownedOrIncapable;
            }
            else if (pawn.IsBleeding())
            {
                return PawnState.IsBleeding;
            }
            else if (pawn.IsSick())
            {
                return PawnState.Sick;
            }
            else if (pawn.Inspired)
            {
                return PawnState.Inspired;
            }
            else
            {
                return PawnState.None;
            }
        }

        public static HashSet<PawnState> GetAllPawnStates(Pawn pawn)
        {
            HashSet<PawnState> newStates = new HashSet<PawnState>();
            if (pawn.IsBleeding())
            {
                newStates.Add(PawnState.IsBleeding);
            }
            return newStates;
        }
    }
}
