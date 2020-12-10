using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace TacticalGroups
{
	public class LordJob_TendWounded : LordJob
	{
		private Faction faction;
		public LordJob_TendWounded()
		{
		}
		public LordJob_TendWounded(Faction faction)
		{
			this.faction = faction;
		}

		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil lordToil = new LordToil_TendWounded();
			stateGraph.AddToil(lordToil);
			return stateGraph;
		}

		public override void ExposeData()
		{
			Scribe_References.Look(ref faction, "faction");
		}
	}
}
