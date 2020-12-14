using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
    public class ColonistGroup : IExposable
    {
        public List<Pawn> pawns;
		public Dictionary<Pawn, Rect> pawnRects = new Dictionary<Pawn, Rect>();
		public Dictionary<Pawn, PawnIcon> pawnIcons = new Dictionary<Pawn, PawnIcon>();
		public Dictionary<Pawn, IntVec3> formations = new Dictionary<Pawn, IntVec3>();
		public bool entireGroupIsVisible;
		private bool pawnWindowIsActive;
		public bool groupButtonRightClicked;

		public string groupName;
		public string defaultGroupName;
		public Texture2D groupIcon;
		public string groupIconName;
		public string groupIconFolder;

		public Rect curRect;
		public bool Visible => pawnWindowIsActive;
		private bool expandPawnIcons;
		public bool showPawnIconsRightClickMenu;

		public bool updateIcon = true;
		public Map Map
        {
			get
            {
				if (this.pawns?.Count > 0)
                {
					foreach (var pawn in this.pawns)
                    {
						if (pawn?.Map != null)
                        {
							return pawn.Map;
                        }
                    }
                }
				else
                {
					Log.Error(this + " has empty or null pawns, this shouldn't happen.");
                }
				return null;
			}
        }
		public bool ShowExpanded
        {
			get
            {
				if (Visible && expandPawnIcons)
                {
					return true;
                }
				return false;
			}
		}
		public float IconScale => ShowExpanded ? 1f : 0.5f;

		protected int groupID;

		protected int pawnRowCount;

		protected float pawnRowXPosShift;

		public string defaultIconFolder;
		public string colorFolder;

		public virtual void Init()
        {
			this.pawns = new List<Pawn>();
			this.pawnIcons = new Dictionary<Pawn, PawnIcon>();
			this.formations = new Dictionary<Pawn, IntVec3>();
		}
		public void Add(Pawn pawn)
        {
			if (!this.pawns.Contains(pawn))
            {
				this.pawns.Add(pawn);
				this.pawnIcons[pawn] = new PawnIcon(pawn);
				Sort();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
		}

		public void Add(List<Pawn> newPawns)
		{
			foreach (var pawn in newPawns)
			{
				Add(pawn);
			}
		}

		public virtual void Disband(Pawn pawn)
        {
			if (pawns.Contains(pawn))
            {
				Log.Message("Remove 9");
				this.pawns.Remove(pawn);
				this.pawnIcons.Remove(pawn);
				Sort();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
		}

		public virtual void Disband(List<Pawn> newPawns)
		{
			foreach (var pawn in newPawns)
			{
				Disband(pawn);
			}
		}
		public virtual void Disband()
		{

		}



		public string GetGroupName()
        {
			if (this.groupName != null)
            {
				return this.groupName;
            }
			else
            {
				return this.defaultGroupName + " " + this.groupID;
            }
        }

		public List<List<Pawn>> GetPawnRows(int columnCount)
        {
			int num = 0;
			List<List<Pawn>> pawnRows = new List<List<Pawn>>();
			List<Pawn> row = new List<Pawn>();
			foreach (var pawn in pawns)
			{
				if (num == columnCount)
				{
					pawnRows.Add(row.ListFullCopy());
					row = new List<Pawn>();
					num = 0;
				}
				num++;
				row.Add(pawn);
			}
			if (row.Any())
			{
				pawnRows.Add(row);
			}
			return pawnRows;
		}

		public void HandleClicks(Rect rect)
        {
			if (Event.current.type == EventType.MouseDown)
			{
				foreach (var group in TacticUtils.AllGroups)
                {
					if (group != this && group.pawnWindowIsActive)
                    {
						return;
                    }
                }
				if (Event.current.button == 0)
                {
					if (Event.current.clickCount == 1)
					{
						Log.Message("HandleClicks");
						if (!expandPawnIcons)
						{
							expandPawnIcons = true;
						}
						else
						{
							expandPawnIcons = false;
						}
					}

					else if (Event.current.clickCount == 2)
					{
						Find.Selector.ClearSelection();
						foreach (var pawn in this.pawns)
						{
							Find.Selector.Select(pawn);
						}
					}
				}
				else if (Event.current.button == 1)
                {
					this.showPawnIconsRightClickMenu = true;
					this.expandPawnIcons = false;
					this.groupButtonRightClicked = true;
					var rect2 = new Rect(rect.x, rect.y + rect.height, rect.width, rect.height);
					TieredFloatMenu floatMenu = new MainFloatMenu(null, this, rect2, Textures.DropMenuRightClick);
					Find.WindowStack.Add(floatMenu);
				}
				Event.current.Use();
			}
		}

		public void UpdateIcon()
        {
			var icons = ContentFinder<Texture2D>.GetAllInFolder("UI/ColonistBar/GroupIcons/" + groupIconFolder);
			var icon = icons.Where(x => x.name == groupIconName).FirstOrDefault();
			if (icon != null)
			{
				this.groupIcon = icon;
			}
			this.updateIcon = false;
		}
		public void Draw(Rect rect)
        {
			this.curRect = rect;
			if (this.updateIcon)
            {
				UpdateIcon();
			}
			GUI.DrawTexture(rect, this.groupIcon);
			if (!groupButtonRightClicked && Mouse.IsOver(rect))
            {
				GUI.DrawTexture(rect, Textures.GroupIconHover);
			}
			else if (groupButtonRightClicked)
            {
				GUI.DrawTexture(rect, Textures.GroupIconSelected);
			}
		}

		public void DrawExtra(Rect rect)
        {
			GUI.color = Color.white;
			Text.Font = GameFont.Tiny;
			var totalRect = new Rect(rect);
			var pawnRows = GetPawnRows(this.pawnRowCount);
			if (ShowExpanded)
			{
				totalRect.height += pawnRows.Count * 35;
				totalRect = totalRect.ScaledBy(2f);
			}
			else
			{
				totalRect.height += pawnRows.Count * 30;
				totalRect = totalRect.ScaledBy(1.1f);
			}

			if (Mouse.IsOver(rect))
			{
				pawnWindowIsActive = true;
				DrawPawnRows(rect, pawnRows);
				if (!ShowExpanded)
				{
					TooltipHandler.TipRegion(rect, new TipSignal("TG.GroupInfoTooltip".Translate(groupName)));
				}
				HandleClicks(rect);
			}
			else if (Mouse.IsOver(totalRect) && pawnWindowIsActive || showPawnIconsRightClickMenu)
			{
				DrawPawnRows(rect, pawnRows);
			}
			else
			{
				pawnWindowIsActive = false;
				expandPawnIcons = false;
				DrawOverlays(rect);
			}
		}

		private int mentalStateBlink;
		private int downedStateBlink;
		public void DrawOverlays(Rect rect)
        {
			var groupLabelRect = new Rect(rect.x, rect.y + rect.height, rect.width, 20f);
			Text.Anchor = TextAnchor.MiddleCenter;

			Widgets.Label(groupLabelRect, this.GetGroupName());
			Text.Anchor = TextAnchor.UpperLeft;

			var pawnRows = GetPawnRows(8);
			var initialRect = new Rect(rect);
			initialRect.y += initialRect.height * 1.2f;
			initialRect.x -= Textures.ColonistDot.width - 3f;

			bool showDownedState = false;
			for (var i = 0; i < pawnRows.Count; i++)
			{
				for (var j = 0; j < pawnRows[i].Count; j++)
				{
					Rect dotRect = new Rect(initialRect.x + ((j + 1) * Textures.ColonistDot.width), initialRect.y + ((i + 1) * Textures.ColonistDot.height),
						Textures.ColonistDot.width, Textures.ColonistDot.height);
					if (pawnRows[i][j].MentalState != null)
                    {
						mentalStateBlink++;
						if (mentalStateBlink < 30)
                        {
							GUI.DrawTexture(dotRect, Textures.ColonistDotMentalState);
                        }
						else if (mentalStateBlink > 60)
                        {
							mentalStateBlink = 0;
						}
					}
					else if (pawnRows[i][j].IsDownedOrIncapable())
					{
						showDownedState = true;
						downedStateBlink++;
						if (downedStateBlink < 30)
						{
							GUI.DrawTexture(dotRect, Textures.ColonistDotDowned);
						}
						else if (downedStateBlink > 60)
						{
							downedStateBlink = 0;
						}
					}
					else if (pawnRows[i][j].IsShotOrBleeding())
					{
						GUI.DrawTexture(dotRect, Textures.ColonistDotDowned);
					}
					else if (pawnRows[i][j].IsSick())
                    {
						GUI.DrawTexture(dotRect, Textures.ColonistDotToxic);
					}
					else
                    {
						GUI.DrawTexture(dotRect, Textures.ColonistDot);
                    }
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(dotRect))
					{
						Event.current.Use();
						CameraJumper.TryJumpAndSelect(pawnRows[i][j]);
					}
				}
			}

			if (showDownedState)
            {

				if (downedStateBlink < 30)
				{
					GUI.DrawTexture(rect, Textures.GroupOverlayColonistDown);
				}
				else if (downedStateBlink > 60)
				{
					downedStateBlink = 0;
				}
			}
		}

		public void DrawPawnRows(Rect rect, List<List<Pawn>> pawnRows)
        {
			if (ShowExpanded)
            {
				var initialRect = new Rect(rect.x, rect.y + rect.height + (rect.height / 5f), rect.width, rect.height);
				initialRect.x -= initialRect.width / 2;
				for (var i = 0; i < pawnRows.Count; i++)
				{
					for (var j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(initialRect.x + (j * 60), initialRect.y + (i * 70), 50, 50);
						DrawColonist(smallRect, pawnRows[i][j], pawnRows[i][j].Map, true, false);
						pawnRects[pawnRows[i][j]] = smallRect;
					}
				}
			}
			else
            {
				var backGroundRect = new Rect(rect.x, rect.y + rect.height, rect.width, pawnRows.Count * 30f);
				Widgets.DrawBox(backGroundRect);
				GUI.DrawTexture(backGroundRect, Textures.BackgroundColonistLayer);
				for (var i = 0; i < pawnRows.Count; i++)
				{
					for (var j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(backGroundRect.x + (j * 25) + 2f, backGroundRect.y + (i * 30) + 3f, 24, 24);
						DrawColonist(smallRect, pawnRows[i][j], pawnRows[i][j].Map, true, false);
						pawnRects[pawnRows[i][j]] = smallRect;
					}
				}
			}
		}
		public void DrawColonist(Rect rect, Pawn colonist, Map pawnMap, bool highlight, bool reordering)
		{
			float alpha = TacticUtils.TacticalColonistBar.GetEntryRectAlpha(rect);
			TacticUtils.TacticalColonistBar.drawer.ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref alpha);
			if (reordering)
			{
				alpha *= 0.5f;
			}
			Color color2 = GUI.color = new Color(1f, 1f, 1f, alpha);
			GUI.DrawTexture(rect, TacticalColonistBar.BGTex);
			if (colonist.needs != null && colonist.needs.mood != null)
			{
				Rect position = rect.ContractedBy(2f);
				float num = position.height * colonist.needs.mood.CurLevelPercentage;
				position.yMin = position.yMax - num;
				position.height = num;
				GUI.DrawTexture(position, ColonistBarColonistDrawer.MoodBGTex);
			}

			//if (highlight)
			//{
			//	int thickness = (rect.width <= 22f) ? 2 : 3;
			//	GUI.color = Color.white;
			//	Widgets.DrawBox(rect, thickness);
			//	GUI.color = color2;
			//}
			Rect rect2 = rect.ContractedBy(-2f * TacticUtils.TacticalColonistBar.Scale);
			if ((colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist.Corpse) : Find.Selector.SelectedObjects.Contains(colonist)) && !WorldRendererUtility.WorldRenderedNow)
			{
				DrawSelectionOverlayOnGUI(colonist, rect2);
			}
			else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
			{
				DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
			}
			GUI.DrawTexture(GetPawnTextureRect(rect.position), PortraitsCache.Get(colonist, ColonistBarColonistDrawer.PawnTextureSize, ColonistBarColonistDrawer.PawnTextureCameraOffset, 1.28205f));
			GUI.color = new Color(1f, 1f, 1f, alpha * 0.8f);
			if (ShowExpanded)
            {
				TacticUtils.TacticalColonistBar.drawer.DrawIcons(rect, colonist);
			}
			GUI.color = color2;
			if (colonist.Dead)
			{
				GUI.DrawTexture(rect, ColonistBarColonistDrawer.DeadColonistTex);
			}

			if (ShowExpanded)
            {
				float num2 = 4f * TacticUtils.TacticalColonistBar.Scale;
				Vector2 pos = new Vector2(rect.center.x, rect.yMax - num2);
				GenMapUI.DrawPawnLabel(colonist, pos, alpha, rect.width + TacticUtils.TacticalColonistBar.SpaceBetweenColonistsHorizontal - 2f, TacticUtils.TacticalColonistBar.drawer.pawnLabelsCache);
			}
			Text.Font = GameFont.Small;
			GUI.color = Color.white;

			if (rect.Contains(Event.current.mousePosition))
			{
				string text = ShouldShowShotReport(colonist) ? TooltipUtility.ShotCalculationTipString(colonist) : null;
				if (colonist.def.hasTooltip || !text.NullOrEmpty())
				{
					TipSignal tooltip = colonist.GetTooltip();
					if (!text.NullOrEmpty())
					{
						ref string text2 = ref tooltip.text;
						text2 = text2 + "\n\n" + text;
					}
					TooltipHandler.TipRegion(rect, tooltip);
				}
			}

			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 2 && Mouse.IsOver(rect))
			{
				Event.current.Use();
				CameraJumper.TryJump(colonist);
			}
		}

		private bool ShouldShowShotReport(Thing t)
		{
			if (!t.def.hasTooltip && !(t is Hive))
			{
				return t is IAttackTarget;
			}
			return true;
		}

		public Rect GetPawnTextureRect(Vector2 pos)
		{
			float x = pos.x;
			float y = pos.y;
			Vector2 vector = new Vector2(46f, 75f);
			//Vector2 vector = ColonistBarColonistDrawer.PawnTextureSize * TacticUtils.TacticalColonistBar.Scale;
			var rect = new Rect(x + 1f, y - ((vector.y - 48f) * IconScale) - 1f, vector.x * IconScale, vector.y * IconScale).ContractedBy(1f);
			return rect;
		}

		private static Vector2[] bracketLocs = new Vector2[4];

		public void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
		{
			Thing obj = colonist;
			if (colonist.Dead)
			{
				obj = colonist.Corpse;
			}
			float num = 0.4f * TacticUtils.TacticalColonistBar.Scale;
			SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<object>(textureSize: new Vector2((float)SelectionDrawerUtility.SelectedTexGUI.width * num, 
				(float)SelectionDrawerUtility.SelectedTexGUI.height * num), bracketLocs: bracketLocs, obj: (object)obj, rect: rect, selectTimes: SelectionDrawer.SelectTimes, 
				jumpDistanceFactor: 20f * TacticUtils.TacticalColonistBar.Scale);
			DrawSelectionOverlayOnGUI(bracketLocs, num);
		}

		public void DrawCaravanSelectionOverlayOnGUI(Caravan caravan, Rect rect)
		{
			float num = 0.4f * TacticUtils.TacticalColonistBar.Scale;
			SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<WorldObject>(textureSize: new Vector2((float)SelectionDrawerUtility.SelectedTexGUI.width * num, 
				(float)SelectionDrawerUtility.SelectedTexGUI.height * num), bracketLocs: bracketLocs, obj: (WorldObject)caravan, rect: rect, selectTimes: WorldSelectionDrawer.SelectTimes,
				jumpDistanceFactor: 20f * TacticUtils.TacticalColonistBar.Scale);
			DrawSelectionOverlayOnGUI(bracketLocs, num);
		}

		public void DrawSelectionOverlayOnGUI(Vector2[] bracketLocs, float selectedTexScale)
		{
			int num = 90;
			for (int i = 0; i < 4; i++)
			{
				Widgets.DrawTextureRotated(bracketLocs[i], SelectionDrawerUtility.SelectedTexGUI, num, selectedTexScale);
				num += 90;
			}
		}

		public SortBy activeSortBy;

		public SkillDef skillDefSort;

		public void InitSort(SortBy newSortBy)
        {
			activeSortBy = newSortBy;
			Sort();
		}
		public void Sort()
        {
			if (activeSortBy == SortBy.Skills)
            {
				this.pawns.SortByDescending(x => x.skills.GetSkill(skillDefSort).Level);
            }
        }


		public virtual void ExposeData()
        {
			Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
			Scribe_Collections.Look(ref pawnIcons, "pawnIcons", LookMode.Reference, LookMode.Deep, ref pawnKeys, ref pawnIconValues);
			Scribe_Collections.Look(ref formations, "formations", LookMode.Reference, LookMode.Deep, ref pawnKeys2, ref intVecValues);
			Scribe_Values.Look(ref groupName, "groupName");
			Scribe_Values.Look(ref groupID, "groupID");
			Scribe_Values.Look(ref groupIconName, "groupIconName");
			Scribe_Values.Look(ref groupIconFolder, "groupIconFolder");
			Scribe_Values.Look(ref activeSortBy, "activeSortBy");
			Scribe_Defs.Look(ref skillDefSort, "skillDefSort");
		}

		private List<Pawn> pawnKeys;
		private List<PawnIcon> pawnIconValues;

		private List<Pawn> pawnKeys2;
		private List<IntVec3> intVecValues;
    }
}
