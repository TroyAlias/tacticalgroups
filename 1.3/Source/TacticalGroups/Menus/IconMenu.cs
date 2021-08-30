using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public class IconMenu : TieredFloatMenu
    {
        protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
        protected override Vector2 InitialFloatOptionPositionShift => new Vector2(backgroundTexture.width / 10, 55f);

        public Dictionary<Texture2D, bool> iconStates = new Dictionary<Texture2D, bool>();
        public Dictionary<Texture2D, bool> bannerStates = new Dictionary<Texture2D, bool>();

        public string groupBannerFolder;
        public string groupIconFolder;
        public bool bannerModeEnabled;
        public IconMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
            : base(parentWindow, colonistGroup, originRect, backgroundTexture)
        {
            groupBannerFolder = colonistGroup.groupBannerFolder;
            groupIconFolder = colonistGroup.groupIconFolder;
            bannerModeEnabled = colonistGroup.bannerModeEnabled;
            ReInitIcons(this.colonistGroup.defaultBannerFolder);
        }

        public void ReInitIcons(string folderName)
        {
            groupBannerFolder = folderName;
            string bannerPath = "";
            if (bannerModeEnabled)
            {
                bannerPath = "UI/ColonistBar/GroupIcons/BannerMode/" + groupBannerFolder;
            }
            else
            {
                bannerPath = "UI/ColonistBar/GroupIcons/" + groupBannerFolder;
            }
            List<Texture2D> banners = ContentFinder<Texture2D>.GetAllInFolder(bannerPath).OrderBy(x => x.name).ToList();
            bannerStates.Clear();
            foreach (Texture2D banner in banners)
            {

                if (colonistGroup.groupBanner == banner)
                {
                    bannerStates[banner] = true;
                }
                else
                {
                    bannerStates[banner] = false;
                }
            }

            if (!bannerStates.Where(x => x.Value).Any())
            {
                colonistGroup.groupBannerFolder = groupBannerFolder;
                colonistGroup.groupBanner = banners.First();
                colonistGroup.groupBannerName = colonistGroup.groupBanner.name;
                bannerStates[colonistGroup.groupBanner] = true;
            }

            string iconPath = "";
            if (bannerModeEnabled)
            {
                iconPath = "UI/ColonistBar/GroupIcons/BannerMode/" + groupIconFolder;
            }
            else
            {
                iconPath = "UI/ColonistBar/GroupIcons/" + groupIconFolder;
            }
            List<Texture2D> icons = ContentFinder<Texture2D>.GetAllInFolder(iconPath).OrderBy(x => x.name).ToList();
            iconStates.Clear();
            foreach (Texture2D icon in icons)
            {
                if (colonistGroup.groupIcon == icon)
                {
                    iconStates[icon] = true;
                }
                else
                {
                    iconStates[icon] = false;
                }
            }
            if (!iconStates.Where(x => x.Value).Any())
            {
                colonistGroup.groupBannerFolder = groupBannerFolder;
                colonistGroup.groupIcon = icons.First();
                colonistGroup.groupIconName = colonistGroup.groupIcon.name;
                iconStates[colonistGroup.groupIcon] = true;
            }
        }

        public List<List<Texture2D>> GetIconRows(int columnCount)
        {
            int num = 0;
            List<List<Texture2D>> iconRows = new List<List<Texture2D>>();
            List<Texture2D> row = new List<Texture2D>();
            foreach (Texture2D icon in iconStates.Keys)
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
            foreach (Texture2D icon in bannerStates.Keys)
            {
                row.Add(icon);
            }
            return row;
        }

        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);

            List<Texture2D> bannerRow = GetBanners();
            Rect bannerRowRect = new Rect(rect);
            bannerRowRect.x += 10f;
            bannerRowRect.y += 20f;
            bannerRowRect.height -= 340f;
            bannerRowRect.width -= 97f;

            float listWidth = bannerRow.Count * bannerRow[0].width;
            Rect rect1 = new Rect(0f, 0f, listWidth, bannerRowRect.height - 16f);
            Widgets.BeginScrollView(bannerRowRect, ref scrollPosition2, rect1);

            for (int b = 0; b < bannerRow.Count; b++)
            {
                Rect iconRect = new Rect(rect1.x + (b * 80) + b * 4, rect1.y, 80, 64);
                GUI.DrawTexture(iconRect, bannerRow[b], ScaleMode.ScaleToFit);
                if (colonistGroup.groupBanner == bannerRow[b])
                {
                    if (bannerModeEnabled)
                    {
                        GUI.DrawTexture(iconRect.ExpandedBy(2f), Textures.BannerGroupBannerSelect);
                    }
                    else
                    {
                        if (colonistGroup.isColonyGroup || colonistGroup.isTaskForce)
                        {
                            GUI.DrawTexture(iconRect.ExpandedBy(3f), Textures.ColonyBannerSelect);
                        }
                        else
                        {
                            GUI.DrawTexture(iconRect.ExpandedBy(3f), Textures.GroupBannerSelect);
                        }
                    }
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(iconRect))
                {
                    Event.current.Use();
                    colonistGroup.groupBannerFolder = groupBannerFolder;
                    colonistGroup.groupBanner = bannerRow[b];
                    colonistGroup.groupBannerName = bannerRow[b].name;
                    colonistGroup.updateIcon = true;
                    colonistGroup.bannerModeEnabled = bannerModeEnabled;
                }
            }
            Widgets.EndScrollView();
            List<List<Texture2D>> iconRows = GetIconRows(4);
            Rect initialRect = new Rect(rect);
            initialRect.y += 115f;
            initialRect.x += 10f;
            initialRect.height -= 145f;
            initialRect.width -= 97f;
            float listHeight = iconRows.Count * iconRows[0][0].height + (iconRows.Count * 4);
            Rect rect2 = new Rect(0f, 0f, initialRect.width - 16f, listHeight);
            Widgets.BeginScrollView(initialRect, ref scrollPosition, rect2);
            for (int i = 0; i < iconRows.Count; i++)
            {
                for (int j = 0; j < iconRows[i].Count; j++)
                {
                    Rect iconRect = new Rect(rect2.x + (j * 80) + j * 4, rect2.y + (i * 64) + i * 4, 80, 64);
                    GUI.DrawTexture(iconRect, iconRows[i][j], ScaleMode.ScaleToFit);
                    if (colonistGroup.groupIcon == iconRows[i][j])
                    {
                        if (colonistGroup.isColonyGroup || colonistGroup.isTaskForce)
                        {
                            GUI.DrawTexture(iconRect.ExpandedBy(3f), Textures.ColonyIconSelect);
                        }
                        else
                        {
                            GUI.DrawTexture(iconRect.ExpandedBy(3f), Textures.GroupIconSelect);
                        }
                    }
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(iconRect))
                    {
                        Event.current.Use();
                        colonistGroup.groupIcon = iconRows[i][j];
                        colonistGroup.groupIconName = iconRows[i][j].name;
                        colonistGroup.updateIcon = true;
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

            if (colonistGroup is PawnGroup)
            {
                Rect bigBannerModeRect = new Rect(rect.x + (rect.width - (Textures.DefaultBannerMode.width + 40)), 23, Textures.DefaultBannerMode.width, Textures.DefaultBannerMode.height);
                GUI.DrawTexture(bigBannerModeRect, Textures.DefaultBannerMode);

                Rect smallBannerModeRect = new Rect(bigBannerModeRect.x + Textures.DefaultBannerMode.width + 10, 23, Textures.SmallBannerMode.width, Textures.SmallBannerMode.height);
                GUI.DrawTexture(smallBannerModeRect, Textures.SmallBannerMode);

                if (bannerModeEnabled)
                {
                    GUI.DrawTexture(smallBannerModeRect, Textures.SmallBannerModeSelect);
                }
                else
                {
                    GUI.DrawTexture(bigBannerModeRect, Textures.DefaultBannerModeSelect);
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
                    if (Mouse.IsOver(bigBannerModeRect))
                    {
                        bannerModeEnabled = false;
                        colonistGroup.bannerModeEnabled = false;
                        colonistGroup.updateIcon = true;
                        ReInitIcons(colonistGroup.defaultBannerFolder);
                        Event.current.Use();
                    }
                    else if (Mouse.IsOver(smallBannerModeRect))
                    {
                        bannerModeEnabled = true;
                        colonistGroup.bannerModeEnabled = true;
                        colonistGroup.updateIcon = true;
                        ReInitIcons(colonistGroup.defaultBannerFolder);
                        Event.current.Use();
                    }
                }
            }

            float xPos = rect.x + (rect.width - (Textures.BlueGroupIcon.width + 12));
            float yPos = 75f;
            Rect blueRect = new Rect(xPos, yPos, Textures.BlueGroupIcon.width, Textures.BlueGroupIcon.height);
            GUI.DrawTexture(blueRect, Textures.BlueGroupIcon);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(blueRect))
            {
                ReInitIcons(colonistGroup.colorFolder + "Blue");
                Event.current.Use();
            }
            if (groupBannerFolder == colonistGroup.colorFolder + "Blue")
            {
                if (colonistGroup.isColonyGroup || colonistGroup.isTaskForce)
                {
                    GUI.DrawTexture(blueRect.ExpandedBy(6f), Textures.ColonyIconSelect);
                }
                else
                {
                    GUI.DrawTexture(blueRect.ExpandedBy(6f), Textures.GroupIconSelect);
                }
            }
            yPos += Textures.BlueGroupIcon.height + 5;
            Rect redRect = new Rect(xPos, yPos, Textures.RedGroupIcon.width, Textures.RedGroupIcon.height);
            GUI.DrawTexture(redRect, Textures.RedGroupIcon);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(redRect))
            {
                ReInitIcons(colonistGroup.colorFolder + "Red");
                Event.current.Use();
            }
            if (groupBannerFolder == colonistGroup.colorFolder + "Red")
            {
                if (colonistGroup.isColonyGroup || colonistGroup.isTaskForce)
                {
                    GUI.DrawTexture(redRect.ExpandedBy(6f), Textures.ColonyIconSelect);
                }
                else
                {
                    GUI.DrawTexture(redRect.ExpandedBy(6f), Textures.GroupIconSelect);
                }
            }
            yPos += Textures.BlueGroupIcon.height + 5;
            Rect darkRect = new Rect(xPos, yPos, Textures.DarkGroupIcon.width, Textures.DarkGroupIcon.height);
            GUI.DrawTexture(darkRect, Textures.DarkGroupIcon);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(darkRect))
            {
                ReInitIcons(colonistGroup.colorFolder + "Dark");
                Event.current.Use();
            }
            if (groupBannerFolder == colonistGroup.colorFolder + "Dark")
            {
                if (colonistGroup.isColonyGroup || colonistGroup.isTaskForce)
                {
                    GUI.DrawTexture(darkRect.ExpandedBy(6f), Textures.ColonyIconSelect);
                }
                else
                {
                    GUI.DrawTexture(darkRect.ExpandedBy(6f), Textures.GroupIconSelect);
                }
            }
            yPos += Textures.BlueGroupIcon.height + 5;
            Rect yellowRect = new Rect(xPos, yPos, Textures.YellowGroupIcon.width, Textures.YellowGroupIcon.height);
            GUI.DrawTexture(yellowRect, Textures.YellowGroupIcon);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(yellowRect))
            {
                ReInitIcons(colonistGroup.colorFolder + "Yellow");
                Event.current.Use();
            }
            if (groupBannerFolder == colonistGroup.colorFolder + "Yellow")
            {
                if (colonistGroup.isColonyGroup || colonistGroup.isTaskForce)
                {
                    GUI.DrawTexture(yellowRect.ExpandedBy(6f), Textures.ColonyIconSelect);
                }
                else
                {
                    GUI.DrawTexture(yellowRect.ExpandedBy(6f), Textures.GroupIconSelect);
                }
            }
            yPos += Textures.BlueGroupIcon.height + 5;
            Rect greenRect = new Rect(xPos, yPos, Textures.GreenGroupIcon.width, Textures.GreenGroupIcon.height);
            GUI.DrawTexture(greenRect, Textures.GreenGroupIcon);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(greenRect))
            {
                ReInitIcons(colonistGroup.colorFolder + "Green");
                Event.current.Use();
            }
            if (groupBannerFolder == colonistGroup.colorFolder + "Green")
            {
                if (colonistGroup.isColonyGroup || colonistGroup.isTaskForce)
                {
                    GUI.DrawTexture(greenRect.ExpandedBy(6f), Textures.ColonyIconSelect);
                }
                else
                {
                    GUI.DrawTexture(greenRect.ExpandedBy(6f), Textures.GroupIconSelect);
                }
            }
        }

        private Vector2 scrollPosition;
        private Vector2 scrollPosition2;
    }
}
