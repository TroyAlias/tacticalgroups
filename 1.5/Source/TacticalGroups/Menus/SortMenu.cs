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
	public class SortMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);

		public SortMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();

			var noneOption = new TieredFloatMenuOption("None".Translate(), null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			noneOption.action = delegate
			{
				TacticDefOf.TG_SortOptionsSFX.PlayOneShotOnCamera();
				this.colonistGroup.activeSortBy = SortBy.None;
			};
			noneOption.extraPartOnGUI = delegate (Rect rect)
			{
				if (this.colonistGroup.activeSortBy == SortBy.None)
				{
					Widgets.DrawBox(rect);
				}
				return false;
			};
			noneOption.bottomIndent = Textures.MenuButton.height + 4;
			options.Add(noneOption);

			var nameOption = new TieredFloatMenuOption("TG.Name".Translate(), null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			nameOption.action = delegate
			{
				TacticDefOf.TG_SortOptionsSFX.PlayOneShotOnCamera();
				this.colonistGroup.activeSortBy = SortBy.Name;
				this.colonistGroup.InitSort(this.colonistGroup.activeSortBy);
			};
			nameOption.extraPartOnGUI = delegate (Rect rect)
			{
				if (this.colonistGroup.activeSortBy == SortBy.Name)
				{
					Widgets.DrawBox(rect);
				}
				return false;
			};
			nameOption.bottomIndent = Textures.MenuButton.height + 4;
			options.Add(nameOption);

			foreach (var skillDef in DefDatabase<SkillDef>.AllDefs)
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
			var option = new SkillSortFloatMenuOption(skillDef, skillDef.LabelCap, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleCenter,
				MenuOptionPriority.High, 0f, -1f);
			option.action = delegate
			{
				TacticDefOf.TG_SortOptionsSFX.PlayOneShotOnCamera();
				this.colonistGroup.skillDefSort = skillDef;
				this.colonistGroup.InitSort(SortBy.Skills);
			};
			option.extraPartOnGUI = delegate (Rect rect)
			{
				if (this.colonistGroup.skillDefSort == option.skillDef)
                {
					Widgets.DrawBox(rect);
                }
				return false;
			};
			option.bottomIndent = Textures.MenuButton.height + 4;
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
				Rect rect2 = new Rect(zero.x, zero.y, (this.backgroundTexture.width - InitialFloatOptionPositionShift.x) / 1.2f, floatMenuOption.curIcon.height);
				
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
