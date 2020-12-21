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
		public Dictionary<Texture2D, bool> bannerStates = new Dictionary<Texture2D, bool>();

		public string groupBannerFolder;
		public string groupIconFolder;
		public IconMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			groupBannerFolder = colonistGroup.groupBannerFolder;
			groupIconFolder = colonistGroup.groupIconFolder;
			Log.Message(groupBannerFolder + " - " + groupIconFolder);
			ReInitIcons(this.colonistGroup.defaultBannerFolder);
		}

		public void ReInitIcons(string folderName)
		{
			this.groupBannerFolder = folderName;
			var banners = ContentFinder<Texture2D>.GetAllInFolder("UI/ColonistBar/GroupIcons/" + groupBannerFolder).OrderBy(x => x.name).ToList();
			bannerStates.Clear();
			foreach (var banner in banners)
			{
				if (this.colonistGroup.groupBanner.name == banner.name)
				{
					bannerStates[banner] = true;
				}
				else
				{
					bannerStates[banner] = false;
				}
			}

			var icons = ContentFinder<Texture2D>.GetAllInFolder("UI/ColonistBar/GroupIcons/" + groupIconFolder).OrderBy(x => x.name).ToList();
			iconStates.Clear();
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

		public List<Texture2D> GetBanners()
		{
			List<Texture2D> row = new List<Texture2D>();
			foreach (var icon in bannerStates.Keys)
			{
				row.Add(icon);
			}
			return row;
		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);

			var bannerRow = GetBanners();
			var bannerRowRect = new Rect(rect);
			bannerRowRect.x += 10f;
			bannerRowRect.y += 20f;
			bannerRowRect.height -= 350f;
			bannerRowRect.width -= 97f;
			
			float listWidth = bannerRow.Count * bannerRow[0].width;
			Rect rect1 = new Rect(0f, 0f, listWidth, bannerRowRect.height - 16f);
			Widgets.BeginScrollView(bannerRowRect, ref scrollPosition2, rect1);
			for (var b = 0; b < bannerRow.Count; b++)
			{
				Rect iconRect = new Rect(rect1.x + (b * 80) + b * 4, rect1.y, 80, 64);
				GUI.DrawTexture(iconRect, bannerRow[b], ScaleMode.ScaleToFit);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(iconRect))
				{
					Event.current.Use();
					this.colonistGroup.groupBannerFolder = groupBannerFolder;
					this.colonistGroup.groupBanner = bannerRow[b];
					this.colonistGroup.groupBannerName = bannerRow[b].name;
					this.colonistGroup.updateIcon = true;
				}
			}
			Widgets.EndScrollView();

			var iconRows = GetIconRows(4);
			var initialRect = new Rect(rect);
			initialRect.y += 115f;
			initialRect.x += 10f;
			initialRect.height -= 145f;
			initialRect.width -= 97f;
			float listHeight = iconRows.Count * iconRows[0][0].height + (iconRows.Count * 4);
			Rect rect2 = new Rect(0f, 0f, initialRect.width - 16f, listHeight);
			Widgets.BeginScrollView(initialRect, ref scrollPosition, rect2);
			for (var i = 0; i < iconRows.Count; i++)
			{
				for (var j = 0; j < iconRows[i].Count; j++)
				{
					Rect iconRect = new Rect(rect2.x + (j * 80) + j * 4, rect2.y + (i * 64) + i * 4, 80, 64);
					GUI.DrawTexture(iconRect, iconRows[i][j], ScaleMode.ScaleToFit);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(iconRect))
					{
						Event.current.Use();
						this.colonistGroup.groupIcon = iconRows[i][j];
						this.colonistGroup.groupIconName = iconRows[i][j].name;
						this.colonistGroup.updateIcon = true;
					}
				}
			}

			Widgets.EndScrollView();
			DrawExtraGui(rect);
			GUI.color = Color.white;
		}

        public override void DrawExtraGui(Rect rect)
        {
            base.DrawExtraGui(rect);
			float xPos = rect.x + (rect.width - (Textures.BlueGroupIcon.width + 12));
			float yPos = 65f;
			var blueRect = new Rect(xPos, yPos, Textures.BlueGroupIcon.width, Textures.BlueGroupIcon.height);
			GUI.DrawTexture(blueRect, Textures.BlueGroupIcon);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(blueRect))
            {
				ReInitIcons(this.colonistGroup.colorFolder + "Blue");
				Event.current.Use();
			}
			yPos += Textures.BlueGroupIcon.height + 5;
			var redRect = new Rect(xPos, yPos, Textures.RedGroupIcon.width, Textures.RedGroupIcon.height);
			GUI.DrawTexture(redRect, Textures.RedGroupIcon);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(redRect))
			{
				ReInitIcons(this.colonistGroup.colorFolder + "Red");
				Event.current.Use();
			}

			yPos += Textures.BlueGroupIcon.height + 5;
			var darkRect = new Rect(xPos, yPos, Textures.DarkGroupIcon.width, Textures.DarkGroupIcon.height);
			GUI.DrawTexture(darkRect, Textures.DarkGroupIcon);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(darkRect))
			{
				ReInitIcons(this.colonistGroup.colorFolder + "Dark");
				Event.current.Use();
			}

			yPos += Textures.BlueGroupIcon.height + 5;
			var yellowRect = new Rect(xPos, yPos, Textures.YellowGroupIcon.width, Textures.YellowGroupIcon.height);
			GUI.DrawTexture(yellowRect, Textures.YellowGroupIcon);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(yellowRect))
			{
				ReInitIcons(this.colonistGroup.colorFolder + "Yellow");
				Event.current.Use();
			}

			yPos += Textures.BlueGroupIcon.height + 5;
			var greenRect = new Rect(xPos, yPos, Textures.GreenGroupIcon.width, Textures.GreenGroupIcon.height);
			GUI.DrawTexture(greenRect, Textures.GreenGroupIcon);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(greenRect))
			{
				ReInitIcons(this.colonistGroup.colorFolder + "Green");
				Event.current.Use();
			}
		}

		private Vector2 scrollPosition;
		private Vector2 scrollPosition2;
	}
}
