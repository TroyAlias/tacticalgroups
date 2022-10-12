using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class Dialog_PresetSave : Dialog_Rename
	{
		private readonly GroupPreset groupPreset;
		public override void SetInitialSizeAndPosition()
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
			closeOnClickedOutside = true;
		}

		protected override AcceptanceReport NameIsValid(string name)
		{
			return true;
		}

		protected override void SetName(string name)
		{
			groupPreset.ClearSettings();
			groupPreset.CopySettingsFrom(colonistGroup);
			GroupPresetSaveable saveable = TacticalGroupsSettings.AllGroupPresetsSaveable.FirstOrDefault(x => x.GetUniqueLoadID() == groupPreset.GetUniqueLoadID());
			TacticalGroupsSettings.AllGroupPresetsSaveable.Remove(saveable);
			if (!name.NullOrEmpty())
			{
				groupPreset.name = name;
			}
			TacticalGroupsSettings.AllGroupPresetsSaveable.Add(groupPreset.SaveToSaveable());
		}
	}
}
