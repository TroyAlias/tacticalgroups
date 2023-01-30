using UnityEngine;
using Verse;

namespace TacticalGroups
{
    public class ColorOption : IExposable
	{
		public Color color;
		public bool pawnFavoriteOnly;
		public ColorOption()
		{

		}

		public ColorOption(Color color)
		{
			this.color = color;
		}
		public ColorOption(bool pawnFavoriteOnly)
		{
			this.pawnFavoriteOnly = pawnFavoriteOnly;
		}

		public Color? GetColor(Pawn pawn)
		{
			return pawnFavoriteOnly ? pawn.story.favoriteColor : color;
		}
		public void ExposeData()
		{
			Scribe_Values.Look(ref color, "color");
			Scribe_Values.Look(ref pawnFavoriteOnly, "pawnFavoriteOnly");
		}
	}
}
