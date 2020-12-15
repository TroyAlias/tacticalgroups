using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	public static class Strings
	{
		public static readonly string Rally = "TG.Rally".Translate();
		public static readonly string Actions = "TG.Actions".Translate();
		public static readonly string Orders = "TG.Orders".Translate();
		public static readonly string Manage = "TG.Manage".Translate();

		public static readonly string Group = "TG.Group".Translate();
		public static readonly string Colony = "TG.Colony".Translate();
		public static readonly string Caravan = "TG.Caravan".Translate();

		public static readonly string GetToWork = "TG.GetToWork".Translate();
		public static readonly string TakeABreak = "TG.TakeABreak".Translate();
		public static readonly string ChowHall = "TG.ChowHall".Translate();
		public static readonly string SocialRelax = "TG.SocialRelax".Translate();
		public static readonly string HaveSomeFun = "TG.HaveSomeFun".Translate();
		public static readonly string Clean = "TG.Clean".Translate();
		public static readonly string Haul = "TG.Haul".Translate();
		public static readonly string VisitMedical = "TG.VisitMedical".Translate();

		public static readonly string Rename = "TG.Rename".Translate();
		public static readonly string Icon = "TG.Icon".Translate();
		public static readonly string SortGroup = "TG.SortGroup".Translate();

		public static readonly string Disband = "TG.Disband".Translate();
		public static readonly string DisbandPawn = "TG.DisbandPawn".Translate();
		public static readonly string DisbandGroup = "TG.DisbandGroup".Translate();

		public static readonly string LookBusy = "TG.LookBusy".Translate();


		public static readonly string ShowAllColonists = "TG.ShowAllColonists".Translate();
		public static readonly string DisplayFood = "TG.DisplayFood".Translate();
		public static readonly string DisplayRest = "TG.DisplayRest".Translate();
		public static readonly string DisplayHealth = "TG.DisplayHealth".Translate();
		public static readonly string DisplayWeapons = "TG.DisplayWeapons".Translate();
		public static readonly string DisplayColorBars = "TG.DisplayColorBars".Translate();
		public static readonly string HidePawnsWhenOffMap = "TG.HidePawnsWhenOffMap".Translate();

		public static readonly string Attack = "TG.Attack".Translate();
		public static readonly string Regroup = "TG.Regroup".Translate();
		public static readonly string BattleStations = "TG.BattleStations".Translate();
		public static readonly string Set = "TG.Set".Translate();
		public static readonly string Clear = "TG.Clear".Translate();
		public static readonly string Formation = "TG.Formation".Translate();
		public static readonly string Medical = "TG.Medical".Translate();

		public static readonly string FireAtWill = "TG.FireAtWill".Translate();
		public static readonly string Strongest = "TG.Strongest".Translate();
		public static readonly string Weakest = "TG.Weakest".Translate();
		public static readonly string PursueFleeing = "TG.PursueFleeing".Translate();
		public static readonly string Management = "TG.Management".Translate();

		public static readonly string Happy = "TG.Happy".Translate();
		public static readonly string Okay = "TG.Okay".Translate();
		public static readonly string Sad = "TG.Sad".Translate();

		public static readonly string ReportToMedical = "TG.ReportToMedical".Translate();
		public static readonly string RescueFallen = "TG.RescueFallen".Translate();
		public static readonly string TendWounded = "TG.TendWounded".Translate();
		public static readonly string TakeFive = "TG.TakeFive".Translate();

		public static readonly string CreateGroupTooltip = "TG.CreateGroupTooltip".Translate();
		public static readonly string OptionsGearTooltip = "TG.OptionsGearTooltip".Translate();
		public static readonly string LookBusyTooltip = "TG.LookBusyTooltip".Translate();

		public static readonly string WorkTaskTooltipConstruction = "TG.WorkTaskTooltipConstruction".Translate();
		public static readonly string WorkTaskTooltipCraft = "TG.WorkTaskTooltipCraft".Translate();
		public static readonly string WorkTaskTooltipHaul = "TG.WorkTaskTooltipHaul".Translate();
		public static readonly string WorkTaskTooltipClean = "TG.WorkTaskTooltipClean".Translate();
		public static readonly string WorkTaskTooltipHunt = "TG.WorkTaskTooltipHunt".Translate();
		public static readonly string WorkTaskTooltipCook = "TG.WorkTaskTooltipCook".Translate();
		public static readonly string WorkTaskTooltipMine = "TG.WorkTaskTooltipMine".Translate();
		public static readonly string WorkTaskTooltipChopWood = "TG.WorkTaskTooltipChopWood".Translate();
		public static readonly string WorkTaskTooltipFarm = "TG.WorkTaskTooltipFarm".Translate();
		public static readonly string WorkTaskTooltipClearSnow = "TG.WorkTaskTooltipClearSnow".Translate();
		public static readonly string WorkTaskTooltipDoctor = "TG.WorkTaskTooltipDoctor".Translate();
		public static readonly string WorkTaskTooltipWarden = "TG.WorkTaskTooltipWarden".Translate();

		public static readonly string TakeFiveTooltip = "TG.TakeFiveTooltip".Translate();
		public static readonly string SocialRelaxTooltip = "TG.SocialRelaxTooltip".Translate();
		public static readonly string EntertainmentTooltip = "TG.EntertainmentTooltip".Translate();
		public static readonly string ChowHallToolTip = "TG.ChowHallToolTip".Translate();
		public static readonly string SleepTooltip = "TG.SleepTooltip".Translate();
		public static readonly string RallyToolTip = "TG.RallyToolTip".Translate();
		public static readonly string ShootersToolTip = "TG.ShootersToolTip".Translate();
		public static readonly string MeleeToolTip = "TG.MeleeToolTip".Translate();
		public static readonly string ShowHideTooltip = "TG.ShowHideTooltip".Translate();
		public static readonly string AddColonistTooltip = "TG.AddColonistTooltip".Translate();
		public static readonly string MiddlePawnCountToolTip = "TG.MiddlePawnCountToolTip".Translate();
		public static readonly string RegroupTooltip = "TG.RegroupTooltip".Translate();
		public static readonly string BattleStationsTooltip = "TG.BattleStationsTooltip".Translate();
		public static readonly string BattleStationsSetTooltip = "TG.BattleStationsSetTooltip".Translate();
		public static readonly string BattleStationsClearTooltip = "TG.BattleStationsClearTooltip".Translate();
		public static readonly string FireAtWillTooltip = "TG.FireAtWillTooltip".Translate();
		public static readonly string StrongestTooltip = "TG.StrongestTooltip".Translate();
		public static readonly string WeakestTooltip = "TG.WeakestTooltip".Translate();
		public static readonly string PursueFleeingTooltip = "TG.PursueFleeingTooltip".Translate();
		public static readonly string ReportToMedicalTooltip = "TG.ReportToMedicalTooltip".Translate();
		public static readonly string RescueDownedTooltip = "TG.RescueDownedTooltip".Translate();
		public static readonly string TendWoundedTooltip = "TG.TendWoundedTooltip".Translate();
		public static readonly string ArmorTooltip = "TG.ArmorTooltip".Translate();
		public static readonly string DPSTooltip = "TG.DPSTooltip".Translate();
		public static readonly string RankTooltip = "TG.RankTooltip".Translate();
		public static readonly string UpgradeWeaponTooltip = "TG.UpgradeWeaponTooltip".Translate();
		public static readonly string UpgradeArmorTooltip = "TG.UpgradeArmorTooltip".Translate();
		public static readonly string MoodIconTooltip = "TG.MoodIconTooltip".Translate();
		public static readonly string HealthIconTooltip = "TG.HealthIconTooltip".Translate();
		public static readonly string RestIconTooltip = "TG.RestIconTooltip".Translate();
		public static readonly string HungerIconTooltip = "TG.HungerIconTooltip".Translate();
	}
}
