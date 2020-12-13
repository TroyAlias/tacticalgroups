using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class ColonistBarDrawLocsFinder
	{
		private List<int> entriesInGroup = new List<int>();

		private List<int> horizontalSlotsPerGroup = new List<int>();

		private const float MarginTop = 21f;
		private static float MaxColonistBarWidth => (float)UI.screenWidth - 520f;

		public static Dictionary<CaravanGroup, Vector2> caravanGroupDrawLoc = new Dictionary<CaravanGroup, Vector2>();
		public static Dictionary<ColonyGroup, Vector2> colonyGroupDrawLoc = new Dictionary<ColonyGroup, Vector2>();
		public static Dictionary<PawnGroup, Vector2> pawnGroupDrawLoc = new Dictionary<PawnGroup, Vector2>();
		public static Rect createGroupRect;
		public void CalculateDrawLocs(List<Vector2> outDrawLocs, out float scale)
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
				float scaleMultiplier = 1;

				var count = TacticUtils.AllColonyGroups.Count;
				if (!WorldRendererUtility.WorldRenderedNow && count > 0)
				{
					var activeColony = TacticUtils.AllColonyGroups.Where(x => x.Map == Find.CurrentMap).FirstOrDefault();
					if (activeColony != null)
					{
						count += TacticUtils.GetAllPawnGroupFor(activeColony).Take(4).Count();
					}
				}

				scaleMultiplier += (float)count / 10f;
				float num3 = ((TacticalColonistBar.BaseSize.x + 24f) * num);
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
		private void CalculateDrawLocs(List<Vector2> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow)
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
			float num3 = (TacticalColonistBar.BaseSize.x + 24f) * scale;

			//float scaleMultiplier = 1;
			//scaleMultiplier += (float)TacticUtils.AllGroups.Count / 50f;

			float num4 = ((float)num * num3 + (float)(num2 - 1) * 25f * scale);

			var allColonyGroups = TacticUtils.AllColonyGroups;
			num4 += allColonyGroups.Sum(x => x.groupIcon.width + 10f);
			if (!WorldRendererUtility.WorldRenderedNow)
			{
				var activeColony = TacticUtils.AllColonyGroups.Where(x => x.Map == Find.CurrentMap).FirstOrDefault();
				if (activeColony != null)
				{
					num4 += TacticUtils.GetAllPawnGroupFor(activeColony).Take(4).Sum(x => x.groupIcon.width + 10);
				}
			}

			List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			int num5 = -1;
			int num6 = -1;
			float num7 = ((float)UI.screenWidth - num4) / 2f;
			for (int j = 0; j < entries.Count; j++)
			{
				if (num5 != entries[j].group)
				{
					if (num5 >= 0)
					{
						num7 += 25f * scale;
						num7 += (float)horizontalSlotsPerGroup[num5] * scale * (TacticalColonistBar.BaseSize.x + 24f);
					}
					if (entries[j].caravanGroup != null)
					{
						caravanGroupDrawLoc[entries[j].caravanGroup] = new Vector2(num7 - (12 * scale), 21f);
						num7 += 100f;
					}
					else if (entries[j].colonyGroup != null)
					{
						colonyGroupDrawLoc[entries[j].colonyGroup] = new Vector2(num7, 21f);
						num7 += entries[j].colonyGroup.groupIcon.width + 10;
						if (entries[j].colonyGroup.Map == Find.CurrentMap)
						{
							var list = TacticUtils.TacticalGroups.pawnGroups.Where(x => x.Map == entries[j].colonyGroup.Map).ToList();
							list.Reverse();
							foreach (var g in list)
							{
							}
							var initPos = num7;
							var xPos = num7;
							var yPos = 21f;
							for (var groupID = 0; groupID < list.Count(); groupID++)
							{
								if (groupID > 0 && groupID % 4 == 0)
								{
									xPos = initPos;
									yPos += list[groupID].groupIcon.height + 25;
								}
								pawnGroupDrawLoc[list[groupID]] = new Vector2(xPos, yPos);
								xPos += list[groupID].groupIcon.width + 10;
							}
							list = list.Take(4).ToList();
							num7 += list.Sum(x => x.groupIcon.width + 10);
							createGroupRect = new Rect(num7, 21f, Textures.CreateGroupIcon.width, Textures.CreateGroupIcon.height);
							num7 += Textures.CreateGroupIcon.width + 20f;
						}
					}
					num6 = 0;
					num5 = entries[j].group;
				}
				else
				{
					num6++;
				}
				Vector2 drawLoc = GetDrawLoc(num7, 21f, entries[j].group, num6, scale);
				outDrawLocs.Add(drawLoc);
			}
		}



		private Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int numInGroup, float scale)
		{
			float num = groupStartX + (float)(numInGroup % horizontalSlotsPerGroup[group]) * scale * (TacticalColonistBar.BaseSize.x + 24f);
			float y = groupStartY + (float)(numInGroup / horizontalSlotsPerGroup[group]) * scale * (TacticalColonistBar.BaseSize.y + 32f);
			if (numInGroup >= entriesInGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group])
			{
				int num2 = horizontalSlotsPerGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group];
				num += (float)num2 * scale * (TacticalColonistBar.BaseSize.x + 24f) * 0.5f;
			}
			return new Vector2(num, y);
		}
	}
}
