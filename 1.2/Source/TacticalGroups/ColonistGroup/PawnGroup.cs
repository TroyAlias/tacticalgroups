using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
    public class PawnGroup : ColonistGroup
	{
		public List<Pawn> formerPawns;
		public override void Init()
        {
            base.Init();
			this.groupIcon = Textures.PawnGroupIcon_Default;
			this.groupBanner = Textures.PawnGroupBanner_Default;
			this.formerPawns = new List<Pawn>();
			this.pawnRowCount = 3;
			this.pawnDocRowCount = 8;
			this.pawnRowXPosShift = 2f;
			this.defaultBannerFolder = "GroupBlue";
			this.groupIconFolder = "GroupIcons";
			this.colorFolder = "Group";
			this.defaultGroupName = Strings.Group;
			this.updateIcon = true;
		}
		public PawnGroup()
		{
			this.Init();
		}

		public PawnGroup(List<Pawn> pawns)
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
			this.groupID = TacticUtils.TacticalGroups.pawnGroups.Count + 1;
		}

		public PawnGroup(Pawn pawn)
        {
			this.Init();
			if (this.Map == null)
			{
				this.Map = pawn.Map;
			}
			this.pawns = new List<Pawn> { pawn } ;
			this.pawnIcons = new Dictionary<Pawn, PawnIcon> { { pawn, new PawnIcon(pawn) } };
			this.groupID = TacticUtils.TacticalGroups.pawnGroups.Count + 1;
		}
        public override void Add(Pawn pawn)
        {
            base.Add(pawn);
			if (this.formerPawns.Contains(pawn))
            {
				this.formerPawns.Remove(pawn);
            }
        }
        public override void Disband()
        {
			TacticUtils.TacticalGroups.pawnGroups.Remove(this);
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}
		public override void Disband(Pawn pawn)
		{
			base.Disband(pawn);
			if (this.pawns.Count == 0 && this.formerPawns.Count == 0)
			{
				TacticUtils.TacticalGroups.pawnGroups.Remove(this);
			}
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}
		public void Disband(List<Pawn> pawns, bool permanent = true)
		{
			foreach (var pawn in pawns)
			{
				if (this.pawns.Contains(pawn))
				{
					this.Disband(pawn);
					if (this.formerPawns.Contains(pawn))
                    {
						this.formerPawns.Remove(pawn);
                    }
				}
				if (!permanent)
                {
					this.formerPawns.Add(pawn);
                }
			}
			if (this.pawns.Count == 0 && this.formerPawns.Count == 0)
			{
				TacticUtils.TacticalGroups.pawnGroups.Remove(this);
			}
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}

		private int curHoverPeriod;
        public override void Draw(Rect rect)
        {
            base.Draw(rect);
			var groupRect = new Rect(rect.x, rect.y, this.groupBanner.width, this.groupBanner.height);

			bool reset = true;
			if (Mouse.IsOver(groupRect))
            {
				curHoverPeriod++;
				reset = false;
			}
			if (curHoverPeriod > 30)
            {
				var rightGroupArrowRect = new Rect(groupRect.x + groupRect.width, ((groupRect.y + groupRect.height) / 2f) - 5f, Textures.GroupArrowRight.width, Textures.GroupArrowRight.height);
				if (Mouse.IsOver(rightGroupArrowRect))
                {
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						var indexOf = TacticUtils.TacticalGroups.pawnGroups.IndexOf(this);
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

				var leftGroupArrowRect = new Rect(groupRect.x - Textures.GroupArrowLeft.width, ((groupRect.y + groupRect.height) / 2f) - 5f, Textures.GroupArrowLeft.width, Textures.GroupArrowLeft.height);
				if (Mouse.IsOver(leftGroupArrowRect))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						var indexOf = TacticUtils.TacticalGroups.pawnGroups.IndexOf(this);
						Log.Message("Index of " + indexOf + " - " + TacticUtils.TacticalGroups.pawnGroups.Count);
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
		public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Collections.Look(ref formerPawns, "formerPawns", LookMode.Reference);
        }
    }
}
