using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TacticalGroups
{
    [HotSwappable]
    [StaticConstructorOnStartup]
    public class TacticalColonistBar
    {
        public struct Entry
        {
            public Pawn pawn;

            public Map map;

            public int group;

            public Action<int, int> reorderAction;

            public Action<int, Vector2> extraDraggedItemOnGUI;

            public CaravanGroup caravanGroup;

            public ColonyGroup colonyGroup;
            public Entry(Pawn pawn, Map map, int group, CaravanGroup caravanGroup, ColonyGroup colonyGroup)
            {
                this.pawn = pawn;
                this.map = map;
                this.group = group;
                this.caravanGroup = caravanGroup;
                this.colonyGroup = colonyGroup;
                reorderAction = delegate (int from, int to)
                {
                    TacticUtils.TacticalColonistBar.Reorder(from, to, group);
                };
                extraDraggedItemOnGUI = delegate (int index, Vector2 dragStartPos)
                {
                    TacticUtils.TacticalColonistBar.DrawColonistMouseAttachment(index, dragStartPos, group);
                };
            }
        }

        public TacticalGroups_ColonistBarColonistDrawer drawer = new TacticalGroups_ColonistBarColonistDrawer();

        private readonly ColonistBarDrawLocsFinder drawLocsFinder = new ColonistBarDrawLocsFinder();

        private readonly List<Entry> cachedEntries = new List<Entry>();

        public List<Rect> DrawLocs { get; } = new List<Rect>();

        private float cachedScale = 1f;

        private bool entriesDirty = true;

        private readonly HashSet<Pawn> colonistsToHighlight = new HashSet<Pawn>();

        public static readonly Texture2D BGTex = Command.BGTex;

        public static Vector2 DefaultBaseSize = new Vector2(48f, 48f);
        public static Vector2 BaseSize = new Vector2(48f, 48f);

        public const float BaseSelectedTexJump = 20f;

        public const float BaseSelectedTexScale = 0.4f;

        public const float EntryInAnotherMapAlpha = 0.4f;

        public const float BaseSpaceBetweenGroups = 25f;

        public const float BaseSpaceBetweenColonistsHorizontal = 24f;

        public const float BaseSpaceBetweenColonistsVertical = 32f;

        public const float FactionIconSpacing = 2f;

        private static List<Pawn> tmpPawns = new List<Pawn>();

        private static readonly List<Map> tmpMaps = new List<Map>();

        private static readonly List<Caravan> tmpCaravans = new List<Caravan>();

        private static readonly List<Pawn> tmpColonistsInOrder = new List<Pawn>();

        private static readonly List<Pair<Thing, Map>> tmpColonistsWithMap = new List<Pair<Thing, Map>>();

        private static readonly List<Thing> tmpColonists = new List<Thing>();

        private static readonly List<Thing> tmpMapColonistsOrCorpsesInScreenRect = new List<Thing>();

        private static readonly List<Pawn> tmpCaravanPawns = new List<Pawn>();
        public void UpdateSizes()
        {
            TacticalGroups_ColonistBarColonistDrawer.PawnTextureCameraOffset = new Vector3(TacticalGroupsSettings.PawnCameraOffsetX, 0, TacticalGroupsSettings.PawnCameraOffsetZ);
            TacticalGroups_ColonistBarColonistDrawer.PawnTextureSize = TacticalGroups_ColonistBarColonistDrawer.DefaultPawnTextureSize;
            TacticalGroups_ColonistBarColonistDrawer.PawnTextureSize.x += TacticalGroupsSettings.XPawnIconOffset;
            TacticalGroups_ColonistBarColonistDrawer.PawnTextureSize.y += TacticalGroupsSettings.YPawnIconOffset;
            TacticalColonistBar.BaseSize = new Vector2(TacticalGroupsSettings.PawnBoxWidth, TacticalGroupsSettings.PawnBoxHeight);
        }
        public List<Entry> Entries
        {
            get
            {
                CheckRecacheEntries();
                return cachedEntries;
            }
        }

        private bool ShowGroupFrames
        {
            get
            {
                List<Entry> entries = Entries;
                int num = -1;
                for (int i = 0; i < entries.Count; i++)
                {
                    num = Mathf.Max(num, entries[i].group);
                }
                return num >= 1;
            }
        }

        public float Scale => cachedScale;

        public Vector2 Size => BaseSize * Scale;

        public float SpaceBetweenColonistsHorizontal => 24f * Scale;

        private bool Visible => UI.screenWidth >= 800 && UI.screenHeight >= 500;

        public void MarkColonistsDirty()
        {
            entriesDirty = true;
        }
        private List<int> cachedReorderableGroups = new List<int>();

        public void ColonistBarOnGUI()
        {
            if (!Visible)
            {
                return;
            }
            if (Event.current.type != EventType.Layout)
            {
                List<Entry> entries = Entries;
                int num = -1;
                bool showGroupFrames = ShowGroupFrames;
                int reorderableGroup = -1;
                GUI.color = Color.white;
                Text.Font = GameFont.Tiny;

                Rect createGroupRect = ColonistBarDrawLocsFinder.createGroupRect;
                if (Mouse.IsOver(createGroupRect))
                {
                    GUI.DrawTexture(createGroupRect, Textures.CreateGroupIconHover);
                    TooltipHandler.TipRegion(createGroupRect, Strings.CreateGroupTooltip);
                }
                else if (!TacticalGroupsSettings.HideCreateGroup)
                {
                    GUI.DrawTexture(createGroupRect, Textures.CreateGroupIcon);
                }

                HandleGroupingClicks(createGroupRect);
                Rect optionsGearRect = new Rect(createGroupRect.x + (createGroupRect.width / 3f), createGroupRect.y + createGroupRect.height + 5, Textures.OptionsGear.width, Textures.OptionsGear.height);
                if (Mouse.IsOver(optionsGearRect))
                {
                    GUI.DrawTexture(optionsGearRect, Textures.OptionsGearHover);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        TieredFloatMenu floatMenu = new OptionsMenu(null, null, optionsGearRect, Textures.OptionsMenu);
                        Find.WindowStack.Add(floatMenu);
                    }
                    TooltipHandler.TipRegion(optionsGearRect, Strings.OptionsGearTooltip);
                }
                else if (!TacticalGroupsSettings.HideCreateGroup)
                {
                    GUI.DrawTexture(optionsGearRect, Textures.OptionsGear);
                }

                for (int i = 0; i < ColonistBarDrawLocsFinder.pawnGroupDrawLoc.Count; i++)
                {
                    ColonistBarDrawLocsFinder.pawnGroupDrawLoc[i].colonistGroup.Draw(ColonistBarDrawLocsFinder.pawnGroupDrawLoc[i].rect);
                }
                for (int i = 0; i < ColonistBarDrawLocsFinder.colonyGroupDrawLoc.Count; i++)
                {
                    ColonistBarDrawLocsFinder.colonyGroupDrawLoc[i].colonistGroup.Draw(ColonistBarDrawLocsFinder.colonyGroupDrawLoc[i].rect);
                }
                for (int i = 0; i < ColonistBarDrawLocsFinder.caravanGroupDrawLoc.Count; i++)
                {
                    ColonistBarDrawLocsFinder.caravanGroupDrawLoc[i].colonistGroup.Draw(ColonistBarDrawLocsFinder.caravanGroupDrawLoc[i].rect);
                }

                for (int i = 0; i < ColonistBarDrawLocsFinder.pawnGroupDrawLoc.Count; i++)
                {
                    ColonistBarDrawLocsFinder.pawnGroupDrawLoc[i].colonistGroup.DrawOverlays(ColonistBarDrawLocsFinder.pawnGroupDrawLoc[i].rect);
                }

                for (int i = 0; i < ColonistBarDrawLocsFinder.colonyGroupDrawLoc.Count; i++)
                {
                    ColonistBarDrawLocsFinder.colonyGroupDrawLoc[i].colonistGroup.DrawOverlays(ColonistBarDrawLocsFinder.colonyGroupDrawLoc[i].rect);
                }

                for (int i = 0; i < ColonistBarDrawLocsFinder.caravanGroupDrawLoc.Count; i++)
                {
                    ColonistBarDrawLocsFinder.caravanGroupDrawLoc[i].colonistGroup.DrawOverlays(ColonistBarDrawLocsFinder.caravanGroupDrawLoc[i].rect);
                }

                for (int i = 0; i < DrawLocs.Count; i++)
                {
                    Entry entry = entries[i];
                    bool flag = num != entry.group;
                    num = entry.group;
                    if (Event.current.type == EventType.Repaint)
                    {
                        if (flag)
                        {
                            reorderableGroup = ReorderableWidget.NewGroup(entry.reorderAction, ReorderableDirection.Horizontal, new Rect(0f, 0f, UI.screenWidth, UI.screenHeight), SpaceBetweenColonistsHorizontal, entry.extraDraggedItemOnGUI);
                        }
                        cachedReorderableGroups[i] = reorderableGroup;
                    }
                    bool reordering;
                    if (entry.pawn != null)
                    {
                        drawer.HandleClicks(DrawLocs[i], entry.pawn, cachedReorderableGroups[i], out reordering);
                    }
                    else
                    {
                        reordering = false;
                    }
                    if (Event.current.type != EventType.Repaint)
                    {
                        continue;
                    }
                    if (flag && showGroupFrames)
                    {
                        drawer.DrawGroupFrame(entry.group);
                    }
                    if (entry.pawn != null)
                    {
                        drawer.DrawColonist(DrawLocs[i], entry.pawn, entry.map, colonistsToHighlight.Contains(entry.pawn), reordering);
                        TacticalGroups_ColonistBarColonistDrawer.DrawHealthBar(DrawLocs[i], entry.pawn, TacticalGroupsSettings.HealthBarWidth);
                        TacticalGroups_ColonistBarColonistDrawer.DrawRestAndFoodBars(DrawLocs[i], entry.pawn, TacticalGroupsSettings.PawnNeedsWidth);
                        TacticalGroups_ColonistBarColonistDrawer.ShowDrafteesWeapon(DrawLocs[i], entry.pawn, TacticalGroupsSettings.WeaponPlacementOffset);
                    }
                }
                num = -1;
                if (showGroupFrames)
                {
                    for (int j = 0; j < DrawLocs.Count; j++)
                    {
                        Entry entry2 = entries[j];
                        bool num3 = num != entry2.group;
                        num = entry2.group;
                        if (num3)
                        {
                            drawer.HandleGroupFrameClicks(entry2.group);
                        }
                    }
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                colonistsToHighlight.Clear();
            }
        }

        public static void HandleGroupingClicks(Rect rect)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Mouse.IsOver(rect))
            {
                List<Pawn> selectedPawns = Find.Selector.SelectedPawns.Where(x => x.Faction == Faction.OfPlayer).ToList();
                if (selectedPawns.Any())
                {
                    TacticDefOf.TG_CreateGroupSFX.PlayOneShotOnCamera();
                    TacticUtils.TacticalGroups.AddGroup(selectedPawns);
                    TacticUtils.TacticalColonistBar.MarkColonistsDirty();
                }
                Event.current.Use();
            }
        }

        private List<Pawn> GetNonHiddenPawns(List<Pawn> pawns)
        {
            var visiblePawns = new HashSet<Pawn>();
            var nonVisiblePawns = new HashSet<Pawn>();
            if (!TacticalGroupsSettings.HideGroups)
            {
                for (int i = pawns.Count - 1; i >= 0; i--)
                {
                    var pawn = pawns[i];
                    var pawnGroups = TacticUtils.AllPawnGroups.Where(x => x.pawns.Contains(pawn)).ToList();
                    foreach (var pawnGroup in pawnGroups)
                    {
                        if (pawnGroup.entireGroupIsVisible is false)
                        {
                            pawns.RemoveAt(i);
                            break;
                            //if (Find.Selector.IsSelected(pawns[i]))
                            //    Log.Message("1 nonVisiblePawns: " + pawns[i] + " - " + pawnGroup.curGroupName);
                        }
                    }
                }
                for (int i = pawns.Count - 1; i >= 0; i--)
                {
                    foreach (PawnGroup pawnGroup in TacticUtils.AllPawnGroups)
                    {
                        if (pawnGroup.pawns.Contains(pawns[i]))
                        {
                            if (pawnGroup.pawnIcons[pawns[i]].isVisibleOnColonistBar && pawns[i].Map != null
                                && (!TacticalGroupsSettings.HidePawnsWhenOffMap || pawns[i].Map == Find.CurrentMap))
                            {
                                //if (Find.Selector.IsSelected(pawns[i]))
                                //    Log.Message("1 VisiblePawns: " + pawns[i] + " - " + pawnGroup.curGroupName);
                                visiblePawns.Add(pawns[i]);
                            }
                        }
                    }
                }

                foreach (ColonyGroup colonyGroup in TacticUtils.AllColonyGroups)
                {
                    for (int i = pawns.Count - 1; i >= 0; i--)
                    {
                        if (colonyGroup.pawns.Contains(pawns[i]))
                        {
                            if (colonyGroup.pawnIcons[pawns[i]].isVisibleOnColonistBar
                                && (!TacticalGroupsSettings.HidePawnsWhenOffMap || pawns[i].Map == Find.CurrentMap))
                            {
                                //visiblePawns.Add(pawns[i]);
                            }
                            else
                            {
                                //if (Find.Selector.IsSelected(pawns[i]))
                                //    Log.Message("2 nonVisibleColonyPawns: " + pawns[i]);
                                nonVisiblePawns.Add(pawns[i]);
                            }
                        }
                    }
                }


                foreach (CaravanGroup caravanGroup in TacticUtils.AllCaravanGroups)
                {
                    for (int i = pawns.Count - 1; i >= 0; i--)
                    {
                        if (caravanGroup.pawns.Contains(pawns[i]))
                        {
                            if (caravanGroup.pawnIcons[pawns[i]].isVisibleOnColonistBar && !TacticalGroupsSettings.HidePawnsWhenOffMap)
                            {
                                visiblePawns.Add(pawns[i]);
                            }
                            else
                            {
                                nonVisiblePawns.Add(pawns[i]);
                            }
                        }
                    }
                }
            }

            nonVisiblePawns.RemoveWhere(x => visiblePawns.Contains(x));
            return pawns.Where(x => visiblePawns.Contains(x) && !nonVisiblePawns.Contains(x) 
            || !visiblePawns.Contains(x) && !nonVisiblePawns.Contains(x)).ToList();
        }
        private void CheckRecacheEntries()
        {
            if (!entriesDirty)
            {
                return;
            }
            entriesDirty = false;
            cachedEntries.Clear();
            if (Find.PlaySettings.showColonistBar)
            {
                tmpMaps.Clear();
                tmpMaps.AddRange(Find.Maps);
                tmpMaps.SortBy((Map x) => !x.IsPlayerHome, (Map x) => x.uniqueID);
                int num = 0;
                for (int i = 0; i < tmpMaps.Count; i++)
                {
                    var oldCount = cachedEntries.Count;
                    tmpPawns.Clear();
                    tmpPawns.AddRange(tmpMaps[i].mapPawns.FreeColonists);
                    tmpPawns.AddRange(tmpMaps[i].mapPawns.ColonySubhumansControllable);
                    List<Thing> list = tmpMaps[i].listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (!list[j].IsDessicated())
                        {
                            Pawn innerPawn = ((Corpse)list[j]).InnerPawn;
                            if (innerPawn != null && innerPawn.IsColonist)
                            {
                                tmpPawns.Add(innerPawn);
                            }
                        }
                    }
                    IReadOnlyList<Pawn> allPawnsSpawned = tmpMaps[i].mapPawns.AllPawnsSpawned;
                    for (int k = 0; k < allPawnsSpawned.Count; k++)
                    {
                        if (allPawnsSpawned[k].carryTracker.CarriedThing is Corpse corpse && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist)
                        {
                            tmpPawns.Add(corpse.InnerPawn);
                        }
                    }
                    foreach (Pawn tmpPawn in tmpPawns)
                    {
                        if (tmpPawn.playerSettings.displayOrder == -9999999)
                        {
                            tmpPawn.playerSettings.displayOrder = Mathf.Max(tmpPawns.MaxBy((Pawn p) => p.playerSettings.displayOrder).playerSettings.displayOrder, 0) + 1;
                        }
                    }
                    tmpPawns = GetNonHiddenPawns(tmpPawns);
                    tmpPawns = tmpPawns.Distinct().ToList();
                    PlayerPawnsDisplayOrderUtility.Sort(tmpPawns);
                    for (int l = 0; l < tmpPawns.Count; l++)
                    {
                        if (TacticUtils.TacticalGroups.colonyGroups.TryGetValue(tmpMaps[i], out ColonyGroup colonyGroup))
                        {
                            cachedEntries.Add(new Entry(tmpPawns[l], tmpMaps[i], num, null, colonyGroup));
                        }
                        else
                        {
                            cachedEntries.Add(new Entry(tmpPawns[l], tmpMaps[i], num, null, null));
                        }
                    }
                    if (!tmpPawns.Any())
                    {
                        ColonyGroup colonyGroup = TacticUtils.AllColonyGroups.Where(x => x.Map == tmpMaps[i]).FirstOrDefault();
                        cachedEntries.Add(new Entry(null, tmpMaps[i], num, null, colonyGroup));
                    }
                    if (oldCount != cachedEntries.Count)
                    {
                        num++;
                    }
                }

                tmpCaravans.Clear();
                tmpCaravans.AddRange(Find.WorldObjects.Caravans);
                tmpCaravans.SortBy((Caravan x) => x.ID);
                for (int m = 0; m < tmpCaravans.Count; m++)
                {
                    var oldCount = cachedEntries.Count;
                    if (!tmpCaravans[m].IsPlayerControlled)
                    {
                        continue;
                    }
                    tmpPawns.Clear();
                    tmpPawns.AddRange(tmpCaravans[m].PawnsListForReading);
                    tmpPawns = GetNonHiddenPawns(tmpPawns);
                    PlayerPawnsDisplayOrderUtility.Sort(tmpPawns);
                    for (int n = 0; n < tmpPawns.Count; n++)
                    {
                        if (tmpPawns[n].IsColonist)
                        {
                            if (TacticUtils.TacticalGroups.caravanGroups.TryGetValue(tmpCaravans[m], out CaravanGroup value))
                            {
                                cachedEntries.Add(new Entry(tmpPawns[n], null, num, value, null));
                            }
                            else
                            {
                                CaravanGroup caravanGroup = TacticUtils.TacticalGroups.AddCaravanGroup(tmpCaravans[m]);
                                cachedEntries.Add(new Entry(tmpPawns[n], null, num, caravanGroup, null));
                            }
                        }
                    }

                    if (!tmpPawns.Any())
                    {
                        KeyValuePair<Caravan, CaravanGroup> caravanGroup = TacticUtils.TacticalGroups.caravanGroups.Where(x => x.Key == tmpCaravans[m]).FirstOrDefault();
                        if (caravanGroup.Value != null)
                        {
                            cachedEntries.Add(new Entry(null, null, num, caravanGroup.Value, null));
                        }
                    }
                    if (oldCount != cachedEntries.Count)
                    {
                        num++;
                    }
                }
            }

            cachedReorderableGroups.Clear();
            foreach (Entry cachedEntry in cachedEntries)
            {
                _ = cachedEntry;
                cachedReorderableGroups.Add(-1);
            }
            drawer.Notify_RecachedEntries();
            tmpPawns.Clear();
            tmpMaps.Clear();
            tmpCaravans.Clear();
            GenMapUIOptimized.ForceUpdateLabels();
            TacticUtils.ForceUpdateGroups();
            drawLocsFinder.CalculateDrawLocs(DrawLocs, out cachedScale);
        }
        public float GetEntryRectAlpha(Rect rect)
        {
            return Messages.CollidesWithAnyMessage(rect, out float messageAlpha) ? Mathf.Lerp(1f, 0.2f, messageAlpha) : 1f;
        }

        public void Highlight(Pawn pawn)
        {
            if (Visible && !colonistsToHighlight.Contains(pawn))
            {
                colonistsToHighlight.Add(pawn);
            }
        }

        public void TryDropColonist(Pawn pawn)
        {
            ColonistGroup colonistGroup = TacticUtils.AllGroups.Where(x => x.curRect.Contains(Event.current.mousePosition)).FirstOrDefault();
            if (colonistGroup != null)
            {
                colonistGroup.Add(pawn);
                MarkColonistsDirty();
                MainTabWindowUtility.NotifyAllPawnTables_PawnsChanged();
            }
        }
        public void Reorder(int from, int to, int entryGroup)
        {
            int num = 0;
            Pawn pawn = null;
            Pawn pawn2 = null;
            Pawn pawn3 = null;
            for (int i = 0; i < cachedEntries.Count; i++)
            {
                if (cachedEntries[i].group == entryGroup && cachedEntries[i].pawn != null)
                {
                    if (num == from)
                    {
                        pawn = cachedEntries[i].pawn;
                    }
                    if (num == to)
                    {
                        pawn2 = cachedEntries[i].pawn;
                    }
                    pawn3 = cachedEntries[i].pawn;
                    num++;
                }
            }
            if (pawn == null)
            {
                return;
            }
            int num2 = pawn2?.playerSettings.displayOrder ?? (pawn3.playerSettings.displayOrder + 1);
            for (int j = 0; j < cachedEntries.Count; j++)
            {
                Pawn pawn4 = cachedEntries[j].pawn;
                if (pawn4 == null)
                {
                    continue;
                }
                if (pawn4.playerSettings.displayOrder == num2)
                {
                    if (pawn2 != null && cachedEntries[j].group == entryGroup)
                    {
                        if (pawn4.thingIDNumber < pawn2.thingIDNumber)
                        {
                            pawn4.playerSettings.displayOrder--;
                        }
                        else
                        {
                            pawn4.playerSettings.displayOrder++;
                        }
                    }
                }
                else if (pawn4.playerSettings.displayOrder > num2)
                {
                    pawn4.playerSettings.displayOrder++;
                }
                else
                {
                    pawn4.playerSettings.displayOrder--;
                }
            }
            pawn.playerSettings.displayOrder = num2;
            MarkColonistsDirty();
            MainTabWindowUtility.NotifyAllPawnTables_PawnsChanged();
        }

        public void DrawColonistMouseAttachment(int index, Vector2 dragStartPos, int entryGroup)
        {
            Pawn pawn = null;
            Vector2 vector = default;
            int num = 0;
            for (int i = 0; i < cachedEntries.Count; i++)
            {
                if (cachedEntries[i].group == entryGroup && cachedEntries[i].pawn != null)
                {
                    if (num == index)
                    {
                        pawn = cachedEntries[i].pawn;
                        vector = new Vector2(DrawLocs[i].x, DrawLocs[i].y);
                        break;
                    }
                    num++;
                }
            }
            if (pawn != null)
            {
                RenderTexture iconTex = PortraitsCache.Get(pawn, TacticalGroups_ColonistBarColonistDrawer.PawnTextureSize, Rot4.South,
                    TacticalGroups_ColonistBarColonistDrawer.PawnTextureCameraOffset, 1.28205f);
                Rect rect = new Rect(vector.x, vector.y, Size.x, Size.y);
                Rect pawnTextureRect = drawer.GetPawnTextureRect(rect.position);
                pawnTextureRect.position += Event.current.mousePosition - dragStartPos;
                GenUI.DrawMouseAttachment(iconTex, "", 0f, default, pawnTextureRect);
            }
        }

        public bool AnyColonistOrCorpseAt(Vector2 pos)
        {
            return TryGetEntryAt(pos, out Entry entry) && entry.pawn != null;
        }

        public bool TryGetEntryAt(Vector2 pos, out Entry entry)
        {
            List<Entry> entries = Entries;
            Vector2 size = Size;
            for (int i = 0; i < DrawLocs.Count; i++)
            {
                if (new Rect(DrawLocs[i].x, DrawLocs[i].y, size.x, size.y).Contains(pos))
                {
                    entry = entries[i];
                    return true;
                }
            }
            entry = default;
            return false;
        }

        public bool TryGetGroupPawnAt(Vector2 pos, out Pawn pawn)
        {
            foreach (ColonistGroup group in TacticUtils.AllGroups)
            {
                if (group.pawnWindowIsActive || group.showPawnIconsRightClickMenu)
                {
                    foreach (KeyValuePair<Pawn, Rect> pawnRect in group.pawnRects)
                    {
                        var adjustedRect = new Rect(pawnRect.Value.x, pawnRect.Value.y - group.scrollPosition.y, 
                            pawnRect.Value.width, pawnRect.Value.height);
                        if (adjustedRect.Contains(pos))
                        {
                            pawn = pawnRect.Key;
                            return true;
                        }
                    }
                }
            }
            pawn = null;
            return false;
        }

        public List<Pawn> GetColonistsInOrder()
        {
            List<Entry> entries = Entries;
            tmpColonistsInOrder.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].pawn != null)
                {
                    tmpColonistsInOrder.Add(entries[i].pawn);
                }
            }
            return tmpColonistsInOrder;
        }

        public List<Thing> ColonistsOrCorpsesInScreenRect(Rect rect)
        {
            List<Entry> entries = Entries;
            Vector2 size = Size;
            tmpColonistsWithMap.Clear();
            for (int i = 0; i < DrawLocs.Count; i++)
            {
                if (rect.Overlaps(new Rect(DrawLocs[i].x, DrawLocs[i].y, size.x, size.y)))
                {
                    Pawn pawn = entries[i].pawn;
                    if (pawn != null)
                    {
                        Thing first = (Thing)((!pawn.Dead || pawn.Corpse == null || !pawn.Corpse.SpawnedOrAnyParentSpawned) ? pawn : ((object)pawn.Corpse));
                        tmpColonistsWithMap.Add(new Pair<Thing, Map>(first, entries[i].map));
                    }
                }
            }
            if (WorldRendererUtility.WorldSelected && tmpColonistsWithMap.Any((Pair<Thing, Map> x) => x.Second == null))
            {
                tmpColonistsWithMap.RemoveAll((Pair<Thing, Map> x) => x.Second != null);
            }
            else if (tmpColonistsWithMap.Any((Pair<Thing, Map> x) => x.Second == Find.CurrentMap))
            {
                tmpColonistsWithMap.RemoveAll((Pair<Thing, Map> x) => x.Second != Find.CurrentMap);
            }
            tmpColonists.Clear();
            for (int j = 0; j < tmpColonistsWithMap.Count; j++)
            {
                tmpColonists.Add(tmpColonistsWithMap[j].First);
            }
            tmpColonistsWithMap.Clear();

            foreach (ColonistGroup group in TacticUtils.AllGroups)
            {
                if (group.pawnWindowIsActive || group.showPawnIconsRightClickMenu)
                {
                    foreach (KeyValuePair<Pawn, Rect> pawnRect in group.pawnRects)
                    {
                        if (rect.Overlaps(pawnRect.Value))
                        {
                            tmpColonists.Add(pawnRect.Key);
                        }
                    }
                }
            }
            return tmpColonists;
        }

        public List<Thing> MapColonistsOrCorpsesInScreenRect(Rect rect)
        {
            tmpMapColonistsOrCorpsesInScreenRect.Clear();
            if (!Visible)
            {
                return tmpMapColonistsOrCorpsesInScreenRect;
            }
            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Spawned)
                {
                    tmpMapColonistsOrCorpsesInScreenRect.Add(list[i]);
                }
            }
            return tmpMapColonistsOrCorpsesInScreenRect;
        }

        public List<Pawn> CaravanMembersInScreenRect(Rect rect)
        {
            tmpCaravanPawns.Clear();
            if (!Visible)
            {
                return tmpCaravanPawns;
            }
            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is Pawn pawn && pawn.IsCaravanMember())
                {
                    tmpCaravanPawns.Add(pawn);
                }
            }

            return tmpCaravanPawns;
        }

        public List<Caravan> CaravanMembersCaravansInScreenRect(Rect rect)
        {
            tmpCaravans.Clear();
            if (!Visible)
            {
                return tmpCaravans;
            }
            List<Pawn> list = CaravanMembersInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                tmpCaravans.Add(list[i].GetCaravan());
            }
            return tmpCaravans;
        }

        public Caravan CaravanMemberCaravanAt(Vector2 at)
        {
            return !Visible ? null : ColonistOrCorpseAt(at) is Pawn pawn && pawn.IsCaravanMember() ? pawn.GetCaravan() : null;
        }

        public Thing ColonistOrCorpseAt(Vector2 pos)
        {
            if (!Visible)
            {
                return null;
            }

            if (!TryGetEntryAt(pos, out Entry entry))
            {
                return TryGetGroupPawnAt(pos, out Pawn groupPawn) ? groupPawn : (Thing)null;
            }
            Pawn pawn = entry.pawn;
            return pawn != null && pawn.Dead && pawn.Corpse != null && pawn.Corpse.SpawnedOrAnyParentSpawned ? pawn.Corpse : (Thing)pawn;
        }
    }
}
