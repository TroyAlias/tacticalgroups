using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

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
            if (colonistGroup.formations is null || !colonistGroup.formations.Any() || colonistGroup.formations.Count != 4)
            {
                colonistGroup.formations = new List<Formation>();
                for (int i = 0; i < 4; i++)
                {
                    colonistGroup.formations.Add(new Formation());
                }
            }
            Vector2 pos = Vector2.zero;
            if (colonistGroup.activeFormation is null)
            {
                colonistGroup.activeFormation = colonistGroup.formations[0];
                colonistGroup.activeFormation.isSelected = true;
            }
            foreach (Formation formation in colonistGroup.formations)
            {
                Texture2D texture = formation.Icon;
                Rect battleStation = new Rect(formation.isSelected ? pos.x + 2 : pos.x + 5f, pos.y, texture.width, texture.height);
                GUI.DrawTexture(battleStation, texture);

                if (formation.formations?.Any() ?? false)
                {
                    string tooltip = "";
                    foreach (KeyValuePair<Pawn, IntVec3> pawn in formation.formations)
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
                        if (colonistGroup.activeFormation != null)
                        {
                            colonistGroup.activeFormation.isSelected = false;
                        }
                        colonistGroup.activeFormation = formation;
                        formation.isSelected = true;
                    }
                }
                pos.y += texture.height + 2;
            }
        }
    }
}
