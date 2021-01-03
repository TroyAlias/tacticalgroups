using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    class TacticalGroupsSettings : ModSettings
    {
        public static bool DisplayFood;
        public static bool DisplayRest;
        public static bool DisplayHealth;
        public static bool DisplayWeapons;
        public static bool DisplayColorBars;
        public static bool HidePawnsWhenOffMap;
        public static bool HideGroups;
        public static bool HideCreateGroup;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DisplayFood, "DisplayFood");
            Scribe_Values.Look(ref DisplayRest, "DisplayRest");
            Scribe_Values.Look(ref DisplayHealth, "DisplayHealth");
            Scribe_Values.Look(ref DisplayWeapons, "DisplayWeapons");
            Scribe_Values.Look(ref DisplayColorBars, "DisplayColorBars");
            Scribe_Values.Look(ref HidePawnsWhenOffMap, "HidePawnsWhenOffMap");
            Scribe_Values.Look(ref HideGroups, "HideGroups");
            Scribe_Values.Look(ref HideCreateGroup, "HideCreateGroup");
        }
        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.CheckboxLabeled(Strings.DisplayFood, ref DisplayFood);
            listingStandard.CheckboxLabeled(Strings.DisplayRest, ref DisplayRest);
            listingStandard.CheckboxLabeled(Strings.DisplayHealth, ref DisplayHealth);
            listingStandard.CheckboxLabeled(Strings.DisplayWeapons, ref DisplayWeapons);
            listingStandard.CheckboxLabeled(Strings.DisplayColorBars, ref DisplayColorBars);
            listingStandard.CheckboxLabeled(Strings.HidePawnsWhenOffMap, ref HidePawnsWhenOffMap);
            listingStandard.CheckboxLabeled(Strings.HideGroups, ref HideGroups);
            listingStandard.CheckboxLabeled(Strings.HideCreateGroup, ref HideCreateGroup);
            listingStandard.End();
            if (TacticUtils.TacticalColonistBar != null)
            {
                TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            }
            base.Write();
        }
        private static Vector2 scrollPosition = Vector2.zero;
    }
}
