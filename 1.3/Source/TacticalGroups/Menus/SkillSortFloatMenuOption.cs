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
	[StaticConstructorOnStartup]
	public class SkillSortFloatMenuOption : TieredFloatMenuOption
	{
		public SkillDef skillDef;
		public SkillSortFloatMenuOption(SkillDef skillDef, string label, Action<TieredFloatMenu> action, Texture2D icon, Texture2D hoverIcon, Texture2D selectedIcon, TextAnchor textAnchor = TextAnchor.MiddleCenter,
		MenuOptionPriority priority = MenuOptionPriority.Default, float leftTextIndent = 0f, float maxFloatMenuWidth = -1f, string toolTip = "", Action mouseoverGuiAction = null, Thing revalidateClickTarget = null,
		float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null) 
			: base(label, action, icon, hoverIcon, selectedIcon, textAnchor, priority, leftTextIndent, maxFloatMenuWidth, toolTip)
		{
			this.skillDef = skillDef;
			this.maxFloatMenuWidth = maxFloatMenuWidth;
			this.labelInt = label;
			this.textAnchor = textAnchor;
			this.leftTextIndent = leftTextIndent;
			this.curIcon = icon;
			this.toolTip = toolTip;
			this.iconHover = hoverIcon;
			this.iconSelected = selectedIcon;
			this.action = action;
			priorityInt = priority;
			this.revalidateClickTarget = revalidateClickTarget;
			this.mouseoverGuiAction = mouseoverGuiAction;
			this.extraPartWidth = extraPartWidth;
			this.extraPartOnGUI = extraPartOnGUI;
			this.revalidateWorldClickTarget = revalidateWorldClickTarget;
		}
	}
}
