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
        public static bool WeaponOverlayInside = true;

        public static float MarginTop = 21f;
        public static float PawnScale = 1f;
        public static float GroupScale = 1f;
        public static int GroupRowCount = 4;
        public static float PawnNeedsWidth = 4f;


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
            Scribe_Values.Look(ref WeaponOverlayInside, "WeaponOverlayInside", true);
            Scribe_Values.Look(ref MarginTop, "MarginTop", 21f);
            Scribe_Values.Look(ref PawnScale, "PawnScale", 1f);
            Scribe_Values.Look(ref GroupScale, "GroupScale", 1f);
            Scribe_Values.Look(ref GroupRowCount, "GroupRowCount", 4);
            Scribe_Values.Look(ref PawnNeedsWidth, "PawnNeedsWidth", 4f);
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
            listingStandard.Label(Strings.WeaponOverlayPlacement);
            if (listingStandard.RadioButton_NewTemp(Strings.WeaponOverlayInside, WeaponOverlayInside))
            {
                WeaponOverlayInside = true;
            }
            else if (listingStandard.RadioButton_NewTemp(Strings.WeaponOverlayUnder, !WeaponOverlayInside))
            {
                WeaponOverlayInside = false;
            }
            listingStandard.CheckboxLabeled(Strings.DisplayColorBars, ref DisplayColorBars);
            listingStandard.CheckboxLabeled(Strings.HidePawnsWhenOffMap, ref HidePawnsWhenOffMap);
            listingStandard.CheckboxLabeled(Strings.HideGroups, ref HideGroups);
            listingStandard.CheckboxLabeled(Strings.HideCreateGroup, ref HideCreateGroup);
            listingStandard.SliderLabeled(Strings.TopMargin, ref MarginTop, MarginTop.ToStringDecimalIfSmall(), 0, 100);
            listingStandard.SliderLabeled(Strings.PawnScale, ref PawnScale, PawnScale.ToStringDecimalIfSmall(), 0.5f, 5f);
            listingStandard.SliderLabeled(Strings.GroupScale, ref GroupScale, GroupScale.ToStringDecimalIfSmall(), 0.5f, 5f);
            listingStandard.SliderLabeled(Strings.GroupRowCount, ref GroupRowCount, GroupRowCount.ToString(), 1, 12);
            listingStandard.SliderLabeled(Strings.PawnNeedsSize, ref PawnNeedsWidth, PawnNeedsWidth.ToString(), 1, 20);
            listingStandard.End();
            if (TacticUtils.TacticalGroups != null && TacticUtils.TacticalColonistBar != null)
            {
                TacticUtils.TacticalColonistBar.UpdateSizes();
                TacticUtils.TacticalColonistBar.MarkColonistsDirty();
            }
            base.Write();
        }
        private static Vector2 scrollPosition = Vector2.zero;
    }
}
