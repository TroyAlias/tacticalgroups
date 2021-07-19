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
	public class Dialog_NewPresetName : Dialog_Rename
	{
		private GroupPreset groupPreset;
		protected override void SetInitialSizeAndPosition()
        {
			windowRect = new Rect(originRect.x, originRect.y, InitialSize.x, InitialSize.y);
			windowRect = windowRect.Rounded();
		}
		public Dialog_NewPresetName(TieredFloatMenu parentWindow, GroupPreset groupPreset, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, string confirmationText)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture, confirmationText)
		{
			this.originRect = new Rect(originRect.x + originRect.width, originRect.y, originRect.width, originRect.height);
			this.colonistGroup = colonistGroup;
			this.groupPreset = groupPreset;
			curName = colonistGroup.groupName;
		}

		protected override AcceptanceReport NameIsValid(string name)
		{
			return true;
		}

		protected override void SetName(string name)
		{
			if (!name.NullOrEmpty())
            {
				groupPreset.SetName(name);
				TacticalGroupsSettings.AddGroupPreset(groupPreset);
				TacticalGroupsMod.instance.WriteSettings();
			}
			TacticDefOf.TG_RenameSFX.PlayOneShotOnCamera();
		}
	}
}
