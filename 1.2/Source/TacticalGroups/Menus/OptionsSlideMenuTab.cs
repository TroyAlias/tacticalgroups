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
	public class OptionsSlideMenuTab : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(-219f, 165f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);


		public OptionsSlideMenuTab(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			var gearButtonRect = new Rect(rect.x + 3, rect.y + 10, rect.width - 3, 14);
			if (Mouse.IsOver(gearButtonRect))
			{
				GUI.DrawTexture(gearButtonRect, Textures.RescueTendHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					if (this.childWindow != null)
                    {
						this.childWindow.Close();
						this.childWindow = null;
                    }
					else
                    {
						this.childWindow = new OptionsSlideMenu(this, this.colonistGroup, windowRect, Textures.OptionsSlideMenu);
						Find.WindowStack.Add(this.childWindow);
					}
				}
			}
		}
	}
}
