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


		public Texture2D Icon => formations != null && formations.Any() ?
			isSelected ? ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/" + colorPrefix + "select")
			: ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/" + colorPrefix + "dark")
			: isSelected ? ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/greyselect")
			: ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/greydark");

		private List<Pawn> pawnKeys2;
		private List<IntVec3> intVecValues;
	}

	public class ColorOption : IExposable
	{
		public Color color;
		public bool pawnFavoriteOnly;
		public ColorOption()
		{

		}

		public ColorOption(Color color)
		{
			this.color = color;
		}
		public ColorOption(bool pawnFavoriteOnly)
		{
			this.pawnFavoriteOnly = pawnFavoriteOnly;
		}

		public Color? GetColor(Pawn pawn)
		{
			return pawnFavoriteOnly ? pawn.story.favoriteColor : color;
		}
		public void ExposeData()
		{
			Scribe_Values.Look(ref color, "color");
			Scribe_Values.Look(ref pawnFavoriteOnly, "pawnFavoriteOnly");
		}
	}
	public class GroupColor : IExposable
	{
		public Dictionary<BodyColor, ColorOption> bodyColors = new Dictionary<BodyColor, ColorOption>();
		public void ExposeData()
		{
			Scribe_Collections.Look(ref bodyColors, "bodyColors", LookMode.Value, LookMode.Deep, ref bodyColorKeys, ref colorValues);
		}

		private List<BodyColor> bodyColorKeys;
		private List<ColorOption> colorValues;
	}
	public class ColonistGroup : IExposable, ILoadReferenceable
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
		public bool ShowExpanded => pawnWindowIsActive && expandPawnIcons;

		public virtual Map Map { get; }
		public virtual List<Pawn> ActivePawns { get; }

		public virtual List<Pawn> VisiblePawns { get; }

		public void ResetDrawOptions()
		{
			groupButtonRightClicked = false;
			expandPawnIcons = false;
			pawnWindowIsActive = false;
			showPawnIconsRightClickMenu = false;
		}
		public virtual void Init()
		{
			pawns = new List<Pawn>();
			pawnIcons = new Dictionary<Pawn, PawnIcon>();
			formations = new List<Formation>(4);
			temporaryWorkers = new Dictionary<Pawn, WorkType>();
			activeWorkTypes = new Dictionary<WorkType, WorkState>();
			entireGroupIsVisible = true;
		}

		public void SetName(string name)
		{
			groupName = name;
			curGroupName = groupName;
			cachedGroupNameHeight = Text.CalcHeight(curGroupName, groupBanner.width);
		}
		public virtual void Add(Pawn pawn)
		{
			if (pawn.Faction != Faction.OfPlayer || (!pawn.RaceProps.Humanlike
				&& pawn.IsPlayerMechanoid() is false && pawn.GetIntelligence() < Intelligence.Humanlike))
			{
				return;
			}

			if (pawns is null)
			{
				pawns = new List<Pawn>();
			}

			if (!pawns.Contains(pawn))
			{
				pawns.Add(pawn);
				pawnIcons[pawn] = new PawnIcon(pawn, entireGroupIsVisible);
				SyncPoliciesFor(pawn);
				Sort();
				UpdateData();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
			TacticUtils.RegisterGroupFor(pawn, this);
		}

		public void Add(List<Pawn> newPawns)
		{
			foreach (Pawn pawn in newPawns)
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
				pawns.Remove(pawn);
				pawnIcons.Remove(pawn);
				Sort();
				UpdateData();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
		}

		private readonly Dictionary<int, List<List<Pawn>>> cachedPawnRows = new Dictionary<int, List<List<Pawn>>>();
		public List<List<Pawn>> GetPawnRows(int columnCount)
		{
			if (cachedPawnRows.TryGetValue(columnCount, out List<List<Pawn>> value))
			{
				return value;
			}
			else
			{
				List<List<Pawn>> value2 = GetPawnRowsInt(columnCount);
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
			if (pawns is null)
			{
				pawns = new List<Pawn>();
			}
			if (pawnIcons is null)
			{
				pawnIcons = new Dictionary<Pawn, PawnIcon>();
			}

			for (int ind = pawns.Count - 1; ind >= 0; ind--)
			{
				Pawn pawn = pawns[ind];
				if (pawn.Destroyed || pawn.Dead)
				{
					pawns.Remove(pawn);
					pawnIcons.Remove(pawn);
					refresh = true;
				}
			}
			if (refresh)
			{
				Sort();
				UpdateData();
				TacticUtils.TacticalColonistBar.MarkColonistsDirty();
			}
			foreach (Pawn pawn in VisiblePawns)
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

		public float GroupIconScale => isColonyGroup
					? TacticalGroupsSettings.ColonyGroupScale
					: isSubGroup ? TacticalGroupsSettings.GroupScale / 2f : TacticalGroupsSettings.GroupScale;

		public List<PawnDot> GetPawnDotsInt(Rect rect)
		{
			List<PawnDot> pawnDots = new List<PawnDot>();
			List<List<Pawn>> pawnRows = GetPawnRows(bannerModeEnabled ? 4 : pawnDocRowCount);
			Rect initialRect = new Rect(rect);
			if (bannerModeEnabled)
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

			for (int i = 0; i < pawnRows.Count; i++)
			{
				for (int j = 0; j < pawnRows[i].Count; j++)
				{
					Rect dotRect = new Rect(initialRect.x + ((j + 1) * (Textures.ColonistDot.width * GroupIconScale)), initialRect.y + ((i + 1)
						* (Textures.ColonistDot.height * GroupIconScale)),
						Textures.ColonistDot.width, Textures.ColonistDot.height);
					Pawn pawn = pawnRows[i][j];
					PawnState state = PawnStateUtility.GetPawnState(pawn);
					pawnDots.Add(new PawnDot(pawn, dotRect, state));
				}
			}
			return pawnDots;
		}
		public void HandleClicks(Rect rect, Rect totalRect)
		{
			if (Event.current.type == EventType.MouseDown)
			{
				foreach (ColonistGroup group in TacticUtils.AllGroups)
				{
					if (group != this && group.pawnWindowIsActive && Mouse.IsOver(rect))
					{
						return;
					}
				}

				foreach (ColonistGroup group in TacticUtils.AllGroups)
				{
					if (group != this)
					{
						group.expandPawnIcons = false;
						group.pawnWindowIsActive = false;
						group.groupButtonRightClicked = false;
						group.showPawnIconsRightClickMenu = false;
						MainFloatMenu window = Find.WindowStack.WindowOfType<MainFloatMenu>();
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
						if ((WorldRendererUtility.WorldRenderedNow && Map != null) || (Map != null && Map != Find.CurrentMap))
						{
							if (ActivePawns.Any())
							{
								CameraJumper.TryJump(ActivePawns.First());
							}
							if (this is CaravanGroup caravanGroup)
							{
								Caravan caravan = TacticUtils.TacticalGroups.caravanGroups.Where(x => x.Value == caravanGroup).FirstOrDefault().Key;
								Find.Selector.Select(caravan);
							}
							Event.current.Use();
							return;
						}
						else if (Map == null)
						{
							CameraJumper.TryJump(pawns.First());
							Event.current.Use();
							return;
						}
						expandPawnIcons = !expandPawnIcons;
					}

					else if (Event.current.clickCount == 2)
					{
						Find.Selector.ClearSelection();
						foreach (Pawn pawn in pawns)
						{
							if (!pawn.IsWorldPawn() && !pawn.InContainerEnclosed)
							{
								Find.Selector.Select(pawn);
							}
						}
					}
				}
				else if (Event.current.button == 1 && !(this is CaravanGroup))
				{
					showPawnIconsRightClickMenu = true;
					expandPawnIcons = false;
					groupButtonRightClicked = true;
					Rect rect2 = new Rect(rect.x, rect.y + rect.height, rect.width, rect.height);
					TieredFloatMenu floatMenu = new MainFloatMenu(null, this, rect2, Textures.DropMenuRightClick);
					Find.WindowStack.Add(floatMenu);
				}
				else if (Event.current.button == 2)
				{
					if (entireGroupIsVisible || !pawnIcons.Where(x => !x.Value.isVisibleOnColonistBar).Any())
					{
						TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
						foreach (KeyValuePair<Pawn, PawnIcon> pawnIcon in pawnIcons)
						{
							pawnIcon.Value.isVisibleOnColonistBar = false;
						}
						entireGroupIsVisible = false;
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					else
					{
						TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
						foreach (KeyValuePair<Pawn, PawnIcon> pawnIcon in pawnIcons)
						{
							pawnIcon.Value.isVisibleOnColonistBar = true;
						}
						entireGroupIsVisible = true;
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
				}
				Event.current.Use();
			}
		}

		public void Notify_WindowsClosed()
		{
			ResetDrawOptions();
		}

		private Texture2D mergedTexture;
		private Texture2D darkenMergedTexture;
		public void UpdateIcon()
		{
			string bannerPath = bannerModeEnabled
				? "UI/ColonistBar/GroupIcons/BannerMode/" + groupBannerFolder
				: "UI/ColonistBar/GroupIcons/" + groupBannerFolder;
			IEnumerable<Texture2D> banners = ContentFinder<Texture2D>.GetAllInFolder(bannerPath);
			Texture2D banner = banners.Where(x => x.name == groupBannerName).FirstOrDefault();
			if (banner != null)
			{
				groupBanner = banner;
			}

			string iconPath = bannerModeEnabled ? "UI/ColonistBar/GroupIcons/BannerMode/" + groupIconFolder : "UI/ColonistBar/GroupIcons/" + groupIconFolder;
			List<Texture2D> icons = ContentFinder<Texture2D>.GetAllInFolder(iconPath).OrderBy(x => x.name).ToList();
			Texture2D icon = icons.Where(x => x.name == groupIconName).FirstOrDefault();

			if (icon != null)
			{
				groupIcon = icon;
			}
			if (this is CaravanGroup)
			{
				mergedTexture = groupIcon;
				darkenMergedTexture = TexturesUtils.GetDarkenTexture(TexturesUtils.GetReadableTexture(groupIcon));
			}
			else
			{
				mergedTexture = TexturesUtils.GetMergedTexture(groupBanner, groupIcon);
				darkenMergedTexture = TexturesUtils.GetMergedDarkenTexture(groupBanner, groupIcon);
			}

			updateIcon = false;
			cachedGroupNameHeight = Text.CalcHeight(curGroupName, groupBanner.width);
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}

		public virtual void Draw(Rect rect)
		{
			GUI.color = Color.white;
			Text.Font = GameFont.Tiny;
			curRect = rect;
			if (updateIcon)
			{
				UpdateIcon();
			}
			if (!hideGroupIcon)
			{
				if (!isColonyGroup || Find.CurrentMap == Map)
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
					if (bannerModeEnabled)
					{
						GUI.DrawTexture(rect, Textures.BannerIconSelected);
					}
					else if (isPawnGroup)
					{
						GUI.DrawTexture(rect, Textures.GroupIconSelected);
					}
					else if (isColonyGroup || isTaskForce)
					{
						GUI.DrawTexture(rect, Textures.ColonyIconSelected);
					}
				}
			}


			if (!isSubGroup)
			{
				if (!bannerModeEnabled && !hideGroupIcon)
				{
					Rect groupLabelRect = new Rect(rect.x, rect.y + rect.height, rect.width, cachedGroupNameHeight);
					Text.Anchor = TextAnchor.UpperCenter;
					Widgets.Label(groupLabelRect, curGroupName);
					Text.Anchor = TextAnchor.UpperLeft;
				}

				if (!hidePawnDots && !hideGroupIcon)
				{
					DrawPawnDots(rect);
				}
			}
		}

		public virtual void DrawOverlays(Rect rect)
		{
			_ = Rect.zero;
			List<List<Pawn>> pawnRows = GetPawnRows(pawnRowCount);
			Rect totalRect;
			if (ShowExpanded)
			{
				totalRect = rect;
				totalRect.height += pawnRows.Count * 75f;
				totalRect.x = rect.x + (rect.width / 2f);
				totalRect.x -= pawnRowCount * 75f / 2f;
				totalRect.width = 75f * pawnRowCount;
			}
			else
			{
				totalRect = bannerModeEnabled
					? new Rect(rect.x - (rect.width / 1.7f), rect.y, 80f, rect.height)
					: new Rect(rect.x, rect.y, rect.width, rect.height);

				totalRect = totalRect.ScaledBy(1.2f);
				totalRect.height += pawnRows.Count * 30;
			}
			totalRect.yMin = rect.yMax;
			if (Mouse.IsOver(rect))
			{
				if (!isSubGroup)
				{
					bool showThisPawnWindow = true;
					foreach (ColonistGroup group in TacticUtils.AllGroups)
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
					Rect subGroupRect = new Rect(rect);
					subGroupRect.x -= rect.width;
					DrawPawnRows(subGroupRect, pawnRows);
					DrawPawnArrows(subGroupRect, pawnRows);
				}
				if (!ShowExpanded)
				{
					TooltipHandler.TipRegion(rect, new TipSignal("TG.GroupInfoTooltip".Translate(curGroupName)));
				}
				HandleClicks(rect, totalRect);
			}
			else if (!isSubGroup && ((Mouse.IsOver(totalRect) && pawnWindowIsActive) || showPawnIconsRightClickMenu))
			{
				DrawPawnRows(rect, pawnRows);
				DrawPawnArrows(rect, pawnRows);
			}
			else if (isSubGroup && showPawnIconsRightClickMenu)
			{
				Rect subGroupRect = new Rect(rect);
				subGroupRect.x -= rect.width;
				DrawPawnRows(subGroupRect, pawnRows);
				DrawPawnArrows(subGroupRect, pawnRows);
			}
			else if (!isSubGroup)
			{
				pawnWindowIsActive = false;
				expandPawnIcons = false;
			}
			else if (isSubGroup && !hideLifeOverlay)
			{
				DrawLifeOverlayWithDisabledDots(rect);
			}
		}

		private int downedStateBlink;
		public virtual void UpdateData()
		{
			cachedPawnRows[pawnRowCount] = GetPawnRowsInt(pawnRowCount);
			int pawnDocCount = bannerModeEnabled ? 4 : pawnDocRowCount;
			cachedPawnRows[pawnDocCount] = GetPawnRowsInt(pawnDocCount);
			cachedPawnDots = null;
		}
		public void DrawPawnDots(Rect rect)
		{
			List<PawnDot> pawnDots = GetPawnDots(rect);
			bool showDownedState = false;
			for (int i = 0; i < pawnDots.Count; i++)
			{
				PawnDot pawnDot = pawnDots[i];
				Pawn pawn = pawnDot.pawn;
				Rect dotRect = pawnDot.rect;
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
					foreach (ColonistGroup group in TacticUtils.AllGroups)
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

			if (!hideLifeOverlay && showDownedState)
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

		private readonly Dictionary<Pawn, PawnDownedStateCache> pawnDownedStates = new Dictionary<Pawn, PawnDownedStateCache>();
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
				pawnDownedStates[pawn] = new PawnDownedStateCache
				{
					downed = pawn.IsDownedOrIncapable()
				};
				return pawnDownedStates[pawn].downed;
			}
		}

		private void DrawLifeOverlayWithDisabledDots(Rect rect)
		{
			if (!hideLifeOverlay)
			{
				bool showDownedState = false;
				foreach (Pawn pawn in pawns)
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
				Rect initialRect = new Rect(rect.x, rect.y + rect.height + (rect.height / 5f), rect.width, rect.height)
				{
					x = rect.x + (rect.width / 2f)
				};
				initialRect.x -= pawnRowCount * 65f / 2f;
				initialRect.x += 8f;
				for (int i = 0; i < pawnRows.Count; i++)
				{
					for (int j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(initialRect.x + (j * 65), initialRect.y + (i * 70), 50, 50);
						DrawColonist(smallRect, pawnRows[i][j], pawnRows[i][j].Map, false);
						pawnRects[pawnRows[i][j]] = smallRect;
					}
				}
			}
			else
			{
				_ = Rect.zero;
				float backGroundWidth = pawnRows.Any() ? (pawnRows[0].Count * 25f) + 3f : rect.width;
				Rect backGroundRect = bannerModeEnabled
					? new Rect(rect.x - (rect.width / 1.7f), rect.y + rect.height, backGroundWidth, pawnRows.Count * 30f)
					: new Rect(rect.x, rect.y + rect.height, backGroundWidth, pawnRows.Count * 30f);

				GUI.DrawTexture(backGroundRect, Textures.BackgroundColonistLayer);
				for (int i = 0; i < pawnRows.Count; i++)
				{
					for (int j = 0; j < pawnRows[i].Count; j++)
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
				Rect initialRect = new Rect(rect.x, rect.y + rect.height + (rect.height / 5f), rect.width, rect.height)
				{
					x = rect.x + (rect.width / 2f)
				};
				initialRect.x -= pawnRowCount * 65f / 2f;
				initialRect.x += 8f;
				for (int i = 0; i < pawnRows.Count; i++)
				{
					for (int j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(initialRect.x + (j * 65), initialRect.y + (i * 70), 50, 50);
						DrawPawnArrows(smallRect, pawnRows[i][j]);
					}
				}
			}
			else
			{
				_ = Rect.zero;
				Rect backGroundRect = bannerModeEnabled
					? new Rect(rect.x - (rect.width / 1.7f), rect.y + rect.height, 80f, pawnRows.Count * 30f)
					: new Rect(rect.x, rect.y + rect.height, rect.width, pawnRows.Count * 30f);
				for (int i = 0; i < pawnRows.Count; i++)
				{
					for (int j = 0; j < pawnRows[i].Count; j++)
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
				Rect rightPawnArrowRect = new Rect(rect.x + rect.width, rect.y, Textures.PawnArrowRight.width, Textures.PawnArrowRight.height);

				if (Mouse.IsOver(rightPawnArrowRect.ExpandedBy(3f)))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(rightPawnArrowRect))
					{
						int indexOf = pawns.IndexOf(pawn);
						if (pawns.Count > indexOf + 1)
						{
							pawns.RemoveAt(indexOf);
							pawns.Insert(indexOf + 1, pawn);
						}
						else if (indexOf != 0)
						{
							pawns.RemoveAt(indexOf);
							pawns.Insert(0, pawn);
						}
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					reset = false;
				}
				GUI.DrawTexture(rightPawnArrowRect, Textures.PawnArrowRight);

				Rect leftPawnArrowRect = new Rect(rect.x - Textures.PawnArrowLeft.width, rect.y, Textures.PawnArrowLeft.width, Textures.PawnArrowLeft.height);
				if (Mouse.IsOver(leftPawnArrowRect.ExpandedBy(3f)))
				{
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(leftPawnArrowRect))
					{
						int indexOf = pawns.IndexOf(pawn);
						if (indexOf > 0)
						{
							pawns.RemoveAt(indexOf);
							pawns.Insert(indexOf - 1, pawn);
						}
						else if (indexOf != pawns.Count)
						{
							pawns.RemoveAt(indexOf);
							pawns.Insert(pawns.Count, pawn);
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
			Rect pawnTextureRect = GetPawnTextureRect(rect.position);
			GUI.DrawTexture(pawnTextureRect, PortraitsCache.Get(colonist, ManagementMenu.PawnTextureSize, Rot4.South,
				ManagementMenu.PawnTextureCameraOffset, ManagementMenu.PawnTextureCameraZoom));
			if (colonist.Drafted)
			{
				GUI.DrawTexture(rect, Textures.PawnDrafted);
			}
			if (!(ModCompatibility.combatExtendedHasAmmo_Method is null))
			{
				ThingWithComps gun = colonist.equipment?.Primary ?? null;
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

			HashSet<PawnState> pawnStates = PawnStateUtility.GetAllPawnStatesCache(colonist);
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
				ColonistBarColonistDrawer.DrawHealthBar(rect, colonist, TacticalGroupsSettings.HealthBarWidth);
				ColonistBarColonistDrawer.DrawRestAndFoodBars(rect, colonist, TacticalGroupsSettings.PawnNeedsWidth);
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
			return t.def.hasTooltip || t is Hive || t is IAttackTarget;
		}

		public Rect GetPawnTextureRect(Vector2 pos)
		{
			float x = pos.x;
			float y = pos.y;
			Vector2 vector = new Vector2(46f, 75f);
			//Vector2 vector = ColonistBarColonistDrawer.PawnTextureSize * TacticUtils.TacticalColonistBar.Scale;
			Rect rect = new Rect(x + 1f, y - ((vector.y - 48f) * IconScale) - 1f, vector.x * IconScale, vector.y * IconScale).ContractedBy(1f);
			return rect;
		}

		private static readonly Vector2[] bracketLocs = new Vector2[4];

		public void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
		{
			Thing obj = colonist;
			if (colonist.Dead)
			{
				obj = colonist.Corpse;
			}
			float num = 0.4f * TacticUtils.TacticalColonistBar.Scale;
			SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<object>(textureSize: new Vector2(SelectionDrawerUtility.SelectedTexGUI.width * num,
				SelectionDrawerUtility.SelectedTexGUI.height * num), bracketLocs: bracketLocs, obj: obj, rect: rect, selectTimes: SelectionDrawer.SelectTimes,
				jumpDistanceFactor: 20f * TacticUtils.TacticalColonistBar.Scale);
			DrawSelectionOverlayOnGUI(bracketLocs, num);
		}

		public void DrawCaravanSelectionOverlayOnGUI(Caravan caravan, Rect rect)
		{
			float num = 0.4f * TacticUtils.TacticalColonistBar.Scale;
			SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<WorldObject>(textureSize: new Vector2(SelectionDrawerUtility.SelectedTexGUI.width * num,
				SelectionDrawerUtility.SelectedTexGUI.height * num), bracketLocs: bracketLocs, obj: caravan, rect: rect, selectTimes: WorldSelectionDrawer.SelectTimes,
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
				pawns.SortByDescending(x => x.skills.GetSkill(skillDefSort).Level);
			}
			else if (activeSortBy == SortBy.Name)
			{
				pawns.SortBy(x => x.Name.ToStringShort);
			}
			UpdateData();
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
				foreach (KeyValuePair<WorkTypeDef, int> workPriority in groupWorkPriorities)
				{
					if (!pawn.WorkTypeIsDisabled(workPriority.Key))
					{
						pawn.workSettings.SetPriority(workPriority.Key, workPriority.Value);
					}
				}
			}
		}

		public Dictionary<WorkType, WorkState> ActiveWorkTypes => activeWorkTypes
			.Where(x => x.Value != WorkState.Inactive && x.Value != WorkState.Temporary)
			.ToDictionary(y => y.Key, y => y.Value);

		public void RemoveWorkState(WorkTypeEnum workTypeEnum)
		{
			List<KeyValuePair<WorkType, WorkState>> workTypes = activeWorkTypes.Where(x => x.Key.workTypeEnum == workTypeEnum).ToList();
			foreach (KeyValuePair<WorkType, WorkState> data in workTypes)
			{
				activeWorkTypes[data.Key] = WorkState.Inactive;

			}
			SetCurrentActiveState();
		}

		public void ChangeWorkState(WorkTypeEnum workTypeEnum)
		{
			List<KeyValuePair<WorkType, WorkState>> workTypes = activeWorkTypes.Where(x => x.Key.workTypeEnum == workTypeEnum).ToList();
			if (workTypes.Any())
			{
				foreach (KeyValuePair<WorkType, WorkState> data in workTypes)
				{
					WorkState state = data.Value;
					activeWorkTypes[data.Key] = state == WorkState.ForcedLabor ? WorkState.Inactive : (WorkState)((int)state + 1);
				}
			}
			else
			{
				activeWorkTypes[new WorkType(workTypeEnum)] = WorkState.Active;
			}
			SetCurrentActiveState();
		}
		public void RemoveWorkState(WorkTypeDef workTypeDef)
		{
			List<KeyValuePair<WorkType, WorkState>> workTypes = activeWorkTypes.Where(x => x.Key.workTypeDef == workTypeDef).ToList();
			foreach (KeyValuePair<WorkType, WorkState> data in workTypes)
			{
				activeWorkTypes[data.Key] = WorkState.Inactive;
			}
			SetCurrentActiveState();
		}

		public void ChangeWorkState(WorkTypeDef workTypeDef)
		{
			List<KeyValuePair<WorkType, WorkState>> workTypes = activeWorkTypes.Where(x => x.Key.workTypeDef == workTypeDef).ToList();
			if (workTypes.Any())
			{
				foreach (KeyValuePair<WorkType, WorkState> data in workTypes)
				{
					WorkState state = data.Value;
					activeWorkTypes[data.Key] = state == WorkState.ForcedLabor ? WorkState.Inactive : (WorkState)((int)state + 1);
				}
			}
			else
			{
				activeWorkTypes[new WorkType(workTypeDef)] = WorkState.Active;
			}
			SetCurrentActiveState();
		}

		private void SetCurrentActiveState()
		{
			activeWorkState = ActiveWorkTypes.Any()
				? ActiveWorkTypes.Any(x => x.Value == WorkState.ForcedLabor)
					? WorkState.ForcedLabor
					: ActiveWorkTypes.Any(x => x.Value == WorkState.Active) ? WorkState.Active : WorkState.Inactive
				: WorkState.Inactive;
		}

		public void AssignTemporaryWorkers(WorkTypeDef workType)
		{
			foreach (Pawn pawn in pawns)
			{
				temporaryWorkers[pawn] = new WorkType(workType);
			}
		}
		public void AssignTemporaryWorkers(WorkTypeEnum workTypeEnum)
		{
			foreach (Pawn pawn in pawns)
			{
				temporaryWorkers[pawn] = new WorkType(workTypeEnum);
			}
		}
		public void SetGroupWorkPriorityFor(WorkTypeDef workType, int priority)
		{
			if (groupWorkPriorities is null)
			{
				groupWorkPriorities = new Dictionary<WorkTypeDef, int>();
			}

			groupWorkPriorities[workType] = priority;
			foreach (Pawn pawn in pawns)
			{
				foreach (KeyValuePair<WorkTypeDef, int> data in groupWorkPriorities)
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
				groupDrugPolicy = preset.groupDrugPolicy;
				groupDrugPolicyEnabled = true;
			}
			if (preset.groupFoodRestriction != null)
			{
				groupFoodRestriction = null;
				groupFoodRestrictionEnabled = true;
			}
			if (preset.groupOutfit != null)
			{
				groupOutfit = preset.groupOutfit;
				groupOutfitEnabled = true;
			}
			if (preset.groupArea != null)
			{
				groupArea = preset.groupArea;
				groupAreaEnabled = true;
			}
			if (preset.activeWorkTypes != null)
			{
				if (this.activeWorkTypes is null)
				{
					this.activeWorkTypes = new Dictionary<WorkType, WorkState>();
				}
				List<KeyValuePair<WorkType, WorkState>> activeWorkTypes = preset.activeWorkTypes.ToList();
				for (int num = activeWorkTypes.Count - 1; num >= 0; num--)
				{
					this.activeWorkTypes[activeWorkTypes[num].Key] = activeWorkTypes[num].Value;
				}
				SetCurrentActiveState();
			}

			if (preset.groupWorkPriorities != null)
			{
				if (groupWorkPriorities is null)
				{
					groupWorkPriorities = new Dictionary<WorkTypeDef, int>();
				}
				List<KeyValuePair<WorkTypeDef, int>> groupPriorities = preset.groupWorkPriorities.ToList();
				for (int num = groupPriorities.Count - 1; num >= 0; num--)
				{
					groupWorkPriorities[groupPriorities[num].Key] = groupPriorities[num].Value;
				}
			}

			foreach (Pawn pawn in pawns)
			{
				SyncPoliciesFor(pawn);
			}
		}
		public void ResetGroupPolicies()
		{
			activeWorkTypes.Clear();
			groupWorkPriorities.Clear();
			groupArea = null;
			groupAreaEnabled = false;
			groupDrugPolicy = null;
			groupDrugPolicyEnabled = false;
			groupFoodRestriction = null;
			groupFoodRestrictionEnabled = false;
			groupOutfit = null;
			groupOutfitEnabled = false;
		}
		public virtual void ExposeData()
		{
			Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
			Scribe_Collections.Look(ref pawnIcons, "pawnIcons", LookMode.Reference, LookMode.Deep, ref pawnKeys, ref pawnIconValues);
			Scribe_Collections.Look(ref temporaryWorkers, "temporaryWorkers", LookMode.Reference, LookMode.Deep, ref pawnKeys3, ref workTypeValues2);
			Scribe_Collections.Look(ref activeWorkTypes, "activeWorkTypes", LookMode.Deep, LookMode.Value, ref workTypesKeys, ref workStateValues);

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
			Scribe_Deep.Look(ref groupColor, "groupColor");

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
				if (temporaryWorkers is null)
				{
					temporaryWorkers = new Dictionary<Pawn, WorkType>();
				}

				if (activeWorkTypes is null)
				{
					activeWorkTypes = new Dictionary<WorkType, WorkState>();
				}

				if (formations is null)
				{
					formations = new List<Formation>(4);
				}

				if (pawnIcons is null)
				{
					pawnIcons = new Dictionary<Pawn, PawnIcon>();
				}

				curGroupName = groupName ?? defaultGroupName + " " + groupID;
				UpdateData();
				foreach (Pawn pawn in pawns)
				{
					TacticUtils.RegisterGroupFor(pawn, this);
				}
			}
		}

		public override string ToString()
		{
			return curGroupName + " - " + pawns?.Count;
		}

		public string GetUniqueLoadID()
		{
			return GetType().ToString() + groupID;
		}

		public List<Pawn> pawns;
		public Dictionary<Pawn, Rect> pawnRects = new Dictionary<Pawn, Rect>();
		public Dictionary<Pawn, PawnIcon> pawnIcons = new Dictionary<Pawn, PawnIcon>();

		public Dictionary<Pawn, WorkType> temporaryWorkers = new Dictionary<Pawn, WorkType>();
		public Dictionary<WorkType, WorkState> activeWorkTypes = new Dictionary<WorkType, WorkState>();

		public Dictionary<WorkTypeDef, int> groupWorkPriorities = new Dictionary<WorkTypeDef, int>();

		public List<Formation> formations = new List<Formation>(4);
		public Formation activeFormation;

		public GroupColor groupColor;

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
	}
}
