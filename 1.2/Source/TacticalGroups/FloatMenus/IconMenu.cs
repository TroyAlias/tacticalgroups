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
	public class IconMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 55f);

		public Dictionary<Texture2D, bool> iconStates = new Dictionary<Texture2D, bool>();
		public IconMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			var icons = ContentFinder<Texture2D>.GetAllInFolder("UI/ColonistBar/GroupIcons").OrderBy(x => x.name);
			foreach (var icon in icons)
            {
				if (this.colonistGroup.groupIcon.name == icon.name)
                {
					iconStates[icon] = true;
				}
				else
                {
					iconStates[icon] = false;
				}
			}
		}

		public List<List<Texture2D>> GetIconRows(int columnCount)
		{
			int num = 0;
			List<List<Texture2D>> iconRows = new List<List<Texture2D>>();
			List<Texture2D> row = new List<Texture2D>();
			foreach (var icon in iconStates.Keys)
			{
				if (num == columnCount)
				{
					iconRows.Add(row.ListFullCopy());
					row = new List<Texture2D>();
					num = 0;
				}
				num++;
				row.Add(icon);
			}
			if (row.Any())
			{
				iconRows.Add(row);
			}
			return iconRows;
		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Vector2 zero = Vector2.zero;

			var iconRows = GetIconRows(4);
			var initialRect = new Rect(rect);
			initialRect.y += 25f;
			initialRect.x += 10f;
			initialRect.height -= 45f;
			initialRect.width -= 16f;
			float listHeight = iconRows.Count * iconRows[0][0].height + (iconRows.Count * 4);
			Rect rect2 = new Rect(0f, 0f, initialRect.width - 16f, listHeight);
			Widgets.BeginScrollView(initialRect, ref scrollPosition, rect2);

			//Widgets.DrawBox(initialRect);
			//Widgets.DrawBox(rect2, 3);

			for (var i = 0; i < iconRows.Count; i++)
			{
				for (var j = 0; j < iconRows[i].Count; j++)
				{
					Rect iconRect = new Rect(rect2.x + (j * iconRows[i][j].width) + j * 4, rect2.y + (i * iconRows[i][j].height) + i * 4,
						iconRows[i][j].width, iconRows[i][j].height);
					GUI.DrawTexture(iconRect, iconRows[i][j]);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(iconRect))
					{
						Event.current.Use();
						this.colonistGroup.groupIcon = iconRows[i][j];
						this.colonistGroup.groupIconName = iconRows[i][j].name;
					}
				}
			}

			Widgets.EndScrollView();
			DrawExtraGui(rect);
			if (Event.current.type == EventType.MouseDown)
			{
				Event.current.Use();
				Close();
			}
			GUI.color = Color.white;
		}

		private Vector2 scrollPosition;

	}
}
