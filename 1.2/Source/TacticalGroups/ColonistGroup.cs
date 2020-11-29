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

		private bool pawnWindowIsActive;
		private string groupName;
		public bool Visible => pawnWindowIsActive;
		private bool expandPawnIcons;
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

		public ColonistGroup(List<Pawn> pawns)
        {
            this.pawns = pawns;
        }

		public ColonistGroup()
        {

        }
        public ColonistGroup(Pawn pawn)
        {
            this.pawns = new List<Pawn> { pawn } ;
        }
        public void Add(Pawn pawn)
        {
            this.pawns.Add(pawn);
        }

		public List<List<Pawn>> GetPawnRows
        {
			get
            {
				int num = 0;
				List<List<Pawn>> pawnRows = new List<List<Pawn>>();
				List<Pawn> row = new List<Pawn>();
				foreach (var pawn in pawns)
                {
					if (num == 3)
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
        }

		public void HandleClicks(Rect rect)
        {
			if (Event.current.type == EventType.MouseDown)
			{
				if (Event.current.button == 0)
                {
					if (Event.current.clickCount == 1)
					{
						if (!expandPawnIcons)
						{
							expandPawnIcons = true;
						}
						else
						{
							expandPawnIcons = false;
						}
						Event.current.Use();
					}
					else if (Event.current.clickCount == 2)
					{
						Find.Selector.ClearSelection();
						foreach (var pawn in this.pawns)
						{
							Find.Selector.Select(pawn);
						}
						Event.current.Use();
					}
				}
				else if (Event.current.button == 1)
                {
					List<ColonistBarFloatMenuOption> list = new List<ColonistBarFloatMenuOption>();
					list.Add(new ColonistBarFloatMenuOption("Test: ", null, ColonistBarFloatMenuOption.RallyIcon, MenuOptionPriority.High, null, null));
					list.Add(new ColonistBarFloatMenuOption("Test: ", null, ColonistBarFloatMenuOption.ActionsIcon, MenuOptionPriority.High, null, null));
					list.Add(new ColonistBarFloatMenuOption("Test: ", null, ColonistBarFloatMenuOption.OrdersIcon, MenuOptionPriority.High, null, null));
					list.Add(new ColonistBarFloatMenuOption("Test: ", null, ColonistBarFloatMenuOption.ManageIcon, MenuOptionPriority.High, null, null));
					ColonistBarFloatMenu floatMenu = new ColonistBarFloatMenu(list, this, rect);
					Find.WindowStack.Add(floatMenu);
				}
			}

		}
        public void Draw(Rect rect)
        {
			var totalRect = new Rect(rect);
			var pawnRows = GetPawnRows;
			totalRect.height += pawnRows.Count * 30;
			totalRect = totalRect.ScaledBy(1.1f);
			if (Mouse.IsOver(rect))
			{
				pawnWindowIsActive = true;
				DrawPawnRows(rect, pawnRows);
				TooltipHandler.TipRegion(rect, new TipSignal("TG.GroupInfoTooltip".Translate(groupName)));
				HandleClicks(rect);
			}
			else if (Mouse.IsOver(totalRect) && pawnWindowIsActive)
			{
				DrawPawnRows(rect, pawnRows);
			}
			else
			{
				pawnWindowIsActive = false;
				expandPawnIcons = false;
			}
		}

		public void DrawPawnRows(Rect rect, List<List<Pawn>> pawnRows)
        {
			if (ShowExpanded)
            {
				var initialRect = new Rect(rect).ScaledBy(3f);
				initialRect.y += initialRect.height * 0.45f;
				for (var i = 0; i < pawnRows.Count; i++)
				{
					for (var j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(initialRect.x + ((j + 1) * 50), initialRect.y + ((i + 1) * 60), 50, 50);
						DrawColonist(smallRect, pawnRows[i][j], pawnRows[i][j].Map, true, false);
						pawnRects[pawnRows[i][j]] = smallRect;
					}
				}
			}
			else
            {
				var initialRect = new Rect(rect);
				initialRect.x -= (initialRect.width / 3.3333333333f) - 1f;
				initialRect.y += initialRect.height * 0.65f;
				for (var i = 0; i < pawnRows.Count; i++)
				{
					for (var j = 0; j < pawnRows[i].Count; j++)
					{
						Rect smallRect = new Rect(initialRect.x + ((j + 1) * 25), initialRect.y + ((i + 1) * 30), 24, 24);
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
			//TacticalGroups.TacticalColonistBar.drawer.DrawIcons(rect, colonist);
			GUI.color = color2;
			if (colonist.Dead)
			{
				GUI.DrawTexture(rect, ColonistBarColonistDrawer.DeadColonistTex);
			}
			//float num2 = 4f * TacticUtils.TacticalColonistBar.Scale;
			//Vector2 pos = new Vector2(rect.center.x, rect.yMax - num2);
			//GenMapUI.DrawPawnLabel(colonist, pos, alpha, rect.width + TacticalGroups.TacticalColonistBar.SpaceBetweenColonistsHorizontal - 2f, TacticalGroups.TacticalColonistBar.drawer.pawnLabelsCache);
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
			Vector2 vector = ColonistBarColonistDrawer.PawnTextureSize * TacticUtils.TacticalColonistBar.Scale;
			return new Rect(x + 1f, y - ((vector.y - (TacticUtils.TacticalColonistBar.Size.y)) * IconScale) - 1f, vector.x * IconScale, vector.y * IconScale).ContractedBy(1f);
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

		public void ExposeData()
        {
			Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
		}

		private List<Pawn> pawnKeys;
		private List<Rect> rectValues;
	}
}
