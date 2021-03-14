using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public class PawnDownedStateCache
	{
		public PawnDownedStateCache()
		{

		}
		public bool downed;
		public int updateCount;
	}

	public class PawnDot
    {
		public Pawn pawn;
		public Rect rect;
		public PawnState state;
		public PawnDot(Pawn pawn, Rect rect, PawnState state)
        {
			this.pawn = pawn;
			this.rect = rect;
			this.state = state;
        }
    }

	public class Formation : IExposable
    {
		public Dictionary<Pawn, IntVec3> formations = new Dictionary<Pawn, IntVec3>();
		public string colorPrefix;
		public bool isSelected;
		public Formation()
        {

        }

		public Formation(string color)
		{
			colorPrefix = color;
		}
		public void ExposeData()
        {
			Scribe_Collections.Look(ref formations, "formations", LookMode.Reference, LookMode.Value, ref pawnKeys2, ref intVecValues);
			Scribe_Values.Look(ref colorPrefix, "colorPrefix");
		}


		public Texture2D Icon => this.formations != null && this.formations.Any() ?
			this.isSelected ? ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/" + colorPrefix + "select")
			: ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/" + colorPrefix + "dark")
			: this.isSelected ? ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/greyselect")
			: ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/greydark");

		private List<Pawn> pawnKeys2;
		private List<IntVec3> intVecValues;
	}
	public class ColonistGroup : IExposable
    {
		public bool pawnWindowIsActive;
		public bool groupButtonRightClicked;
		public Rect curRect;
		private bool expandPawnIcons;
		public bool showPawnIconsRightClickMenu;
		public float IconScale => ShowExpanded ? 1f : 0.5f;
		public bool updateIcon = true;
		protected int pawnRowCount;
		protected int pawnDocRowCount;
		protected float pawnRowXPosShift;
		public string curGroupName;
		private float cachedGroupNameHeight;
		public bool ShowExpanded
        {
			get
            {
				if (pawnWindowIsActive && expandPawnIcons)
                {
					return true;
                }
				return false;
			}
		}

		public virtual Map Map { get; }
		public virtual List<Pawn> ActivePawns { get; }

		public virtual List<Pawn> VisiblePawns { get;  }

		public void ResetDrawOptions()
        {
			this.groupButtonRightClicked = false;
			this.expandPawnIcons = false;
			this.pawnWindowIsActive = false;
			this.showPawnIconsRightClickMenu = false;
		}
		public virtual void Init()
        {
			this.pawns = new List<Pawn>();
			this.pawnIcons = new Dictionary<Pawn, PawnIcon>();
			this.formations = new List<Formation>(4);
			this.temporaryWorkers = new Dictionary<Pawn, WorkType>();
			this.activeWorkTypes = new Dictionary<WorkType, WorkState>();
			this.entireGroupIsVisible = true;
		}

		public void SetName(string name)
        {
			this.groupName = name;
			this.curGroupName = this.groupName;
			this.cachedGroupNameHeight = Text.CalcHeight(this.curGroupName, groupBanner.width);
		}
		public virtual void Add(Pawn pawn)
        {
			if (pawn.Faction != Faction.OfPlayer || !pawn.RaceProps.Humanlike)
            {
				return;
            }

			if (this.pawns is null) 
				this.pawns = new List<Pawn>();

			if (!this.pawns.Contains(pawn))
            {
				this.pawns.Add(pawn);
				this.pawnIcons[pawn] = new PawnIcon(pawn, this.entireGroupIsVisible ? true : false);
				SyncPoliciesFor(pawn);
				Sort();
				this.UpdateData();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
			TacticUtils.RegisterGroupFor(pawn, this);
		}

		public void Add(List<Pawn> newPawns)
		{
			foreach (var pawn in newPawns)
			{
				Add(pawn);
			}
		}

		public virtual void Disband()
		{

		}
		public virtual void Disband(Pawn pawn)
        {
			if (pawns.Contains(pawn))
            {
				this.pawns.Remove(pawn);
				this.pawnIcons.Remove(pawn);
				Sort();
				this.UpdateData();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
		}

		private Dictionary<int, List<List<Pawn>>> cachedPawnRows = new Dictionary<int, List<List<Pawn>>>();
		public List<List<Pawn>> GetPawnRows(int columnCount)
        {
			if (cachedPawnRows.TryGetValue(columnCount, out List<List<Pawn>> value))
			{
				return value;
			}
			else
			{
				var value2 = GetPawnRowsInt(columnCount);
				cachedPawnRows[columnCount] = value2;
				return value2;
			}
		}
		public List<List<Pawn>> GetPawnRowsInt(int columnCount)
        {
			int num = 0;
			List<List<Pawn>> pawnRows = new List<List<Pawn>>();
			List<Pawn> row = new List<Pawn>();
			bool refresh = false;
			for (int ind = pawns.Count - 1; ind >= 0; ind--)
            {
				if (pawns[ind].Destroyed || pawns[ind].Dead)
                {
					this.pawns.Remove(pawns[ind]);
					this.pawnIcons.Remove(pawns[ind]);
					refresh = true;
				}
            }
			if (refresh)
            {
				Sort();
				this.UpdateData();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
			foreach (var pawn in VisiblePawns)
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

		private List<PawnDot> cachedPawnDots = new List<PawnDot>();
		public List<PawnDot> GetPawnDots(Rect rect)
        {
			if (cachedPawnDots != null)
			{
				return cachedPawnDots;
			}
			else
			{
				cachedPawnDots = GetPawnDotsInt(rect);
				return cachedPawnDots;
			}
		}

		public float GroupIconScale
        {
			get
            {
				if (this.isColonyGroup)
				{
					return TacticalGroupsSettings.ColonyGroupScale;
				}
				else
				{
					if (this.isSubGroup)
					{
						return TacticalGroupsSettings.GroupScale / 2f;
					}
					return TacticalGroupsSettings.GroupScale;
				}
			}
        }

		public List<PawnDot> GetPawnDotsInt(Rect rect)
		{
			var pawnDots = new List<PawnDot>();
			var pawnRows = GetPawnRows(this.bannerModeEnabled ? 4 : this.pawnDocRowCount);
			var initialRect = new Rect(rect);
			if (this.bannerModeEnabled)
			{
				initialRect.y += rect.height;
				initialRect.x -= 4f;
			}
			else
			{
				initialRect.y += initialRect.height * 1.2f;
			}
			initialRect.x -= Textures.ColonistDot.width - 3f;

			initialRect.y -= 3f;

			for (var i = 0; i < pawnRows.Count; i++)
			{
				for (var j = 0; j < pawnRows[i].Count; j++)
				{
					Rect dotRect = new Rect(initialRect.x + ((j + 1) * (Textures.ColonistDot.width * GroupIconScale)), initialRect.y + ((i + 1)
						* (Textures.ColonistDot.height * GroupIconScale)),
						Textures.ColonistDot.width, Textures.ColonistDot.height);
					var pawn = pawnRows[i][j];
					var state = PawnStateUtility.GetPawnState(pawn);
					pawnDots.Add(new PawnDot(pawn, dotRect, state));
				}
			}
			return pawnDots;
		}
		public void HandleClicks(Rect rect, Rect totalRect)
		{
			if (Event.current.type == EventType.MouseDown)
			{
				foreach (var group in TacticUtils.AllGroups)
				{
					
					if (group != this && group.pawnWindowIsActive && Mouse.IsOver(rect) && group.curRect.y > rect.y)
					{
						return;
					}
				}

				foreach (var group in TacticUtils.AllGroups)
				{
					if (group != this)
					{
						group.expandPawnIcons = false;
						group.pawnWindowIsActive = false;
						group.groupButtonRightClicked = false;
						group.showPawnIconsRightClickMenu = false;
						var window = Find.WindowStack.WindowOfType<MainFloatMenu>();
						if (window != null)
                        {
							window.CloseAllWindows();
                        }
					}
				}

				if (Event.current.button == 0)
				{
					if (Event.current.clickCount == 1)
					{
						TacticDefOf.TG_LeftClickGroupSFX.PlayOneShotOnCamera();
						if (WorldRendererUtility.WorldRenderedNow && this.Map != null || this.Map != null && this.Map != Find.CurrentMap)
						{
							if (this.ActivePawns.Any())
                            {
								CameraJumper.TryJump(this.ActivePawns.First());
                            }
							if (this is CaravanGroup caravanGroup)
							{
								var caravan = TacticUtils.TacticalGroups.caravanGroups.Where(x => x.Value == caravanGroup).FirstOrDefault().Key;
								Find.Selector.Select(caravan);
							}
							Event.current.Use();
							return;
						}
						else if (this.Map == null)
						{
							CameraJumper.TryJump(this.pawns.First());
							Event.current.Use();
							return;
						}
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
							if (!pawn.IsWorldPawn())
							{
								Find.Selector.Select(pawn);
							}
						}
					}
				}
				else if (Event.current.button == 1 && !(this is CaravanGroup))
				{
					this.showPawnIconsRightClickMenu = true;
					this.expandPawnIcons = false;
					this.groupButtonRightClicked = true;
					var rect2 = new Rect(rect.x, rect.y + rect.height, rect.width, rect.height);
					TieredFloatMenu floatMenu = new MainFloatMenu(null, this, rect2, Textures.DropMenuRightClick);
					Find.WindowStack.Add(floatMenu);
				}
				else if (Event.current.button == 2)
				{
					if (this.entireGroupIsVisible || !this.pawnIcons.Where(x => !x.Value.isVisibleOnColonistBar).Any())
					{
						TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
						foreach (var pawnIcon in this.pawnIcons)
						{
							pawnIcon.Value.isVisibleOnColonistBar = false;
						}
						this.entireGroupIsVisible = false;
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					else
					{
						TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
						foreach (var pawnIcon in this.pawnIcons)
						{
							pawnIcon.Value.isVisibleOnColonistBar = true;
						}
						this.entireGroupIsVisible = true;
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
				}
				Event.current.Use();
			}
		}

		public void Notify_WindowsClosed()
        {
			this.ResetDrawOptions();
        }

		private Texture2D mergedTexture;
		private Texture2D darkenMergedTexture;
		public void UpdateIcon()
		{
			var bannerPath = "";
			if (this.bannerModeEnabled)
			{
				bannerPath = "UI/ColonistBar/GroupIcons/BannerMode/" + groupBannerFolder;
			}
			else
			{
				bannerPath = "UI/ColonistBar/GroupIcons/" + groupBannerFolder;
			}
			var banners = ContentFinder<Texture2D>.GetAllInFolder(bannerPath);
			var banner = banners.Where(x => x.name == groupBannerName).FirstOrDefault();
			if (banner != null)
			{
				this.groupBanner = banner;
			}

			var iconPath = "";
			if (bannerModeEnabled)
			{
				iconPath = "UI/ColonistBar/GroupIcons/BannerMode/" + groupIconFolder;
			}
			else
			{
				iconPath = "UI/ColonistBar/GroupIcons/" + groupIconFolder;
			}
			var icons = ContentFinder<Texture2D>.GetAllInFolder(iconPath).OrderBy(x => x.name).ToList();
			var icon = icons.Where(x => x.name == groupIconName).FirstOrDefault();

			if (icon != null)
			{
				this.groupIcon = icon;
			}
			if (this is CaravanGroup)
			{
				mergedTexture = this.groupIcon;
				darkenMergedTexture = TexturesUtils.GetDarkenTexture(TexturesUtils.GetReadableTexture(this.groupIcon));
			}
			else
			{
				mergedTexture = TexturesUtils.GetMergedTexture(this.groupBanner, this.groupIcon);
				darkenMergedTexture = TexturesUtils.GetMergedDarkenTexture(this.groupBanner, this.groupIcon);
			}

			this.updateIcon = false;
			this.cachedGroupNameHeight = Text.CalcHeight(this.curGroupName, groupBanner.width);
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}

		public virtual void Draw(Rect rect)
        {
			GUI.color = Color.white;
			Text.Font = GameFont.Tiny;
			this.curRect = rect;
			if (this.updateIcon)
            {
				UpdateIcon();
			}
			if (!hideGroupIcon)
			{
				if (!this.isColonyGroup || Find.CurrentMap == this.Map)
                {
					GUI.DrawTexture(rect, mergedTexture);
				}
				else
                {
					GUI.DrawTexture(rect, darkenMergedTexture);
				}
			}
			else if (Mouse.IsOver(rect))
            {
				GUI.DrawTexture(rect, mergedTexture);
			}

			if (!hideGroupIcon)
            {
				if (!groupButtonRightClicked && Mouse.IsOver(rect))
				{
					GUI.DrawTexture(rect, Textures.GroupIconHover);
				}
				else if (groupButtonRightClicked)
				{
					if (this.bannerModeEnabled)
					{
						GUI.DrawTexture(rect, Textures.BannerIconSelected);
					}
					else if (this.isPawnGroup)
					{
						GUI.DrawTexture(rect, Textures.GroupIconSelected);
					}
					else if (this.isColonyGroup || this.isTaskForce)
					{
						GUI.DrawTexture(rect, Textures.ColonyIconSelected);
					}
				}
			}


			if (!this.isSubGroup)
			{
				if (!this.bannerModeEnabled && !this.hideGroupIcon)
                {
					var groupLabelRect = new Rect(rect.x, rect.y + rect.height, rect.width, cachedGroupNameHeight);
					Text.Anchor = TextAnchor.UpperCenter;
					Widgets.Label(groupLabelRect, this.curGroupName);
					Text.Anchor = TextAnchor.UpperLeft;
				}

				if (!this.hidePawnDots && !this.hideGroupIcon)
				{
					DrawPawnDots(rect);
				}
			}
		}

		public virtual void DrawOverlays(Rect rect)
        {
			var totalRect = Rect.zero;
			var pawnRows = GetPawnRows(this.pawnRowCount);
			if (ShowExpanded)
			{
				totalRect = rect;
				totalRect.height += pawnRows.Count * 75f;
				totalRect.x = (rect.x + (rect.width / 2f));
				totalRect.x -= ((this.pawnRowCount * 75f) / 2f);
				totalRect.width = 75f * pawnRowCount;
			}
			else
			{
				if (this.bannerModeEnabled)
				{
					totalRect = new Rect(rect.x - (rect.width / 1.7f), rect.y, 80f, rect.height);
				}
				else
				{
					totalRect = new Rect(rect.x, rect.y, rect.width, rect.height);
				}

				totalRect = totalRect.ScaledBy(1.2f);
				totalRect.height += pawnRows.Count * 30;
			}
			totalRect.yMin = rect.yMax;
			if (Mouse.IsOver(rect))
			{
				if (!this.isSubGroup)
                {
					bool showThisPawnWindow = true;
					foreach (var group in TacticUtils.AllGroups)
					{
						if (group != this && group.pawnWindowIsActive)
                        {
							showThisPawnWindow = false;
							break;
						}
					}
					if (showThisPawnWindow)
                    {
						pawnWindowIsActive = true;
						DrawPawnRows(rect, pawnRows);
						DrawPawnArrows(rect, pawnRows);
					}
				}
				else if (showPawnIconsRightClickMenu)
                {
					var subGroupRect = new Rect(rect);
					subGroupRect.x -= (rect.width);
					DrawPawnRows(subGroupRect, pawnRows);
					DrawPawnArrows(subGroupRect, pawnRows);
				}
				if (!ShowExpanded)
				{
					TooltipHandler.TipRegion(rect, new TipSignal("TG.GroupInfoTooltip".Translate(this.curGroupName)));
				}
				HandleClicks(rect, totalRect);
			}
			else if (!this.isSubGroup && (Mouse.IsOver(totalRect) && pawnWindowIsActive || showPawnIconsRightClickMenu))
			{
				DrawPawnRows(rect, pawnRows);
				DrawPawnArrows(rect, pawnRows);
			}
			else if (this.isSubGroup && showPawnIconsRightClickMenu)
            {
				var subGroupRect = new Rect(rect);
				subGroupRect.x -= (rect.width);
				DrawPawnRows(subGroupRect, pawnRows);
				DrawPawnArrows(subGroupRect, pawnRows);
			}
			else if (!this.isSubGroup)
			{
				pawnWindowIsActive = false;
				expandPawnIcons = false;
			}
			else if (this.isSubGroup && !this.hideLifeOverlay)
			{
				DrawLifeOverlayWithDisabledDots(rect);
			}
		}

		private int downedStateBlink;
		public void UpdateData()
        {
			cachedPawnRows[this.pawnRowCount] = GetPawnRowsInt(this.pawnRowCount);
			var pawnDocCount = this.bannerModeEnabled ? 4 : this.pawnDocRowCount;
			cachedPawnRows[pawnDocCount] = GetPawnRowsInt(pawnDocCount);
			cachedPawnDots = null;
		}
		public void DrawPawnDots(Rect rect)
        {
			var pawnDots = GetPawnDots(rect);
			bool showDownedState = false;
			for (var i = 0; i < pawnDots.Count; i++)
			{
				var pawnDot = pawnDots[i];
				var pawn = pawnDot.pawn;
				var dotRect = pawnDot.rect;
				switch (pawnDot.state)
				{
					case PawnState.MentalState: GUI.DrawTexture(dotRect, Textures.ColonistDotMentalState); break;
					case PawnState.IsDownedOrIncapable:
						if (!showDownedState)
						{
							downedStateBlink++;
						}
						showDownedState = true;
						if (downedStateBlink < 30)
						{
							GUI.DrawTexture(dotRect, Textures.ColonistDotDowned);
						}
						else if (downedStateBlink > 60)
						{
							downedStateBlink = 0;
						}
						break;
					case PawnState.IsBleeding: GUI.DrawTexture(dotRect, Textures.ColonistDotDowned); break;
					case PawnState.Sick: GUI.DrawTexture(dotRect, Textures.ColonistDotToxic); break;
					case PawnState.Inspired: GUI.DrawTexture(dotRect, Textures.ColonistDotInspired); break;
					case PawnState.None: GUI.DrawTexture(dotRect, Textures.ColonistDot); break;
					default: break;
				}

				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(dotRect))
				{
					bool select = true;
					foreach (var group in TacticUtils.AllGroups)
					{
						if (group != this)
						{
							if (group.pawnWindowIsActive)
							{
								select = false;
								break;
							}
						}
					}
					if (select)
                    {
						Event.current.Use();
						CameraJumper.TryJumpAndSelect(pawn);
					}
				}
			}

			if (!this.hideLifeOverlay && showDownedState)
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

		private Dictionary<Pawn, PawnDownedStateCache> pawnDownedStates = new Dictionary<Pawn, PawnDownedStateCache>();
		private bool GetPawnDownedState(Pawn pawn)
		{
			if (pawnDownedStates.TryGetValue(pawn, out PawnDownedStateCache pawnDownedStateCache))
			{

				if (pawnDownedStateCache.updateCount == 0)
				{
					pawnDownedStateCache.downed = pawn.IsDownedOrIncapable();
					pawnDownedStateCache.updateCount = 60;
				}
				pawnDownedStateCache.updateCount--;
				return pawnDownedStateCache.downed;
			}
			else
			{
				pawnDownedStates[pawn] = new PawnDownedStateCache();
				pawnDownedStates[pawn].downed = pawn.IsDownedOrIncapable();
				return pawnDownedStates[pawn].downed;
			}
		}

		private void DrawLifeOverlayWithDisabledDots(Rect rect)
        {
			if (!this.hideLifeOverlay)
            {
				bool showDownedState = false;
				foreach (var pawn in this.pawns)
				{
					if (GetPawnDownedState(pawn))
					{
						if (!showDownedState)
						{
							downedStateBlink++;
						}
						showDownedState = true;
						break;
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

		}

		public void DrawPawnRows(Rect rect, List<List<Pawn>> pawnRows)
        {
			if (ShowExpanded)
            {
				Rect initialRect = new Rect(rect.x, rect.y + rect.height + (rect.height / 5f), rect.width, rect.height);
				initialRect.x = (rect.x + (rect.width / 2f));
				initialRect.x -= ((this.pawnRowCount * 65f) / 2f);
				initialRect.x += 8f;
				for (var i = 0; i < pawnRows.Count; i++)
				{
					for (var j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(initialRect.x + (j * 65), initialRect.y + (i * 70), 50, 50);
						DrawColonist(smallRect, pawnRows[i][j], pawnRows[i][j].Map, false);
						pawnRects[pawnRows[i][j]] = smallRect;
					}
				}
			}
			else
            {
				Rect backGroundRect = Rect.zero;
				if (this.bannerModeEnabled)
                {
					backGroundRect = new Rect(rect.x - (rect.width / 1.7f), rect.y + rect.height, 80f, pawnRows.Count * 30f);
				}
				else
                {
					backGroundRect = new Rect(rect.x, rect.y + rect.height, rect.width, pawnRows.Count * 30f);
				}
				GUI.DrawTexture(backGroundRect, Textures.BackgroundColonistLayer);
				for (var i = 0; i < pawnRows.Count; i++)
				{
					for (var j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(backGroundRect.x + (j * 25) + 2f, backGroundRect.y + (i * 30) + 3f, 24, 24);
						DrawColonist(smallRect, pawnRows[i][j], pawnRows[i][j].Map, false);
						pawnRects[pawnRows[i][j]] = smallRect;
					}
				}
			}
		}
		public void DrawPawnArrows(Rect rect, List<List<Pawn>> pawnRows)
		{
			if (ShowExpanded)
			{
				Rect initialRect = new Rect(rect.x, rect.y + rect.height + (rect.height / 5f), rect.width, rect.height);
				initialRect.x = (rect.x + (rect.width / 2f));
				initialRect.x -= ((this.pawnRowCount * 65f) / 2f);
				initialRect.x += 8f;
				for (var i = 0; i < pawnRows.Count; i++)
				{
					for (var j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(initialRect.x + (j * 65), initialRect.y + (i * 70), 50, 50);
						DrawPawnArrows(smallRect, pawnRows[i][j]);
					}
				}
			}
			else
			{
				Rect backGroundRect = Rect.zero;
				if (this.bannerModeEnabled)
				{
					backGroundRect = new Rect(rect.x - (rect.width / 1.7f), rect.y + rect.height, 80f, pawnRows.Count * 30f);
				}
				else
				{
					backGroundRect = new Rect(rect.x, rect.y + rect.height, rect.width, pawnRows.Count * 30f);
				}
				for (var i = 0; i < pawnRows.Count; i++)
				{
					for (var j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(backGroundRect.x + (j * 25) + 2f, backGroundRect.y + (i * 30) + 3f, 24, 24);
						DrawPawnArrows(smallRect, pawnRows[i][j]);
					}
				}
			}
		}

		public Dictionary<Pawn, bool> pawnReorderingMode = new Dictionary<Pawn, bool>();
		public void DrawPawnArrows(Rect rect, Pawn pawn)
		{
			bool reset = true;
			if (Mouse.IsOver(rect))
			{
				reset = false;
				if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.clickCount == 1)
				{
					Event.current.Use();
					pawnReorderingMode[pawn] = true;
				}
			}

			if (pawnReorderingMode.TryGetValue(pawn, out bool value) && value)
			{
				var rightPawnArrowRect = new Rect(rect.x + rect.width, rect.y, Textures.PawnArrowRight.width, Textures.PawnArrowRight.height);
				
				if (Mouse.IsOver(rightPawnArrowRect.ExpandedBy(3f)))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(rightPawnArrowRect))
					{
						var indexOf = this.pawns.IndexOf(pawn);
						if (this.pawns.Count > indexOf + 1)
						{
							this.pawns.RemoveAt(indexOf);
							this.pawns.Insert(indexOf + 1, pawn);
						}
						else if (indexOf != 0)
						{
							this.pawns.RemoveAt(indexOf);
							this.pawns.Insert(0, pawn);
						}
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					reset = false;
				}
				GUI.DrawTexture(rightPawnArrowRect, Textures.PawnArrowRight);

				var leftPawnArrowRect = new Rect(rect.x - Textures.PawnArrowLeft.width, rect.y, Textures.PawnArrowLeft.width, Textures.PawnArrowLeft.height);
				if (Mouse.IsOver(leftPawnArrowRect.ExpandedBy(3f)))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(leftPawnArrowRect))
					{
						var indexOf = this.pawns.IndexOf(pawn);
						if (indexOf > 0)
						{
							this.pawns.RemoveAt(indexOf);
							this.pawns.Insert(indexOf - 1, pawn);
						}
						else if (indexOf != this.pawns.Count)
						{
							this.pawns.RemoveAt(indexOf);
							this.pawns.Insert(this.pawns.Count, pawn);
						}
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					reset = false;
				}
				GUI.DrawTexture(leftPawnArrowRect, Textures.PawnArrowLeft);
			}

			if (reset)
			{
				pawnReorderingMode[pawn] = false;
			}
		}

		public void DrawColonist(Rect rect, Pawn colonist, Map pawnMap, bool reordering)
		{
			float alpha = TacticUtils.TacticalColonistBar.GetEntryRectAlpha(rect);
			bool inCryptosleep = !colonist.Spawned && colonist.ParentHolder is Building_CryptosleepCasket;
			if (!inCryptosleep)
            {
				TacticUtils.TacticalColonistBar.drawer.ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref alpha);
            }
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
				if (TacticalGroupsSettings.DisplayColorBars && ShowExpanded)
				{
					GUI.DrawTexture(position, ColonistBarColonistDrawer.GetMoodBarTexture(colonist));
				}
				else
				{
					GUI.DrawTexture(position, ColonistBarColonistDrawer.MoodBGTex);
				}
			}

			Rect rect2 = rect.ContractedBy(-2f * TacticUtils.TacticalColonistBar.Scale);
			if ((colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist.Corpse) : Find.Selector.SelectedObjects.Contains(colonist)) && !WorldRendererUtility.WorldRenderedNow)
			{
				DrawSelectionOverlayOnGUI(colonist, rect2);
			}
			else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
			{
				DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
			}
			var pawnTextureRect = GetPawnTextureRect(rect.position);
			GUI.DrawTexture(pawnTextureRect, PortraitsCache.Get(colonist, ColonistBarColonistDrawer.PawnTextureSize, ColonistBarColonistDrawer.PawnTextureCameraOffset, 1.28205f));
			if (colonist.Drafted)
			{
				GUI.DrawTexture(rect, Textures.PawnDrafted);
			}
			if (ModCompatibility.CombatExtendedIsActive)
            {
				var gun = colonist.equipment?.Primary ?? null;
				if (gun != null && gun.def.IsRangedWeapon && (!(bool)ModCompatibility.combatExtendedHasAmmo_Method.Invoke(null, new object[]
				{
						gun
				})))
				{
					GUI.DrawTexture(rect, Textures.PawnOutofAmmo);
				}
			}
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
			else if (colonist.IsPrisoner)
            {
				GUI.DrawTexture(rect, Textures.PawnPrisoner);
			}

			var pawnStates = PawnStateUtility.GetAllPawnStatesCache(colonist);
			if (pawnStates.Contains(PawnState.IsBleeding))
			{
				GUI.DrawTexture(rect, Textures.PawnBleeding);
			}

			if (inCryptosleep)
			{
				GUI.DrawTexture(rect, Textures.CryosleepOverlay);
			}
			if (ShowExpanded)
            {
				float num2 = 4f * TacticUtils.TacticalColonistBar.Scale;
				Vector2 pos = new Vector2(rect.center.x, rect.yMax - num2);
				GenMapUI.DrawPawnLabel(colonist, pos, alpha, rect.width + TacticUtils.TacticalColonistBar.SpaceBetweenColonistsHorizontal - 2f, TacticUtils.TacticalColonistBar.drawer.pawnLabelsCache);
			}
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			if (ShowExpanded)
            {
				ColonistBarColonistDrawer.DrawHealthBar(colonist, rect);
				ColonistBarColonistDrawer.DrawRestAndFoodBars(colonist, rect, Textures.RestFood.width);
				ColonistBarColonistDrawer.ShowDrafteesWeapon(rect, colonist, 10);
			}

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
			else if (activeSortBy == SortBy.Name)
            {
				this.pawns.SortBy(x => x.Name.ToStringFull);
			}
			this.UpdateData();
        }
		public void SyncPoliciesFor(Pawn pawn)
		{
			if (groupFoodRestriction != null)
			{
				pawn.foodRestriction.CurrentFoodRestriction = groupFoodRestriction;
			}

			if (groupOutfit != null)
			{
				pawn.outfits.CurrentOutfit = groupOutfit;
			}

			if (groupDrugPolicy != null)
			{
				pawn.drugs.CurrentPolicy = groupDrugPolicy;
			}

			if (pawn.playerSettings != null && groupArea != null)
			{
				pawn.playerSettings.AreaRestriction = groupArea;
			}

			if (pawn.workSettings != null && groupWorkPriorities != null)
            {
				foreach (var workPriority in groupWorkPriorities)
                {
					if (!pawn.WorkTypeIsDisabled(workPriority.Key))
					{
						pawn.workSettings.SetPriority(workPriority.Key, workPriority.Value);
					}
				}
            }
		}

		public Dictionary<WorkType, WorkState> ActiveWorkTypes => this.activeWorkTypes
			.Where(x => x.Value != WorkState.Inactive && x.Value != WorkState.Temporary && x.Key != WorkType.RescueFallen && x.Key != WorkType.TendWounded)
			.ToDictionary(y => y.Key, y => y.Value);
		public void RemoveWorkState(WorkType workType)
		{
			if (this.activeWorkTypes.ContainsKey(workType))
			{
				this.activeWorkTypes[workType] = WorkState.Inactive;
			}

			SetCurrentActiveState();
		}

		public void ChangeWorkState(WorkType workType)
        {
			if (this.activeWorkTypes.ContainsKey(workType))
            {
				var state = this.activeWorkTypes[workType];
				if (state == WorkState.ForcedLabor)
                {
					this.activeWorkTypes[workType] = WorkState.Inactive;
				}
				else
                {
					this.activeWorkTypes[workType] = (WorkState)((int)state + 1);
				}
			}
			else
            {
				this.activeWorkTypes[workType] = WorkState.Active;
			}

			SetCurrentActiveState();
		}

		private void SetCurrentActiveState()
        {
			if (this.ActiveWorkTypes.Count > 0)
			{
				if (this.ActiveWorkTypes.Where(x => x.Value == WorkState.ForcedLabor).Count() == this.ActiveWorkTypes.Count())
				{
					activeWorkState = WorkState.ForcedLabor;
				}
				else if (this.ActiveWorkTypes.Where(x => x.Value == WorkState.Active).Count() == this.ActiveWorkTypes.Count())
				{
					activeWorkState = WorkState.Active;
				}
				else
				{
					activeWorkState = WorkState.Inactive;
				}
			}
			else
            {
				activeWorkState = WorkState.Inactive;
			}
		}

		public void AssignTemporaryWorkers(WorkType workType)
        {
			foreach (var pawn in pawns)
            {
				this.temporaryWorkers[pawn] = workType;
			}
		}

		public void SetGroupWorkPriorityFor(WorkTypeDef workType, int priority)
        {
			if (groupWorkPriorities is null) 
				groupWorkPriorities = new Dictionary<WorkTypeDef, int>();
			groupWorkPriorities[workType] = priority;
			foreach (var pawn in this.pawns)
			{
				foreach (var data in groupWorkPriorities)
                {
					if (!pawn.WorkTypeIsDisabled(data.Key))
					{
						pawn.workSettings.SetPriority(data.Key, data.Value);
					}
				}
			}
		}

		public void ActivatePreset(GroupPreset preset)
		{
			if (preset.groupDrugPolicy != null)
			{
				this.groupDrugPolicy = preset.groupDrugPolicy;
				this.groupDrugPolicyEnabled = true;
			}
			if (preset.groupFoodRestriction != null)
			{
				this.groupFoodRestriction = null;
				this.groupFoodRestrictionEnabled = true;
			}
			if (preset.groupOutfit != null)
			{
				this.groupOutfit = preset.groupOutfit;
				this.groupOutfitEnabled = true;
			}
			if (preset.groupArea != null)
			{
				this.groupArea = preset.groupArea;
				this.groupAreaEnabled = true;
			}
			if (preset.activeWorkTypes != null)
			{
				if (this.activeWorkTypes is null)
                {
					this.activeWorkTypes = new Dictionary<WorkType, WorkState>();
				}

				foreach (var activeWorkType in preset.activeWorkTypes)
				{
					this.activeWorkTypes[activeWorkType.Key] = activeWorkType.Value;
				}
				SetCurrentActiveState();
			}

			if (preset.groupWorkPriorities != null)
			{
				if (this.groupWorkPriorities is null)
				{
					this.groupWorkPriorities = new Dictionary<WorkTypeDef, int>();
				}
				foreach (var groupWorkPriority in preset.groupWorkPriorities)
				{
					this.groupWorkPriorities[groupWorkPriority.Key] = groupWorkPriority.Value;
				}
			}

			foreach (var pawn in this.pawns)
            {
				SyncPoliciesFor(pawn);
			}
		}
		//public void RemovePreset(GroupPreset preset)
        //{
		//	this.activeGroupPresets.Remove(preset);
		//	if (this.groupDrugPolicy == preset.groupDrugPolicy)
        //    {
		//		this.groupDrugPolicy = null;
		//		this.groupDrugPolicyEnabled = false;
        //    }
		//	if (this.groupFoodRestriction == preset.groupFoodRestriction)
		//	{
		//		this.groupFoodRestriction = null;
		//		this.groupFoodRestrictionEnabled = false;
		//	}
		//	if (this.groupOutfit == preset.groupOutfit)
		//	{
		//		this.groupOutfit = null;
		//		this.groupOutfitEnabled = false;
		//	}
		//	if (this.groupArea == preset.groupArea)
		//	{
		//		this.groupArea = null;
		//		this.groupAreaEnabled = false;
		//	}
		//	if (this.activeWorkTypes == preset.activeWorkTypes)
        //    {
		//		this.activeWorkTypes = new Dictionary<WorkType, WorkState>();
		//		this.activeWorkState = WorkState.Inactive;
        //    }
		//	if (this.groupWorkPriorities == preset.groupWorkPriorities)
        //    {
		//		this.groupWorkPriorities = new Dictionary<WorkTypeDef, int>();
		//	}
		//
		//	foreach (var pawn in this.pawns)
		//	{
		//		SyncPoliciesFor(pawn);
		//	}
		//}

		public void ResetGroupPolicies()
        {
			this.activeWorkTypes.Clear();
			this.groupWorkPriorities.Clear();
			this.groupArea = null;
			this.groupAreaEnabled = false;
			this.groupDrugPolicy = null;
			this.groupDrugPolicyEnabled = false;
			this.groupFoodRestriction = null;
			this.groupFoodRestrictionEnabled = false;
			this.groupOutfit = null;
			this.groupOutfitEnabled = false;
		}
		public virtual void ExposeData()
        {
			Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
			Scribe_Collections.Look(ref pawnIcons, "pawnIcons", LookMode.Reference, LookMode.Deep, ref pawnKeys, ref pawnIconValues);
			Scribe_Collections.Look(ref temporaryWorkers, "temporaryWorkers", LookMode.Reference, LookMode.Value, ref pawnKeys3, ref workTypeValues2);
			Scribe_Collections.Look(ref activeWorkTypes, "activeWorkTypes", LookMode.Value, LookMode.Value, ref workTypesKeys, ref workStateValues);
			Scribe_Collections.Look(ref groupWorkPriorities, "groupWorkPriorities", LookMode.Def, LookMode.Value, ref workTypesDefKeys, ref intValues);
			Scribe_Collections.Look(ref formations, "formations", LookMode.Deep);

			Scribe_Values.Look(ref groupName, "groupName");
			Scribe_Values.Look(ref groupID, "groupID");
			Scribe_Values.Look(ref groupIconName, "groupIconName");
			Scribe_Values.Look(ref groupBannerName, "groupBannerName");
			Scribe_Values.Look(ref groupIconFolder, "groupIconFolder");
			Scribe_Values.Look(ref groupBannerFolder, "groupBannerFolder");
			Scribe_Values.Look(ref activeSortBy, "activeSortBy");
			Scribe_Values.Look(ref bannerModeEnabled, "bannerModeEnabled");
			Scribe_Values.Look(ref entireGroupIsVisible, "entireGroupIsVisible");
			Scribe_Values.Look(ref isColonyGroup, "isColonyGroup");
			Scribe_Values.Look(ref isTaskForce, "isTaskForce");
			Scribe_Values.Look(ref isPawnGroup, "isPawnGroup");
			Scribe_Values.Look(ref isSubGroup, "isSubGroup");
			Scribe_Values.Look(ref colorFolder, "colorFolder");
			Scribe_Values.Look(ref activeWorkState, "activeWorkState");
			Scribe_Values.Look(ref hideGroupIcon, "hideGroupIcon");
			Scribe_Values.Look(ref hidePawnDots, "hidePawnDots");
			Scribe_Values.Look(ref hideLifeOverlay, "hideLifeOverlay");
			Scribe_Values.Look(ref hideWeaponOverlay, "hideWeaponOverlay");

			Scribe_Values.Look(ref travelSuppliesEnabled, "travelSuppliesEnabled", true);
			Scribe_Values.Look(ref bedrollsEnabled, "bedrollsEnabled");


			Scribe_Values.Look(ref groupAreaEnabled, "groupAreaEnabled");
			Scribe_Values.Look(ref groupDrugPolicyEnabled, "groupDrugPolicyEnabled");
			Scribe_Values.Look(ref groupFoodRestrictionEnabled, "groupFoodRestrictionEnabled");
			Scribe_Values.Look(ref groupOutfitEnabled, "groupOutfitEnabled");
			try
			{
				Scribe_References.Look(ref groupArea, "groupArea");
			}
			catch
			{
				groupAreaEnabled = false;
			};
			try
			{
				Scribe_References.Look(ref groupDrugPolicy, "groupDrugPolicy");
			}
			catch
			{
				groupDrugPolicyEnabled = false;
			}

			try
			{
				Scribe_References.Look(ref groupFoodRestriction, "groupFoodRestriction");
			}
			catch
			{
				groupFoodRestrictionEnabled = false;
			}
			try
			{
				Scribe_References.Look(ref groupOutfit, "groupOutfit");
			}
			catch
			{
				groupOutfitEnabled = false;
			}

			//Scribe_Values.Look(ref subGroupsExpanded, "subGroupsExpanded");
			Scribe_Defs.Look(ref skillDefSort, "skillDefSort");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
				if (this.temporaryWorkers is null) this.temporaryWorkers = new Dictionary<Pawn, WorkType>();
				if (this.activeWorkTypes is null) this.activeWorkTypes = new Dictionary<WorkType, WorkState>();
				if (this.formations is null) this.formations = new List<Formation>(4);
				if (this.pawnIcons is null) this.pawnIcons = new Dictionary<Pawn, PawnIcon>();
				if (this.groupName != null)
				{
					this.curGroupName = this.groupName;
				}
				else
				{
					this.curGroupName = this.defaultGroupName + " " + this.groupID;
				}
				this.UpdateData();
				foreach (var pawn in pawns)
                {
					TacticUtils.RegisterGroupFor(pawn, this);
				}
			}
		}

        public override string ToString()
        {
			return this.curGroupName + " - " + this.pawns?.Count;
        }


        public List<Pawn> pawns;
		public Dictionary<Pawn, Rect> pawnRects = new Dictionary<Pawn, Rect>();
		public Dictionary<Pawn, PawnIcon> pawnIcons = new Dictionary<Pawn, PawnIcon>();
		public Dictionary<Pawn, WorkType> temporaryWorkers = new Dictionary<Pawn, WorkType>();

		public Dictionary<WorkType, WorkState> activeWorkTypes = new Dictionary<WorkType, WorkState>();
		public Dictionary<WorkTypeDef, int> groupWorkPriorities = new Dictionary<WorkTypeDef, int>();
		//public List<GroupPreset> activeGroupPresets = new List<GroupPreset>();
		public List<Formation> formations = new List<Formation>(4);
		public Formation activeFormation;


		public int groupID;
		public bool entireGroupIsVisible;
		public bool hideGroupIcon;
		public bool hidePawnDots;
		public bool hideLifeOverlay;
		public bool hideWeaponOverlay;
		public bool isColonyGroup;
		public bool isTaskForce;
		public bool isPawnGroup;
		public bool isSubGroup;
		//public bool subGroupsExpanded;
		public string groupName;
		public string defaultGroupName;
		public string defaultBannerFolder;

		public Texture2D groupBanner;
		public Texture2D groupIcon;
		public float GroupIconHeight => groupBanner.height * GroupIconScale;
		public float GroupIconWidth => groupBanner.width * GroupIconScale;
		public float GroupIconMargin => GroupIconWidth / 3f;

		public string groupBannerName;
		public string groupBannerFolder;
		public string groupIconFolder;
		public string groupIconName;
		public bool bannerModeEnabled;
		public string colorFolder;

		public Outfit groupOutfit;
		public bool groupOutfitEnabled;

		public Area groupArea;
		public bool groupAreaEnabled;

		public DrugPolicy groupDrugPolicy;
		public bool groupDrugPolicyEnabled;

		public FoodRestriction groupFoodRestriction;
		public bool groupFoodRestrictionEnabled;

		protected WorkState activeWorkState;

		public bool bedrollsEnabled;
		public bool travelSuppliesEnabled = true;

		private List<Pawn> pawnKeys;
		private List<PawnIcon> pawnIconValues;

		private List<WorkType> workTypesKeys;
		private List<WorkState> workStateValues;

		private List<Pawn> pawnKeys3;
		private List<WorkType> workTypeValues2;

		private List<WorkTypeDef> workTypesDefKeys;
		private List<int> intValues;

		private List<PrisonerInteractionModeDef> prisonerInteractionDefKeys;
		private List<bool> boolValues;
	}
}
