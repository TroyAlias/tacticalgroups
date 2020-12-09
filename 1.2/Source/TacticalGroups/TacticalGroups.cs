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

        private TacticalColonistBar colonistBar;
        public TacticalColonistBar TacticalColonistBar
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

        private List<ColonistGroup> groups;
        public List<ColonistGroup> Groups
        {
            get
            {
                if (groups is null)
                {
                    groups = new List<ColonistGroup>();
                }
                return groups;
            }
        }



        public void CreateGroup(List<Pawn> pawns)
        {
            Find.WindowStack.Add(new Dialog_NamePawn(pawns.First()));
        }

        public void AddGroup(List<Pawn> pawns)
        {
            this.groups.Add(new ColonistGroup(pawns));
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            MedicalCareUtilityGroup.Reset();
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            TacticUtils.ResetTacticGroups();

        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            TacticUtils.ResetTacticGroups();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref groups, "groups", LookMode.Deep);
        }
    }
}
