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
	public class Dialog_RenameColonistGroup : Dialog_Rename
	{
		private ColonistGroup colonistGroup;
		private Rect originRect;
		private TieredFloatMenuOption option;
		protected override void SetInitialSizeAndPosition()
        {
			windowRect = new Rect(originRect.x, originRect.y, InitialSize.x, InitialSize.y);
			windowRect = windowRect.Rounded();
		}
        public Dialog_RenameColonistGroup(ColonistGroup colonistGroup, Rect originRect, TieredFloatMenuOption option)
		{
			this.originRect = new Rect(originRect.x + originRect.width, originRect.y, originRect.width, originRect.height);
			this.colonistGroup = colonistGroup;

			curName = colonistGroup.groupName;
			this.option = option;
		}

		protected override AcceptanceReport NameIsValid(string name)
		{
			return true;
		}

		protected override void SetName(string name)
		{
			colonistGroup.groupName = name;
			option.selected = false;
		}
	}
}
