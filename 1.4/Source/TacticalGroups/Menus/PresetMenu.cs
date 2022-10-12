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
	public class PresetMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(-388f, -165f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);
		public PresetMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleLeft;
			var groupsLabel = new Rect(rect.x + 50, rect.y + 30, 80, 30);
			Widgets.Label(groupsLabel, Strings.GroupsLabel);
			Text.Font = GameFont.Small;

			Text.Anchor = TextAnchor.MiddleCenter;
			Rect createNew = new Rect(groupsLabel.xMax + 5, groupsLabel.y, 100, groupsLabel.height);
			GUI.DrawTexture(createNew, Textures.AOMButton);
			Widgets.Label(createNew, Strings.CreateNew);
			TooltipHandler.TipRegion(createNew, Strings.CreateNewTooltip);
			if (Mouse.IsOver(createNew))
			{
				GUI.DrawTexture(createNew, Textures.RescueTendHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
					var newGroupPreset = new GroupPreset();
					newGroupPreset.id = TacticalGroupsSettings.AllGroupPresets.Count + 1;
					newGroupPreset.CopySettingsFrom(this.colonistGroup);
					TieredFloatMenu floatMenu = new Dialog_NewPresetName(this, newGroupPreset, this.colonistGroup, windowRect, Textures.RenameTab, Strings.CreateNewPreset);
					OpenNewMenu(floatMenu);
				}
			}

			Rect resetGroup = new Rect(createNew.xMax + 10, createNew.y, Textures.ResetIcon.width, Textures.ResetIcon.height);
			GUI.DrawTexture(resetGroup, Textures.ResetIcon);
			TooltipHandler.TipRegion(resetGroup, Strings.ResetGroupTooltip);
			if (Mouse.IsOver(resetGroup))
			{
				GUI.DrawTexture(resetGroup, Textures.RescueTendHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_SlideMenuOptionSFX.PlayOneShotOnCamera();
					TieredFloatMenu floatMenu = new Dialog_ResetGroup(this, this.colonistGroup, windowRect, Textures.ResetTab);
					OpenNewMenu(floatMenu);
				}
			}

			Text.Anchor = TextAnchor.UpperLeft;
			Vector2 pos = Vector2.zero;
			pos.y = createNew.yMax + 20;
			pos.x = 20;

			if (TacticalGroupsSettings.AllGroupPresets != null)
            {
				float listHeight = TacticalGroupsSettings.AllGroupPresets.Count() * 35f;
				Rect viewRect = new Rect(pos.x, pos.y, rect.width - 26f, (rect.height - pos.y) - 20);
				Rect scrollRect = new Rect(pos.x, pos.y, rect.width - 43f, listHeight);
				Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);

				var groupPresets = Enumerable.Reverse(TacticalGroupsSettings.AllGroupPresets).ToList();
				for (int num = groupPresets.Count - 1; num >= 0; num--)
                {
					var preset = groupPresets[num];
					Rect trashCan = new Rect(pos.x, pos.y, Textures.TrashCan.width, Textures.TrashCan.height);
					GUI.DrawTexture(trashCan, Textures.TrashCan);
					TooltipHandler.TipRegion(trashCan, Strings.TrashCanTooltip);
					if (Mouse.IsOver(trashCan))
					{
						GUI.DrawTexture(trashCan, Textures.RescueTendHover);
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
						{
							TieredFloatMenu floatMenu = new Dialog_PresetRemove(this, preset, this.colonistGroup, windowRect, Textures.DeleteTab);
							OpenNewMenu(floatMenu);
						}
					}

					Rect savePreset = new Rect(trashCan.xMax + 5, pos.y, Textures.SaveIcon.width, Textures.SaveIcon.height);
					GUI.DrawTexture(savePreset, Textures.SaveIcon);
					TooltipHandler.TipRegion(savePreset, Strings.SavePresetTooltip);
					if (Mouse.IsOver(savePreset))
					{
						GUI.DrawTexture(savePreset, Textures.RescueTendHover);
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
						{
							TieredFloatMenu floatMenu = new Dialog_PresetSave(this, preset, this.colonistGroup, windowRect, Textures.RenameTab, Strings.SavePreset);
							OpenNewMenu(floatMenu);
						}
					}

					Rect presetName = new Rect(savePreset.xMax + 5, pos.y, 155f, 35f);
					Text.Anchor = TextAnchor.UpperLeft;
					Widgets.Label(presetName, preset.name);
					var tooltip = "";
					if (preset.groupWorkPriorities != null && preset.groupWorkPriorities.Any())
                    {
						tooltip += Strings.PresetWorkPrioritiesTooltip + "\n";
						foreach (var workPriority in preset.groupWorkPriorities)
						{
							tooltip += workPriority.Key.labelShort + ": " + workPriority.Value + "\n";
						}
						tooltip += "--------------\n";
					}

					if (preset.activeWorkTypes != null && preset.activeWorkTypes.Any())
					{
						tooltip += Strings.PresetActiveWorkStatesTooltip + "\n";
						foreach (var activeWorkType in preset.activeWorkTypes)
						{
							tooltip += activeWorkType.Key.Label + ": " + ("TG." + activeWorkType.Value.ToString()).Translate() + "\n";
						}
						tooltip += "--------------\n";
					}

					if (preset.groupArea != null)
					{
						tooltip += Strings.PresetAreaTooltip + preset.groupArea.Label + "\n";
					}
					if (preset.groupOutfit != null)
					{
						tooltip += Strings.PresetOutfitPolicyTooltip + preset.groupOutfit.label + "\n";
					}

					if (preset.groupFoodRestriction != null)
					{
						tooltip += Strings.PresetFoodPolicyTooltip + preset.groupFoodRestriction.label + "\n";
					}

					if (preset.groupDrugPolicy != null)
					{
						tooltip += Strings.PresetDrugPolicyTooltip + preset.groupDrugPolicy.label + "\n";
					}
					if (tooltip.EndsWith("--------------\n"))
					{
						tooltip = tooltip.Remove(tooltip.Length - "--------------\n".Length);
					}
					TooltipHandler.TipRegion(presetName, tooltip);

					Rect applyPreset = new Rect(presetName.xMax + 5, pos.y, Textures.ApplyButton.width, Textures.ApplyButton.height);
					GUI.DrawTexture(applyPreset, Textures.ApplyButton);
					Text.Anchor = TextAnchor.MiddleCenter;
					Widgets.Label(applyPreset, Strings.Apply);
					TooltipHandler.TipRegion(applyPreset, Strings.ActivatePresetTooltip);
					if (Mouse.IsOver(applyPreset))
					{
						GUI.DrawTexture(applyPreset, Textures.RescueTendHover);
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
						{
							this.colonistGroup.ActivatePreset(preset);
						}
					}
					pos.y += 35f;
				}
			}

			Widgets.EndScrollView();
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;

		}

		private Vector2 scrollPosition;
	}
}
