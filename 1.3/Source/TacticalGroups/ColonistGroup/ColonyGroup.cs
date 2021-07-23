using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

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
					map = this.pawns?.FirstOrDefault()?.Map;
                }
				return map;
            }
        }

		public override List<Pawn> ActivePawns => this.pawns.Where(x => x.Map == this.Map && x.Spawned).ToList();
		public override List<Pawn> VisiblePawns => this.pawns.Where(x => x.Map == this.Map && x.Spawned || x.ParentHolder is Building_CryptosleepCasket).ToList();

		public override void Init()
        {
            base.Init();
			this.pawnRowCount = 4;
			this.pawnDocRowCount = 11;
			this.groupIcon = Textures.ColonyGroupIcon_Default;
			this.groupBanner = Textures.ColonyGroupBanner_Default;
			this.groupIconFolder = "ColonyIcons";
			this.groupBannerFolder = "ColonyBlue";
			this.defaultBannerFolder = "ColonyBlue";
			this.isColonyGroup = true;
			this.colorFolder = "Colony";
			this.defaultGroupName = Strings.Colony;
			this.updateIcon = true;
		}
		public ColonyGroup()
		{
			this.Init();
		}
		public ColonyGroup(List<Pawn> pawns)
        {
			this.Init();
			this.pawns = new List<Pawn>();
			foreach (var pawn in pawns)
            {
				if (this.map == null)
				{
					this.map = pawn.Map;
				}
				this.Add(pawn);
			}
			this.groupID = TacticUtils.TacticalGroups.colonyGroups.Count + 1;
			this.curGroupName = this.defaultGroupName + " " + this.groupID;
		}
		public ColonyGroup(Pawn pawn)
        {
			this.Init();
			if (this.Map == null)
			{
				this.map = pawn.Map;
			}
			this.Add(pawn);
			this.groupID = TacticUtils.TacticalGroups.colonyGroups.Where(x => x.Value != this && x.Value.isColonyGroup).Count() + 1;
			this.curGroupName = this.defaultGroupName + " " + this.groupID;
		}
		public void ConvertToTaskForce()
        {
			this.defaultGroupName = Strings.TaskForce;
			this.isTaskForce = true;
			this.isColonyGroup = false;
			this.groupIcon = Textures.TaskForceIcon_Default;
			this.groupBanner = Textures.TaskForceBanner_Default;
			this.groupID = TacticUtils.TacticalGroups.colonyGroups.Where(x => x.Value != this && x.Value.isTaskForce).Count() + 1;
			this.curGroupName = this.defaultGroupName + " " + this.groupID;
			this.pawnRowCount = 3;
			this.pawnDocRowCount = 8;
			this.updateIcon = true;
		}

		public void ConvertToColonyGroup()
		{
			this.defaultGroupName = Strings.Colony;
			this.isTaskForce = false;
			this.isColonyGroup = true;
			this.groupIcon = Textures.ColonyGroupIcon_Default;
			this.groupBanner = Textures.ColonyGroupBanner_Default;
			this.groupID = TacticUtils.TacticalGroups.colonyGroups.Where(x => x.Value != this && x.Value.isColonyGroup).Count() + 1;
			this.curGroupName = this.defaultGroupName + " " + this.groupID;
			this.updateIcon = true;
		}

		public override void Draw(Rect rect)
        {
            base.Draw(rect);
			if (this.activeWorkState == WorkState.ForcedLabor)
            {
				GUI.DrawTexture(rect, Textures.ColonySlave);
            }
			else if (this.activeWorkState == WorkState.Active)
            {
				GUI.DrawTexture(rect, Textures.DefaultColonyWork);
            }
        }

        public override void Disband()
		{
			TacticUtils.TacticalGroups.colonyGroups.Remove(this.Map);
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}

		public void Disband(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
            {
				if (this.pawns.Contains(pawn))
                {
					this.Disband(pawn);
                }
            }
			if (this.pawns.Count == 0)
            {
				TacticUtils.TacticalGroups.colonyGroups.Remove(this.Map);
			}
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}

		public override void Disband(Pawn pawn)
		{
			base.Disband(pawn);
			if (this.pawns.Count == 0)
			{
				TacticUtils.TacticalGroups.colonyGroups.Remove(this.Map);
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
				if (this.isTaskForce)
                {
					this.ConvertToTaskForce();
                }
			}
		}
	}
}
