using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public class ColonyGroup : ColonistGroup
    {
        private Map map;
        public override Map Map
        {
            get
            {
                if (map is null)
                {
                    map = pawns?.FirstOrDefault()?.Map;
                }
                return map;
            }
        }

        public override List<Pawn> ActivePawns => pawns.Where(x => x.Map == Map && x.Spawned).ToList();
        public override List<Pawn> VisiblePawns => pawns.Where(x => x.Map == Map && x.Spawned || x.ParentHolder is Building_CryptosleepCasket).ToList();

        public override void Init()
        {
            base.Init();
            pawnRowCount = 4;
            pawnDocRowCount = 11;
            groupIcon = Textures.ColonyGroupIcon_Default;
            groupBanner = Textures.ColonyGroupBanner_Default;
            groupIconFolder = "ColonyIcons";
            groupBannerFolder = "ColonyBlue";
            defaultBannerFolder = "ColonyBlue";
            isColonyGroup = true;
            colorFolder = "Colony";
            defaultGroupName = Strings.Colony;
            updateIcon = true;
        }
        public ColonyGroup()
        {
            Init();
        }
        public ColonyGroup(List<Pawn> pawns)
        {
            Init();
            this.pawns = new List<Pawn>();
            foreach (Pawn pawn in pawns)
            {
                if (map == null)
                {
                    map = pawn.Map;
                }
                Add(pawn);
            }
            groupID = TacticUtils.TacticalGroups.colonyGroups.Count + 1;
            curGroupName = defaultGroupName + " " + groupID;
        }
        public ColonyGroup(Pawn pawn)
        {
            Init();
            if (Map == null)
            {
                map = pawn.Map;
            }
            Add(pawn);
            groupID = TacticUtils.TacticalGroups.colonyGroups.Where(x => x.Value != this && x.Value.isColonyGroup).Count() + 1;
            curGroupName = defaultGroupName + " " + groupID;
        }
        public void ConvertToTaskForce()
        {
            defaultGroupName = Strings.TaskForce;
            isTaskForce = true;
            isColonyGroup = false;
            groupIcon = Textures.TaskForceIcon_Default;
            groupBanner = Textures.TaskForceBanner_Default;
            groupID = TacticUtils.TacticalGroups.colonyGroups.Where(x => x.Value != this && x.Value.isTaskForce).Count() + 1;
            curGroupName = defaultGroupName + " " + groupID;
            pawnRowCount = 3;
            pawnDocRowCount = 8;
            updateIcon = true;
        }

        public void ConvertToColonyGroup()
        {
            defaultGroupName = Strings.Colony;
            isTaskForce = false;
            isColonyGroup = true;
            groupIcon = Textures.ColonyGroupIcon_Default;
            groupBanner = Textures.ColonyGroupBanner_Default;
            groupID = TacticUtils.TacticalGroups.colonyGroups.Where(x => x.Value != this && x.Value.isColonyGroup).Count() + 1;
            curGroupName = defaultGroupName + " " + groupID;
            updateIcon = true;
        }

        public override void Draw(Rect rect)
        {
            base.Draw(rect);
            if (activeWorkState == WorkState.ForcedLabor)
            {
                GUI.DrawTexture(rect, Textures.ColonySlave);
            }
            else if (activeWorkState == WorkState.Active)
            {
                GUI.DrawTexture(rect, Textures.DefaultColonyWork);
            }
        }

        public override void Disband()
        {
            TacticUtils.TacticalGroups.colonyGroups.Remove(Map);
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public void Disband(List<Pawn> pawns)
        {
            foreach (Pawn pawn in pawns)
            {
                if (this.pawns.Contains(pawn))
                {
                    Disband(pawn);
                }
            }
            if (this.pawns.Count == 0)
            {
                TacticUtils.TacticalGroups.colonyGroups.Remove(Map);
            }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }

        public override void Disband(Pawn pawn)
        {
            base.Disband(pawn);
            if (pawns.Count == 0)
            {
                TacticUtils.TacticalGroups.colonyGroups.Remove(Map);
            }
            TacticUtils.TacticalColonistBar.MarkColonistsDirty();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref colorFolder, "colorFolder", "Colony");
            Scribe_References.Look(ref map, "map");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (isTaskForce)
                {
                    ConvertToTaskForce();
                }
            }
        }
    }
}
