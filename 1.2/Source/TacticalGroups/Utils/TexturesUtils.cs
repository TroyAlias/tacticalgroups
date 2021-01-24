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
		public static Texture2D GetMergedTexture(Texture2D background, Texture2D overlay)
		{
			var readableBackground = GetReadableTexture(background);
			var readableOverlay = GetReadableTexture(overlay);
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

		public static Texture2D GetReadableTexture(Texture2D texture)
		{
			RenderTexture previous = RenderTexture.active;
			RenderTexture temporary = RenderTexture.GetTemporary(
					texture.width,
					texture.height,
					0,
					RenderTextureFormat.Default,
					RenderTextureReadWrite.Linear);
			
			Graphics.Blit(texture, temporary);
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(texture.width, texture.height);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(temporary);
			return texture2D;
		}
	}
}
