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
	public static class TextUtils
	{
		//static TextUtils()
        //{
		//	//ModMetaData modMetaData = ModLister.AllInstalledMods.FirstOrDefault((ModMetaData x) =>
		//	//x != null && x.Name != null && x.Active && x.Name.StartsWith("RPG Framework"));
		//	//string path = Path.GetFullPath(modMetaData.RootDir.ToString() + "/Presets/" + this.name + ".xml");
		//
		//	//Log.Message("Test 1");
		//	//AssetBundle assetBundle = AssetBundle.LoadFromFile("c:/GAMES/Rimworld/Mods/tacticalgroups/Fonts/RimWordFont");
		//	//Log.Message("Test 2: " + assetBundle);
		//	//var font = (Font)assetBundle.LoadAsset("RimWordFont");
		//	//Log.Message("Font: " + font);
		//}

		public static void Label(Rect rect, GUIContent content)
		{
				
			GUIStyle guistyle = new GUIStyle
			{
				fontStyle = FontStyle.Normal,
				normal =
				{
					textColor = Color.white
				},
				padding = new RectOffset(0, 0, 12, 6),
				font = (Font)Resources.Load("Fonts/FontName")
			};
			GUI.Label(rect, content, guistyle);
		}

		public static void Label(Rect rect, string label)
		{
			GUIStyle guistyle = new GUIStyle
			{
				fontStyle = FontStyle.Normal,
				normal =
				{
					textColor = Color.white
				},
				padding = new RectOffset(0, 0, 12, 6),
			};

			Rect position = rect;
			float num = Prefs.UIScale / 2f;
			if (Prefs.UIScale > 1f && Math.Abs(num - Mathf.Floor(num)) > float.Epsilon)
			{
				position.xMin = AdjustCoordToUIScalingFloor(rect.xMin);
				position.yMin = AdjustCoordToUIScalingFloor(rect.yMin);
				position.xMax = AdjustCoordToUIScalingCeil(rect.xMax + 1E-05f);
				position.yMax = AdjustCoordToUIScalingCeil(rect.yMax + 1E-05f);
			}
			GUI.Label(position, label, guistyle);
		}
	}
}
