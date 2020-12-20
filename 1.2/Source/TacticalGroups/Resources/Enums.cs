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

	public enum WorkType
	{
		None,
		Construction,
		Crafting,
		Hauling,
		Cleaning,
		Hunting,
		Cooking,
		Mining,
		WoodChopping,
		Plants,
		ClearSnow,
		Doctor,
		Warden,
		Tailor,
		Smith,
		Handle,
		FireExtinguish,
		Art,
		RescueFallen,
		TendWounded,
		Research
	}
	public enum BreakType
	{
		None,
		Socialize,
		Entertainment,
		ChowHall,
		LightsOut,
	}
}
