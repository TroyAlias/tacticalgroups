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
		protected override Vector2 InitialPositionShift => new Vector2(-originRect.width, originRect.height);
		public CaravanOptionsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			var bedrollsCheckbox = new Rect(rect.x + 10, rect.y + 10, Textures.Xbox.width, Textures.Xbox.height);
			Checkbox(bedrollsCheckbox, ref this.colonistGroup.bedrollsEnabled, null);
			var bedrollsLabel = new Rect(bedrollsCheckbox.xMax + 10, bedrollsCheckbox.y, 120, 30);
			Widgets.Label(bedrollsLabel, Strings.Bedrolls);

			var travelSuppliesCheckbox = new Rect(bedrollsCheckbox.x, bedrollsCheckbox.yMax + 10, Textures.Xbox.width, Textures.Xbox.height);
			Checkbox(travelSuppliesCheckbox, ref this.colonistGroup.travelSuppliesEnabled, null);
			var travelSuppliesLabel = new Rect(travelSuppliesCheckbox.xMax + 10, travelSuppliesCheckbox.y, 120, 30);
			Widgets.Label(travelSuppliesLabel, Strings.TravelSupplies);
		}

		private void Checkbox(Rect checkboxRec, ref bool checkboxVar, Action action)
        {
			if (checkboxVar)
            {
				GUI.DrawTexture(checkboxRec, Textures.Ybox);
			}
			else
            {
				GUI.DrawTexture(checkboxRec, Textures.Xbox);
			}

			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount >= 1 && Mouse.IsOver(checkboxRec))
			{
				checkboxVar = !checkboxVar;
				action?.Invoke();
				Event.current.Use();
			}
		}
	}
}
