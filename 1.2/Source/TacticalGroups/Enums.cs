using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public enum CombatSearchMode
    {
        Default,
        Strongest,
        Weakest,
        PursueFleeing
    }

    public enum SortBy
    {
        None,
        Skills
    }
}
