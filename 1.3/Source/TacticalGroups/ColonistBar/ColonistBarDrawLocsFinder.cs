using RimWorld.Planet;
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
	public class ColonistBarDrawLocsFinder
	{
		private List<int> entriesInGroup = new List<int>();

		private List<int> horizontalSlotsPerGroup = new List<int>();
		private static float MaxColonistBarWidth => (float)UI.screenWidth - 520f;


		public static List<MappedValue> caravanGroupDrawLoc = new List<MappedValue>();
		public static List<MappedValue> colonyGroupDrawLoc = new List<MappedValue>();
		public static List<MappedValue> pawnGroupDrawLoc = new List<MappedValue>();

		//public static Dictionary<CaravanGroup, Rect> caravanGroupDrawLoc = new Dictionary<CaravanGroup, Rect>();
		//public static Dictionary<ColonyGroup, Rect> colonyGroupDrawLoc = new Dictionary<ColonyGroup, Rect>();
		//public static Dictionary<PawnGroup, Rect> pawnGroupDrawLoc = new Dictionary<PawnGroup, Rect>();

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
			float num = 1f;
			List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			int num2 = CalculateGroupsCount();
			while (true)
			{
				float scaleMultiplier = 1f;
				if (!TacticalGroupsSettings.HideGroups)
                {
					var count = TacticUtils.AllColonyGroups.Count;
					count += TacticUtils.AllCaravanGroups.Count;
					if (!WorldRendererUtility.WorldRenderedNow && count > 0)
					{
						var activeColony = TacticUtils.AllColonyGroups.Where(x => x.Map == Find.CurrentMap).FirstOrDefault();
						if (activeColony != null)
						{
							count += TacticUtils.GetAllPawnGroupFor(activeColony).Take(TacticalGroupsSettings.GroupRowCount).Count();
							count += TacticUtils.GetAllSubGroupFor(activeColony).Take(TacticalGroupsSettings.SubGroupRowCount).Count();
						}
					}
					scaleMultiplier += (float)count / 10f;
				}

				float num3 = ((TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacing) * num);
				float num4 = (MaxColonistBarWidth - (float)(num2 - 1) * 25f * num) / scaleMultiplier;
				maxPerGlobalRow = Mathf.FloorToInt(num4 / num3);
				onlyOneRow = true;
				if (TryDistributeHorizontalSlotsBetweenGroups(maxPerGlobalRow))
				{
					int allowedRowsCountForScale = GetAllowedRowsCountForScale(num);
					bool flag = true;
					int num5 = -1;
					for (int i = 0; i < entries.Count; i++)
					{
						if (num5 != entries[i].group)
						{
							num5 = entries[i].group;
							int num6 = Mathf.CeilToInt((float)entriesInGroup[entries[i].group] / (float)horizontalSlotsPerGroup[entries[i].group]);
							if (num6 > 1)
							{
								onlyOneRow = false;
							}
							if (num6 > allowedRowsCountForScale)
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
				num *= 0.95f;
			}
			return num;
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
					if (num2 <= 1)
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

		private void CalculateDrawLocs(List<Rect> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow)
		{
			outDrawLocs.Clear();
			int num = maxPerGlobalRow;
			if (onlyOneRow)
			{
				for (int i = 0; i < horizontalSlotsPerGroup.Count; i++)
				{
					horizontalSlotsPerGroup[i] = Mathf.Min(horizontalSlotsPerGroup[i], entriesInGroup[i]);
				}
				num = TacticUtils.TacticalColonistBar.Entries.Count;
			}
			int num2 = CalculateGroupsCount();
			float num3 = (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacing) * scale;
			float num4 = ((float)num * num3 + (float)(num2 - 1) * 25f * scale);
			if (!TacticalGroupsSettings.HideGroups)
            {
				var allColonyGroups = TacticUtils.AllColonyGroups;
				if (allColonyGroups != null)
                {
					num4 += allColonyGroups.Sum(x => x.GroupIconWidth + x.GroupIconMargin);
					if (TacticUtils.AllCaravanGroups != null)
                    {
						num4 += TacticUtils.AllCaravanGroups.Sum(x => x.GroupIconWidth + x.GroupIconMargin);
                    }

					if (!WorldRendererUtility.WorldRenderedNow)
					{
						var activeColony = allColonyGroups.Where(x => x.Map == Find.CurrentMap).FirstOrDefault();
						if (activeColony != null)
						{
							num4 += TacticUtils.GetAllPawnGroupFor(activeColony).Take(TacticalGroupsSettings.GroupRowCount).Sum(x => x.GroupIconWidth + x.GroupIconMargin);
							num4 += TacticUtils.GetAllSubGroupFor(activeColony).Take(TacticalGroupsSettings.SubGroupRowCount).Sum(x => x.GroupIconWidth + x.GroupIconMargin);
						}
					}
				}

			}

			List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			int num5 = -1;
			int num6 = -1;
			float num7 = ((float)UI.screenWidth - num4) / 2f;
			num7 += TacticalGroupsSettings.ColonistBarPositionX;
			bool createGroupAssigned = false;
			for (int j = 0; j < entries.Count; j++)
			{
				if (num5 != entries[j].group)
				{
					if (num5 >= 0)
					{
						num7 += 25f * scale;
						num7 += (float)horizontalSlotsPerGroup[num5] * scale * (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacing);
					}
					if (!TacticalGroupsSettings.HideGroups)
                    {
						if (entries[j].caravanGroup != null)
						{
							caravanGroupDrawLoc.Add(new MappedValue(entries[j].caravanGroup, new Rect(num7 - (25f * scale), TacticalGroupsSettings.ColonistBarPositionY,
								entries[j].caravanGroup.GroupIconWidth, entries[j].caravanGroup.GroupIconHeight)));
							num7 += entries[j].caravanGroup.GroupIconWidth;
						}
						else if (entries[j].colonyGroup != null)
						{
							if (entries[j].colonyGroup.Map == Find.CurrentMap)
							{
								if (!WorldRendererUtility.WorldRenderedNow)
								{
									var list = TacticUtils.GetAllSubGroupFor(entries[j].colonyGroup);
									if (list.Any())
									{
										list.Reverse();
										var initPos = num7;
										var xPos = num7;
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
										num7 += list.Sum(x => x.GroupIconWidth + 5);
									}
								}
							}
							colonyGroupDrawLoc.Add(new MappedValue(entries[j].colonyGroup, new Rect(num7, TacticalGroupsSettings.ColonistBarPositionY, 
								entries[j].colonyGroup.GroupIconWidth, entries[j].colonyGroup.GroupIconHeight)));

							num7 += entries[j].colonyGroup.GroupIconWidth + entries[j].colonyGroup.GroupIconMargin;
							if (entries[j].colonyGroup.Map == Find.CurrentMap)
							{
								if (!WorldRendererUtility.WorldRenderedNow)
								{
									var list = TacticUtils.GetAllPawnGroupFor(entries[j].colonyGroup);
									if (list.Any())
                                    {
										list.Reverse();
										var initPos = num7;
										var xPos = num7;
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
										num7 += list.Sum(x => x.GroupIconWidth + x.GroupIconMargin);
									}
								}
							}
						}
					}
					if (entries[j].colonyGroup != null)
                    {
						if (entries[j].colonyGroup?.Map == Find.CurrentMap)
						{
							createGroupRect = new Rect(num7, TacticalGroupsSettings.ColonistBarPositionY, Textures.CreateGroupIcon.width, Textures.CreateGroupIcon.height);
							num7 += Textures.CreateGroupIcon.width + 20f;
							createGroupAssigned = true;
						}
					}
					else if (!createGroupAssigned)
                    {
						createGroupRect = new Rect(num7, TacticalGroupsSettings.ColonistBarPositionY, Textures.CreateGroupIcon.width, Textures.CreateGroupIcon.height);
						num7 += Textures.CreateGroupIcon.width + 20f;
					}
					num6 = 0;
					num5 = entries[j].group;
				}
				else
				{
					num6++;
				}
				Vector2 drawLoc = GetDrawLoc(num7, TacticalGroupsSettings.ColonistBarPositionY, entries[j].group, num6, scale);
				outDrawLocs.Add(new Rect(drawLoc.x, drawLoc.y, TacticUtils.TacticalColonistBar.Size.x, TacticUtils.TacticalColonistBar.Size.y));
			}
		}



		private Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int numInGroup, float scale)
		{
			float num = groupStartX + (float)(numInGroup % horizontalSlotsPerGroup[group]) * scale * (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacing);
			float y = groupStartY + (float)(numInGroup / horizontalSlotsPerGroup[group]) * scale * (TacticalColonistBar.BaseSize.y + 32f);
			if (numInGroup >= entriesInGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group])
			{
				int num2 = horizontalSlotsPerGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group];
				num += (float)num2 * scale * (TacticalColonistBar.BaseSize.x + TacticalGroupsSettings.ColonistBarSpacing) * 0.5f;
			}
			return new Vector2(num, y);
		}
	}
}
