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
	public class OptionsMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(-45f, 35f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);
		public OptionsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Text.Anchor = TextAnchor.LowerLeft;
			Vector2 topLeft = new Vector2(rect.x + (rect.width - 30f), rect.y + 25f);


			var showAllColonistsRect = new Rect(rect.x + 10, topLeft.y, Textures.MenuButton.width, 25f);
			GUI.DrawTexture(showAllColonistsRect, Textures.MenuButton);
			if (Mouse.IsOver(showAllColonistsRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					foreach (var group in TacticUtils.Groups)
                    {
						group.entireGroupIsVisible = true;
						foreach (var pawn in group.pawnIcons)
                        {
							pawn.Value.isVisibleOnColonistBar = true;
                        }
                    }
					Event.current.Use();
				}
			}
			Widgets.Label(new Rect(rect.x + 20, topLeft.y, rect.width, 25f), Strings.ShowAllColonists);

			topLeft.y += 35f;
			Widgets.Checkbox(topLeft, ref TacticalGroupsSettings.DisplayFood);
			Widgets.Label(new Rect(rect.x + 20, topLeft.y, rect.width, 25f), Strings.DisplayFood);

			topLeft.y += 25f;
			Widgets.Checkbox(topLeft, ref TacticalGroupsSettings.DisplayRest);
			Widgets.Label(new Rect(rect.x + 20, topLeft.y, rect.width, 25f), Strings.DisplayRest);

			topLeft.y += 25f;
			Widgets.Checkbox(topLeft, ref TacticalGroupsSettings.DisplayHealth);
			Widgets.Label(new Rect(rect.x + 20, topLeft.y, rect.width, 25f), Strings.DisplayHealth);

			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			var mod = LoadedModManager.GetMod(typeof(TacticalGroupsMod));
			mod.WriteSettings();
			TacticUtils.TacticalColonistBar.MarkColonistsDirty();
		}
	}
}
