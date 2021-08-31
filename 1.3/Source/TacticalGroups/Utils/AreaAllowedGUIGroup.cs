using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using static Verse.Widgets;

namespace TacticalGroups
{
	public static class AreaAllowedGUIGroup
	{
		private static bool dragging;

		public static void DoAllowedAreaSelectors(Rect rect, ColonistGroup group)
		{
			List<Area> allAreas = group.Map.areaManager.AllAreas;
			int num = 1;
			for (int i = 0; i < allAreas.Count; i++)
			{
				if (allAreas[i].AssignableAsAllowed())
				{
					num++;
				}
			}
			float num2 = rect.width / (float)num;
			Text.WordWrap = false;
			Text.Font = GameFont.Tiny;
			DoAreaSelector(new Rect(rect.x + 0f, rect.y, num2, rect.height), group, null);
			int num3 = 1;
			for (int j = 0; j < allAreas.Count; j++)
			{
				if (allAreas[j].AssignableAsAllowed())
				{
					float num4 = (float)num3 * num2;
					DoAreaSelector(new Rect(rect.x + num4, rect.y, num2, rect.height), group, allAreas[j]);
					num3++;
				}
			}
			Text.WordWrap = true;
			Text.Font = GameFont.Small;
		}

		private static void DoAreaSelector(Rect rect, ColonistGroup group, Area area)
		{
			MouseoverSounds.DoRegion(rect);
			rect = rect.ContractedBy(1f);
			GUI.DrawTexture(rect, (area != null) ? area.ColorTexture : BaseContent.GreyTex);
			Text.Anchor = TextAnchor.MiddleLeft;
			string text = AreaUtility.AreaAllowedLabel_Area(area);
			Rect rect2 = rect;
			rect2.xMin += 3f;
			rect2.yMin += 2f;
			Widgets.Label(rect2, text);
			if (group.ActivePawns.First().playerSettings.AreaRestriction == area)
			{
				Widgets.DrawBox(rect, 2);
			}
			if (Event.current.rawType == EventType.MouseUp && Event.current.button == 0)
			{
				dragging = false;
			}
			if (!Input.GetMouseButton(0) && Event.current.type != 0)
			{
				dragging = false;
			}
			if (Mouse.IsOver(rect))
			{
				area?.MarkForDraw();
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
				{
					dragging = true;
				}
				if (dragging)
				{
					foreach (var p in group.ActivePawns)
                    {
						if (p.playerSettings.AreaRestriction != area)
                        {
							p.playerSettings.AreaRestriction = area;
							SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
						}
					}
					if (group.groupAreaEnabled)
                    {
						group.groupArea = area;
					}
				}
			}
			Text.Anchor = TextAnchor.UpperLeft;
			TooltipHandler.TipRegion(rect, text);
		}
	}
}
