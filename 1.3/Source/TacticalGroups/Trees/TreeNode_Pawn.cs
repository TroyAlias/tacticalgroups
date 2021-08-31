using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public class TreeNode_Pawns : TreeNode
	{
		public List<Pawn> pawns;

		private string prefix;
		public string Label => prefix + (pawns != null ? " (" + pawns.Count.ToString() + ")" : " (0)");
		public TreeNode_Pawns(List<Pawn> pawns, string prefix)
		{
			this.pawns = pawns;
			this.prefix = prefix;
		}
	}
}
