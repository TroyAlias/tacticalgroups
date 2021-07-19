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
		public static readonly string Work = "TG.Work".Translate();
		public static readonly string Orders = "TG.Orders".Translate();
		public static readonly string Manage = "TG.Manage".Translate();

		public static readonly string Group = "TG.Group".Translate();
		public static readonly string Colony = "TG.Colony".Translate();
		public static readonly string Caravan = "TG.Caravan".Translate();
		public static readonly string TaskForce = "TG.TaskForce".Translate();

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
		public static readonly string DisplayBreakRiskOverlay = "TG.DisplayBreakRiskOverlay".Translate();
		public static readonly string HidePawnsWhenOffMap = "TG.HidePawnsWhenOffMap".Translate();
		public static readonly string HideGroups = "TG.HideGroups".Translate();
		public static readonly string HideCreateGroup = "TG.HideCreateGroup".Translate();
		public static readonly string HideCreateGroupDesc = "TG.HideCreateGroupDesc".Translate();

		public static readonly string Attack = "TG.Attack".Translate();
		public static readonly string Regroup = "TG.Regroup".Translate();
		public static readonly string BattleStations = "TG.BattleStations".Translate();
		public static readonly string Set = "TG.Set".Translate();
		public static readonly string Clear = "TG.Clear".Translate();
		public static readonly string Send = "TG.Send".Translate();
		public static readonly string Unload = "TG.Unload".Translate();
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
		public static readonly string WorkTaskTooltipTailor = "TG.WorkTaskTooltipTailor".Translate();
		public static readonly string WorkTaskTooltipSmith = "TG.WorkTaskTooltipSmith".Translate();
		public static readonly string WorkTaskTooltipHandle = "TG.WorkTaskTooltipHandle".Translate();
		public static readonly string WorkTaskTooltipFireExtinguish = "TG.WorkTaskTooltipFireExtinguish".Translate();
		public static readonly string WorkTaskTooltipArt = "TG.WorkTaskTooltipArt".Translate();
		public static readonly string WorkTaskTooltipResearch = "TG.WorkTaskTooltipResearch".Translate();
		public static readonly string WorkTaskTooltipResearchMenu = "TG.WorkTaskTooltipResearchMenu".Translate();
		public static readonly string ForcedLaborTooltip = "TG.ForcedLaborTooltip".Translate();
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
		public static readonly string TakeBuffTooltip = "TG.TakeBuffTooltip".Translate();
		public static readonly string MoodIconTooltip = "TG.MoodIconTooltip".Translate();
		public static readonly string HealthIconTooltip = "TG.HealthIconTooltip".Translate();
		public static readonly string RestIconTooltip = "TG.RestIconTooltip".Translate();
		public static readonly string HungerIconTooltip = "TG.HungerIconTooltip".Translate();
		public static readonly string Diplomacy = "TG.Diplomacy".Translate();

		public static readonly string ColonistBarPositionY = "TG.ColonistBarPositionY".Translate();
		public static readonly string ColonistBarPositionX = "TG.ColonistBarPositionX".Translate();
		public static readonly string ColonistBarSpacing = "TG.ColonistBarSpacing".Translate();
		public static readonly string GroupRowCount = "TG.GroupRowCount".Translate();
		public static readonly string OverallPawnDisplayScale = "TG.OverallPawnDisplayScale".Translate();
		public static readonly string ColonyGroupScale = "TG.ColonyGroupScale".Translate();
		public static readonly string GroupScale = "TG.GroupScale".Translate();
		public static readonly string PawnNeedsSize = "TG.PawnNeedsSize".Translate();
		public static readonly string WeaponOverlayInside = "TG.WeaponOverlayInside".Translate();
		public static readonly string WeaponOverlayUnder = "TG.WeaponOverlayUnder".Translate();
		public static readonly string WeaponOverlayPlacement = "TG.WeaponOverlayPlacement".Translate();
		public static readonly string DisableLabelBackground = "TG.DisableLabelBackground".Translate();

		public static readonly string GroupHideOptionsTooltip = "TG.GroupHideOptionsTooltip".Translate();
		public static readonly string HideGroupPawnDotsOptionsTooltip = "TG.HideGroupPawnDotsOptionsTooltip".Translate();
		public static readonly string HideGroupHealthAlertOverlayOptionsTooltip = "TG.HideGroupHealthAlertOverlayOptionsTooltip".Translate();
		public static readonly string HideWeaponOverlayOptionsTooltip = "TG.HideWeaponOverlayOptionsTooltip".Translate();
		public static readonly string BanishPawnTooltip = "TG.BanishPawnTooltip".Translate();
		public static readonly string HidePawnGroupOptionsTooltip = "TG.HidePawnGroupOptionsTooltip".Translate();
		public static readonly string PresetMenuOverlayOptionsTooltip = "TG.PresetMenuOverlayOptionsTooltip".Translate();
		public static readonly string PresetLabel = "TG.PresetLabel".Translate();
		public static readonly string GroupsLabel = "TG.GroupsLabel".Translate();
		public static readonly string CreateNew = "TG.CreateNew".Translate();
		public static readonly string CreateNewTooltip = "TG.CreateNewTooltip".Translate();
		public static readonly string TrashCanTooltip = "TG.TrashCanTooltip".Translate();
		public static readonly string SavePresetTooltip = "TG.SavePresetTooltip".Translate();
		public static readonly string ActivatePresetTooltip = "TG.ActivatePresetTooltip".Translate();
		public static readonly string DisactivatePresetTooltip = "TG.DisactivatePresetTooltip".Translate();

		public static readonly string CreateNewPreset = "TG.CreateNewPreset".Translate();
		public static readonly string RemovePreset = "TG.RemovePreset".Translate();
		public static readonly string SavePreset = "TG.SavePreset".Translate();


		public static readonly string ColorBarModeLabel = "TG.ColorBarModeLabel".Translate();
		public static readonly string ColorBarModeDefault = "TG.ColorBarModeDefault".Translate();
		public static readonly string ColorBarModeExtended = "TG.ColorBarModeExtended".Translate();

		public static readonly string WeaponModeShowDrafted = "TG.WeaponModeShowDrafted".Translate();
		public static readonly string WeaponModeShowAlways = "TG.WeaponModeShowAlways".Translate();

		public static readonly string ActivityIconFormingCaravan = "ActivityIconFormingCaravan".Translate();
		public static readonly string ActivityIconMedicalRest = "ActivityIconMedicalRest".Translate();
		public static readonly string ActivityIconSleeping = "ActivityIconSleeping".Translate();
		public static readonly string ActivityIconFleeing = "ActivityIconFleeing".Translate();
		public static readonly string ActivityIconAttacking = "ActivityIconAttacking".Translate();
		public static readonly string ActivityIconIdle = "ActivityIconIdle".Translate();
		public static readonly string ActivityIconBurning = "ActivityIconBurning".Translate();

		public static readonly string GroupAreaTooltip = "TG.GroupAreaTooltip".Translate();
		public static readonly string GroupOutfitTooltip = "TG.GroupOutfitTooltip".Translate();
		public static readonly string GroupDrugsTooltip = "TG.GroupDrugsTooltip".Translate();
		public static readonly string GroupFoodTooltip = "TG.GroupFoodTooltip".Translate();

		public static readonly string PresetWorkPrioritiesTooltip = "TG.PresetWorkPrioritiesTooltip".Translate();
		public static readonly string PresetActiveWorkStatesTooltip = "TG.PresetActiveWorkStatesTooltip".Translate();
		public static readonly string PresetAreaTooltip = "TG.PresetAreaTooltip".Translate();
		public static readonly string PresetDrugPolicyTooltip = "TG.PresetDrugPolicyTooltip".Translate();
		public static readonly string PresetFoodPolicyTooltip = "TG.PresetFoodPolicyTooltip".Translate();
		public static readonly string PresetOutfitPolicyTooltip = "TG.PresetOutfitPolicyTooltip".Translate();
		public static readonly string Apply = "TG.Apply".Translate();
		public static readonly string ResetGroupTitle = "TG.ResetGroupTitle".Translate();
		public static readonly string ResetGroupTooltip = "TG.ResetGroupTooltip".Translate();

		public static readonly string TravelSupplies = "TG.TravelSupplies".Translate();
		public static readonly string Bedrolls = "TG.Bedrolls".Translate();

		public static readonly string CaptureTooltip = "TG.CaptureTooltip".Translate();
		public static readonly string ExecuteTooltip = "TG.ExecuteTooltip".Translate();

		public static readonly string PrisonerMenuTooltip = "TG.PrisonerMenuTooltip".Translate();
		public static readonly string AnimalMenuTooltip = "TG.AnimalMenuTooltip".Translate();
		public static readonly string GuestMenuTooltip = "TG.GuestMenuTooltip".Translate();
		public static readonly string Prisoners = "TG.Prisoners".Translate();
		public static readonly string Animals = "TG.Animals".Translate();
		public static readonly string Guests = "TG.Guests".Translate();

		public static readonly string ColorBarModeDefaultTooltip = "TG.ColorBarModeDefaultTooltip".Translate();
		public static readonly string ColorBarModeExtendedTooltip = "TG.ColorBarModeExtendedTooltip".Translate();
		public static readonly string Pawn = "TG.Pawn".Translate();
		public static readonly string Box = "TG.Box".Translate();
		public static readonly string ResetToDefault = "TG.ResetToDefault".Translate();
		public static readonly string ResetPawnView = "TG.ResetPawnView".Translate();

	}
}
