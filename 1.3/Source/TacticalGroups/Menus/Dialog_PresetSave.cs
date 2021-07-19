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
	public class Dialog_PresetSave : Dialog_Rename
	{
		private GroupPreset groupPreset;
		protected override void SetInitialSizeAndPosition()
        {
			windowRect = new Rect(originRect.x, originRect.y, InitialSize.x, InitialSize.y);
			windowRect = windowRect.Rounded();
		}
		public Dialog_PresetSave(TieredFloatMenu parentWindow, GroupPreset groupPreset, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, string confirmationText)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture, confirmationText)
		{
			this.originRect = new Rect(originRect.x + originRect.width, originRect.y, originRect.width, originRect.height);
			this.colonistGroup = colonistGroup;
			this.groupPreset = groupPreset;
			curName = colonistGroup.groupName;
			this.closeOnClickedOutside = true;
		}

		protected override AcceptanceReport NameIsValid(string name)
		{
			return true;
		}

		protected override void SetName(string name)
		{
			groupPreset.ClearSettings();
			groupPreset.CopySettingsFrom(this.colonistGroup);
			var saveable = TacticalGroupsSettings.AllGroupPresetsSaveable.FirstOrDefault(x => x.GetUniqueLoadID() == groupPreset.GetUniqueLoadID());
			TacticalGroupsSettings.AllGroupPresetsSaveable.Remove(saveable);
			if (!name.NullOrEmpty())
			{
				groupPreset.name = name;
			}
			TacticalGroupsSettings.AllGroupPresetsSaveable.Add(groupPreset.SaveToSaveable());
		}
	}
}
