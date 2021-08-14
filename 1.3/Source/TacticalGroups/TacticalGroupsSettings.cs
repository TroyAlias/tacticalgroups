using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    [StaticConstructorOnStartup]
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
        public static float ColonistBarSpacingX = 20f;
        public static float ColonistBarSpacingY = 32f;

        public static float PawnScale = 1f;
        public static float XPawnIconOffset = 0f;
        public static float YPawnIconOffset = 0f;
        public static float PawnBoxHeight = 48f;
        public static float PawnBoxWidth = 48f;
        public static float PawnCameraOffsetX = 0f;
        public static float PawnCameraOffsetZ = 0.3f;

        public static float ColonyGroupScale = 0.8f;
        public static float GroupScale = 0.7f;
        public static int GroupRowCount = 4;
        public static int SubGroupRowCount = 4;
        public static float PawnNeedsWidth = 4f;
        public static float HealthBarWidth = 6f;
        public static int WeaponPlacementOffset = -10;
        public static ColorBarMode ColorBarMode = ColorBarMode.Default;
        public static WeaponShowMode WeaponShowMode = WeaponShowMode.Drafted;
        public static float WeaponShowScale = 1f;


        public static Color DefaultMoodBarLower = new ColorInt(196, 0, 30, 255).ToColor;
        public static Color DefaultMoodBarMiddle = new ColorInt(245, 194, 15, 255).ToColor;
        public static Color DefaultMoodBarUpper = new ColorInt(9, 237, 90, 255).ToColor;

        public static Color ExtendedMoodBarLowerII = new ColorInt(100, 45, 50, 255).ToColor;
        public static Color ExtendedMoodBarLower = new ColorInt(91, 92, 61, 255).ToColor;
        public static Color ExtendedMoodBarMiddle = new ColorInt(245, 194, 15, 255).ToColor;
        public static Color ExtendedMoodBarUpper = new ColorInt(61, 119, 140, 255).ToColor;
        public static Color ExtendedMoodBarUpperII = new ColorInt(9, 237, 90, 255).ToColor;

        public static Texture2D DefaultMoodBarLowerBar;
        public static Texture2D DefaultMoodBarMiddleBar;
        public static Texture2D DefaultMoodBarUpperBar;

        public static Texture2D ExtendedMoodBarLowerIIBar;
        public static Texture2D ExtendedMoodBarLowerBar;
        public static Texture2D ExtendedMoodBarMiddleBar;
        public static Texture2D ExtendedMoodBarUpperBar;
        public static Texture2D ExtendedMoodBarUpperIIBar;

        public static Color NeedFoodBarColor = new ColorInt(45, 127, 59, 255).ToColor;
        public static Color NeedRestBarColor = new ColorInt(58, 96, 152, 255).ToColor;
        public static Color NeedHealthBarColor = new ColorInt(154, 55, 55, 255).ToColor;

        public static Texture2D FoodNeedBar;
        public static Texture2D RestNeedBar;
        public static Texture2D HealthNeedBar;

        public static bool OverridePawnRowCount;
        public static int PawnRowCount = 2;

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
            Scribe_Values.Look(ref WeaponPlacementOffset, "WeaponPlacementOffset", -10);
            Scribe_Values.Look(ref ColonistBarPositionY, "MarginTop", 21f);
            Scribe_Values.Look(ref ColonistBarPositionX, "ColonistBarPositionX", 24f);
            Scribe_Values.Look(ref ColonistBarSpacingX, "ColonistBarSpacingX", 20f);
            Scribe_Values.Look(ref ColonistBarSpacingY, "ColonistBarSpacingY", 32f);

            Scribe_Values.Look(ref ColonyGroupScale, "ColonyGroupScale", 0.8f);
            Scribe_Values.Look(ref GroupScale, "GroupScale", 0.70f);
            Scribe_Values.Look(ref GroupRowCount, "GroupRowCount", 4);
            Scribe_Values.Look(ref PawnNeedsWidth, "PawnNeedsWidth", 4f);
            Scribe_Values.Look(ref HealthBarWidth, "HealthBarWidth", 6f);

            Scribe_Values.Look(ref XPawnIconOffset, "XPawnIconOffset", 0f);
            Scribe_Values.Look(ref YPawnIconOffset, "YPawnIconOffset", 0f);
            Scribe_Values.Look(ref PawnBoxHeight, "PawnBoxHeight", 48f);
            Scribe_Values.Look(ref PawnBoxWidth, "PawnBoxWidth", 48f);
            Scribe_Values.Look(ref PawnCameraOffsetX, "PawnCameraOffsetX", 0f);
            Scribe_Values.Look(ref PawnCameraOffsetZ, "PawnCameraOffsetZ", 0.3f);
            Scribe_Values.Look(ref PawnScale, "PawnScale", 1f);

            Scribe_Values.Look(ref ColorBarMode, "ColorBarMode", ColorBarMode.Default);
            Scribe_Values.Look(ref WeaponShowMode, "WeaponShowMode", WeaponShowMode.Drafted);
            Scribe_Values.Look(ref WeaponShowScale, "WeaponShowScale", 1f);

            Scribe_Values.Look(ref DefaultMoodBarLower, "DefaultMoodBarLower", new ColorInt(196, 0, 30, 255).ToColor);
            Scribe_Values.Look(ref DefaultMoodBarMiddle, "DefaultMoodBarMiddle", new ColorInt(245, 194, 15, 255).ToColor);
            Scribe_Values.Look(ref DefaultMoodBarUpper, "DefaultMoodBarUpper", new ColorInt(9, 237, 90, 255).ToColor);

            Scribe_Values.Look(ref ExtendedMoodBarLowerII, "ExtendedMoodBarLowerII", new ColorInt(100, 45, 50, 255).ToColor);
            Scribe_Values.Look(ref ExtendedMoodBarLower, "ExtendedMoodBarLower", new ColorInt(91, 92, 61, 255).ToColor);
            Scribe_Values.Look(ref ExtendedMoodBarMiddle, "ExtendedMoodBarMiddle", new ColorInt(245, 194, 15, 255).ToColor);
            Scribe_Values.Look(ref ExtendedMoodBarUpper, "ExtendedMoodBarUpper", new ColorInt(61, 119, 140, 255).ToColor);
            Scribe_Values.Look(ref ExtendedMoodBarUpperII, "ExtendedMoodBarUpperII", new ColorInt(9, 237, 90, 255).ToColor);

            Scribe_Values.Look(ref NeedFoodBarColor, "NeedFoodBarColor", new ColorInt(45, 127, 59, 255).ToColor);
            Scribe_Values.Look(ref NeedRestBarColor, "NeedRestBarColor", new ColorInt(58, 96, 152, 255).ToColor);
            Scribe_Values.Look(ref NeedHealthBarColor, "NeedHealthBarColor", new ColorInt(154, 55, 55, 255).ToColor);

            Scribe_Values.Look(ref OverridePawnRowCount, "OverridePawnRowCount", false);
            Scribe_Values.Look(ref PawnRowCount, "PawnRowCount", 2);

            Scribe_Collections.Look(ref AllGroupPresetsSaveable, "AllGroupPresetsSaveable", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                InitColorBars();
            }
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
            ColonistBarSpacingX = 20f;
            ColonistBarSpacingY = 32f;
            DoPawnViewReset();

            ColonyGroupScale = 0.8f;
            GroupScale = 0.7f;
            GroupRowCount = 4;
            PawnNeedsWidth = 4f;
            HealthBarWidth = 6f;
            WeaponPlacementOffset = 10;
            ColorBarMode = ColorBarMode.Default;
            WeaponShowMode = WeaponShowMode.Drafted;
            WeaponShowScale = 1f;

            ResetColorBars();
            InitColorBars();

            OverridePawnRowCount = false;
            PawnRowCount = 2;
        }

        public static void ResetColorBars()
        {
            DefaultMoodBarLower = new ColorInt(196, 0, 30, 255).ToColor;
            DefaultMoodBarMiddle = new ColorInt(245, 194, 15, 255).ToColor;
            DefaultMoodBarUpper = new ColorInt(9, 237, 90, 255).ToColor;

            ExtendedMoodBarLowerII = new ColorInt(100, 45, 50, 255).ToColor;
            ExtendedMoodBarLower = new ColorInt(91, 92, 61, 255).ToColor;
            ExtendedMoodBarMiddle = new ColorInt(245, 194, 15, 255).ToColor;
            ExtendedMoodBarUpper = new ColorInt(61, 119, 140, 255).ToColor;
            ExtendedMoodBarUpperII = new ColorInt(9, 237, 90, 255).ToColor;

            NeedFoodBarColor = new ColorInt(45, 127, 59, 255).ToColor;
            NeedRestBarColor = new ColorInt(58, 96, 152, 255).ToColor;
            NeedHealthBarColor = new ColorInt(154, 55, 55, 255).ToColor;
        }
    public static void InitColorBars()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                DefaultMoodBarLowerBar = SolidColorMaterials.NewSolidColorTexture(DefaultMoodBarLower);
                DefaultMoodBarMiddleBar = SolidColorMaterials.NewSolidColorTexture(DefaultMoodBarMiddle);
                DefaultMoodBarUpperBar = SolidColorMaterials.NewSolidColorTexture(DefaultMoodBarUpper);

                ExtendedMoodBarLowerIIBar = SolidColorMaterials.NewSolidColorTexture(ExtendedMoodBarLowerII);
                ExtendedMoodBarLowerBar = SolidColorMaterials.NewSolidColorTexture(ExtendedMoodBarLower);
                ExtendedMoodBarMiddleBar = SolidColorMaterials.NewSolidColorTexture(ExtendedMoodBarMiddle);
                ExtendedMoodBarUpperBar = SolidColorMaterials.NewSolidColorTexture(ExtendedMoodBarUpper);
                ExtendedMoodBarUpperIIBar = SolidColorMaterials.NewSolidColorTexture(ExtendedMoodBarUpperII);

                FoodNeedBar = SolidColorMaterials.NewSolidColorTexture(NeedFoodBarColor);
                RestNeedBar = SolidColorMaterials.NewSolidColorTexture(NeedRestBarColor);
                HealthNeedBar = SolidColorMaterials.NewSolidColorTexture(NeedHealthBarColor);
            });
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

        [TweakValue("0CG", 0, 2000)] public static float yTest = 10;
        [TweakValue("0CG", 0, 2000)] public static float width = 10;
        [TweakValue("0CG", 0, 2000)] public static float height = 10;
        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            if (listingStandard.ButtonText(Strings.ResetToDefault))
            {
                DoReset();
            }
            if (listingStandard.ButtonText(Strings.ResetPawnView))
            {
                DoPawnViewReset();
            }

            var optionsText = new Rect(rect.x, 80, inRect.width, 24f);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(optionsText, Strings.ModSettingsText);
            Text.Anchor = TextAnchor.UpperLeft;

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
