using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class TacticalGroups : GameComponent
	{
        public TacticalGroups(Game game)
        {
        }
        public static TacticalColonistBar ColonistBar
        {
            get
            {
                if (colonistBar is null)
                {
                    colonistBar = new TacticalColonistBar();
                }
                return colonistBar;
            }
        }

        public void CreateGroup(List<Pawn> pawns)
        {
            Find.WindowStack.Add(new Dialog_NamePawn(pawns.First()));
        }

        private static TacticalColonistBar colonistBar;
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            colonistBar = new TacticalColonistBar();
        }
    }
}
