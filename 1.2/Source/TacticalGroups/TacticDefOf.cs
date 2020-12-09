using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	[DefOf]
	public static class TacticDefOf
	{
		public static DutyDef AssaultThingsStrongest;
		public static DutyDef AssaultThingsWeakest;
		public static DutyDef AssaultThingsFleeing;

		public static SoundDef TG_RenameSFX;
		public static SoundDef TG_RallySFX;
		public static SoundDef TG_BattleStationsSFX;
		public static SoundDef TG_MenuButtonOpenMenus;
		public static SoundDef TG_ClickSFX;
		public static SoundDef TG_HoverSFX;

		public static PawnColumnDef Age;
		public static PawnColumnDef AllowedArea;
		public static PawnColumnDef AllowedAreaWide;
		public static PawnColumnDef DrugPolicy;
		public static PawnColumnDef FoodRestriction;
		public static PawnColumnDef HostilityResponse;
		public static PawnColumnDef Label;
		public static PawnColumnDef Master;
		public static PawnColumnDef MedicalCare;
		public static PawnColumnDef Outfit;
		public static PawnColumnDef Timetable;

		public static PawnTableDef Work;
		public static PawnTableDef Assign;
		public static PawnTableDef Restrict;
		public static PawnTableDef Animals;
		public static PawnTableDef Wildlife;

	}
}
