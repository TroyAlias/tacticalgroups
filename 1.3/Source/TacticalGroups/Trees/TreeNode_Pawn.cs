using System.Collections.Generic;
using Verse;

namespace TacticalGroups
{
    public class TreeNode_Pawns : TreeNode
    {
        public List<Pawn> pawns;

        private string prefix;
        public string Label => prefix + (pawns != null ? " (" + pawns.Count.ToString() + ")" : " (0)");
        public TreeNode_Pawns(List<Pawn> pawns, string prefix)
        {
            this.pawns = pawns;
            this.prefix = prefix;
        }
    }
}
