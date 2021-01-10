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
	[StaticConstructorOnStartup]
	public static class TexturesUtils
	{
		public static Texture2D GetDarkenTexture(this Texture2D origin)
        {
			var copy = new Texture2D(origin.width, origin.height);
			var pixels = origin.GetPixels();
			copy.SetPixels(pixels);
			copy.Apply();
			for (int i = 0; i < copy.width; i++)
			{
				for (int j = 0; j < copy.height; j++)
				{
					var originColor = origin.GetPixel(i, j);
					var darkenColor = Color.Lerp(originColor, Color.black, 0.12f);
					copy.SetPixel(i, j, darkenColor);
				}
			}
			return copy;
		}

		public static Texture2D MakeGrayscale(this Texture2D tex)
		{
			Color[] pixels = tex.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				float grayscale = pixels[i].grayscale;
				pixels[i] = new Color(grayscale, grayscale, grayscale, pixels[i].a);
			}
			tex.SetPixels(pixels);
			tex.Apply();
			return tex;
		}
	}
}
