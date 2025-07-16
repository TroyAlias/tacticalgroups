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
        public static TacticalGroupsMod instance;
        public TacticalGroupsMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<TacticalGroupsSettings>();
            instance = this;
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
        public override string SettingsCategory()
        {
            return "Colony Groups";
        }
    }
}
