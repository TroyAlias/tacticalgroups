using HarmonyLib;
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
	public class CaravanOptionsMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(100f, -200f);
		public CaravanOptionsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
		}
	}
}
