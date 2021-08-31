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
	public static class ColorUtils
	{
		private static List<Color> allColors;

		public static List<Color> AllColors
        {
            get
            {
				if (allColors is null)
                {
					allColors = new List<Color>
					{
						new Color(1.000f, 1.000f, 1.000f),
						new Color(0.753f, 0.753f, 0.753f),
						new Color(0.502f, 0.502f, 0.502f),
						new Color(1.000f, 0.451f, 0.451f),
						new Color(0.749f, 0.337f, 0.337f),
						new Color(0.580f, 0.263f, 0.263f),
						new Color(1.000f, 0.780f, 0.451f),
						new Color(0.749f, 0.584f, 0.337f),
						new Color(0.580f, 0.451f, 0.263f),
						new Color(0.890f, 1.000f, 0.451f),
						new Color(0.667f, 0.749f, 0.337f),
						new Color(0.518f, 0.580f, 0.263f),
						new Color(0.561f, 1.000f, 0.451f),
						new Color(0.420f, 0.749f, 0.337f),
						new Color(0.325f, 0.580f, 0.263f),
						new Color(0.451f, 1.000f, 0.671f),
						new Color(0.337f, 0.749f, 0.502f),
						new Color(0.263f, 0.580f, 0.388f),
						new Color(0.451f, 1.000f, 1.000f),
						new Color(0.337f, 0.749f, 0.749f),
						new Color(0.263f, 0.580f, 0.580f),
						new Color(0.451f, 0.671f, 1.000f),
						new Color(0.337f, 0.502f, 0.749f),
						new Color(0.263f, 0.388f, 0.580f),
						new Color(0.561f, 0.451f, 1.000f),
						new Color(0.420f, 0.337f, 0.749f),
						new Color(0.325f, 0.263f, 0.580f),
						new Color(0.890f, 0.451f, 1.000f),
						new Color(0.667f, 0.337f, 0.749f),
						new Color(0.518f, 0.263f, 0.580f),
						new Color(1.000f, 0.451f, 0.780f),
						new Color(0.749f, 0.337f, 0.584f),
						new Color(0.580f, 0.263f, 0.451f),
						new Color(0.100f, 0.100f, 0.100f),
						new Color(0.200f, 0.200f, 0.200f),
						new Color(0.310f, 0.280f, 0.260f),
						new Color(0.250f, 0.200f, 0.150f),
						new Color(0.300f, 0.200f, 0.100f),
						new Color(0.353f, 0.227f, 0.125f),
						new Color(0.518f, 0.325f, 0.184f),
						new Color(0.757f, 0.573f, 0.333f),
						new Color(0.929f, 0.792f, 0.612f),
						new Color(0.482f, 0.490f, 0.490f),
						new Color(0.592f, 0.604f, 0.604f),
						new Color(0.816f, 0.827f, 0.831f),
						new Color(0.925f, 0.941f, 0.945f),
						new Color(0.431f, 0.173f, 0.000f),
						new Color(0.529f, 0.212f, 0.000f),
						new Color(0.729f, 0.290f, 0.000f),
						new Color(0.827f, 0.329f, 0.000f),
						new Color(0.471f, 0.259f, 0.071f),
						new Color(0.576f, 0.318f, 0.086f),
						new Color(0.792f, 0.435f, 0.118f),
						new Color(0.902f, 0.494f, 0.133f),
						new Color(0.494f, 0.318f, 0.035f),
						new Color(0.612f, 0.392f, 0.047f),
						new Color(0.839f, 0.537f, 0.063f),
						new Color(0.953f, 0.612f, 0.071f),
						new Color(0.490f, 0.400f, 0.031f),
						new Color(0.604f, 0.490f, 0.039f),
						new Color(0.831f, 0.675f, 0.051f),
						new Color(0.945f, 0.769f, 0.059f),
						new Color(0.094f, 0.416f, 0.231f),
						new Color(0.114f, 0.514f, 0.282f),
						new Color(0.157f, 0.706f, 0.388f),
						new Color(0.180f, 0.800f, 0.443f),
						new Color(0.078f, 0.353f, 0.196f),
						new Color(0.098f, 0.435f, 0.239f),
						new Color(0.133f, 0.600f, 0.329f),
						new Color(0.153f, 0.682f, 0.376f),
						new Color(0.043f, 0.325f, 0.271f),
						new Color(0.055f, 0.400f, 0.333f),
						new Color(0.075f, 0.553f, 0.459f),
						new Color(0.086f, 0.627f, 0.522f),
						new Color(0.055f, 0.384f, 0.318f),
						new Color(0.067f, 0.471f, 0.392f),
						new Color(0.090f, 0.647f, 0.537f),
						new Color(0.102f, 0.737f, 0.612f),
						new Color(0.106f, 0.310f, 0.447f),
						new Color(0.129f, 0.380f, 0.549f),
						new Color(0.180f, 0.525f, 0.757f),
						new Color(0.204f, 0.596f, 0.859f),
						new Color(0.082f, 0.263f, 0.376f),
						new Color(0.102f, 0.322f, 0.463f),
						new Color(0.141f, 0.443f, 0.639f),
						new Color(0.161f, 0.502f, 0.725f),
						new Color(0.290f, 0.137f, 0.353f),
						new Color(0.357f, 0.173f, 0.435f),
						new Color(0.490f, 0.235f, 0.596f),
						new Color(0.557f, 0.267f, 0.678f),
						new Color(0.318f, 0.180f, 0.373f),
						new Color(0.388f, 0.224f, 0.455f),
						new Color(0.533f, 0.306f, 0.627f),
						new Color(0.157f, 0.157f, 0.157f),
						new Color(0.235f, 0.235f, 0.235f),
						new Color(0.275f, 0.275f, 0.275f),
						new Color(0.196f, 0.196f, 0.196f),
						new Color(0.200f, 0.000f, 0.000f),
						new Color(0.400f, 0.000f, 0.000f),
						new Color(0.600f, 0.000f, 0.000f),
						new Color(1.000f, 0.200f, 0.200f),
						new Color(1.000f, 0.800f, 0.800f),
						new Color(0.000f, 0.200f, 0.000f),
						new Color(0.000f, 0.400f, 0.000f),
						new Color(0.000f, 0.600f, 0.000f),
						new Color(0.000f, 0.800f, 0.000f),
						new Color(0.000f, 1.000f, 0.000f),
						new Color(0.000f, 0.000f, 0.200f),
						new Color(0.000f, 0.000f, 0.400f),
						new Color(0.000f, 0.000f, 0.600f),
						new Color(0.000f, 0.000f, 0.800f),
						new Color(0.000f, 0.000f, 1.000f),
					};
					allColors.SortByColor((Color x) => x);
				}
				return allColors;
            }
        }


		public static Color? GetDesiredColor(Pawn pawn, Apparel apparel)
		{
			if (pawn.TryGetGroups(out var groups))
			{
				foreach (var group in groups.SortPawnGroupFirst())
				{
					if (group.groupColor?.bodyColors != null)
					{
						if (apparel.def.IsHeadgear() && group.groupColor.bodyColors.TryGetValue(BodyColor.Head, out var headColor))
						{
							return headColor.GetColor(pawn);
						}
						else if (apparel.def.IsFeetGear() && group.groupColor.bodyColors.TryGetValue(BodyColor.Feet, out var feetColor))
						{
							return feetColor.GetColor(pawn);
						}
						else if (apparel.def.IsLegsGear() && group.groupColor.bodyColors.TryGetValue(BodyColor.Legs, out var legsColor))
						{
							return legsColor.GetColor(pawn);
						}
						else if (apparel.def.IsTorsoGear() && group.groupColor.bodyColors.TryGetValue(BodyColor.Torso, out var torsoColor))
						{
							return torsoColor.GetColor(pawn);
						}
						else if (apparel.def.IsArmsGear() && group.groupColor.bodyColors.TryGetValue(BodyColor.Hands, out var armsColor))
						{
							return armsColor.GetColor(pawn);
						}
						else if (group.groupColor.bodyColors.TryGetValue(BodyColor.All, out var allColor))
						{
							return allColor.GetColor(pawn);
						}
					}
				}
			}
			return null;
		}

		public static Color? GetDesiredHairColor(Pawn pawn)
		{
			if (pawn.TryGetGroups(out var groups))
			{
				foreach (var group in groups.SortPawnGroupFirst())
				{
					if (group.groupColor?.bodyColors != null)
					{
						if (group.groupColor.bodyColors.TryGetValue(BodyColor.Hair, out var hairColor))
                        {
							return hairColor.GetColor(pawn);
                        }
					}
				}
			}
			return null;
		}

	}
}
