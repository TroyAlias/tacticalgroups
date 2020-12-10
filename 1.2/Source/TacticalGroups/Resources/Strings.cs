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
		public static readonly string DisplayRest = "TG.DisplayFood".Translate();
		public static readonly string DisplayHealth = "TG.DisplayHealth".Translate();
		public static readonly string PawnIconScale = "TG.PawnIconScale".Translate();
		public static readonly string GroupIconScale = "TG.GroupIconScale".Translate();

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

		public static readonly string RescueFallen = "TG.RescueFallen".Translate();
		public static readonly string TendWounded = "TG.TendWounded".Translate();
	}
}
