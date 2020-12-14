using RimWorld;
using RimWorld.Planet;
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
	public class OrderMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 63f);

		public Dictionary<float, Texture2D> ranks = new Dictionary<float, Texture2D>
			{
				{0, Textures.Rank_0},
				{37, Textures.Rank_1},
				{74, Textures.Rank_2},
				{111, Textures.Rank_3},
				{148, Textures.Rank_4},
				{185, Textures.Rank_5},
				{222, Textures.Rank_6},
				{300, Textures.Rank_7},
			};
		public Texture2D GetCurRank(float curRankValue)
		{
			var keys = ranks.Keys.OrderBy(x => x);
			Texture2D result = Textures.Rank_0;
			foreach (var key in keys)
			{
				if (curRankValue >= key)
				{
					result = ranks[key];
				}
			}
			return result;
		}
		public OrderMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddAttackButton();
			AddRegroupButton();
			AddBattleStationsButton();
			AddMedicalButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public override void PostOpen()
		{
			base.PostOpen();
			AddAttackWindow(options[0]);
		}

		public void AddAttackButton()
		{
			var option = new TieredFloatMenuOption(Strings.Attack, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddAttackWindow(option);
			};
			option.bottomIndent = Textures.MenuButton.height + 10;
			options.Add(option);
		}

		public void AddAttackWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new AttackMenu(this, colonistGroup, windowRect, Textures.AttackMenu);
			OpenNewMenu(floatMenu);
		}

		public void AddRegroupButton()
		{
			var option = new TieredFloatMenuOption(Strings.Regroup, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				this.colonistGroup.RemoveOldLord();
				this.colonistGroup.Draft();
				var firstPawn = this.colonistGroup.pawns.First();
				foreach (var pawn in this.colonistGroup.pawns)
                {
					if (pawn != null)
                    {
						var job = JobMaker.MakeJob(JobDefOf.Goto, firstPawn);
						job.locomotionUrgency = LocomotionUrgency.Sprint;
						pawn.jobs.TryTakeOrderedJob(job);
                    }
                }
			};
			option.bottomIndent = Textures.MenuButton.height + 25;
			options.Add(option);
		}

		public void AddBattleStationsButton()
		{
			var option = new TieredFloatMenuOption(Strings.BattleStations, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				TacticDefOf.TG_BattleStationsSFX.PlayOneShotOnCamera();
				foreach (var pawn in this.colonistGroup.pawns)
				{
					if (this.colonistGroup.formations?.ContainsKey(pawn) ?? false && pawn.Drafted)
					{
						var job = JobMaker.MakeJob(JobDefOf.Goto, this.colonistGroup.formations[pawn]);
						job.locomotionUrgency = LocomotionUrgency.Sprint;
						pawn.jobs.TryTakeOrderedJob(job);
					}
				}
			};
			option.bottomIndent = Textures.MenuButton.height + 72;
			options.Add(option);
		}

		public void AddMedicalButton()
		{
			var option = new TieredFloatMenuOption(Strings.ReportToMedical, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				JobGiver_PatientGoToBed jgp = new JobGiver_PatientGoToBed
				{
					respectTimetable = false
				};
				foreach (var pawn in this.colonistGroup.pawns)
				{
					ThinkResult thinkResult = jgp.TryIssueJobPackage(pawn, default(JobIssueParams));
					if (thinkResult.IsValid)
					{
						pawn.jobs.TryTakeOrderedJob(thinkResult.Job);
					}
				}
			};
			option.bottomIndent = Textures.MenuButton.height + 25;
			options.Add(option);
		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Vector2 zero = Vector2.zero;
			zero += InitialFloatOptionPositionShift;
			for (int i = 0; i < options.Count; i++)
			{
				TieredFloatMenuOption floatMenuOption = options[i];
				Rect rect2 = new Rect(zero.x, zero.y, (this.backgroundTexture.width - InitialFloatOptionPositionShift.x) / 1.2f, floatMenuOption.curIcon.height);
				if (floatMenuOption.DoGUI(rect2, this))
				{
					Find.WindowStack.TryRemove(this);
					break;
				}
				zero.y += floatMenuOption.bottomIndent;
			}
			DrawExtraGui(rect);
			GUI.color = Color.white;
		}

        public override void DrawExtraGui(Rect rect)
        {
            base.DrawExtraGui(rect);
			Vector2 zero = Vector2.zero + InitialFloatOptionPositionShift;
			var tendWounded = new Rect(zero.x, rect.y + 330, Textures.TendWounded.width, Textures.TendWounded.height);
			GUI.DrawTexture(tendWounded, Textures.TendWounded);
			if (Mouse.IsOver(tendWounded))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					this.colonistGroup.RemoveOldLord();
					LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_TendWounded(Faction.OfPlayer), this.colonistGroup.Map, this.colonistGroup.pawns);
					this.colonistGroup.SearchForJob();
					Event.current.Use();
				}
			}

			var rescureFallen = new Rect((zero.x + Textures.LookBusyButton.width) - (Textures.RescueFallen.width + 4f), tendWounded.y, Textures.RescueFallen.width, Textures.RescueFallen.height);
			GUI.DrawTexture(rescureFallen, Textures.RescueFallen);
			if (Mouse.IsOver(rescureFallen))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					this.colonistGroup.RemoveOldLord();
					LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_RescueFallen(Faction.OfPlayer), this.colonistGroup.Map, this.colonistGroup.pawns);
					this.colonistGroup.SearchForJob();
					Event.current.Use();
				}
			}

			var shooterIconRect = new Rect(rect.x + 10, rect.y + 25f, Textures.ShootingIcon.width, Textures.ShootingIcon.height);
			GUI.DrawTexture(shooterIconRect, Textures.ShootingIcon);
			if (Mouse.IsOver(shooterIconRect))
			{
				GUI.DrawTexture(shooterIconRect, Textures.ShootingMeleeHover);
			}
			var shooters = this.colonistGroup.pawns.Where(x => x.equipment?.Primary?.def.IsRangedWeapon ?? false);
			var shooterCountRect = new Rect(shooterIconRect.x + shooterIconRect.width + 2f, shooterIconRect.y, 30, shooterIconRect.height);

			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(shooterCountRect, shooters.Count().ToString());
			var totalShooterRect = new Rect(shooterIconRect.x, shooterIconRect.y, Textures.ShootingIcon.width + 25, shooterIconRect.height).ExpandedBy(5f);
			if (Mouse.IsOver(totalShooterRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					Find.Selector.ClearSelection();
					foreach (var shooter in shooters)
                    {
						Find.Selector.Select(shooter);
                    }
					Event.current.Use();
				}
			}
			Text.Anchor = TextAnchor.MiddleCenter;
			var middlePawnCountRect = new Rect(totalShooterRect.x + totalShooterRect.width, totalShooterRect.y, 70, totalShooterRect.height);
			var capablePawns = this.colonistGroup.pawns.Where(x => !x.IsDownedOrIncapable());
			Widgets.Label(middlePawnCountRect, capablePawns.Count() + " / " + this.colonistGroup.pawns.Count);
			if (Mouse.IsOver(middlePawnCountRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					Find.Selector.ClearSelection();
					foreach (var pawn in capablePawns)
					{
						Find.Selector.Select(pawn);
					}
					Event.current.Use();
				}
			}

			var meleeIconRect = new Rect((rect.x + rect.width) - 35, rect.y + 25f, Textures.MeleeIcon.width, Textures.MeleeIcon.height);
			GUI.DrawTexture(meleeIconRect, Textures.MeleeIcon);
			if (Mouse.IsOver(meleeIconRect))
            {
				GUI.DrawTexture(meleeIconRect, Textures.ShootingMeleeHover);
			}

			var melees = this.colonistGroup.pawns.Where(x => x.equipment?.Primary?.def.IsMeleeWeapon ?? false);
			var meleeCountRect = new Rect(meleeIconRect.x - (Textures.MeleeIcon.width + 5f), meleeIconRect.y, 30, meleeIconRect.height);

			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(meleeCountRect, melees.Count().ToString());

			var totalMeleeRect = new Rect(meleeCountRect.x, meleeIconRect.y, meleeIconRect.width + meleeCountRect.width, meleeIconRect.height).ExpandedBy(5f);
			if (Mouse.IsOver(totalMeleeRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					Find.Selector.ClearSelection();
					foreach (var melee in melees)
					{
						Find.Selector.Select(melee);
					}
					Event.current.Use();
				}
			}
			Text.Anchor = TextAnchor.MiddleCenter;

			var rectY = (rect.height / 2f) - 30f;
			var setRect = new Rect(zero.x, rectY, Textures.SetClearButton.width, Textures.SetClearButton.height);
			GUI.DrawTexture(setRect, Textures.SetClearButton);
			Widgets.Label(setRect, Strings.Set);

			if (Mouse.IsOver(setRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					this.colonistGroup.SetBattleStations();
					Event.current.Use();
				}
			}
			var clearRect = new Rect(zero.x + (Textures.MenuButton.width - Textures.SetClearButton.width - 3f), rectY, Textures.SetClearButton.width, Textures.SetClearButton.height);
			GUI.DrawTexture(clearRect, Textures.SetClearButton);
			Widgets.Label(clearRect, Strings.Clear);
			if (Mouse.IsOver(clearRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					this.colonistGroup.ClearBattleStations();
					Event.current.Use();
				}
			}

			var upgradeArmorRect = new Rect(zero.x + 20, rect.height / 1.26f, Textures.UpgradeArmorIcon.width, Textures.UpgradeArmorIcon.height);
			GUI.DrawTexture(upgradeArmorRect, Textures.UpgradeArmorIcon);
			if (Mouse.IsOver(upgradeArmorRect))
            {
				GUI.DrawTexture(upgradeArmorRect, Textures.UpgradeIconHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					foreach (var pawn in this.colonistGroup.pawns)
                    {
						var thing = TacticUtils.PickBestArmorFor(pawn);
						if (thing != null)
                        {
							var job = JobMaker.MakeJob(JobDefOf.Wear, thing);
							job.locomotionUrgency = LocomotionUrgency.Sprint;
							pawn.jobs.TryTakeOrderedJob(job);
                        }
					}
					Event.current.Use();
				}
			}

			var upgradeWeaponRect = new Rect((rect.x + rect.width) - (rect.x + (upgradeArmorRect.width * 2)), rect.height / 1.26f, Textures.UpgradeWeaponIcon.width, Textures.UpgradeWeaponIcon.height);
			GUI.DrawTexture(upgradeWeaponRect, Textures.UpgradeWeaponIcon);
			if (Mouse.IsOver(upgradeWeaponRect))
			{
				GUI.DrawTexture(upgradeWeaponRect, Textures.UpgradeIconHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					foreach (var pawn in this.colonistGroup.pawns)
					{
						var thing = TacticUtils.PickBestWeaponFor(pawn);
						if (thing != null)
						{
							if (pawn.inventory?.innerContainer?.InnerListForReading?.Contains(thing) ?? false)
                            {
								TacticUtils.TrySwitchToWeapon(thing as ThingWithComps, pawn);
							}
							else if (thing != pawn.equipment?.Primary)
                            {
								var job = JobMaker.MakeJob(JobDefOf.Equip, thing);
								job.locomotionUrgency = LocomotionUrgency.Sprint;
								pawn.jobs.TryTakeOrderedJob(job);
							}
						}
					}
					Event.current.Use();
				}
			}

			var totalArmorRect = new Rect(rect.x + 10f, rect.height - 43, Textures.ArmorIcon.width, Textures.ArmorIcon.height);
			GUI.DrawTexture(totalArmorRect, Textures.ArmorIcon);
			Text.Anchor = TextAnchor.LowerLeft;
			var totalArmorLabel = new Rect(totalArmorRect.x + totalArmorRect.width + 2, totalArmorRect.y - 3, 30, 24);
			var armorValues = new List<float>();
			foreach (var pawn in this.colonistGroup.pawns)
            {
				if (pawn.apparel.WornApparel != null)
                {
					var armorValue = TacticUtils.OverallArmorValue(pawn);
					armorValues.Add(armorValue);
                }
            }
			var averageArmor = armorValues.Sum() / this.colonistGroup.pawns.Count();
			Widgets.Label(totalArmorLabel, averageArmor.ToStringDecimalIfSmall());

			Text.Anchor = TextAnchor.MiddleCenter;
			var totalDPSLabel = new Rect(totalArmorRect.x + totalArmorRect.width + 50, totalArmorRect.y - 3, 30, 24);
			var dpsValues = new List<float>();
			foreach (var pawn in this.colonistGroup.pawns)
			{
				if (pawn.equipment.Primary != null)
				{
					dpsValues.Add(TacticUtils.WeaponScoreGain(pawn.equipment.Primary));
				}
				else
                {
					dpsValues.Add(pawn.GetStatValue(StatDefOf.MeleeDPS));
                }
			}

			var averageDPS = dpsValues.Average();
			Widgets.Label(totalDPSLabel, averageDPS.ToStringDecimalIfSmall());
			var rankTexture = GetCurRank(averageDPS + averageArmor);
			var rankRect = new Rect((rect.x + rect.width) - (rankTexture.width + 12f), totalArmorRect.y - 10f, rankTexture.width, rankTexture.height);
			GUI.DrawTexture(rankRect, rankTexture);
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
