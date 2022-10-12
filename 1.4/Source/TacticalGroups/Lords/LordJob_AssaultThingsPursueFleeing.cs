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
	public class LordJob_AssaultThingsPursueFleeing : LordJob
	{
		private Faction assaulterFaction;

		private List<Thing> things;

		private bool useAvoidGridSmart;

		private float damageFraction;

		public LordJob_AssaultThingsPursueFleeing()
		{
		}

		public LordJob_AssaultThingsPursueFleeing(Faction assaulterFaction, List<Thing> things, float damageFraction = 1f, bool useAvoidGridSmart = false)
		{
			this.assaulterFaction = assaulterFaction;
			this.things = things;
			this.useAvoidGridSmart = useAvoidGridSmart;
			this.damageFraction = damageFraction;
		}

		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil lordToil = new LordToil_AssaultThingsPursueFleeing(things);
			if (useAvoidGridSmart)
			{
				lordToil.useAvoidGrid = true;
			}
			stateGraph.AddToil(lordToil);
			return stateGraph;
		}

		public override void ExposeData()
		{
			Scribe_References.Look(ref assaulterFaction, "assaulterFaction");
			Scribe_Collections.Look(ref things, "things", LookMode.Reference);
			Scribe_Values.Look(ref useAvoidGridSmart, "useAvoidGridSmart", defaultValue: false);
			Scribe_Values.Look(ref damageFraction, "damageFraction", 0f);
		}
	}
}
