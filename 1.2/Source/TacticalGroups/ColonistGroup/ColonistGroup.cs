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
    public class ColonistGroup : IExposable
    {
		public bool Visible => pawnWindowIsActive;
		public Map Map;
		private bool pawnWindowIsActive;
		public bool groupButtonRightClicked;
		public Rect curRect;
		private bool expandPawnIcons;
		public bool showPawnIconsRightClickMenu;
		public float IconScale => ShowExpanded ? 1f : 0.5f;
		public bool updateIcon = true;
		protected int pawnRowCount;
		protected int pawnDocRowCount;
		protected float pawnRowXPosShift;
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
		public List<Pawn> VisiblePawns
        {
			get
            {
				if (TacticalGroupsSettings.HidePawnsWhenOffMap)
                {
					return this.pawns.Where(x => x.Map == this.Map).ToList();
                }
				return this.pawns;
            }
        }

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
			this.formations = new Dictionary<Pawn, IntVec3>();
			this.activeWorkTypes = new Dictionary<WorkType, WorkState>();
			this.entireGroupIsVisible = true;
		}
		public virtual void Add(Pawn pawn)
        {
			if (pawn.Faction != Faction.OfPlayer || !pawn.RaceProps.Humanlike)
            {
				return;
            }
			if (this.Map == null)
            {
				this.Map = pawn.Map;
            }
			if (!this.pawns.Contains(pawn))
            {
				this.pawns.Add(pawn);
				this.pawnIcons[pawn] = new PawnIcon(pawn, this.entireGroupIsVisible ? true : false);
				//SyncPoliciesWithRestOfMembers(pawn);
				Sort();
				this.UpdateData();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
			if (TacticUtils.pawnsWithGroups.ContainsKey(pawn))
			{
				TacticUtils.pawnsWithGroups[pawn].Add(this);
			}
			else
			{
				TacticUtils.pawnsWithGroups[pawn] = new HashSet<ColonistGroup> { this };
			}
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

		public void HandleClicks(Rect rect)
		{
			if (Event.current.type == EventType.MouseDown)
			{
				foreach (var group in TacticUtils.AllGroups)
				{
					if (group != this)
					{
						if (group.groupButtonRightClicked && !this.groupButtonRightClicked)
						{
							//Log.Message("1 group.expandPawnIcons: " + group.expandPawnIcons);
							//Log.Message("1 this.expandPawnIcons: " + this.expandPawnIcons);
							//Log.Message("1 group.groupButtonRightClicked: " + group.groupButtonRightClicked);
							//Log.Message("1 this.groupButtonRightClicked: " + this.groupButtonRightClicked);
							//Log.Message("1 group.pawnWindowIsActive: " + group.pawnWindowIsActive);
							//Log.Message("1 this.pawnWindowIsActive: " + this.pawnWindowIsActive);
							group.groupButtonRightClicked = false;
						}
						else if (group.expandPawnIcons && !this.pawnWindowIsActive)
						{
							//Log.Message("2 group.expandPawnIcons: " + group.expandPawnIcons);
							//Log.Message("2 this.expandPawnIcons: " + this.expandPawnIcons);
							//Log.Message("2 group.groupButtonRightClicked: " + group.groupButtonRightClicked);
							//Log.Message("2 this.groupButtonRightClicked: " + this.groupButtonRightClicked);
							//Log.Message("2 group.pawnWindowIsActive: " + group.pawnWindowIsActive);
							//Log.Message("2 this.pawnWindowIsActive: " + this.pawnWindowIsActive);
							group.expandPawnIcons = false;
						}
						else if (group.expandPawnIcons && group.pawnWindowIsActive && this.pawnWindowIsActive)
                        {
							group.pawnWindowIsActive = false;
							group.expandPawnIcons = false;
						}
						else if (group.pawnWindowIsActive)
						{
							//Log.Message("3 group.expandPawnIcons: " + group.expandPawnIcons);
							//Log.Message("3 this.expandPawnIcons: " + this.expandPawnIcons);
							//Log.Message("3 group.groupButtonRightClicked: " + group.groupButtonRightClicked);
							//Log.Message("3 this.groupButtonRightClicked: " + this.groupButtonRightClicked);
							//Log.Message("3 group.pawnWindowIsActive: " + group.pawnWindowIsActive);
							//Log.Message("3 this.pawnWindowIsActive: " + this.pawnWindowIsActive);
							return;
						}
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
							CameraJumper.TryJump(this.pawns.First());
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
				else if (Event.current.button == 1)
				{
					this.showPawnIconsRightClickMenu = true;
					this.expandPawnIcons = false;
					this.groupButtonRightClicked = true;
					if (this.isColonyGroup && this is ColonyGroup colonyGroup)
                    {
						colonyGroup.subGroupsExpanded = true;
					}
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

        }

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
			this.updateIcon = false;
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}

		public virtual void Draw(Rect rect)
        {
			GUI.color = Color.white;
			Text.Font = GameFont.Tiny;
			var groupRect = new Rect(rect.x, rect.y, this.groupBanner.width * TacticalGroupsSettings.GroupScale, this.groupBanner.height * TacticalGroupsSettings.GroupScale);
			if (this.isSubGroup)
            {
				groupRect.width /= 2;
				groupRect.height /= 2;
			}
			this.curRect = groupRect;
			if (this.updateIcon)
            {
				UpdateIcon();
			}
			if (!hideGroupIcon)
			{
				GUI.DrawTexture(groupRect, this.groupBanner);
				GUI.DrawTexture(groupRect, this.groupIcon);
			}
			else if (Mouse.IsOver(groupRect))
            {
				GUI.DrawTexture(groupRect, this.groupBanner);
				GUI.DrawTexture(groupRect, this.groupIcon);
			}

			if (!hideGroupIcon)
            {
				if (!groupButtonRightClicked && Mouse.IsOver(groupRect))
				{
					GUI.DrawTexture(groupRect, Textures.GroupIconHover);
				}
				else if (groupButtonRightClicked)
				{
					if (this.bannerModeEnabled)
					{
						GUI.DrawTexture(groupRect, Textures.BannerIconSelected);
					}
					else if (this.isPawnGroup)
					{
						GUI.DrawTexture(groupRect, Textures.GroupIconSelected);
					}
					else if (this.isColonyGroup)
					{
						GUI.DrawTexture(groupRect, Textures.ColonyIconSelected);
					}
				}
			}


			if (!this.isSubGroup && !this.bannerModeEnabled && !this.hideGroupIcon)
			{
				var label = this.GetGroupName();
				var labelHeight = Text.CalcHeight(label, groupRect.width);
				var groupLabelRect = new Rect(groupRect.x, groupRect.y + groupRect.height, groupRect.width, labelHeight);
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(groupLabelRect, label);
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}

		public virtual void DrawOverlays(Rect rect)
        {
			var totalRect = Rect.zero;
			var pawnRows = GetPawnRows(this.pawnRowCount);
			var groupRect = new Rect(rect.x, rect.y, this.groupBanner.width * TacticalGroupsSettings.GroupScale, this.groupBanner.height * TacticalGroupsSettings.GroupScale);
			if (this.isSubGroup)
			{
				groupRect.width /= 2;
				groupRect.height /= 2;
			}
			if (ShowExpanded)
			{
				totalRect = new Rect(groupRect.x, groupRect.y, groupRect.width, groupRect.height);
				totalRect.height += pawnRows.Count * 75f;
				totalRect.x = (groupRect.x + (groupRect.width / 2f));
				totalRect.x -= ((this.pawnRowCount * 75f) / 2f);
				totalRect.width = 75f * pawnRowCount;
			}
			else
			{
				if (this.bannerModeEnabled)
				{
					totalRect = new Rect(rect.x - (rect.width / 1.7f), rect.y, 80f, groupRect.height);
				}
				else
				{
					totalRect = new Rect(rect.x, rect.y, rect.width, groupRect.height);
				}

				totalRect = totalRect.ScaledBy(1.2f);
				totalRect.height += pawnRows.Count * 30;
			}

			if (Mouse.IsOver(groupRect))
			{
				if (!this.isSubGroup)
                {
					pawnWindowIsActive = true;
					if (!subGroupsExpanded)
                    {
						subGroupsExpanded = true;
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
                    }
					DrawPawnRows(groupRect, pawnRows);
					DrawPawnArrows(groupRect, pawnRows);
					if (!ShowExpanded)
					{
						TooltipHandler.TipRegion(groupRect, new TipSignal("TG.GroupInfoTooltip".Translate(groupName)));
					}
				}
				HandleClicks(groupRect);
			}
			else if (!this.isSubGroup && (Mouse.IsOver(totalRect) && pawnWindowIsActive || showPawnIconsRightClickMenu))
			{
				DrawPawnRows(groupRect, pawnRows);
				DrawPawnArrows(groupRect, pawnRows);
			}
			else if (!this.isSubGroup)
			{
				pawnWindowIsActive = false;
				expandPawnIcons = false;
				if (!this.hidePawnDots)
                {
					DrawPawnDots(groupRect);
                }
				else if (!this.hideLifeOverlay)
                {
					DrawLifeOverlayWithDisabledDots(rect);
				}
			}
		}

		private int downedStateBlink;

		private Dictionary<Pawn, bool> pawnsDownedOrIncapable = new Dictionary<Pawn, bool>();
		private bool IsDownedOrIncapable(Pawn pawn)
        {
			if (pawnsDownedOrIncapable.TryGetValue(pawn, out bool value))
            {
				return value;
            }
			else
            {
				var value2 = pawn.IsDownedOrIncapable();
				pawnsDownedOrIncapable[pawn] = value2;
				return value2;
			}
		}

		private Dictionary<Pawn, bool> pawnsShotOrBleeding = new Dictionary<Pawn, bool>();
		private bool IsShotOrBleeding(Pawn pawn)
		{
			if (pawnsShotOrBleeding.TryGetValue(pawn, out bool value))
			{
				return value;
			}
			else
			{
				var value2 = pawn.IsShotOrBleeding();
				pawnsShotOrBleeding[pawn] = value2;
				return value2;
			}
		}

		private Dictionary<Pawn, bool> pawnsSick = new Dictionary<Pawn, bool>();
		private bool IsSick(Pawn pawn)
		{
			if (pawnsShotOrBleeding.TryGetValue(pawn, out bool value))
			{
				return value;
			}
			else
			{
				var value2 = pawn.IsSick();
				pawnsSick[pawn] = value2;
				return value2;
			}
		}

		public void UpdateData()
        {
			cachedPawnRows[this.pawnRowCount] = GetPawnRowsInt(this.pawnRowCount);
			var pawnDocCount = this.bannerModeEnabled ? 4 : this.pawnDocRowCount;
			cachedPawnRows[pawnDocCount] = GetPawnRowsInt(pawnDocCount);
			foreach (var pawn in this.pawns) 
			{
				pawnsDownedOrIncapable[pawn] = pawn.IsDownedOrIncapable();
				pawnsShotOrBleeding[pawn] = pawn.IsShotOrBleeding();
				pawnsSick[pawn] = pawn.IsSick();
			}
		}
		public void DrawPawnDots(Rect rect)
        {
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

			bool showDownedState = false;
			for (var i = 0; i < pawnRows.Count; i++)
			{
				for (var j = 0; j < pawnRows[i].Count; j++)
				{
					Rect dotRect = new Rect(initialRect.x + ((j + 1) * Textures.ColonistDot.width), initialRect.y + ((i + 1) * Textures.ColonistDot.height),
						Textures.ColonistDot.width, Textures.ColonistDot.height);
					if (pawnRows[i][j].MentalState != null)
                    {
						GUI.DrawTexture(dotRect, Textures.ColonistDotMentalState);
					}
					else if (IsDownedOrIncapable(pawnRows[i][j]))
					{
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
					}
					else if (IsShotOrBleeding(pawnRows[i][j]))
					{
						GUI.DrawTexture(dotRect, Textures.ColonistDotDowned);
					}
					else if (IsSick(pawnRows[i][j]))
                    {
						GUI.DrawTexture(dotRect, Textures.ColonistDotToxic);
					}
					else if (pawnRows[i][j].Inspired)
                    {
						GUI.DrawTexture(dotRect, Textures.ColonistDotInspired);
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

		private void DrawLifeOverlayWithDisabledDots(Rect rect)
        {
			bool showDownedState = false;
			foreach (var pawn in this.pawns)
            {
				if (IsDownedOrIncapable(pawn))
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
				this.UpdateData();
            }
        }
		//public void SyncPoliciesWithRestOfMembers(Pawn pawn)
		//{
		//	var otherMembers = this.pawns.Where(x => x != pawn);
		//	if (otherMembers.Any())
		//    {
		//		var curFoodPolicy = otherMembers.FirstOrDefault().foodRestriction.CurrentFoodRestriction;
		//		if (otherMembers.Where(x => x.foodRestriction.CurrentFoodRestriction == curFoodPolicy).Count() == otherMembers.Count())
		//        {
		//			pawn.foodRestriction.CurrentFoodRestriction = curFoodPolicy;
		//		}
		//
		//		var curOutfitPolicy = otherMembers.FirstOrDefault().outfits.CurrentOutfit;
		//		if (otherMembers.Where(x => x.outfits.CurrentOutfit == curOutfitPolicy).Count() == otherMembers.Count())
		//		{
		//			pawn.outfits.CurrentOutfit = curOutfitPolicy;
		//		}
		//
		//		var curDrugsPolicy = otherMembers.FirstOrDefault().drugs.CurrentPolicy;
		//		if (otherMembers.Where(x => x.drugs.CurrentPolicy == curDrugsPolicy).Count() == otherMembers.Count())
		//		{
		//			pawn.drugs.CurrentPolicy = curDrugsPolicy;
		//		}
		//
		//		var curAreaPolicy = otherMembers.FirstOrDefault().playerSettings?.AreaRestriction;
		//		if (pawn.playerSettings != null && curAreaPolicy != null && otherMembers.Where(x => x.playerSettings?.AreaRestriction == curAreaPolicy).Count() == otherMembers.Count())
		//		{
		//			pawn.playerSettings.AreaRestriction = curAreaPolicy;
		//		}
		//
		//		var curHostilityPolicy = otherMembers.FirstOrDefault().playerSettings?.hostilityResponse;
		//		if (pawn.playerSettings != null && curHostilityPolicy.HasValue && otherMembers.Where(x => x.playerSettings?.hostilityResponse == curHostilityPolicy).Count() == otherMembers.Count())
		//		{
		//			pawn.playerSettings.hostilityResponse = curHostilityPolicy.Value;
		//		}
		//
		//		if (AssignTimeValue(otherMembers.ToList()))
		//        {
		//			for (int i = 0; i < 24; i++)
		//			{
		//				pawn.timetable.SetAssignment(i, otherMembers.First().timetable.GetAssignment(i));
		//			}
		//		}
		//	}
		//}

		//private bool AssignTimeValue(List<Pawn> otherMembers)
		//{
		//	for (int i = 0; i < 24; i++)
		//	{
		//		foreach (var p in otherMembers)
		//		{
		//			foreach (var p2 in otherMembers)
		//			{
		//				if (p != p2 && p.timetable.GetAssignment(i) != p2.timetable.GetAssignment(i))
		//                {
		//					return false;
		//                }
		//			}
		//		}
		//	}
		//	return true;
		//}

		public void RemoveWorkState(WorkType workType)
		{
			if (this.activeWorkTypes.ContainsKey(workType))
			{
				this.activeWorkTypes[workType] = WorkState.Inactive;
			}

			activeWorkState = this.activeWorkTypes.Where(x => x.Value == WorkState.ForcedLabor).Count() == this.activeWorkTypes.Count();
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

			activeWorkState = this.activeWorkTypes.Where(x => x.Value == WorkState.ForcedLabor).Count() == this.activeWorkTypes.Count();
		}
		public virtual void ExposeData()
        {
			Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
			Scribe_Collections.Look(ref pawnIcons, "pawnIcons", LookMode.Reference, LookMode.Deep, ref pawnKeys, ref pawnIconValues);
			Scribe_Collections.Look(ref formations, "formations", LookMode.Reference, LookMode.Value, ref pawnKeys2, ref intVecValues);
			Scribe_Collections.Look(ref activeWorkTypes, "activeWorkTypes", LookMode.Value, LookMode.Value, ref workTypesKeys, ref workStateValues);
			Scribe_References.Look(ref Map, "Map");
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
			Scribe_Values.Look(ref subGroupsExpanded, "subGroupsExpanded");
			Scribe_Defs.Look(ref skillDefSort, "skillDefSort");
		}

		public List<Pawn> pawns;
		public Dictionary<Pawn, Rect> pawnRects = new Dictionary<Pawn, Rect>();
		public Dictionary<Pawn, PawnIcon> pawnIcons = new Dictionary<Pawn, PawnIcon>();
		public Dictionary<Pawn, IntVec3> formations = new Dictionary<Pawn, IntVec3>();
		public Dictionary<WorkType, WorkState> activeWorkTypes = new Dictionary<WorkType, WorkState>();

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
		public bool subGroupsExpanded;
		public string groupName;
		public string defaultGroupName;
		public string defaultBannerFolder;
		public Texture2D groupBanner;
		public Texture2D groupIcon;
		public string groupBannerName;
		public string groupBannerFolder;
		public string groupIconFolder;
		public string groupIconName;
		public bool bannerModeEnabled;
		public string colorFolder;
		protected bool activeWorkState;


		private List<Pawn> pawnKeys;
		private List<PawnIcon> pawnIconValues;

		private List<Pawn> pawnKeys2;
		private List<IntVec3> intVecValues;

		private List<WorkType> workTypesKeys;
		private List<WorkState> workStateValues;
	}
}
