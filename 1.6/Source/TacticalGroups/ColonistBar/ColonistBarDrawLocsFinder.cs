using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class MappedValue
    {
		public MappedValue(ColonistGroup colonistGroup, Rect rect)
        {
			this.colonistGroup = colonistGroup;
			this.rect = rect;
        }
		public ColonistGroup colonistGroup;
		public Rect rect;
	}

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }

    [HotSwappable]
	public class ColonistBarDrawLocsFinder
	{
		private List<int> entriesInGroup = new List<int>();

		private List<int> horizontalSlotsPerGroup = new List<int>();
		private static float MaxColonistBarWidth => ((float)UI.screenWidth - 520f) + TacticalGroupsSettings.ColonistBarWidthOffset;

		public static List<MappedValue> caravanGroupDrawLoc = new List<MappedValue>();
		public static List<MappedValue> colonyGroupDrawLoc = new List<MappedValue>();
		public static List<MappedValue> pawnGroupDrawLoc = new List<MappedValue>();

		public static Rect createGroupRect;
		public void CalculateDrawLocs(List<Rect> outDrawLocs, out float scale)
		{
			caravanGroupDrawLoc.Clear();
			colonyGroupDrawLoc.Clear();
			pawnGroupDrawLoc.Clear();
			if (TacticUtils.TacticalColonistBar.Entries.Count == 0)
			{
				outDrawLocs.Clear();
				scale = 1f;
			}
			else
			{
				CalculateColonistsInGroup();
				scale = FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow);
				CalculateDrawLocs(outDrawLocs, scale, onlyOneRow, maxPerGlobalRow);
			}
		}

		private void CalculateColonistsInGroup()
		{
			entriesInGroup.Clear();
			List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			int num = CalculateGroupsCount();
			//Log.Message("CalculatedGroupsCount: " + num + " - entries: " + entries.Count + " - " + string.Join(", ", entries.Select(x => x.group)));
			for (int i = 0; i < num; i++)
			{
				entriesInGroup.Add(0);
			}
			for (int j = 0; j < entries.Count; j++)
			{
				entriesInGroup[entries[j].group]++;
			}
        }

        private int CalculateGroupsCount()
		{
			List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < entries.Count; i++)
			{
				if (num != entries[i].group)
				{
					num2++;
					num = entries[i].group;
				}
			}
			return num2;
		}

		private float FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow)
		{
            float scale = 1f;
			List <TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			var colonistCount = entries.Count(x => x.pawn != null);
			int groupsCount = CalculateGroupsCount();
			float scaleMultiplier = 1f;
			var majorGroupsCount = 0;
			if (!TacticalGroupsSettings.HideGroups)
			{
				var cgGroups = 0;
				cgGroups = TacticUtils.AllColonyGroups.Count;
				cgGroups += TacticUtils.AllCaravanGroups.Count;
				majorGroupsCount = cgGroups;
				if (!WorldRendererUtility.WorldSelected && cgGroups > 0)
				{
					var activeColony = TacticUtils.AllColonyGroups.FirstOrDefault(x => x.Map == Find.CurrentMap);
					if (activeColony != null)
					{

						cgGroups += TacticUtils.GetAllPawnGroupFor(activeColony).Take(TacticalGroupsSettings.GroupRowCount).Count();
						cgGroups += TacticUtils.GetAllSubGroupFor(activeColony).Take(TacticalGroupsSettings.SubGroupRowCount).Count();
					}
				}
				scaleMultiplier += (float)cgGroups / 10f;
			}
			float minX = ((TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacingX) * scale);
			float maxX = (MaxColonistBarWidth - (float)(groupsCount - 1) * 25f * scale) / scaleMultiplier;
			var initialValue = Mathf.FloorToInt(maxX / minX);
			while (true)
			{
				minX = ((TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacingX) * scale);
				maxX = (MaxColonistBarWidth - (float)(groupsCount - 1) * 25f * scale) / scaleMultiplier;
				maxPerGlobalRow = Mathf.Max(1, Mathf.FloorToInt(maxX / minX));
                var log = new List<string>();
                log.Add("initialValue: " + initialValue);
                log.Add("maxPerGlobalRow: " + maxPerGlobalRow);
                log.Add("minX: " + minX);
                log.Add("maxX: " + maxX);
                //Log.Message("FindBestScale: " + string.Join(", ", log));
				Log.ResetMessageCount();
                onlyOneRow = true;
				if (TryDistributeHorizontalSlotsBetweenGroups(maxPerGlobalRow))
				{
					int allowedRowsCountForScale = GetAllowedRowsCountForScale(scale);
					bool flag = true;
					int group = -1;
					for (int i = 0; i < entries.Count; i++)
					{
						if (group != entries[i].group)
						{
							group = entries[i].group;
							int rowCount = Mathf.CeilToInt((float)entriesInGroup[entries[i].group] / (float)GetHorizontalSlotsPerGroup(entries[i].group));
							if (rowCount > 1)
							{
								onlyOneRow = false;
							}
							if (rowCount > allowedRowsCountForScale)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						break;
					}
				}
				scale *= 0.95f;
			}
			return scale;
		}

		private bool TryDistributeHorizontalSlotsBetweenGroups(int maxPerGlobalRow)
		{
			int num = CalculateGroupsCount();
			horizontalSlotsPerGroup.Clear();
			for (int j = 0; j < num; j++)
			{
				horizontalSlotsPerGroup.Add(0);
			}
			GenMath.DHondtDistribution(horizontalSlotsPerGroup, (int i) => entriesInGroup[i], maxPerGlobalRow);
			for (int k = 0; k < horizontalSlotsPerGroup.Count; k++)
			{
				if (horizontalSlotsPerGroup[k] == 0)
				{
					int num2 = horizontalSlotsPerGroup.Max();
					if (TacticalGroupsSettings.OverridePawnRowCount && num2 > maxPerGlobalRow || !TacticalGroupsSettings.OverridePawnRowCount && num2 <= 1)
					{
						return false;
					}
					int index = horizontalSlotsPerGroup.IndexOf(num2);
					horizontalSlotsPerGroup[index]--;
					horizontalSlotsPerGroup[k]++;
				}
			}
			return true;
		}

		private static int GetAllowedRowsCountForScale(float scale)
		{
			if (TacticalGroupsSettings.OverridePawnRowCount)
			{
				return TacticalGroupsSettings.PawnRowCount;
            }
			if (scale > 0.58f)
			{
				return 1;
			}
			if (scale > 0.42f)
			{
				return 2;
			}
			return 3;
		}

		public int GetHorizontalSlotsPerGroup(int group)
		{
			if (TacticalGroupsSettings.OverridePawnRowCount)
			{
				List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
				var pawnCount = entries.Count(x => x.group == group && x.pawn != null);
				if ((float)pawnCount / (float)((horizontalSlotsPerGroup[group])) > GetPawnRowCount(pawnCount))
				{
					return horizontalSlotsPerGroup[group] + 1;
				}
			}
			return horizontalSlotsPerGroup[group];
		}

		public int GetPawnRowCount(int pawnCount)
        {
			return Mathf.Max(1, Mathf.Min(TacticalGroupsSettings.PawnRowCount, pawnCount));
		}

		private void CalculateDrawLocs(List<Rect> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow)
		{
			outDrawLocs.Clear();
			int maxPawnsPerRow = maxPerGlobalRow;
			if (onlyOneRow)
			{
				for (int i = 0; i < horizontalSlotsPerGroup.Count; i++)
				{
					horizontalSlotsPerGroup[i] = Mathf.Min(horizontalSlotsPerGroup[i], entriesInGroup[i]);
				}
				maxPawnsPerRow = TacticUtils.TacticalColonistBar.Entries.Count;
			}
			int groupCount = CalculateGroupsCount();
			float colonistScale = (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacingX) * scale;
			float colonistBarWidth = ((float)maxPawnsPerRow * colonistScale + (float)(groupCount - 1) * 25f * scale);
			var log = new List<string>();
            log.Add("maxPawnsPerRow: " + maxPawnsPerRow);
            log.Add("cgGroups: " + groupCount);
            log.Add("colonistBarWidth: " + colonistBarWidth);
            log.Add("colonistScale: " + colonistScale);
			//Log.Message("CalculateDrawLocs: " + string.Join(", ", log));

            if (!TacticalGroupsSettings.HideGroups)
            {
				if (!WorldRendererUtility.WorldSelected)
				{
					var activeColony = TacticUtils.AllColonyGroups.FirstOrDefault(x => x.Map == Find.CurrentMap);
					if (activeColony != null)
					{
						colonistBarWidth += TacticUtils.GetAllPawnGroupFor(activeColony).Take(TacticalGroupsSettings.GroupRowCount).Sum(x => x.GroupIconWidth + x.GroupIconMargin);
						colonistBarWidth += TacticUtils.GetAllSubGroupFor(activeColony).Take(TacticalGroupsSettings.SubGroupRowCount).Sum(x => x.GroupIconWidth + x.GroupIconMargin);
					}
				}
			}

			List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			int group = -1;
			int numInGroup = -1;
			float groupStartX = ((float)UI.screenWidth - colonistBarWidth) / 2f;
			groupStartX += TacticalGroupsSettings.ColonistBarPositionX;
			bool createGroupAssigned = false;

			for (int j = 0; j < entries.Count; j++)
			{
				if (group != entries[j].group)
				{
					if (group >= 0)
					{
						groupStartX += 25f * scale;
						groupStartX += ((float)GetHorizontalSlotsPerGroup(group)) * scale * (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacingX);
					}
					if (!TacticalGroupsSettings.HideGroups)
                    {
						if (entries[j].caravanGroup != null)
						{
							caravanGroupDrawLoc.Add(new MappedValue(entries[j].caravanGroup, new Rect(groupStartX - (25f * scale), TacticalGroupsSettings.ColonistBarPositionY,
								entries[j].caravanGroup.GroupIconWidth, entries[j].caravanGroup.GroupIconHeight)));
							groupStartX += entries[j].caravanGroup.GroupIconWidth;
						}
						else if (entries[j].colonyGroup != null)
						{
							if (entries[j].colonyGroup.Map == Find.CurrentMap)
							{
								if (!WorldRendererUtility.WorldSelected)
								{
									var list = TacticUtils.GetAllSubGroupFor(entries[j].colonyGroup);
									if (list.Any())
									{
										list.Reverse();
										var initPos = groupStartX;
										var xPos = groupStartX;
										var yPos = TacticalGroupsSettings.ColonistBarPositionY;
										for (var groupID = 0; groupID < list.Count(); groupID++)
										{
											if (groupID > 0 && groupID % TacticalGroupsSettings.SubGroupRowCount == 0)
											{
												xPos = initPos;
												yPos += list[groupID].GroupIconHeight + 7;
											}
											pawnGroupDrawLoc.Add(new MappedValue(list[groupID], new Rect(xPos, yPos, list[groupID].GroupIconWidth, list[groupID].GroupIconHeight)));
											xPos += list[groupID].GroupIconWidth + 5;
										}
										list = list.Take(TacticalGroupsSettings.SubGroupRowCount).ToList();
										groupStartX += list.Sum(x => x.GroupIconWidth + 5);
									}
								}
							}

							colonyGroupDrawLoc.Add(new MappedValue(entries[j].colonyGroup, new Rect(groupStartX, TacticalGroupsSettings.ColonistBarPositionY, 
								entries[j].colonyGroup.GroupIconWidth, entries[j].colonyGroup.GroupIconHeight)));

							groupStartX += entries[j].colonyGroup.GroupIconWidth + entries[j].colonyGroup.GroupIconMargin;
							if (entries[j].colonyGroup.Map == Find.CurrentMap)
							{
								if (!WorldRendererUtility.WorldSelected)
								{
									var list = TacticUtils.GetAllPawnGroupFor(entries[j].colonyGroup);
									if (list.Any())
                                    {
										list.Reverse();
										var initPos = groupStartX;
										var xPos = groupStartX;
										var yPos = TacticalGroupsSettings.ColonistBarPositionY;
										for (var groupID = 0; groupID < list.Count(); groupID++)
										{
											if (groupID > 0 && groupID % TacticalGroupsSettings.GroupRowCount == 0)
											{
												xPos = initPos;
												yPos += list[groupID].GroupIconHeight + 25;
											}
											pawnGroupDrawLoc.Add(new MappedValue(list[groupID], new Rect(xPos, yPos, list[groupID].GroupIconWidth, list[groupID].GroupIconHeight)));
											xPos += list[groupID].GroupIconWidth + list[groupID].GroupIconMargin;
										}
										list = list.Take(TacticalGroupsSettings.GroupRowCount).ToList();
										groupStartX += list.Sum(x => x.GroupIconWidth + x.GroupIconMargin);
									}
								}
							}
						}
					}
					if (entries[j].colonyGroup != null)
                    {
						if (entries[j].colonyGroup?.Map == Find.CurrentMap)
						{
							createGroupRect = new Rect(groupStartX, TacticalGroupsSettings.ColonistBarPositionY, Textures.CreateGroupIcon.width, Textures.CreateGroupIcon.height);
							groupStartX += Textures.CreateGroupIcon.width + 20f;
							createGroupAssigned = true;
						}
					}
					else if (!createGroupAssigned)
                    {
						createGroupRect = new Rect(groupStartX, TacticalGroupsSettings.ColonistBarPositionY, Textures.CreateGroupIcon.width, Textures.CreateGroupIcon.height);
						groupStartX += Textures.CreateGroupIcon.width + 20f;
					}
					numInGroup = 0;
					group = entries[j].group;
				}
				else
				{
					numInGroup++;
				}
				Vector2 drawLoc = GetDrawLoc(groupStartX, TacticalGroupsSettings.ColonistBarPositionY, entries[j].group, numInGroup, scale);
				outDrawLocs.Add(new Rect(drawLoc.x, drawLoc.y, TacticUtils.TacticalColonistBar.Size.x, TacticUtils.TacticalColonistBar.Size.y));
			}
		}

		private Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int numInGroup, float scale)
		{
			var horisontalSlotsPerGroup = Mathf.Max(horizontalSlotsPerGroup[group], 1);
			List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			var pawnCount = entries.Count(x => x.group == group && x.pawn != null);
			if (TacticalGroupsSettings.OverridePawnRowCount && (float)pawnCount / (float)(horisontalSlotsPerGroup) > GetPawnRowCount(pawnCount))
			{
				float num = groupStartX + (float)((numInGroup) % (horisontalSlotsPerGroup + 1)) * scale * (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacingX);
				float y = groupStartY + (float)((numInGroup) / (horisontalSlotsPerGroup + 1)) * scale * (TacticalColonistBar.BaseSize.y + TacticalGroupsSettings.ColonistBarSpacingY);
				return new Vector2(num, y);
			}
			else
			{
				float num = groupStartX + (float)(numInGroup % horisontalSlotsPerGroup) * scale * (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacingX);
				float y = groupStartY + (float)(numInGroup / horisontalSlotsPerGroup) * scale * (TacticalColonistBar.BaseSize.y + TacticalGroupsSettings.ColonistBarSpacingY);
				if (numInGroup >= entriesInGroup[group] - entriesInGroup[group] % horisontalSlotsPerGroup)
				{
					int num2 = horisontalSlotsPerGroup - entriesInGroup[group] % horisontalSlotsPerGroup;
					num += (float)num2 * scale * (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacingX) * 0.5f;
				}
				return new Vector2(num, y);
			}
		}
	}
}
