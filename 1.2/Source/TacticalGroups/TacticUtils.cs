using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	internal static class TacticUtils
	{
		public static TacticalGroups TacticalGroups
		{
			get
			{
				if (tacticalGroups == null)
				{
					tacticalGroups = Current.Game.GetComponent<TacticalGroups>();
					return tacticalGroups;
				}
				return tacticalGroups;
			}
		}
		public static void ResetTacticGroups()
		{
			tacticalGroups = Current.Game.GetComponent<TacticalGroups>();
		}

		private static TacticalGroups tacticalGroups;
		public static TacticalColonistBar TacticalColonistBar => TacticalGroups.TacticalColonistBar;
		public static List<ColonistGroup> Groups => TacticalGroups.Groups;

	}
}
