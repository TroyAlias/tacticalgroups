using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TacticalGroups
{
	public static class ApparelUtils
	{

		public static bool IsHeadgear(this ThingDef td)
		{
			if (!td.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead))
			{
				return td.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead);
			}
			return true;
		}

		public static bool IsFeetGear(this ThingDef td)
		{
			return td.apparel.bodyPartGroups.Contains(TacticDefOf.Feet);
		}

		public static bool IsTorsoGear(this ThingDef td)
		{
			return td.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso);
		}
		public static bool IsLegsGear(this ThingDef td)
		{
			return td.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs);
		}
		public static bool IsArmsGear(this ThingDef td)
		{
			if (td.apparel.bodyPartGroups.Contains(TacticDefOf.Arms) || td.apparel.bodyPartGroups.Contains(TacticDefOf.Hands) 
				|| td.apparel.bodyPartGroups.Contains(TacticDefOf.LeftHand) || td.apparel.bodyPartGroups.Contains(TacticDefOf.RightHand))
            {
				return true;
            }
			return false;
		}
	}
}