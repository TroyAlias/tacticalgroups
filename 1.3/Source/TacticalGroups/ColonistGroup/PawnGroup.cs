using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public class PawnGroup : ColonistGroup
    {
        public List<Pawn> formerPawns;

        public ColonyGroup prevColonyGroup;
        public override Map Map => Find.CurrentMap;
        public override List<Pawn> ActivePawns => pawns.Where(x => x.Map == Map && x.Spawned).ToList();
        public override List<Pawn> VisiblePawns => pawns.Where(x => x.Map == Map && x.Spawned
            || x.ParentHolder is Building_CryptosleepCasket casket && casket.Map == Map).ToList();

        public override void Init()
        {
            base.Init();
            isPawnGroup = true;
            groupIcon = Textures.PawnGroupIcon_Default;
            groupBanner = Textures.PawnGroupBanner_Default;
            formerPawns = new List<Pawn>();
            pawnRowCount = 3;
            pawnDocRowCount = 8;
            pawnRowXPosShift = 2f;
            groupBannerFolder = "GroupBlue";
            defaultBannerFolder = "GroupBlue";
            groupIconFolder = "GroupIcons";
            colorFolder = "Group";
            isSubGroup = false;
            defaultGroupName = Strings.Group;
            updateIcon = true;
        }
        public PawnGroup()
        {
            Init();
        }
        public PawnGroup(List<Pawn> pawns)
        {
            Init();
            foreach (Pawn pawn in pawns)
            {
                Add(pawn);
            }
            groupID = CreateGroupID();
            curGroupName = defaultGroupName + " " + groupID;

            Map map = this.pawns.FirstOrDefault(x => x.Spawned && x.Map != null)?.Map;
            if (map != null)
            {
                prevColonyGroup = TacticUtils.AllColonyGroups.FirstOrDefault(x => x.Map == map);
            }
        }

        public PawnGroup(Pawn pawn)
        {
            Init();
            Add(pawn);
            groupID = CreateGroupID();
            curGroupName = defaultGroupName + " " + groupID;

            Map map = pawn.Map;
            if (map != null)
            {
                prevColonyGroup = TacticUtils.AllColonyGroups.FirstOrDefault(x => x.Map == map);
            }
        }

        public int CreateGroupID()
        {
            int groupID = TacticUtils.TacticalGroups.pawnGroups.Count + 1;
            foreach (CaravanGroup caravan in TacticUtils.AllCaravanGroups)
            {
                groupID += caravan.formerGroups.Count;
            }
            return groupID;
        }

        public void ConvertToSubGroup()
        {
            isSubGroup = true;
        }
        public void ConvertToPawnGroup()
        {
            isSubGroup = false;
        }
        public override void Add(Pawn pawn)
        {
            base.Add(pawn);
            if (formerPawns.Contains(pawn))
            {
                formerPawns.Remove(pawn);
            }
        }
        public override void Disband()
        {
            TacticUtils.TacticalGroups.pawnGroups.Remove(this);
            TacticUtils.RemoveReferencesFor(this);
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }
        public override void Disband(Pawn pawn)
        {
            base.Disband(pawn);
            if (pawns.Count == 0 && formerPawns.Count == 0 && autoDisbandWithoutPawns)
            {
                TacticUtils.TacticalGroups.pawnGroups.Remove(this);
                TacticUtils.RemoveReferencesFor(this);
            }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }
        public void Disband(List<Pawn> pawns, bool permanent = true)
        {
            foreach (Pawn pawn in pawns)
            {
                if (this.pawns.Contains(pawn))
                {
                    Disband(pawn);
                    if (formerPawns.Contains(pawn))
                    {
                        formerPawns.Remove(pawn);
                    }
                }
                if (!permanent)
                {
                    formerPawns.Add(pawn);
                }
            }
            if (this.pawns.Count == 0 && formerPawns.Count == 0 && autoDisbandWithoutPawns)
            {
                TacticUtils.TacticalGroups.pawnGroups.Remove(this);
                TacticUtils.RemoveReferencesFor(this);
            }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public override void Draw(Rect rect)
        {
            base.Draw(rect);
            if (isSubGroup)
            {
                Rect workIconRect = new Rect(rect.x, rect.y, Textures.ClockSlaveSubGroup.width, Textures.ClockSlaveSubGroup.height);
                if (activeWorkState == WorkState.ForcedLabor)
                {
                    GUI.DrawTexture(workIconRect, Textures.ClockSlaveSubGroup);
                }
                else if (activeWorkState == WorkState.Active)
                {
                    GUI.DrawTexture(workIconRect, Textures.Clock);
                }
            }
            else
            {
                if (activeWorkState == WorkState.ForcedLabor)
                {
                    if (bannerModeEnabled)
                    {
                        GUI.DrawTexture(rect, Textures.BannerGroupSlave);
                    }
                    else
                    {
                        GUI.DrawTexture(rect, Textures.DefaultGroupSlave);
                    }
                }
                else if (activeWorkState == WorkState.Active)
                {
                    if (bannerModeEnabled)
                    {
                        GUI.DrawTexture(rect, Textures.DefaultGroupWorkBanner);
                    }
                    else
                    {
                        GUI.DrawTexture(rect, Textures.DefaultGroupWork);
                    }
                }

                if (!(ModCompatibility.combatExtendedHasAmmo_Method is null))
                {
                    for (int i = 0; i < pawns.Count; i++)
                    {
                        ThingWithComps gun = pawns[i].equipment?.Primary ?? null;
                        if (gun != null && gun.def.IsRangedWeapon && (!(bool)ModCompatibility.combatExtendedHasAmmo_Method.Invoke(null, new object[]
                        {
                            gun
                        })))
                        {
                            if (bannerModeEnabled)
                            {
                                GUI.DrawTexture(rect, Textures.OutofAmmoBanner);
                            }
                            else
                            {
                                GUI.DrawTexture(rect, Textures.OutofAmmoDefault);
                            }
                        }
                    }
                }
            }

        }

        private int curHoverPeriod;
        public override void DrawOverlays(Rect rect)
        {
            base.DrawOverlays(rect);
            bool reset = true;
            if (Mouse.IsOver(rect))
            {
                curHoverPeriod++;
                reset = false;
            }
            if (curHoverPeriod > 30)
            {
                Rect rightGroupArrowRect = new Rect(rect.x + rect.width, rect.y, Textures.GroupArrowRight.width, Textures.GroupArrowRight.height);
                if (Mouse.IsOver(rightGroupArrowRect))
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                    {
                        int indexOf = TacticUtils.TacticalGroups.pawnGroups.IndexOf(this);
                        if (indexOf > 0)
                        {
                            TacticUtils.TacticalGroups.pawnGroups.RemoveAt(indexOf);
                            TacticUtils.TacticalGroups.pawnGroups.Insert(indexOf - 1, this);
                        }
                        else if (indexOf != TacticUtils.TacticalGroups.pawnGroups.Count)
                        {
                            TacticUtils.TacticalGroups.pawnGroups.RemoveAt(indexOf);
                            TacticUtils.TacticalGroups.pawnGroups.Insert(TacticUtils.TacticalGroups.pawnGroups.Count, this);
                        }
                        TacticUtils.TacticalColonistBar.MarkColonistsDirty();
                    }
                    GUI.DrawTexture(rightGroupArrowRect, Textures.GroupArrowRightHover);
                    reset = false;
                }
                else
                {
                    GUI.DrawTexture(rightGroupArrowRect, Textures.GroupArrowRight);
                }

                Rect leftGroupArrowRect = new Rect(rect.x - Textures.GroupArrowLeft.width, rect.y, Textures.GroupArrowLeft.width, Textures.GroupArrowLeft.height);
                if (Mouse.IsOver(leftGroupArrowRect))
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                    {
                        int indexOf = TacticUtils.TacticalGroups.pawnGroups.IndexOf(this);
                        if (TacticUtils.TacticalGroups.pawnGroups.Count > indexOf + 1)
                        {
                            TacticUtils.TacticalGroups.pawnGroups.RemoveAt(indexOf);
                            TacticUtils.TacticalGroups.pawnGroups.Insert(indexOf + 1, this);
                        }
                        else if (indexOf != 0)
                        {
                            TacticUtils.TacticalGroups.pawnGroups.RemoveAt(indexOf);
                            TacticUtils.TacticalGroups.pawnGroups.Insert(0, this);
                        }
                        TacticUtils.TacticalColonistBar.MarkColonistsDirty();
                    }
                    GUI.DrawTexture(leftGroupArrowRect, Textures.GroupArrowLeftHover);
                    reset = false;
                }
                else
                {
                    GUI.DrawTexture(leftGroupArrowRect, Textures.GroupArrowLeft);
                }
            }

            if (reset)
            {
                curHoverPeriod = 0;
            }
        }

        public override void UpdateData()
        {
            base.UpdateData();
            if (prevColonyGroup is null)
            {
                Map map = pawns.FirstOrDefault(x => x.Spawned && x.Map != null)?.Map;
                if (map != null)
                {
                    prevColonyGroup = TacticUtils.AllColonyGroups.FirstOrDefault(x => x.Map == map);
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref formerPawns, "formerPawns", LookMode.Reference);
            Scribe_Values.Look(ref autoDisbandWithoutPawns, "autoDisbandWithoutPawns");
            Scribe_References.Look(ref prevColonyGroup, "prevColonyGroup");
        }

        public bool autoDisbandWithoutPawns;
    }
}
