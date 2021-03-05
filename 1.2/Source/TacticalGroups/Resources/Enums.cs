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
        Skills,
		Name
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
		Research,
		UnloadCaravan
	}
	public enum BreakType
	{
		None,
		Socialize,
		Entertainment,
		ChowHall,
		LightsOut,
	}

	public enum WorkState
    {
		Inactive,
		Active,
		ForcedLabor,
		Temporary
    }
	public enum ColorBarMode
	{
		Default,
		Extended
	}

	public enum WeaponShowMode
	{
		Drafted,
		Always
	}

	public enum PawnState
    {
		MentalState,
		IsDownedOrIncapable,
		IsBleeding,
		Sick,
		Inspired,
		None
	}
}

