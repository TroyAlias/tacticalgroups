using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
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

        public ColonistBarColonistDrawer drawer = new ColonistBarColonistDrawer();

        private ColonistBarDrawLocsFinder drawLocsFinder = new ColonistBarDrawLocsFinder();

        private List<Entry> cachedEntries = new List<Entry>();

        private List<Vector2> cachedDrawLocs = new List<Vector2>();
        public List<Vector2> DrawLocs => cachedDrawLocs;

        private float cachedScale = 1f;

        private bool entriesDirty = true;

        private List<Pawn> colonistsToHighlight = new List<Pawn>();

        public static readonly Texture2D BGTex = Command.BGTex;

        public static readonly Vector2 BaseSize = new Vector2(48f, 48f);

        public const float BaseSelectedTexJump = 20f;

        public const float BaseSelectedTexScale = 0.4f;

        public const float EntryInAnotherMapAlpha = 0.4f;

        public const float BaseSpaceBetweenGroups = 25f;

        public const float BaseSpaceBetweenColonistsHorizontal = 24f;

        public const float BaseSpaceBetweenColonistsVertical = 32f;

        public const float FactionIconSpacing = 2f;

        private static List<Pawn> tmpPawns = new List<Pawn>();

        private static List<Map> tmpMaps = new List<Map>();

        private static List<Caravan> tmpCaravans = new List<Caravan>();

        private static List<Pawn> tmpColonistsInOrder = new List<Pawn>();

        private static List<Pair<Thing, Map>> tmpColonistsWithMap = new List<Pair<Thing, Map>>();

        private static List<Thing> tmpColonists = new List<Thing>();

        private static List<Thing> tmpMapColonistsOrCorpsesInScreenRect = new List<Thing>();

        private static List<Pawn> tmpCaravanPawns = new List<Pawn>();

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

        private bool Visible
        {
            get
            {
                if (UI.screenWidth < 800 || UI.screenHeight < 500)
                {
                    return false;
                }
                return true;
            }
        }

        public void MarkColonistsDirty()
        {
            entriesDirty = true;
        }

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

                var createGroupRect = ColonistBarDrawLocsFinder.createGroupRect;
                if (Mouse.IsOver(createGroupRect))
                {
                    GUI.DrawTexture(createGroupRect, Textures.CreateGroupIconHover);
                }
                else
                {
                    GUI.DrawTexture(createGroupRect, Textures.CreateGroupIcon);
                }
                TooltipHandler.TipRegion(createGroupRect, Strings.CreateGroupTooltip);

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
                }
                else
                {
                    GUI.DrawTexture(optionsGearRect, Textures.OptionsGear);
                }
                TooltipHandler.TipRegion(optionsGearRect, Strings.OptionsGearTooltip);

                if (!WorldRendererUtility.WorldRenderedNow)
                {
                    for (int i = 0; i < ColonistBarDrawLocsFinder.pawnGroupDrawLoc.Count; i++)
                    {
                        var data = ColonistBarDrawLocsFinder.pawnGroupDrawLoc.ElementAt(i);
                        var pawnGroupIconRect = new Rect(data.Value.x, data.Value.y, data.Key.groupIcon.width, data.Key.groupIcon.height);
                        data.Key.Draw(pawnGroupIconRect);
                    }
                }

                for (int i = 0; i < ColonistBarDrawLocsFinder.colonyGroupDrawLoc.Count; i++)
                {
                    var data = ColonistBarDrawLocsFinder.colonyGroupDrawLoc.ElementAt(i);
                    var colonyGroupIconRect = new Rect(data.Value.x, data.Value.y, data.Key.groupIcon.width, data.Key.groupIcon.height);
                    data.Key.Draw(colonyGroupIconRect);
                }
                for (int i = 0; i < ColonistBarDrawLocsFinder.caravanGroupDrawLoc.Count; i++)
                {
                    var data = ColonistBarDrawLocsFinder.caravanGroupDrawLoc.ElementAt(i);
                    var caravanIconRect = new Rect(data.Value.x, data.Value.y, data.Key.groupIcon.width, data.Key.groupIcon.height);
                    data.Key.Draw(caravanIconRect);
                }

                for (int i = 0; i < cachedDrawLocs.Count; i++)
                {
                    Rect rect = new Rect(cachedDrawLocs[i].x, cachedDrawLocs[i].y, Size.x, Size.y);
                    Entry entry = entries[i];
                    bool flag = num != entry.group;
                    num = entry.group;
                    if (flag)
                    {
                        reorderableGroup = ReorderableWidget.NewGroup(entry.reorderAction, ReorderableDirection.Horizontal, SpaceBetweenColonistsHorizontal, entry.extraDraggedItemOnGUI);
                    }
                    bool reordering;
                    if (entry.pawn != null)
                    {
                        drawer.HandleClicks(rect, entry.pawn, reorderableGroup, out reordering);
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
                        drawer.DrawColonist(rect, entry.pawn, entry.map, colonistsToHighlight.Contains(entry.pawn), reordering);
                        Faction faction = null;
                        if (entry.pawn.HasExtraMiniFaction())
                        {
                            faction = entry.pawn.GetExtraMiniFaction();
                        }
                        else if (entry.pawn.HasExtraHomeFaction())
                        {
                            faction = entry.pawn.GetExtraHomeFaction();
                        }
                        if (faction != null)
                        {
                            GUI.color = faction.Color;
                            float num2 = rect.width * 0.5f;
                            GUI.DrawTexture(new Rect(rect.xMax - num2 - 2f, rect.yMax - num2 - 2f, num2, num2), faction.def.FactionIcon);
                            GUI.color = Color.white;
                        }
                    }
                }
                num = -1;
                if (showGroupFrames)
                {
                    for (int j = 0; j < cachedDrawLocs.Count; j++)
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

            if (Find.TickManager.TicksGame % 1000 == 0)
            {
                foreach (var group in TacticUtils.AllGroups)
                {
                    group.Sort();
                }
            }
        }

        public void HandleGroupingClicks(Rect rect)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Mouse.IsOver(rect))
            {
                var selectedPawns = Find.Selector.SelectedPawns.Where(x => x.Faction == Faction.OfPlayer).ToList();
                if (selectedPawns.Any())
                {
                    TacticUtils.TacticalGroups.AddGroup(selectedPawns);
                    MarkColonistsDirty();
                    CheckRecacheEntries();
                }
                Event.current.Use();
            }
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
                    tmpPawns.Clear();
                    tmpPawns.AddRange(tmpMaps[i].mapPawns.FreeColonists);
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
                    List<Pawn> allPawnsSpawned = tmpMaps[i].mapPawns.AllPawnsSpawned;
                    for (int k = 0; k < allPawnsSpawned.Count; k++)
                    {
                        Corpse corpse = allPawnsSpawned[k].carryTracker.CarriedThing as Corpse;
                        if (corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist)
                        {
                            tmpPawns.Add(corpse.InnerPawn);
                        }
                    }
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
                        cachedEntries.Add(new Entry(null, tmpMaps[i], num, null, null));
                    }
                    num++;
                }
                tmpCaravans.Clear();
                tmpCaravans.AddRange(Find.WorldObjects.Caravans);
                tmpCaravans.SortBy((Caravan x) => x.ID);
                for (int m = 0; m < tmpCaravans.Count; m++)
                {
                    if (!tmpCaravans[m].IsPlayerControlled)
                    {
                        continue;
                    }
                    tmpPawns.Clear();
                    tmpPawns.AddRange(tmpCaravans[m].PawnsListForReading);
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
                                Log.Error("Found pawn with caravan and without Caravan group. This should never happen.");
                                cachedEntries.Add(new Entry(tmpPawns[n], null, num, null, null));
                            }
                        }
                    }
                    num++;
                }
            }

            if (!TacticalGroupsSettings.HideGroups)
            {
                foreach (var pawnGroup in TacticUtils.AllPawnGroups)
                {
                    for (int i = cachedEntries.Count - 1; i >= 0; i--)
                    {
                        if (pawnGroup.pawns.Contains(cachedEntries[i].pawn) && (pawnGroup.pawnIcons?.ContainsKey(cachedEntries[i].pawn) ?? false))
                        {
                            if ((!pawnGroup.entireGroupIsVisible && !pawnGroup.pawnIcons[cachedEntries[i].pawn].isVisibleOnColonistBar) && cachedEntries[i].pawn.Map != null)
                            {
                                var group = cachedEntries[i].group;
                                cachedEntries.RemoveAt(i);
                                if (!cachedEntries.Where(x => x.group == group).Any())
                                {
                                    var colonyGroup = TacticUtils.AllColonyGroups.Where(x => x.Map == pawnGroup.Map).FirstOrDefault();
                                    cachedEntries.Add(new Entry(null, pawnGroup.Map, group, null, colonyGroup));
                                }
                            }
                        }
                    }
                }

                foreach (var colonyGroup in TacticUtils.AllColonyGroups)
                {
                    for (int i = cachedEntries.Count - 1; i >= 0; i--)
                    {
                        if (colonyGroup.pawns.Contains(cachedEntries[i].pawn) && (colonyGroup.pawnIcons?.ContainsKey(cachedEntries[i].pawn) ?? false))
                        {
                            if (!colonyGroup.entireGroupIsVisible && !colonyGroup.pawnIcons[cachedEntries[i].pawn].isVisibleOnColonistBar)
                            {
                                var group = cachedEntries[i].group;
                                cachedEntries.RemoveAt(i);
                                if (!cachedEntries.Where(x => x.group == group).Any())
                                {
                                    cachedEntries.Add(new Entry(null, colonyGroup.Map, group, null, colonyGroup));
                                }
                            }
                        }
                    }
                }

                foreach (var caravanGroup in TacticUtils.AllCaravanGroups)
                {
                    for (int i = cachedEntries.Count - 1; i >= 0; i--)
                    {
                        if (caravanGroup.pawns.Contains(cachedEntries[i].pawn) && (caravanGroup.pawnIcons?.ContainsKey(cachedEntries[i].pawn) ?? false))
                        {
                            if (!caravanGroup.entireGroupIsVisible && !caravanGroup.pawnIcons[cachedEntries[i].pawn].isVisibleOnColonistBar)
                            {
                                var group = cachedEntries[i].group;
                                cachedEntries.RemoveAt(i);
                                if (!cachedEntries.Where(x => x.group == group).Any())
                                {
                                    cachedEntries.Add(new Entry(null, null, group, caravanGroup, null));
                                }
                            }
                        }
                    }
                }
            }

            cachedEntries.SortBy(x => x.group);
            drawer.Notify_RecachedEntries();
            tmpPawns.Clear();
            tmpMaps.Clear();
            tmpCaravans.Clear();
            drawLocsFinder.CalculateDrawLocs(cachedDrawLocs, out cachedScale);
        }
        public float GetEntryRectAlpha(Rect rect)
        {
            if (Messages.CollidesWithAnyMessage(rect, out float messageAlpha))
            {
                return Mathf.Lerp(1f, 0.2f, messageAlpha);
            }
            return 1f;
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
            var colonistGroup = TacticUtils.AllGroups.Where(x => x.curRect.Contains(Event.current.mousePosition)).FirstOrDefault();
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
            Vector2 vector = default(Vector2);
            int num = 0;
            for (int i = 0; i < cachedEntries.Count; i++)
            {
                if (cachedEntries[i].group == entryGroup && cachedEntries[i].pawn != null)
                {
                    if (num == index)
                    {
                        pawn = cachedEntries[i].pawn;
                        vector = cachedDrawLocs[i];
                        break;
                    }
                    num++;
                }
            }
            if (pawn != null)
            {
                RenderTexture iconTex = PortraitsCache.Get(pawn, ColonistBarColonistDrawer.PawnTextureSize, ColonistBarColonistDrawer.PawnTextureCameraOffset, 1.28205f);
                Rect rect = new Rect(vector.x, vector.y, Size.x, Size.y);
                Rect pawnTextureRect = drawer.GetPawnTextureRect(rect.position);
                pawnTextureRect.position += Event.current.mousePosition - dragStartPos;
                GenUI.DrawMouseAttachment(iconTex, "", 0f, default(Vector2), pawnTextureRect);
            }
        }

        public bool AnyColonistOrCorpseAt(Vector2 pos)
        {
            Log.Message("AnyColonistOrCorpseAt");
            if (!TryGetEntryAt(pos, out Entry entry))
            {
                return false;
            }
            return entry.pawn != null;
        }

        public bool TryGetEntryAt(Vector2 pos, out Entry entry)
        {
            Log.Message("TryGetEntryAt");
            List<Entry> entries = Entries;
            Vector2 size = Size;
            for (int i = 0; i < cachedDrawLocs.Count; i++)
            {
                if (new Rect(cachedDrawLocs[i].x, cachedDrawLocs[i].y, size.x, size.y).Contains(pos))
                {
                    entry = entries[i];
                    return true;
                }
            }
            entry = default(Entry);
            return false;
        }

        public bool TryGetGroupPawnAt(Vector2 pos, out Pawn pawn)
        {
            Log.Message("TryGetGroupPawnAt");
            foreach (var group in TacticUtils.AllGroups)
            {
                if (group.Visible)
                {
                    foreach (var pawnRect in group.pawnRects)
                    {
                        if (pawnRect.Value.Contains(pos))
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
            for (int i = 0; i < cachedDrawLocs.Count; i++)
            {
                if (rect.Overlaps(new Rect(cachedDrawLocs[i].x, cachedDrawLocs[i].y, size.x, size.y)))
                {
                    Pawn pawn = entries[i].pawn;
                    if (pawn != null)
                    {
                        Thing first = (Thing)((!pawn.Dead || pawn.Corpse == null || !pawn.Corpse.SpawnedOrAnyParentSpawned) ? ((object)pawn) : ((object)pawn.Corpse));
                        tmpColonistsWithMap.Add(new Pair<Thing, Map>(first, entries[i].map));
                    }
                }
            }
            if (WorldRendererUtility.WorldRenderedNow && tmpColonistsWithMap.Any((Pair<Thing, Map> x) => x.Second == null))
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

            foreach (var group in TacticUtils.AllGroups)
            {
                if (group.Visible)
                {
                    foreach (var pawnRect in group.pawnRects)
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
            Log.Message("MapColonistsOrCorpsesInScreenRect");
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
            Log.Message("CaravanMembersInScreenRect");
            tmpCaravanPawns.Clear();
            if (!Visible)
            {
                return tmpCaravanPawns;
            }
            List<Thing> list = ColonistsOrCorpsesInScreenRect(rect);
            for (int i = 0; i < list.Count; i++)
            {
                Pawn pawn = list[i] as Pawn;
                if (pawn != null && pawn.IsCaravanMember())
                {
                    tmpCaravanPawns.Add(pawn);
                }
            }

            return tmpCaravanPawns;
        }

        public List<Caravan> CaravanMembersCaravansInScreenRect(Rect rect)
        {
            Log.Message("CaravanMembersCaravansInScreenRect");
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
            Log.Message("CaravanMemberCaravanAt");
            if (!Visible)
            {
                return null;
            }
            Pawn pawn = ColonistOrCorpseAt(at) as Pawn;
            if (pawn != null && pawn.IsCaravanMember())
            {
                return pawn.GetCaravan();
            }
            return null;
        }

        public Thing ColonistOrCorpseAt(Vector2 pos)
        {
            Log.Message("ColonistOrCorpseAt");
            if (!Visible)
            {
                return null;
            }
            if (!TryGetEntryAt(pos, out Entry entry))
            {
                if (TryGetGroupPawnAt(pos, out Pawn groupPawn))
                {
                    return groupPawn;
                }
                return null;
            }
            Pawn pawn = entry.pawn;
            if (pawn != null && pawn.Dead && pawn.Corpse != null && pawn.Corpse.SpawnedOrAnyParentSpawned)
            {
                return pawn.Corpse;
            }
            return pawn;
        }
    }
}
