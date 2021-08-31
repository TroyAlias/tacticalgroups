using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TacticalGroups
{
    public class SortMenu : TieredFloatMenu
    {
        protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
        protected override Vector2 InitialFloatOptionPositionShift => new Vector2(backgroundTexture.width / 10, 25f);

        public SortMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
            : base(parentWindow, colonistGroup, originRect, backgroundTexture)
        {
            options = new List<TieredFloatMenuOption>();

            TieredFloatMenuOption noneOption = new TieredFloatMenuOption("None".Translate(), null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f)
            {
                action = delegate
                {
                    TacticDefOf.TG_SortOptionsSFX.PlayOneShotOnCamera();
                    this.colonistGroup.activeSortBy = SortBy.None;
                },
                bottomIndent = Textures.MenuButton.height + 4
            };
            options.Add(noneOption);

            TieredFloatMenuOption nameOption = new TieredFloatMenuOption("TG.Name".Translate(), null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f)
            {
                action = delegate
                {
                    TacticDefOf.TG_SortOptionsSFX.PlayOneShotOnCamera();
                    this.colonistGroup.activeSortBy = SortBy.Name;
                    this.colonistGroup.InitSort(this.colonistGroup.activeSortBy);
                },
                bottomIndent = Textures.MenuButton.height + 4
            };
            options.Add(nameOption);

            foreach (SkillDef skillDef in DefDatabase<SkillDef>.AllDefs)
            {
                AddSkillSortButton(skillDef);
            }
            for (int i = 0; i < options.Count; i++)
            {
                options[i].SetSizeMode(SizeMode);
            }
        }

        public void AddSkillSortButton(SkillDef skillDef)
        {
            SkillSortFloatMenuOption option = new SkillSortFloatMenuOption(skillDef, skillDef.LabelCap, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleCenter,
                MenuOptionPriority.High, 0f, -1f)
            {
                action = delegate
                {
                    TacticDefOf.TG_SortOptionsSFX.PlayOneShotOnCamera();
                    colonistGroup.skillDefSort = skillDef;
                    colonistGroup.InitSort(SortBy.Skills);
                },
                bottomIndent = Textures.MenuButton.height + 4
            };
            options.Add(option);
        }

        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);
            Vector2 zero = Vector2.zero;
            zero += InitialFloatOptionPositionShift;
            for (int i = 0; i < options.Count; i++)
            {
                TieredFloatMenuOption floatMenuOption = options[i];
                Rect rect2 = new Rect(zero.x, zero.y, (backgroundTexture.width - InitialFloatOptionPositionShift.x) / 1.2f, floatMenuOption.curIcon.height);

                if (floatMenuOption.DoGUI(rect2, this))
                {
                    Find.WindowStack.TryRemove(this);
                    break;
                }
                zero.y += floatMenuOption.bottomIndent;
            }
            DrawExtraGui(rect);
            GUI.color = Color.white;
        }

    }
}
