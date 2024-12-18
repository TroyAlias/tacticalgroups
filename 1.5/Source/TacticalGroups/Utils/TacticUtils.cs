using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	public static class TacticUtils
	{
		private static TacticalGroups tacticalGroups;
		public static TacticalGroups TacticalGroups
		{
			get
			{
				if (tacticalGroups == null && Find.World != null)
				{
					tacticalGroups = Find.World.GetComponent<TacticalGroups>();
					return tacticalGroups;
				}
				return tacticalGroups;
			}
		}

		public static List<ColonistGroup> AllGroups
		{
			get
			{
				List<ColonistGroup> list = new List<ColonistGroup>();
				TacticalGroups tacticalGroups = TacticalGroups;
				if (tacticalGroups != null)
				{
					if (tacticalGroups.pawnGroups != null)
					{
						list.AddRange(tacticalGroups.pawnGroups);
					}
					if (tacticalGroups.caravanGroups != null)
					{
						list.AddRange(tacticalGroups.caravanGroups.Values);
					}
					if (tacticalGroups.colonyGroups != null)
					{
						list.AddRange(tacticalGroups.colonyGroups.Values);
					}
				}

				return list;
			}
		}

		public static List<ColonyGroup> AllColonyGroups
		{
			get
			{
				List<ColonyGroup> list = new List<ColonyGroup>();
				list.AddRange(TacticalGroups.colonyGroups.Values);
				return list;
			}
		}

		public static List<CaravanGroup> AllCaravanGroups
		{
			get
			{
				List<CaravanGroup> list = new List<CaravanGroup>();
				TacticalGroups.caravanGroups.RemoveAll(x => !x.Key.pawns.Any || x.Key.Destroyed || !x.Key.PawnsListForReading.Any(y => x.Value.pawns.Contains(y)));
				if (TacticalGroups.caravanGroups.Any())
				{
					list.AddRange(TacticalGroups.caravanGroups.Values);
				}
				return list;
			}
		}

		public static List<PawnGroup> AllPawnGroups
		{
			get
			{
				List<PawnGroup> list = new List<PawnGroup>();
				list.AddRange(TacticalGroups.pawnGroups);
				return list;
			}
		}

		public static bool IsPlayerMechanoid(this Pawn pawn)
		{
			return pawn.RaceProps.IsMechanoid && pawn.Faction == Faction.OfPlayer;
		}
		public static void ForceUpdateGroups()
		{
			foreach (PawnGroup group in TacticalGroups.pawnGroups)
			{
				group.UpdateData();
			}
			foreach (CaravanGroup group in TacticalGroups.caravanGroups.Values)
			{
				group.UpdateData();
			}
			foreach (ColonyGroup group in TacticalGroups.colonyGroups.Values)
			{
				group.UpdateData();
			}
		}

		private static readonly Dictionary<Pawn, HashSet<ColonistGroup>> pawnsWithGroups = new Dictionary<Pawn, HashSet<ColonistGroup>>();

		public static void RemoveReferencesFor(ColonistGroup group)
		{
			foreach (KeyValuePair<Pawn, HashSet<ColonistGroup>> cachedData in pawnsWithGroups)
			{
				if (cachedData.Value.Contains(group))
				{
					cachedData.Value.Remove(group);
				}
			}
		}
		public static void RegisterGroupFor(Pawn pawn, ColonistGroup group)
		{
			if (pawnsWithGroups.ContainsKey(pawn))
			{
				pawnsWithGroups[pawn].Add(group);
			}
			else
			{
				pawnsWithGroups[pawn] = new HashSet<ColonistGroup> { group };
			}
		}
		public static bool TryGetGroups(this Pawn pawn, out HashSet<ColonistGroup> groups)
		{
			if (pawnsWithGroups.TryGetValue(pawn, out HashSet<ColonistGroup> value))
			{
				groups = value;
				return true;
			}
			groups = new HashSet<ColonistGroup>();
			return false;
		}

		public static HashSet<ColonistGroup> SortPawnGroupFirst(this HashSet<ColonistGroup> groups)
		{
			HashSet<ColonistGroup> colonistGroups = groups.Where(x => x is PawnGroup).ToHashSet();
			colonistGroups.AddRange(groups.Where(x => !(x is PawnGroup)).ToHashSet());
			return colonistGroups;
		}
		public static List<PawnGroup> GetAllPawnGroupFor(ColonyGroup colonyGroup)
		{
			return TacticalGroups.pawnGroups.Where(x => !x.isSubGroup && (x.pawns.Any(y => y.Map == colonyGroup.Map) || x.prevColonyGroup == colonyGroup)).ToList();
		}
		public static List<PawnGroup> GetAllSubGroupFor(ColonyGroup colonyGroup)
		{
			return TacticalGroups.pawnGroups.Where(x => x.isSubGroup && (x.pawns.Any(y => y.Map == colonyGroup.Map) || x.prevColonyGroup == colonyGroup)).ToList();
		}
		public static void ResetTacticGroups()
		{
			tacticalGroups = Find.World.GetComponent<TacticalGroups>();
			TacticalColonistBar = new TacticalColonistBar();
			TacticalColonistBar.UpdateSizes();
		}

		public static bool IsDownedOrIncapable(this Pawn pawn)
		{
			if (LifeStageUtility.AlwaysDowned(pawn))
			{
				return false;
			}
			if (pawn.Downed)
			{
				return true;
			}
			PawnCapacitiesHandler pawnCapacitiesHandler;
			if (pawn == null)
			{
				pawnCapacitiesHandler = null;
			}
			else
			{
				Pawn_HealthTracker health = pawn.health;
				pawnCapacitiesHandler = health?.capacities;
			}
			PawnCapacitiesHandler pawnCapacitiesHandler2 = pawnCapacitiesHandler;
			return pawnCapacitiesHandler2 == null
|| pawnCapacitiesHandler2.GetLevel(PawnCapacityDefOf.Consciousness) < 0.1f
|| pawnCapacitiesHandler2.GetLevel(PawnCapacityDefOf.Moving) < 0.1f;
		}

		public static bool IsBleeding(this Pawn pawn)
		{
			float num = 0f;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i] is Hediff_Injury hediff_Injury && hediff_Injury.Bleeding)
				{
					num += hediff_Injury.Severity;
				}
			}
			return num > 8f * pawn.RaceProps.baseHealthScale;
		}
		public static bool IsSick(this Pawn pawn)
		{
			return pawn.health.hediffSet.HasImmunizableNotImmuneHediff();
		}

		public static void SelectAll(this ColonistGroup colonistGroup)
		{
			Find.Selector.ClearSelection();
			foreach (Pawn pawn in colonistGroup.ActivePawns)
			{
				Find.Selector.Select(pawn);
			}
		}
		public static void Draft(this ColonistGroup colonistGroup)
		{
			foreach (Pawn pawn in colonistGroup.ActivePawns)
			{
				if (pawn.CanBeDrafted())
				{
					pawn.drafter.Drafted = true;
				}
			}
		}

		public static bool CanBeDrafted(this Pawn pawn)
		{
			return !pawn.Downed && !pawn.InMentalState && pawn.drafter != null && pawn.Faction == Faction.OfPlayer && !pawn.IsPrisoner;
		}

		public static void Undraft(this ColonistGroup colonistGroup)
		{
			foreach (Pawn pawn in colonistGroup.ActivePawns)
			{
				if (pawn.drafter != null)
				{
					pawn.drafter.Drafted = false;
				}
			}
		}

		public static void SwitchToAttackMode(this ColonistGroup colonistGroup)
		{
			foreach (Pawn pawn in colonistGroup.ActivePawns)
			{
				if (pawn.playerSettings != null)
				{
					pawn.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
				}
			}
		}

		public static void RemoveOldLord(this ColonistGroup colonistGroup)
		{
			foreach (Pawn pawn in colonistGroup.ActivePawns)
			{
				Lord lord = pawn.GetLord();
				if (lord != null)
				{
					lord.ownedPawns.Remove(pawn);
					pawn.Map.lordManager.RemoveLord(lord);
				}
			}
		}

		public static void SearchForJob(this ColonistGroup colonistGroup)
		{
			foreach (Pawn pawn in colonistGroup.ActivePawns)
			{
				try
				{
					ThinkResult job = pawn.thinker.MainThinkNodeRoot.TryIssueJobPackage(pawn, default);
					if (job.Job != null)
					{
						pawn.jobs.TryTakeOrderedJob(job.Job);
					}
				}
				catch { };
			}
		}

		public static void SetBattleStations(this ColonistGroup colonistGroup)
		{
			if (colonistGroup.formations is null)
			{
				colonistGroup.formations = new List<Formation>(4);
			}

			Formation formation = colonistGroup.formations.FirstOrDefault(x => x == colonistGroup.activeFormation);
			if (formation != null)
			{
				if (formation.formations is null)
				{
					formation.formations = new Dictionary<Pawn, IntVec3>();
				}

				foreach (Pawn pawn in colonistGroup.ActivePawns)
				{
					if (Find.Selector.SelectedPawns.Contains(pawn))
					{
						formation.formations[pawn] = pawn.Position;
						switch (colonistGroup.formations.IndexOf(formation))
						{
							case 0: formation.colorPrefix = "blue"; break;
							case 1: formation.colorPrefix = "red"; break;
							case 2: formation.colorPrefix = "green"; break;
							case 3: formation.colorPrefix = "yellow"; break;
						}
					}
				}
			}

		}
		public static void ClearBattleStations(this ColonistGroup colonistGroup)
		{
			if (colonistGroup.formations is null)
			{
				colonistGroup.formations = new List<Formation>(4);
			}

			Formation formation = colonistGroup.formations.FirstOrDefault(x => x == colonistGroup.activeFormation);
			if (formation != null)
			{
				if (formation.formations is null)
				{
					formation.formations = new Dictionary<Pawn, IntVec3>();
				}

				foreach (Pawn pawn in colonistGroup.ActivePawns)
				{
					if (pawn != null && formation.formations.ContainsKey(pawn))
					{
						formation.formations.Remove(pawn);
					}
				}
			}
		}

		public static Thing PickBestWeaponFor(Pawn pawn)
		{
			if (pawn.Map == null || pawn.equipment == null || pawn.Faction != Faction.OfPlayer || pawn.IsQuestLodger()
				|| !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || pawn.WorkTagIsDisabled(WorkTags.Violent))
			{
				return null;
			}

			bool isBrawler = pawn.story?.traits?.HasTrait(TraitDefOf.Brawler) ?? false;
			bool preferRanged = !isBrawler && (!pawn.equipment.Primary?.def.IsMeleeWeapon ?? (false || pawn.equipment.Primary == null));

			bool validator(Thing t)
			{
				if (!t.def.IsWeapon)
				{
					return false;
				}
				if (t.IsForbidden(pawn))
				{
					return false;
				}
				if (isBrawler && t.def.IsRangedWeapon)
				{
					return false;
				}
				if (preferRanged && t.def.IsMeleeWeapon)
				{
					return false;
				}
				return (preferRanged || !t.def.IsRangedWeapon)
&& (t.def.weaponTags == null || !t.def.weaponTags.Where(x => x.ToLower().Contains("grenade")).Any())
&& (!t.def.IsRangedWeapon || !t.def.Verbs.Where(x => x.verbClass == typeof(Verb_ShootOneUse)).Any());
			}

			Thing thing = null;
			float maxValue = 0f;
			List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon).Where(x => validator(x)).ToList();

			List<Thing> weapons = pawn.inventory.innerContainer.InnerListForReading.Where(x => validator(x)).ToList();
			list.AddRange(weapons);
			if (pawn.equipment.Primary != null)
			{
				list.Add(pawn.equipment.Primary);
			}
			for (int j = 0; j < list.Count; j++)
			{
				Thing weapon = list[j];
				if (!weapon.IsBurning())
				{
					float weaponScore = WeaponScoreGain(weapon, StatDefOf.AccuracyMedium);
					if (!(weaponScore < 0.05f) && !(weaponScore < maxValue) && (!CompBiocodable.IsBiocoded(weapon) || CompBiocodable.IsBiocodedFor(weapon, pawn))
						&& EquipmentUtility.CanEquip(weapon, pawn) && pawn.CanReserveAndReach(weapon, PathEndMode.OnCell, pawn.NormalMaxDanger()))
					{
						thing = weapon;
						maxValue = weaponScore;
					}
				}
			}
			return thing;
		}
		public static float WeaponScoreGain(Thing weapon)
		{
			if (weapon?.def != null)
			{
				if (weapon.def.IsRangedWeapon)
				{
					VerbProperties verbProperties = weapon.def.Verbs?.Where(x => x.range > 0).FirstOrDefault();
					if (verbProperties?.defaultProjectile?.projectile != null)
					{
						double num = verbProperties.defaultProjectile.projectile.GetDamageAmount(weapon, null) * (float)verbProperties.burstShotCount;
						float num2 = (StatExtension.GetStatValue(weapon, StatDefOf.RangedWeapon_Cooldown, true) + verbProperties.warmupTime) * 60f;
						float num3 = verbProperties.burstShotCount * verbProperties.ticksBetweenBurstShots;
						float num4 = (num2 + num3) / 60f;
						float dps = (float)Math.Round(num / num4, 2);
						return (float)Math.Round(dps, 1);
					}
				}
				else if (weapon.def.IsMeleeWeapon)
				{
					return StatExtension.GetStatValue(weapon, StatDefOf.MeleeWeapon_AverageDPS, true);
				}
			}
			return 0f;
		}
		private static float WeaponScoreGain(Thing weapon, StatDef accuracyDef)
		{
			if (weapon.def.IsRangedWeapon)
			{
				VerbProperties verbProperties = weapon.def.Verbs.Where(x => x.range > 0).First();
				double num = verbProperties.defaultProjectile.projectile.GetDamageAmount(weapon, null) * (float)verbProperties.burstShotCount;
				float num2 = (StatExtension.GetStatValue(weapon, StatDefOf.RangedWeapon_Cooldown, true) + verbProperties.warmupTime) * 60f;
				float num3 = verbProperties.burstShotCount * verbProperties.ticksBetweenBurstShots;
				float num4 = (num2 + num3) / 60f;
				float dps = (float)Math.Round(num / num4, 2);
				float accuracy = StatExtension.GetStatValue(weapon, accuracyDef, true) * 100f;
				return (float)Math.Round(dps * accuracy / 100f, 1);
			}
			else if (weapon.def.IsMeleeWeapon)
			{
				return StatExtension.GetStatValue(weapon, StatDefOf.MeleeWeapon_AverageDPS, true);
			}
			return 0f;
		}

		public static void TrySwitchToWeapon(ThingWithComps newEq, Pawn pawn)
		{
			if (newEq == null || pawn.equipment == null || !pawn.inventory.innerContainer.Contains(newEq))
			{
				return;
			}

			if (newEq.def.stackLimit > 1 && newEq.stackCount > 1)
			{
				newEq = (ThingWithComps)newEq.SplitOff(1);
			}

			if (pawn.equipment.Primary != null)
			{
				if (MassUtility.FreeSpace(pawn) > 0)
				{
					pawn.equipment.TryTransferEquipmentToContainer(pawn.equipment.Primary, pawn.inventory.innerContainer);
				}
				else
				{
					pawn.equipment.MakeRoomFor(newEq);
				}
			}

			pawn.equipment.GetDirectlyHeldThings().TryAddOrTransfer(newEq);
			if (newEq.def.soundInteract != null)
			{
				newEq.def.soundInteract.PlayOneShot(new TargetInfo(pawn.Position, pawn.MapHeld, false));
			}
		}

		private static NeededWarmth neededWarmth;
		private static List<float> wornApparelScores = new List<float>();

		private const int ApparelOptimizeCheckIntervalMin = 6000;

		private const int ApparelOptimizeCheckIntervalMax = 9000;

		private const float MinScoreGainToCare = 0.05f;

		private const float ScoreFactorIfNotReplacing = 10f;

		private static readonly SimpleCurve InsulationColdScoreFactorCurve_NeedWarm = new SimpleCurve
		{
			new CurvePoint(0f, 1f),
			new CurvePoint(30f, 8f)
		};

		private static readonly SimpleCurve HitPointsPercentScoreFactorCurve = new SimpleCurve
		{
			new CurvePoint(0f, 0f),
			new CurvePoint(0.2f, 0.2f),
			new CurvePoint(0.22f, 0.6f),
			new CurvePoint(0.5f, 0.6f),
			new CurvePoint(0.52f, 1f)
		};

		private static readonly HashSet<BodyPartGroupDef> tmpBodyPartGroupsWithRequirement = new HashSet<BodyPartGroupDef>();

		private static readonly HashSet<ThingDef> tmpAllowedApparels = new HashSet<ThingDef>();

		private static readonly HashSet<ThingDef> tmpRequiredApparels = new HashSet<ThingDef>();
		public static Thing PickBestArmorFor(Pawn pawn)
		{
			if (pawn.outfits == null)
			{
				return null;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return null;
			}
			if (pawn.IsQuestLodger())
			{
				return null;
			}

			ApparelPolicy currentApparelPolicy = pawn.outfits.CurrentApparelPolicy;
			Thing thing = null;
			float num2 = 0f;
			List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Apparel);
			if (list.Count == 0)
			{
				return null;
			}
			neededWarmth = PawnApparelGenerator.CalculateNeededWarmth(pawn, pawn.Map.Tile, GenLocalDate.Twelfth(pawn));
			wornApparelScores = new List<float>();
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			for (int i = 0; i < wornApparel.Count; i++)
			{
				wornApparelScores.Add(ApparelScoreRaw(pawn, wornApparel[i]));
			}
			for (int j = 0; j < list.Count; j++)
			{
				Apparel apparel = (Apparel)list[j]; // && apparel.IsInAnyStorage() 
				if (!apparel.IsBurning() && currentApparelPolicy.filter.Allows(apparel) && !apparel.IsForbidden(pawn)
					&& (apparel.def.apparel.gender == Gender.None || apparel.def.apparel.gender == pawn.gender))
				{
					float num3 = ApparelScoreGain_NewTmp(pawn, apparel, wornApparelScores);
					if (!(num3 < 0.05f) && !(num3 < num2) && (!CompBiocodable.IsBiocoded(apparel) || CompBiocodable.IsBiocodedFor(apparel, pawn))
						&& ApparelUtility.HasPartsToWear(pawn, apparel.def)
						&& pawn.CanReserveAndReach(apparel, PathEndMode.OnCell, pawn.NormalMaxDanger()))
					{
						thing = apparel;
						num2 = num3;
					}
				}
			}
			return thing;
		}
		public static float ApparelScoreGain_NewTmp(Pawn pawn, Apparel ap, List<float> wornScoresCache)
		{
			if (ap.def == ThingDefOf.Apparel_ShieldBelt && pawn.equipment.Primary != null && pawn.equipment.Primary.def.IsWeaponUsingProjectiles)
			{
				return -1000f;
			}
			float num = ApparelScoreRaw(pawn, ap);
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			bool flag = false;
			for (int i = 0; i < wornApparel.Count; i++)
			{
				if (!ApparelUtility.CanWearTogether(wornApparel[i].def, ap.def, pawn.RaceProps.body))
				{
					if (!pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[i]) || pawn.apparel.IsLocked(wornApparel[i]))
					{
						return -1000f;
					}
					num -= wornScoresCache[i];
					flag = true;
				}
			}
			if (!flag)
			{
				num *= 10f;
			}
			return num;
		}

		public static float ApparelScoreGain(Pawn pawn, Apparel ap)
		{
			wornApparelScores.Clear();
			for (int i = 0; i < pawn.apparel.WornApparel.Count; i++)
			{
				wornApparelScores.Add(ApparelScoreRaw(pawn, pawn.apparel.WornApparel[i]));
			}
			return ApparelScoreGain_NewTmp(pawn, ap, wornApparelScores);
		}

		public static float OverallArmorValue(Pawn pawn)
		{
			return (GetArmorValue(pawn, StatDefOf.ArmorRating_Blunt) * 100f) + (GetArmorValue(pawn, StatDefOf.ArmorRating_Sharp) * 100f);
		}

		private static float GetArmorValue(Pawn pawn, StatDef stat)
		{
			float num = 0f;
			float num2 = Mathf.Clamp01(pawn.GetStatValue(stat) / 2f);
			List<BodyPartRecord> allParts = pawn.RaceProps.body.AllParts;
			List<Apparel> list = pawn.apparel?.WornApparel;
			for (int i = 0; i < allParts.Count; i++)
			{
				float num3 = 1f - num2;
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j].def.apparel.CoversBodyPart(allParts[i]))
						{
							float num4 = Mathf.Clamp01(list[j].GetStatValue(stat) / 2f);
							num3 *= 1f - num4;
						}
					}
				}
				num += allParts[i].coverageAbs * (1f - num3);
			}
			return Mathf.Clamp(num * 2f, 0f, 2f);
		}

		public static float ApparelScoreRaw(Pawn pawn, Apparel ap)
		{
			float num = 0.1f + ap.def.apparel.scoreOffset;
			float num2 = ap.GetStatValue(StatDefOf.ArmorRating_Sharp) + ap.GetStatValue(StatDefOf.ArmorRating_Blunt) + ap.GetStatValue(StatDefOf.ArmorRating_Heat);
			num += num2;
			if (ap.def.useHitPoints)
			{
				float x = ap.HitPoints / (float)ap.MaxHitPoints;
				num *= HitPointsPercentScoreFactorCurve.Evaluate(x);
			}
			num += ap.GetSpecialApparelScoreOffset();
			float num3 = 1f;
			//if (neededWarmth == NeededWarmth.Warm)
			//{
			//	float statValue = ap.GetStatValue(StatDefOf.Insulation_Cold);
			//	num3 *= InsulationColdScoreFactorCurve_NeedWarm.Evaluate(statValue);
			//}
			num *= num3;
			if (ap.WornByCorpse && (pawn == null || ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.DeadMansApparel, checkIfNullified: true)))
			{
				num -= 0.5f;
				if (num > 0f)
				{
					num *= 0.1f;
				}
			}
			if (ap.Stuff == ThingDefOf.Human.race.leatherDef)
			{
				if (pawn == null || ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.HumanLeatherApparelSad, checkIfNullified: true))
				{
					num -= 0.5f;
					if (num > 0f)
					{
						num *= 0.1f;
					}
				}
				if (pawn != null && ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.HumanLeatherApparelHappy, checkIfNullified: true))
				{
					num += 0.12f;
				}
			}
			if (pawn != null && !ap.def.apparel.CorrectGenderForWearing(pawn.gender))
			{
				num *= 0.01f;
			}
			if (pawn != null && pawn.royalty != null && pawn.royalty.AllTitlesInEffectForReading.Count > 0)
			{
				tmpAllowedApparels.Clear();
				tmpRequiredApparels.Clear();
				tmpBodyPartGroupsWithRequirement.Clear();
				QualityCategory qualityCategory = QualityCategory.Awful;
				foreach (RoyalTitle item in pawn.royalty.AllTitlesInEffectForReading)
				{
					if (item.def.requiredApparel != null)
					{
						for (int i = 0; i < item.def.requiredApparel.Count; i++)
						{
							tmpAllowedApparels.AddRange(item.def.requiredApparel[i].AllAllowedApparelForPawn(pawn, ignoreGender: false, includeWorn: true));
							tmpRequiredApparels.AddRange(item.def.requiredApparel[i].AllRequiredApparelForPawn(pawn, ignoreGender: false, includeWorn: true));
							tmpBodyPartGroupsWithRequirement.AddRange(item.def.requiredApparel[i].bodyPartGroupsMatchAny);
						}
					}
					if ((int)item.def.requiredMinimumApparelQuality > (int)qualityCategory)
					{
						qualityCategory = item.def.requiredMinimumApparelQuality;
					}
				}
				bool num4 = ap.def.apparel.bodyPartGroups.Any((BodyPartGroupDef bp) => tmpBodyPartGroupsWithRequirement.Contains(bp));
				if (ap.TryGetQuality(out QualityCategory qc) && (int)qc < (int)qualityCategory)
				{
					num *= 0.25f;
				}
				if (num4)
				{
					foreach (ThingDef tmpRequiredApparel in tmpRequiredApparels)
					{
						tmpAllowedApparels.Remove(tmpRequiredApparel);
					}
					if (tmpAllowedApparels.Contains(ap.def))
					{
						num *= 10f;
					}
					if (tmpRequiredApparels.Contains(ap.def))
					{
						num *= 25f;
					}
				}
			}
			return num;
		}

		public static void ShowAllColonists()
		{
			foreach (PawnGroup group in TacticalGroups.pawnGroups)
			{
				group.entireGroupIsVisible = true;
				foreach (KeyValuePair<Pawn, PawnIcon> pawn in group.pawnIcons)
				{
					pawn.Value.isVisibleOnColonistBar = true;
				}
			}

			foreach (KeyValuePair<RimWorld.Planet.Caravan, CaravanGroup> group in TacticalGroups.caravanGroups)
			{
				group.Value.entireGroupIsVisible = true;
				foreach (KeyValuePair<Pawn, PawnIcon> pawn in group.Value.pawnIcons)
				{
					pawn.Value.isVisibleOnColonistBar = true;
				}
			}
			foreach (KeyValuePair<Map, ColonyGroup> group in TacticalGroups.colonyGroups)
			{
				group.Value.entireGroupIsVisible = true;
				foreach (KeyValuePair<Pawn, PawnIcon> pawn in group.Value.pawnIcons)
				{
					pawn.Value.isVisibleOnColonistBar = true;
				}
			}
		}

		public static TacticalColonistBar TacticalColonistBar;
	}
}
