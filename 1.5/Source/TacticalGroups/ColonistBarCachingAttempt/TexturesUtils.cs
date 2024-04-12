using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using static Verse.Widgets;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	public static class TexturesUtils
	{
		public static Dictionary<Color, Texture2D> needBars = new Dictionary<Color, Texture2D>();

		[TweakValue("0CG", -50, 150)] public static int xStart = -2;
		[TweakValue("0CG", -50, 150)] public static int yStart = 17;
		[TweakValue("0CG", -50, 150)] public static int xEnd = 22;
		[TweakValue("0CG", -50, 150)] public static int yEnd = 59;
		public static Texture2D GetMergedTexture(Rect original, Rect current, Texture2D background, Texture2D overlay)
		{
			var readableBackground = GetReadableTexture2D(background, (int)original.width, (int)original.height);
			var readableOverlay = GetReadableTexture2D(overlay, (int)current.width, (int)current.height);

			int startX = (int)(current.x - original.x);
			int startY = startX;
			if (overlay.name.ToLower().Contains("awake"))
            {
				Log.Message("original: " + original + " - current: " + current + " - background: " + background + " - overlay: " + overlay);
            }
			return CombineTextures(readableBackground, readableOverlay, startX, startY);
		}

		public static Texture2D CombineTextures(Texture2D background, Texture2D overlay, int startX, int startY)
		{
			for (int x = startX; x < background.width; x++)
			{
				for (int y = startY; y < background.height; y++)
				{
					if (x - startX < overlay.width && y - startY < overlay.height)
					{
						var bgColor = background.GetPixel(x, y);
						var wmColor = overlay.GetPixel(x - startX, y - startY);

						var finalColor = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

						background.SetPixel(x, y, finalColor);
					}
				}
			}

			background.Apply();
			return background;
		}
		public static Texture2D GetMergedTexture(Texture2D background, Texture2D overlay)
		{
			var readableBackground = GetReadableTexture2D(background, background.width, background.height);
			var readableOverlay = GetReadableTexture2D(overlay, overlay.width, overlay.height);
			return MergeTextures(readableBackground, readableOverlay, 0, 0);
		}
		public static Texture2D GetMergedDarkenTexture(Texture2D background, Texture2D overlay)
		{
			return GetDarkenTexture(GetMergedTexture(background, overlay));
		}

		public static Texture2D GetDarkenTexture(this Texture2D origin)
        {
			var copy = new Texture2D(origin.width, origin.height);
			var pixels = origin.GetPixels();
			var darkenColors = new Color[pixels.Length];
			for (int i = 0; i < pixels.Length; i++)
			{
				darkenColors[i] = Color.Lerp(pixels[i], Color.black, 0.25f);
			}
			copy.SetPixels(darkenColors);
			copy.Apply();
			return copy;
		}

		public static Texture2D MergeTextures(Texture2D background, Texture2D overlay, int startX, int startY, int endX, int endY)
		{
			Texture2D newTex = new Texture2D(background.width, background.height, background.format, false);
			for (int x = 0; x < background.width; x++)
			{
				for (int y = 0; y < background.height; y++)
				{
					if (x >= startX && y >= startY && x < endX && y < endY)
					{
						Color bgColor = background.GetPixel(x, y);
						Color wmColor = overlay.GetPixel(x - startX, y - startY);
						Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);
						newTex.SetPixel(x, y, new Color(wmColor.r, wmColor.g, wmColor.b, 1f));

						//newTex.SetPixel(x, y, final_color);
					}
					else
						newTex.SetPixel(x, y, background.GetPixel(x, y));
				}
			}

			newTex.Apply();
			return newTex;
		}
		public static Texture2D MergeTextures(Texture2D background, Texture2D overlay, int startX, int startY)
		{
			Texture2D newTex = new Texture2D(background.width, background.height, background.format, false);
			for (int x = 0; x < background.width; x++)
			{
				for (int y = 0; y < background.height; y++)
				{
					if (x >= startX && y >= startY && x < overlay.width && y < overlay.height)
					{
						Color bgColor = background.GetPixel(x, y);
						Color wmColor = overlay.GetPixel(x - startX, y - startY);

						Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

						newTex.SetPixel(x, y, final_color);
					}
					else
						newTex.SetPixel(x, y, background.GetPixel(x, y));
				}
			}

			newTex.Apply();
			return newTex;
		}
		public static Texture2D GetReadableTexture2D(this Texture texture, int width, int height)
		{
			RenderTexture previous = RenderTexture.active;
			RenderTexture temporary = RenderTexture.GetTemporary(
					width,
					height,
					0,
					RenderTextureFormat.Default,
					RenderTextureReadWrite.Linear);
			
			Graphics.Blit(texture, temporary);
			RenderTexture.active = temporary;

			Texture2D texture2D = new Texture2D(width, height);
			texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
			texture2D.Apply();

			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(temporary);
			return texture2D;
		}
	}
}
