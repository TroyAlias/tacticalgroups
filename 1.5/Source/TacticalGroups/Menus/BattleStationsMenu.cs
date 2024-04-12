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
	public class BattleStationsMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(-224f, 165f);
		public BattleStationsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			if (this.colonistGroup.formations is null || !this.colonistGroup.formations.Any() || this.colonistGroup.formations.Count != 4)
            {
				this.colonistGroup.formations = new List<Formation>();
				for (var i = 0; i < 4; i++)
                {
					this.colonistGroup.formations.Add(new Formation());
				}
			}
			Vector2 pos = Vector2.zero;
			if (this.colonistGroup.activeFormation is null)
            {
				this.colonistGroup.activeFormation = this.colonistGroup.formations[0];
				this.colonistGroup.activeFormation.isSelected = true;
			}
			foreach (var formation in this.colonistGroup.formations)
            {
				var texture = formation.Icon;
				Rect battleStation = new Rect(formation.isSelected ? pos.x + 2 : pos.x + 5f, pos.y, texture.width, texture.height);
				GUI.DrawTexture(battleStation, texture);

				if (formation.formations?.Any() ?? false)
                {
					var tooltip = "";
					foreach (var pawn in formation.formations)
					{
						tooltip += pawn.Key.LabelShortCap + ": " + pawn.Value + "\n";
					}
					TooltipHandler.TipRegion(battleStation, tooltip);
				}

				if (Mouse.IsOver(battleStation))
				{
					GUI.DrawTexture(battleStation, Textures.RescueTendHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						if (this.colonistGroup.activeFormation != null)
                        {
							this.colonistGroup.activeFormation.isSelected = false;
						}
						this.colonistGroup.activeFormation = formation;
						formation.isSelected = true;
					}
				}
				pos.y += texture.height + 2;
			}
		}
	}
}
