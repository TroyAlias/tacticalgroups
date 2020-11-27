using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public class ColonistGroup : IExposable
    {
        public List<Pawn> pawns;
        public ColonistGroup(List<Pawn> pawns)
        {
            this.pawns = pawns;
        }

        public ColonistGroup(Pawn pawn)
        {
            this.pawns = new List<Pawn> { pawn } ;
        }
        public void Add(Pawn pawn)
        {
            this.pawns.Add(pawn);
        }

        public void ExposeData()
        {

        }
    }
}
