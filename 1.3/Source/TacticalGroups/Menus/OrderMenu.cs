using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
    public class OrderMenu : TieredFloatMenu
    {
        protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
        protected override Vector2 InitialFloatOptionPositionShift => new Vector2(30, 63f);

        public Dictionary<float, Texture2D> ranks = new Dictionary<float, Texture2D>
            {
                {0, Textures.Rank_0},
                {50, Textures.Rank_1},
                {100, Textures.Rank_2},
                {150, Textures.Rank_3},
                {200, Textures.Rank_4},
                {250, Textures.Rank_5},
                {300, Textures.Rank_6},
                {350, Textures.Rank_7},
                {400, Textures.Rank_8},
                {450, Textures.Rank_9},
                {500, Textures.Rank_10},
                {550, Textures.Rank_11},
                {600, Textures.Rank_12},
                {650, Textures.Rank_13},
            };
        public Texture2D GetCurRank(float curRankValue)
        {
            IOrderedEnumerable<float> keys = ranks.Keys.OrderBy(x => x);
            Texture2D result = Textures.Rank_0;
            foreach (float key in keys)
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
            options = new List<TieredFloatMenuOption>();
            AddAttackButton();
            AddRegroupButton();
            AddBattleStationsButton();
            AddMedicalButton();
            for (int i = 0; i < options.Count; i++)
            {
                options[i].SetSizeMode(SizeMode);
            }
        }


        private BattleStationsMenu battleStationsMenu;
        public override void PostOpen()
        {
            base.PostOpen();
            AddAttackWindow(options[0]);
            battleStationsMenu = new BattleStationsMenu(this, colonistGroup, windowRect, Textures.INVISIBLEMENU);
            Find.WindowStack.Add(battleStationsMenu);
        }

        public override void PostClose()
        {
            base.PostClose();
            battleStationsMenu?.Close();
        }

        public void AddAttackButton()
        {
            TieredFloatMenuOption option = new TieredFloatMenuOption(Strings.Attack, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
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
            TieredFloatMenuOption option = new TieredFloatMenuOption(Strings.Regroup, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress,
                TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f, -1f, Strings.RegroupTooltip)
            {
                action = delegate
                {
                    TacticDefOf.TG_RegroupSFX.PlayOneShotOnCamera();
                    colonistGroup.RemoveOldLord();
                    colonistGroup.Draft();
                    Pawn firstPawn = colonistGroup.ActivePawns.First();
                    foreach (Pawn pawn in colonistGroup.ActivePawns)
                    {
                        if (pawn != null)
                        {
                            Job job = JobMaker.MakeJob(JobDefOf.Goto, firstPawn);
                            job.locomotionUrgency = LocomotionUrgency.Sprint;
                            pawn.jobs.TryTakeOrderedJob(job);
                        }
                    }
                },
                bottomIndent = Textures.MenuButton.height + 25
            };
            options.Add(option);
        }

        public void AddBattleStationsButton()
        {
            TieredFloatMenuOption option = new TieredFloatMenuOption(Strings.BattleStations, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f, -1f, Strings.BattleStationsTooltip)
            {
                action = delegate
                {
                    TacticDefOf.TG_BattleStationsSFX.PlayOneShotOnCamera();
                    foreach (Pawn pawn in colonistGroup.ActivePawns)
                    {
                        colonistGroup.Draft();
                        colonistGroup.SelectAll();
                        if (colonistGroup.activeFormation.formations.TryGetValue(pawn, out IntVec3 cell))
                        {
                            Job job = JobMaker.MakeJob(JobDefOf.Goto, cell);
                            job.locomotionUrgency = LocomotionUrgency.Sprint;
                            pawn.jobs.TryTakeOrderedJob(job);
                        }
                    }
                },
                bottomIndent = Textures.MenuButton.height + 145
            };
            options.Add(option);
        }

        public void AddMedicalButton()
        {
            TieredFloatMenuOption option = new TieredFloatMenuOption(Strings.ReportToMedical, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter,
                MenuOptionPriority.High, 0f, -1f, Strings.ReportToMedicalTooltip)
            {
                action = delegate
                {
                    JobGiver_PatientGoToBed jgp = new JobGiver_PatientGoToBed
                    {
                        respectTimetable = false
                    };
                    foreach (Pawn pawn in colonistGroup.ActivePawns)
                    {
                        ThinkResult thinkResult = jgp.TryIssueJobPackage(pawn, default(JobIssueParams));
                        if (thinkResult.IsValid)
                        {
                            pawn.jobs.TryTakeOrderedJob(thinkResult.Job);
                        }
                    }
                    TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                },
                bottomIndent = Textures.MenuButton.height + 25
            };
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
                Rect rect2 = new Rect(zero.x, zero.y, (backgroundTexture.width - InitialFloatOptionPositionShift.x) / 1.2f, floatMenuOption.curIcon.height);
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
            Rect tendWounded = new Rect(zero.x + 10, rect.y + 407, Textures.TendWounded.width, Textures.TendWounded.height);
            GUI.DrawTexture(tendWounded, Textures.TendWounded);

            IEnumerable<KeyValuePair<WorkType, WorkState>> states = colonistGroup.activeWorkTypes.Where(x => x.Key.workTypeEnum == WorkTypeEnum.TendWounded);
            Rect tendWoundedState = new Rect(tendWounded.x, tendWounded.y, Textures.WorkSelectEmpty.width, Textures.WorkSelectEmpty.height);
            if (states != null && states.Any())
            {
                switch (states.First().Value)
                {
                    case WorkState.Temporary:
                    case WorkState.Inactive:
                        GUI.DrawTexture(tendWoundedState, Textures.WorkSelectEmpty);
                        break;
                    case WorkState.Active:
                        GUI.DrawTexture(tendWoundedState, Textures.WorkSelectBlue);
                        break;
                    case WorkState.ForcedLabor:
                        GUI.DrawTexture(tendWoundedState, Textures.WorkSelectRed);
                        break;
                }
            }
            else
            {
                GUI.DrawTexture(tendWoundedState, Textures.WorkSelectEmpty);
            }

            if (Mouse.IsOver(tendWounded))
            {
                GUI.DrawTexture(tendWounded, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    colonistGroup.RemoveWorkState(WorkTypeEnum.TendWounded);
                    TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                    WorkSearchUtility.SearchForWork(WorkTypeEnum.TendWounded, colonistGroup.ActivePawns);
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.clickCount == 1)
                {
                    colonistGroup.ChangeWorkState(WorkTypeEnum.TendWounded);
                    if (colonistGroup.activeWorkTypes.Any(x => x.Key.workTypeEnum == WorkTypeEnum.TendWounded && x.Value == WorkState.ForcedLabor))
                    {
                        TacticDefOf.TG_SlaveLaborSFX.PlayOneShotOnCamera();
                    }
                    else
                    {
                        TacticDefOf.TG_WorkSFX.PlayOneShotOnCamera();
                    }
                    WorkSearchUtility.SearchForWork(WorkTypeEnum.TendWounded, colonistGroup.ActivePawns);
                    Event.current.Use();
                }
                TooltipHandler.TipRegion(tendWounded, Strings.ForcedLaborTooltip);
            }
            TooltipHandler.TipRegion(tendWounded, Strings.TendWoundedTooltip);

            Rect rescureFallen = new Rect(((zero.x + Textures.LookBusyButton.width) - (Textures.RescueFallen.width + 11f)) - 10f, tendWounded.y, Textures.RescueFallen.width, Textures.RescueFallen.height);
            GUI.DrawTexture(rescureFallen, Textures.RescueFallen);
            Rect rescureFallenState = new Rect(rescureFallen.x, rescureFallen.y, Textures.WorkSelectEmpty.width, Textures.WorkSelectEmpty.height);

            states = colonistGroup.activeWorkTypes.Where(x => x.Key.workTypeEnum == WorkTypeEnum.RescueFallen);
            if (states != null && states.Any())
            {
                switch (states.First().Value)
                {
                    case WorkState.Temporary:
                    case WorkState.Inactive:
                        GUI.DrawTexture(rescureFallenState, Textures.WorkSelectEmpty);
                        break;
                    case WorkState.Active:
                        GUI.DrawTexture(rescureFallenState, Textures.WorkSelectBlue);
                        break;
                    case WorkState.ForcedLabor:
                        GUI.DrawTexture(rescureFallenState, Textures.WorkSelectRed);
                        break;
                }
            }
            else
            {
                GUI.DrawTexture(rescureFallenState, Textures.WorkSelectEmpty);
            }

            if (Mouse.IsOver(rescureFallen))
            {
                GUI.DrawTexture(rescureFallen, Textures.RescueTendHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    colonistGroup.RemoveWorkState(WorkTypeEnum.RescueFallen);
                    TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                    WorkSearchUtility.SearchForWork(WorkTypeEnum.RescueFallen, colonistGroup.ActivePawns);
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.clickCount == 1)
                {
                    colonistGroup.ChangeWorkState(WorkTypeEnum.RescueFallen);
                    if (colonistGroup.activeWorkTypes.Any(x => x.Key.workTypeEnum == WorkTypeEnum.RescueFallen && x.Value == WorkState.ForcedLabor))
                    {
                        TacticDefOf.TG_SlaveLaborSFX.PlayOneShotOnCamera();
                    }
                    else
                    {
                        TacticDefOf.TG_WorkSFX.PlayOneShotOnCamera();
                    }
                    WorkSearchUtility.SearchForWork(WorkTypeEnum.RescueFallen, colonistGroup.ActivePawns);
                    Event.current.Use();
                }
                GUI.DrawTexture(rescureFallen, Textures.RescueTendHover);
                TooltipHandler.TipRegion(rescureFallen, Strings.ForcedLaborTooltip);
            }
            TooltipHandler.TipRegion(rescureFallen, Strings.RescueDownedTooltip);
            Rect shooterIconRect = new Rect(rect.x + 10, rect.y + 25f, Textures.ShootingIcon.width, Textures.ShootingIcon.height);
            GUI.DrawTexture(shooterIconRect, Textures.ShootingIcon);
            if (Mouse.IsOver(shooterIconRect))
            {
                GUI.DrawTexture(shooterIconRect, Textures.ShootingMeleeHover);
            }
            IEnumerable<Pawn> shooters = colonistGroup.ActivePawns.Where(x => x.equipment?.Primary?.def.IsRangedWeapon ?? false);
            Rect shooterCountRect = new Rect(shooterIconRect.x + shooterIconRect.width + 2f, shooterIconRect.y, 30, shooterIconRect.height);

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(shooterCountRect, shooters.Count().ToString());
            Rect totalShooterRect = new Rect(shooterIconRect.x, shooterIconRect.y, Textures.ShootingIcon.width + 25, shooterIconRect.height).ExpandedBy(5f);
            TooltipHandler.TipRegion(totalShooterRect, Strings.ShootersToolTip);
            if (Mouse.IsOver(totalShooterRect))
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                    Find.Selector.ClearSelection();
                    foreach (Pawn shooter in shooters)
                    {
                        Find.Selector.Select(shooter);
                    }
                    Event.current.Use();
                }
            }
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect middlePawnCountRect = new Rect(totalShooterRect.x + totalShooterRect.width, totalShooterRect.y, 70, totalShooterRect.height);
            IEnumerable<Pawn> capablePawns = colonistGroup.ActivePawns.Where(x => !x.IsDownedOrIncapable());
            Widgets.Label(middlePawnCountRect, capablePawns.Count() + " / " + colonistGroup.ActivePawns.Count);

            TooltipHandler.TipRegion(middlePawnCountRect, Strings.MiddlePawnCountToolTip);
            if (Mouse.IsOver(middlePawnCountRect))
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                    Find.Selector.ClearSelection();
                    foreach (Pawn pawn in capablePawns)
                    {
                        Find.Selector.Select(pawn);
                    }
                    Event.current.Use();
                }
            }

            Rect meleeIconRect = new Rect((rect.x + rect.width) - 35, rect.y + 25f, Textures.MeleeIcon.width, Textures.MeleeIcon.height);
            GUI.DrawTexture(meleeIconRect, Textures.MeleeIcon);
            if (Mouse.IsOver(meleeIconRect))
            {
                GUI.DrawTexture(meleeIconRect, Textures.ShootingMeleeHover);
            }

            IEnumerable<Pawn> melees = colonistGroup.ActivePawns.Where(x => x.equipment?.Primary?.def.IsMeleeWeapon ?? false);
            Rect meleeCountRect = new Rect(meleeIconRect.x - (Textures.MeleeIcon.width + 5f), meleeIconRect.y, 30, meleeIconRect.height);

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(meleeCountRect, melees.Count().ToString());

            Rect totalMeleeRect = new Rect(meleeCountRect.x, meleeIconRect.y, meleeIconRect.width + meleeCountRect.width, meleeIconRect.height).ExpandedBy(5f);
            TooltipHandler.TipRegion(totalMeleeRect, Strings.MeleeToolTip);

            if (Mouse.IsOver(totalMeleeRect))
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                    Find.Selector.ClearSelection();
                    foreach (Pawn melee in melees)
                    {
                        Find.Selector.Select(melee);
                    }
                    Event.current.Use();
                }
            }
            Text.Anchor = TextAnchor.MiddleCenter;

            float rectY = (rect.height / 2f) - 30f;
            Rect setRect = new Rect(zero.x, rectY, Textures.SetClearButton.width, Textures.SetClearButton.height);
            GUI.DrawTexture(setRect, Textures.SetClearButton);
            Widgets.Label(setRect, Strings.Set);
            TooltipHandler.TipRegion(setRect, Strings.BattleStationsSetTooltip);

            if (Mouse.IsOver(setRect))
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                    colonistGroup.SetBattleStations();
                    Event.current.Use();
                }
                GUI.DrawTexture(setRect, Textures.WorkButtonHover);

            }
            Rect clearRect = new Rect((zero.x + (Textures.MenuButton.width - Textures.SetClearButton.width)) - 12f, rectY, Textures.SetClearButton.width, Textures.SetClearButton.height);
            GUI.DrawTexture(clearRect, Textures.SetClearButton);
            Widgets.Label(clearRect, Strings.Clear);
            TooltipHandler.TipRegion(clearRect, Strings.BattleStationsClearTooltip);
            if (Mouse.IsOver(clearRect))
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
                    colonistGroup.ClearBattleStations();
                    Event.current.Use();
                }
                GUI.DrawTexture(clearRect, Textures.WorkButtonHover);
            }

            Rect upgradeArmorRect = new Rect(zero.x - 8f, rect.height / 1.76f, Textures.UpgradeArmorIcon.width, Textures.UpgradeArmorIcon.height);
            GUI.DrawTexture(upgradeArmorRect, Textures.UpgradeArmorIcon);
            TooltipHandler.TipRegion(upgradeArmorRect, Strings.UpgradeArmorTooltip);

            if (Mouse.IsOver(upgradeArmorRect))
            {
                GUI.DrawTexture(upgradeArmorRect, Textures.UpgradeIconHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_UpgradeArmorWeaponsSFX.PlayOneShotOnCamera();
                    foreach (Pawn pawn in colonistGroup.ActivePawns)
                    {
                        Thing thing = TacticUtils.PickBestArmorFor(pawn);
                        if (thing != null)
                        {
                            Job job = JobMaker.MakeJob(JobDefOf.Wear, thing);
                            job.locomotionUrgency = LocomotionUrgency.Sprint;
                            pawn.jobs.TryTakeOrderedJob(job);
                        }
                    }
                    Event.current.Use();
                }
            }

            Rect takeBuffRect = new Rect(upgradeArmorRect.x + 56, upgradeArmorRect.y, Textures.TakeBuffButton.width, Textures.TakeBuffButton.height);
            GUI.DrawTexture(takeBuffRect, Textures.TakeBuffButton);
            TooltipHandler.TipRegion(takeBuffRect, Strings.TakeBuffTooltip);
            if (Mouse.IsOver(takeBuffRect))
            {
                GUI.DrawTexture(takeBuffRect, Textures.TakeBuffButtonHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_TakeBuffSFX.PlayOneShotOnCamera();
                    foreach (Pawn pawn in colonistGroup.ActivePawns)
                    {
                        JobGiver_TakeCombatEnhancingDrug jbg = new JobGiver_TakeCombatEnhancingDrug();
                        ThinkResult result = jbg.TryIssueJobPackage(pawn, default(JobIssueParams));
                        if (result.Job != null)
                        {
                            pawn.jobs.TryTakeOrderedJob(result.Job);
                        }
                    }
                    Event.current.Use();
                }
            }

            Rect upgradeWeaponRect = new Rect((rect.x + rect.width) - (20 + Textures.UpgradeWeaponIcon.width), upgradeArmorRect.y, Textures.UpgradeWeaponIcon.width, Textures.UpgradeWeaponIcon.height);
            GUI.DrawTexture(upgradeWeaponRect, Textures.UpgradeWeaponIcon);
            TooltipHandler.TipRegion(upgradeWeaponRect, Strings.UpgradeWeaponTooltip);
            if (Mouse.IsOver(upgradeWeaponRect))
            {
                GUI.DrawTexture(upgradeWeaponRect, Textures.UpgradeIconHover);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    TacticDefOf.TG_UpgradeWeaponSFX.PlayOneShotOnCamera();
                    foreach (Pawn pawn in colonistGroup.ActivePawns)
                    {
                        Thing thing = TacticUtils.PickBestWeaponFor(pawn);
                        if (thing != null)
                        {
                            if (pawn.inventory?.innerContainer?.InnerListForReading?.Contains(thing) ?? false)
                            {
                                TacticUtils.TrySwitchToWeapon(thing as ThingWithComps, pawn);
                            }
                            else if (thing != pawn.equipment?.Primary)
                            {
                                Job job = JobMaker.MakeJob(JobDefOf.Equip, thing);
                                job.locomotionUrgency = LocomotionUrgency.Sprint;
                                pawn.jobs.TryTakeOrderedJob(job);
                            }
                        }
                    }
                    Event.current.Use();
                }
            }

            Rect totalArmorRect = new Rect(rect.x + 10f, rect.height - 43, Textures.ArmorIcon.width, Textures.ArmorIcon.height);
            GUI.DrawTexture(totalArmorRect, Textures.ArmorIcon);
            Text.Anchor = TextAnchor.LowerLeft;
            Rect totalArmorLabel = new Rect(totalArmorRect.x + totalArmorRect.width + 2, totalArmorRect.y - 3, 30, 24);
            List<float> armorValues = new List<float>();
            foreach (Pawn pawn in colonistGroup.ActivePawns)
            {
                if (pawn.apparel.WornApparel != null)
                {
                    float armorValue = TacticUtils.OverallArmorValue(pawn);
                    armorValues.Add(armorValue);
                }
            }
            float averageArmor = armorValues.Sum() / colonistGroup.ActivePawns.Count();
            Widgets.Label(totalArmorLabel, averageArmor.ToStringDecimalIfSmall());
            TooltipHandler.TipRegion(totalArmorLabel, Strings.ArmorTooltip);

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect totalDPSLabel = new Rect(totalArmorRect.x + totalArmorRect.width + 50, totalArmorRect.y - 3, 30, 24);
            List<float> dpsValues = new List<float>();
            foreach (Pawn pawn in colonistGroup.ActivePawns)
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

            float averageDPS = dpsValues.Average();
            Widgets.Label(totalDPSLabel, averageDPS.ToStringDecimalIfSmall());
            TooltipHandler.TipRegion(totalDPSLabel, Strings.DPSTooltip);
            Texture2D rankTexture = GetCurRank(averageDPS + averageArmor);
            Rect rankRect = new Rect((rect.x + rect.width) - (rankTexture.width + 12f), totalArmorRect.y - 10f, rankTexture.width, rankTexture.height);
            GUI.DrawTexture(rankRect, rankTexture);
            TooltipHandler.TipRegion(rankRect, Strings.RankTooltip);
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}
