using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public class Formation : IExposable
	{
		public Dictionary<Pawn, IntVec3> formations = new Dictionary<Pawn, IntVec3>();
		public string colorPrefix;
		public bool isSelected;
		public Formation()
		{

		}

		public Formation(string color)
		{
			colorPrefix = color;
		}
		public void ExposeData()
		{
			Scribe_Collections.Look(ref formations, "formations", LookMode.Reference, LookMode.Value, ref pawnKeys2, ref intVecValues);
			Scribe_Values.Look(ref colorPrefix, "colorPrefix");
		}


		public Texture2D Icon => formations != null && formations.Any() ?
			isSelected ? ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/" + colorPrefix + "select")
			: ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/" + colorPrefix + "dark")
			: isSelected ? ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/greyselect")
			: ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/battlestations/greydark");

		private List<Pawn> pawnKeys2;
		private List<IntVec3> intVecValues;
	}
}
