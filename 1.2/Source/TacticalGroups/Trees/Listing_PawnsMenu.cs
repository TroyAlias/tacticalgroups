using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public class Listing_PawnsMenu : Listing_Tree
	{
		protected override float LabelWidth => base.ColumnWidth;
		public Listing_PawnsMenu()
		{

		}

		public List<List<Pawn>> GetPawnRows(TreeNode_Pawns node, int columnCount)
		{
			int num = 0;
			List<List<Pawn>> pawnRows = new List<List<Pawn>>();
			List<Pawn> row = new List<Pawn>();
			foreach (var icon in node.pawns)
			{
				if (num == columnCount)
				{
					pawnRows.Add(row.ListFullCopy());
					row = new List<Pawn>();
					num = 0;
				}
				num++;
				row.Add(icon);
			}
			if (row.Any())
			{
				pawnRows.Add(row);
			}
			return pawnRows;
		}

		public void SetCurY(float newY)
        {
			curY = newY;
		}

		[TweakValue("0TacticGroups", 0, 50f)] public static float xPawnIconMargin = 6f;
		[TweakValue("0TacticGroups", 0, 50f)] public static float yPawnIconMargin = 20f;
		public void DoCategory(TreeNode_Pawns node, int nestLevel, int openMask)
		{
			OpenCloseWidget(node, nestLevel, openMask);
			Rect rect = new Rect(15f, curY, LabelWidth, lineHeight);
			Widgets.Label(rect, node.Label);
			EndLine();

			if (node.IsOpen(openMask))
			{
				rect.xMin = XAtIndentLevel(nestLevel) + 18f;
				if (node.pawns != null)
                {
					var pawnRows = GetPawnRows(node, 5);
					for (var i = 0; i < pawnRows.Count; i++)
					{
						for (var j = 0; j < pawnRows[i].Count; j++)
						{
							Rect pawnRect = new Rect(10 + (j * (TacticalColonistBar.DefaultBaseSize.x + xPawnIconMargin)),
								rect.yMax + 10 + (i * (TacticalColonistBar.DefaultBaseSize.y + yPawnIconMargin)),
								TacticalColonistBar.DefaultBaseSize.x, TacticalColonistBar.DefaultBaseSize.y);
							ManagementMenu.DrawColonist(pawnRect, pawnRows[i][j], pawnRows[i][j].Map, false, false);
							if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(pawnRect))
                            {
								Event.current.Use();
								Find.Selector.ClearSelection();
								Find.Selector.Select(pawnRows[i][j]);
                            }
						}
						curY += TacticalColonistBar.DefaultBaseSize.y + yPawnIconMargin;
					}
				}
			}
		}
	}
}
