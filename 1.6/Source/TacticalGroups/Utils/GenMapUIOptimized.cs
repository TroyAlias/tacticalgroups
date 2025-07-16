using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using static Verse.Widgets;

namespace TacticalGroups
{
	public class PawnLabelCache
	{
		public PawnLabelCache()
		{

		}
		public float width;
		public string label;
		public int updateWidthCount;
		public int updateLabelCount;
	}

	[StaticConstructorOnStartup]
	public static class GenMapUIOptimized
	{
		public static Dictionary<Pawn, PawnLabelCache> pawnLabelCaches = new Dictionary<Pawn, PawnLabelCache>();

		public static readonly Texture2D OverlayHealthTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0f, 0f, 0.25f));

		public static void ForceUpdateLabels()
        {
			foreach (var cache in pawnLabelCaches)
            {
				cache.Value.updateLabelCount = 0;
				cache.Value.updateWidthCount = 0;
			}
		}
		public static void DrawPawnLabel(Pawn pawn, Vector2 pos, float alpha = 1f, float truncateToWidth = 9999f, Dictionary<string, string> truncatedLabelsCache = null, GameFont font = GameFont.Tiny, bool alwaysDrawBg = true, bool alignCenter = true)
		{
			float pawnLabelNameWidth = GetPawnLabelNameWidth(pawn, truncateToWidth, truncatedLabelsCache);
			Rect bgRect = new Rect(pos.x - pawnLabelNameWidth / 2f - 4f, pos.y, pawnLabelNameWidth + 8f, 12f);
			//if (!pawn.RaceProps.Humanlike)
			//{
			//	bgRect.y -= 4f;
			//}
			GUI.color = new Color(1f, 1f, 1f, alpha);
			Text.Font = font;
			string pawnLabel = GetPawnLabel(pawn, truncateToWidth, truncatedLabelsCache);
			float summaryHealthPercent = pawn.health.summaryHealth.SummaryHealthPercent;
			if (!TacticalGroupsSettings.DisableLabelBackground)
            {
				if (alwaysDrawBg || summaryHealthPercent < 0.999f)
				{
					GUI.DrawTexture(bgRect, TexUI.GrayTextBG);
				}
			}

			if (summaryHealthPercent < 0.999f)
			{
				Widgets.FillableBar(bgRect.ContractedBy(1f), summaryHealthPercent, OverlayHealthTex, BaseContent.ClearTex, doBorder: false);
			}
			Color color = PawnNameColorUtility.PawnNameColorOf(pawn);
			color.a = alpha;
			GUI.color = color;
			Rect rect;
			if (alignCenter)
			{
				Text.Anchor = TextAnchor.UpperCenter;
				rect = new Rect(bgRect.center.x - pawnLabelNameWidth / 2f, bgRect.y - 2f, pawnLabelNameWidth, 100f);
			}
			else
			{
				Text.Anchor = TextAnchor.UpperLeft;
				rect = new Rect(bgRect.x + 2f, bgRect.center.y - Text.CalcSize(pawnLabel).y / 2f, pawnLabelNameWidth, 100f);
			}
			Widgets.Label(rect, pawnLabel);
			if (pawn.Drafted)
			{
				Widgets.DrawLineHorizontal(bgRect.center.x - pawnLabelNameWidth / 2f, bgRect.y + 11f, pawnLabelNameWidth);
			}
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		private static float GetPawnLabelNameWidth(Pawn pawn, float truncateToWidth, Dictionary<string, string> truncatedLabelsCache)
		{
			if (pawnLabelCaches.TryGetValue(pawn, out PawnLabelCache pawnLabelCache))
			{
				if (pawnLabelCache.updateWidthCount == 0)
				{
					pawnLabelCache.width = GetPawnLabelNameWidthRecheck(pawn, truncateToWidth, truncatedLabelsCache);
					pawnLabelCache.updateWidthCount = 300;
				}
				pawnLabelCache.updateWidthCount--;
				return pawnLabelCache.width;
			}
			else
			{
				pawnLabelCaches[pawn] = new PawnLabelCache();
				pawnLabelCaches[pawn].width = GetPawnLabelNameWidthRecheck(pawn, truncateToWidth, truncatedLabelsCache);
				return pawnLabelCaches[pawn].width;
			}
		}

		private static float GetPawnLabelNameWidthRecheck(Pawn pawn, float truncateToWidth, Dictionary<string, string> truncatedLabelsCache)
		{
			string pawnLabel = GetPawnLabel(pawn, truncateToWidth, truncatedLabelsCache);
			float num = pawnLabel.GetWidthCached();
			if (Math.Abs(Math.Round(Prefs.UIScale) - (double)Prefs.UIScale) > 1.4012984643248171E-45)
			{
				num += 0.5f;
			}
			if (num < 20f)
			{
				num = 20f;
			}
			return num;
		}


		private static string GetPawnLabel(Pawn pawn, float truncateToWidth, Dictionary<string, string> truncatedLabelsCache)
		{
			if (pawnLabelCaches.TryGetValue(pawn, out PawnLabelCache pawnLabelCache))
			{
				if (pawnLabelCache.updateLabelCount == 0)
				{
					pawnLabelCache.label = pawn.LabelShortCap.Truncate(truncateToWidth, truncatedLabelsCache);
					pawnLabelCache.updateLabelCount = 60;
				}
				pawnLabelCache.updateLabelCount--;
				return pawnLabelCache.label;
			}
			else
			{
				pawnLabelCaches[pawn] = new PawnLabelCache();
				pawnLabelCaches[pawn].label = pawn.LabelShortCap.Truncate(truncateToWidth, truncatedLabelsCache);
				return pawnLabelCache.label;
			}
		}
	}
}