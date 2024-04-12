using System.Collections.Generic;
using Verse;

namespace TacticalGroups
{
    public class GroupColor : IExposable
	{
		public Dictionary<BodyColor, ColorOption> bodyColors = new Dictionary<BodyColor, ColorOption>();
		public void ExposeData()
		{
			Scribe_Collections.Look(ref bodyColors, "bodyColors", LookMode.Value, LookMode.Deep, ref bodyColorKeys, ref colorValues);
		}

		private List<BodyColor> bodyColorKeys;
		private List<ColorOption> colorValues;
	}
}
