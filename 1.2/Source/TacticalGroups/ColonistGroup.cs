using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public class ColonistGroup : IExposable
    {
        public List<Pawn> pawns;
		public bool pawnWindowIsActive;
        public ColonistGroup(List<Pawn> pawns)
        {
            this.pawns = pawns;
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

        public void Draw(Rect rect)
        {
			var totalRect = new Rect(rect);
			var pawnRows = GetPawnRows;
			//totalRect.height += pawnRows.Count * 15;
			totalRect = totalRect.ScaledBy(1.5f);
			if (Mouse.IsOver(rect))
            {
				pawnWindowIsActive = true;
				DrawPawnRows(rect, pawnRows);
			}
			else if (Mouse.IsOver(totalRect) && pawnWindowIsActive)
			{
				DrawPawnRows(rect, pawnRows);
			}
			else
            {
				pawnWindowIsActive = false;
			}
        }

		public void DrawPawnRows(Rect rect, List<List<Pawn>> pawnRows)
        {
			var initialRect = new Rect(rect);
			initialRect.x -= initialRect.width / 1.5f;
			initialRect.y += initialRect.height * 0.7f;
			for (var i = 0; i < pawnRows.Count; i++)
			{
				for (var j = 0; j < pawnRows[i].Count; j++)
				{
					Rect smallRect = new Rect(initialRect.x + ((j + 1) * 25), initialRect.y + ((i + 1) * 30), initialRect.width / 2f, initialRect.height / 2f);
					DrawColonist(smallRect, pawnRows[i][j], pawnRows[i][j].Map, true, false);
				}
			}
		}
		public void DrawColonist(Rect rect, Pawn colonist, Map pawnMap, bool highlight, bool reordering)
		{
			float alpha = TacticalGroups.TacticalColonistBar.GetEntryRectAlpha(rect);
			TacticalGroups.TacticalColonistBar.drawer.ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref alpha);
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
			//Rect rect2 = rect.ContractedBy(-2f * TacticalGroups.TacticalColonistBar.Scale);
			//if ((colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist.Corpse) : Find.Selector.SelectedObjects.Contains(colonist)) && !WorldRendererUtility.WorldRenderedNow)
			//{
			//	TacticalGroups.TacticalColonistBar.drawer.DrawSelectionOverlayOnGUI(colonist, rect2);
			//}
			//else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
			//{
			//	TacticalGroups.TacticalColonistBar.drawer.DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
			//}
			GUI.DrawTexture(GetPawnTextureRect(rect.position), PortraitsCache.Get(colonist, ColonistBarColonistDrawer.PawnTextureSize * 0.5f, 
				ColonistBarColonistDrawer.PawnTextureCameraOffset, 1.28205f));
			GUI.color = new Color(1f, 1f, 1f, alpha * 0.8f);
			//TacticalGroups.TacticalColonistBar.drawer.DrawIcons(rect, colonist);
			GUI.color = color2;
			if (colonist.Dead)
			{
				GUI.DrawTexture(rect, ColonistBarColonistDrawer.DeadColonistTex);
			}
			float num2 = 4f * TacticalGroups.TacticalColonistBar.Scale;
			Vector2 pos = new Vector2(rect.center.x, rect.yMax - num2);
			//GenMapUI.DrawPawnLabel(colonist, pos, alpha, rect.width + TacticalGroups.TacticalColonistBar.SpaceBetweenColonistsHorizontal - 2f, TacticalGroups.TacticalColonistBar.drawer.pawnLabelsCache);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
		}

		public Rect GetPawnTextureRect(Vector2 pos)
		{
			float x = pos.x;
			float y = pos.y;
			Vector2 vector = ColonistBarColonistDrawer.PawnTextureSize * TacticalGroups.TacticalColonistBar.Scale;
			return new Rect(x + 1f, y - ((vector.y - (TacticalGroups.TacticalColonistBar.Size.y)) * 0.5f) - 1f, vector.x * 0.5f, vector.y * 0.5f).ContractedBy(1f);
		}
		public void ExposeData()
        {

        }
    }
}
