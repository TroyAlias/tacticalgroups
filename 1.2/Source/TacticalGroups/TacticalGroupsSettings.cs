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
        public static bool DisplayBreakRiskOverlay;

        public static bool HidePawnsWhenOffMap;
        public static bool HideGroups;
        public static bool HideCreateGroup;
        public static bool DisableLabelBackground;

        public static float ColonistBarPositionY = 21f;
        public static float ColonistBarPositionX = 24f;
        public static float ColonistBarSpacing = 20f;

        //public static float OverallPawnIconScale = 1f;
        //public static float PawnBoxScale = 1f;


        public static float PawnScale = 1f;
        public static float XPawnIconOffset = 0f;
        public static float YPawnIconOffset = 0f;
        public static float PawnBoxHeight = 48f;
        public static float PawnBoxWidth = 48f;
        public static float PawnCameraOffsetX = 0f;
        public static float PawnCameraOffsetZ = 0.3f;

        public static float ColonyGroupScale = 1f;
        public static float GroupScale = 1f;
        public static int GroupRowCount = 4;
        public static int SubGroupRowCount = 4;
        public static float PawnNeedsWidth = 4f;
        public static int WeaponPlacementOffset = 10;
        public static ColorBarMode ColorBarMode = ColorBarMode.Default;
        public static WeaponShowMode WeaponShowMode = WeaponShowMode.Drafted;

        public static List<GroupPresetSaveable> AllGroupPresetsSaveable;

        private static List<GroupPreset> allGroupPresets;
        public static List<GroupPreset> AllGroupPresets
        {
            get
            {
                if (allGroupPresets is null)
                {
                    allGroupPresets = new List<GroupPreset>();
                    if (AllGroupPresetsSaveable != null)
                    {
                        foreach (var group in AllGroupPresetsSaveable)
                        {
                            allGroupPresets.Add(group.LoadFromSaveable());
                        }
                    }
                }
                return allGroupPresets;
            }
        }

        public static void AddGroupPreset(GroupPreset groupPreset)
        {
            var allGroupPresets = AllGroupPresets;
            if (AllGroupPresetsSaveable is null) AllGroupPresetsSaveable = new List<GroupPresetSaveable>();
            AllGroupPresetsSaveable.Add(groupPreset.SaveToSaveable());
            allGroupPresets.Add(groupPreset);
        }

        public static void RemoveGroupPreset(GroupPreset groupPreset)
        {
            var allGroupPresets = AllGroupPresets;
            allGroupPresets.Remove(groupPreset);
            var groupSaveable = AllGroupPresetsSaveable.FirstOrDefault(x => x.GetUniqueLoadID() == groupPreset.GetUniqueLoadID());
            if (groupSaveable != null)
            {
                AllGroupPresetsSaveable.Remove(groupSaveable);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DisplayFood, "DisplayFood");
            Scribe_Values.Look(ref DisplayRest, "DisplayRest");
            Scribe_Values.Look(ref DisplayHealth, "DisplayHealth");
            Scribe_Values.Look(ref DisplayWeapons, "DisplayWeapons");
            Scribe_Values.Look(ref DisplayColorBars, "DisplayColorBars");
            Scribe_Values.Look(ref DisplayBreakRiskOverlay, "DisplayBreakRiskOverlay");
            Scribe_Values.Look(ref HidePawnsWhenOffMap, "HidePawnsWhenOffMap");
            Scribe_Values.Look(ref HideGroups, "HideGroups");
            Scribe_Values.Look(ref HideCreateGroup, "HideCreateGroup");
            Scribe_Values.Look(ref DisableLabelBackground, "DisableLabelBackground");
            Scribe_Values.Look(ref WeaponPlacementOffset, "WeaponPlacementOffset", 10);
            Scribe_Values.Look(ref ColonistBarPositionY, "MarginTop", 21f);
            Scribe_Values.Look(ref ColonistBarPositionX, "ColonistBarPositionX", 24f);
            Scribe_Values.Look(ref ColonistBarSpacing, "ColonistBarSpacing", 20f);

            Scribe_Values.Look(ref ColonyGroupScale, "ColonyGroupScale", 1f);
            Scribe_Values.Look(ref GroupScale, "GroupScale", 1f);
            Scribe_Values.Look(ref GroupRowCount, "GroupRowCount", 4);
            Scribe_Values.Look(ref PawnNeedsWidth, "PawnNeedsWidth", 4f);
            //Scribe_Values.Look(ref OverallPawnIconScale, "OverallPawnIconScale", 1f);
            //Scribe_Values.Look(ref PawnScale, "PawnScale", 1f);
            //Scribe_Values.Look(ref PawnBoxScale, "PawnBoxScale", 1f);

            Scribe_Values.Look(ref XPawnIconOffset, "XPawnIconOffset", 0f);
            Scribe_Values.Look(ref YPawnIconOffset, "YPawnIconOffset", 0f);
            Scribe_Values.Look(ref PawnBoxHeight, "PawnBoxHeight", 48f);
            Scribe_Values.Look(ref PawnBoxWidth, "PawnBoxWidth", 48f);
            Scribe_Values.Look(ref PawnCameraOffsetX, "PawnCameraOffsetX", 0f);
            Scribe_Values.Look(ref PawnCameraOffsetZ, "PawnCameraOffsetZ", 0.3f);
            Scribe_Values.Look(ref PawnScale, "PawnScale", 1f);

            Scribe_Values.Look(ref ColorBarMode, "ColorBarMode", ColorBarMode.Default);
            Scribe_Values.Look(ref WeaponShowMode, "WeaponShowMode", WeaponShowMode.Drafted);
            Scribe_Collections.Look(ref AllGroupPresetsSaveable, "AllGroupPresetsSaveable", LookMode.Deep);
        }

        public static void DoReset()
        {
            DisplayFood = false;
            DisplayRest = false;
            DisplayHealth = false;
            DisplayWeapons = false;
            DisplayColorBars = false;
            DisplayBreakRiskOverlay = false;
            HidePawnsWhenOffMap = false;
            HideGroups = false;
            HideCreateGroup = false;
            DisableLabelBackground = false;

            ColonistBarPositionY = 21f;
            ColonistBarPositionX = 24f;
            ColonistBarSpacing = 20f;

            DoPawnViewReset();

            ColonyGroupScale = 1f;
            GroupScale = 1f;
            GroupRowCount = 4;
            PawnNeedsWidth = 4f;
            WeaponPlacementOffset = 10;
            ColorBarMode = ColorBarMode.Default;
            WeaponShowMode = WeaponShowMode.Drafted;
        }

        public static void DoPawnViewReset()
        {
            PawnScale = 1f;
            XPawnIconOffset = 0f;
            YPawnIconOffset = 0f;
            PawnBoxHeight = 48f;
            PawnBoxWidth = 48f;
            PawnCameraOffsetX = 0f;
            PawnCameraOffsetZ = 0.3f;
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
            listingStandard.SliderLabeled(Strings.WeaponOverlayPlacement, ref WeaponPlacementOffset, WeaponPlacementOffset.ToString(), 0, 100);
            listingStandard.CheckboxLabeled(Strings.DisplayColorBars, ref DisplayColorBars);
            listingStandard.CheckboxLabeled(Strings.HidePawnsWhenOffMap, ref HidePawnsWhenOffMap);
            listingStandard.CheckboxLabeled(Strings.HideGroups, ref HideGroups);
            listingStandard.CheckboxLabeled(Strings.HideCreateGroup, ref HideCreateGroup);
            listingStandard.CheckboxLabeled(Strings.DisableLabelBackground, ref DisableLabelBackground);
            listingStandard.SliderLabeled(Strings.ColonistBarPositionY, ref ColonistBarPositionY, ColonistBarPositionY.ToStringDecimalIfSmall(), 0, 100);
            listingStandard.SliderLabeled(Strings.ColonistBarPositionX, ref ColonistBarPositionX, ColonistBarPositionX.ToStringDecimalIfSmall(), 0, 100);
            listingStandard.SliderLabeled(Strings.ColonistBarSpacing, ref ColonistBarSpacing, ColonistBarSpacing.ToStringDecimalIfSmall(), 0, 100);
            //listingStandard.SliderLabeled(Strings.OverallPawnDisplayScale, ref PawnScale, PawnScale.ToStringDecimalIfSmall(), 0.5f, 5f);
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
