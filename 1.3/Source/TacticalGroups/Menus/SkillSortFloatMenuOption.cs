using RimWorld;
using RimWorld.Planet;
using System;
using UnityEngine;
using Verse;

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
            labelInt = label;
            this.textAnchor = textAnchor;
            this.leftTextIndent = leftTextIndent;
            curIcon = icon;
            this.toolTip = toolTip;
            iconHover = hoverIcon;
            iconSelected = selectedIcon;
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
