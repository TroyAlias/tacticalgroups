using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    class TacticalGroupsMod : Mod
    {
        public static TacticalGroupsSettings settings;
        public TacticalGroupsMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<TacticalGroupsSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Colony Groups";
        }
    }
}
