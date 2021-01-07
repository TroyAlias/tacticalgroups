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
				if (this.Map == null)
				{
					this.Map = pawn.Map;
				}
				this.pawns.Add(pawn);
				this.pawnIcons[pawn] = new PawnIcon(pawn);
			}
			this.groupID = TacticUtils.TacticalGroups.colonyGroups.Count + 1;
		}
		public ColonyGroup(Pawn pawn)
        {
			this.Init();
			if (this.Map == null)
			{
				this.Map = pawn.Map;
			}
			this.pawns = new List<Pawn> { pawn } ;
			this.pawnIcons = new Dictionary<Pawn, PawnIcon> { { pawn, new PawnIcon(pawn) } };
			this.groupID = TacticUtils.TacticalGroups.colonyGroups.Where(x => x.Value.isColonyGroup).Count() + 1;
		}

		public void ConvertToWarband()
        {
			this.defaultGroupName = Strings.Warband;
			this.isWarband = true;
			this.isColonyGroup = false;
			this.groupIcon = Textures.WarbandIcon_Default;
			this.groupBanner = Textures.WarbandBanner_Default;
		}

		public void ConvertToScout()
		{
			this.defaultGroupName = Strings.Scout;
			this.isScout = true;
			this.isColonyGroup = false;
			this.groupIcon = Textures.ScoutIcon_Default;
			this.groupBanner = Textures.WarbandBanner_Default;
		}
		public override void Draw(Rect rect)
        {
            base.Draw(rect);
			if (this.activeWorkState)
            {
				GUI.DrawTexture(rect, Textures.ColonySlave);
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
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
				if (this.isWarband)
                {
					this.ConvertToWarband();
                }
				else if (this.isScout)
                {
					this.ConvertToScout();
                }
            }
		}

		public override string ToString()
        {
			return GetGroupName() + " - " + this.pawns.Count;
		}
	}
}
