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
	public class LordJob_RescueFallen : LordJob
	{
		private Faction faction;
		public LordJob_RescueFallen()
		{
		}
		public LordJob_RescueFallen(Faction faction)
		{
			this.faction = faction;
		}

		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil lordToil = new LordToil_RescueFallen();
			stateGraph.AddToil(lordToil);
			return stateGraph;
		}

		public override void ExposeData()
		{
			Scribe_References.Look(ref faction, "faction");
		}
	}
}
