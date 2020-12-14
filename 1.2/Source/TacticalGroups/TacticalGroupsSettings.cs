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
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DisplayFood, "DisplayFood");
            Scribe_Values.Look(ref DisplayRest, "DisplayRest");
            Scribe_Values.Look(ref DisplayHealth, "DisplayHealth");
            Scribe_Values.Look(ref DisplayWeapons, "DisplayWeapons");
            Scribe_Values.Look(ref DisplayColorBars, "DisplayColorBars");
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
            listingStandard.End();
            base.Write();
        }
        private static Vector2 scrollPosition = Vector2.zero;
    }
}
