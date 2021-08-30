using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TacticalGroups
{
    public class GroupPresetSaveable : IExposable
    {
        public int id;
        public string name;
        public string groupOutfitIndex;
        public string groupAreaIndex;
        public string groupDrugPolicyIndex;
        public string groupFoodRestrictionIndex;
        public Dictionary<string, WorkState> activeWorkTypes = new Dictionary<string, WorkState>();
        public Dictionary<string, int> groupWorkPrioritiesDefnames = new Dictionary<string, int>();
        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");
            Scribe_Values.Look(ref name, "name");
            Scribe_Collections.Look(ref activeWorkTypes, "activeWorkTypes", LookMode.Value, LookMode.Value, ref workTypesKeys, ref workStateValues);
            Scribe_Collections.Look(ref groupWorkPrioritiesDefnames, "groupWorkPriorities", LookMode.Value, LookMode.Value, ref workTypesDefKeys, ref intValues);
            Scribe_Values.Look(ref groupAreaIndex, "groupArea");
            Scribe_Values.Look(ref groupDrugPolicyIndex, "groupDrugPolicy");
            Scribe_Values.Look(ref groupFoodRestrictionIndex, "groupFoodRestriction");
            Scribe_Values.Look(ref groupOutfitIndex, "groupOutfit");
        }

        private List<string> workTypesKeys;
        private List<WorkState> workStateValues;

        private List<string> workTypesDefKeys;
        private List<int> intValues;

        public GroupPreset LoadFromSaveable()
        {
            GroupPreset groupPreset = new GroupPreset
            {
                name = name,
                id = id
            };
            if (!groupOutfitIndex.NullOrEmpty())
            {
                Outfit outfitPolicy = Current.Game.outfitDatabase.AllOutfits.FirstOrDefault(x => x.GetUniqueLoadID() == groupOutfitIndex);
                if (outfitPolicy != null)
                {
                    groupPreset.groupOutfit = outfitPolicy;
                }
            }

            if (!groupAreaIndex.NullOrEmpty())
            {
                Area area = GetArea();
                if (area != null)
                {
                    groupPreset.groupArea = area;
                }
            }

            if (!groupDrugPolicyIndex.NullOrEmpty())
            {
                DrugPolicy drugPolicy = Current.Game.drugPolicyDatabase.AllPolicies.FirstOrDefault(x => x.GetUniqueLoadID() == groupDrugPolicyIndex);
                if (drugPolicy != null)
                {
                    groupPreset.groupDrugPolicy = drugPolicy;
                }
            }

            if (!groupFoodRestrictionIndex.NullOrEmpty())
            {
                FoodRestriction foodRestriction = Current.Game.foodRestrictionDatabase.AllFoodRestrictions.FirstOrDefault(x => x.GetUniqueLoadID() == groupFoodRestrictionIndex);
                if (foodRestriction != null)
                {
                    groupPreset.groupFoodRestriction = foodRestriction;
                }
            }

            if (groupWorkPrioritiesDefnames != null && groupWorkPrioritiesDefnames.Any())
            {
                groupPreset.groupWorkPriorities = new Dictionary<WorkTypeDef, int>();
                foreach (KeyValuePair<string, int> data in groupWorkPrioritiesDefnames)
                {
                    WorkTypeDef workDef = DefDatabase<WorkTypeDef>.GetNamedSilentFail(data.Key);
                    if (workDef != null)
                    {
                        groupPreset.groupWorkPriorities[workDef] = data.Value;
                    }
                }
            }
            if (activeWorkTypes != null && activeWorkTypes.Any())
            {
                if (groupPreset.activeWorkTypes is null)
                {
                    groupPreset.activeWorkTypes = new Dictionary<WorkType, WorkState>();
                }
                foreach (KeyValuePair<string, WorkState> workTypeData in activeWorkTypes)
                {
                    WorkTypeDef def = DefDatabase<WorkTypeDef>.GetNamedSilentFail(workTypeData.Key);
                    if (def != null)
                    {

                        groupPreset.activeWorkTypes[new WorkType(def)] = workTypeData.Value;
                    }
                }
            }
            return groupPreset;
        }

        private Area GetArea()
        {
            foreach (Map map in Find.Maps)
            {
                foreach (Area area in map.areaManager.AllAreas)
                {
                    if (area.GetUniqueLoadID() == groupAreaIndex)
                    {
                        return area;
                    }
                }
            }
            return null;
        }

        public string GetUniqueLoadID()
        {
            return "GroupPreset_" + name + id;
        }
    }

    public class GroupPreset
    {
        public int id;
        public string name;

        public Outfit groupOutfit;
        public Area groupArea;
        public DrugPolicy groupDrugPolicy;
        public FoodRestriction groupFoodRestriction;
        public Dictionary<WorkType, WorkState> activeWorkTypes = new Dictionary<WorkType, WorkState>();
        public Dictionary<WorkTypeDef, int> groupWorkPriorities = new Dictionary<WorkTypeDef, int>();

        public GroupPreset()
        {

        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void ClearSettings()
        {
            activeWorkTypes = null;
            groupWorkPriorities = null;
            groupArea = null;
            groupDrugPolicy = null;
            groupFoodRestriction = null;
            groupOutfit = null;
        }
        public void CopySettingsFrom(ColonistGroup colonistGroup)
        {
            if (colonistGroup.activeWorkTypes?.Count > 0)
            {
                activeWorkTypes = colonistGroup.activeWorkTypes;
            }
            if (colonistGroup.groupWorkPriorities?.Count > 0)
            {
                groupWorkPriorities = colonistGroup.groupWorkPriorities;
            }
            if (colonistGroup.groupAreaEnabled && colonistGroup.groupArea != null)
            {
                groupArea = colonistGroup.groupArea;
            }
            if (colonistGroup.groupDrugPolicyEnabled && colonistGroup.groupDrugPolicy != null)
            {
                groupDrugPolicy = colonistGroup.groupDrugPolicy;
            }
            if (colonistGroup.groupFoodRestrictionEnabled && colonistGroup.groupFoodRestriction != null)
            {
                groupFoodRestriction = colonistGroup.groupFoodRestriction;
            }
            if (colonistGroup.groupOutfitEnabled && colonistGroup.groupOutfit != null)
            {
                groupOutfit = colonistGroup.groupOutfit;
            }
        }

        public GroupPresetSaveable SaveToSaveable()
        {
            GroupPresetSaveable groupPresetSaveable = new GroupPresetSaveable
            {
                id = id,
                name = name
            };
            if (groupOutfit != null)
            {
                groupPresetSaveable.groupOutfitIndex = groupOutfit.GetUniqueLoadID();
            }

            if (groupArea != null)
            {
                groupPresetSaveable.groupAreaIndex = groupArea.GetUniqueLoadID();
            }

            if (groupDrugPolicy != null)
            {
                groupPresetSaveable.groupDrugPolicyIndex = groupDrugPolicy.GetUniqueLoadID();
            }

            if (groupFoodRestriction != null)
            {
                groupPresetSaveable.groupDrugPolicyIndex = groupFoodRestriction.GetUniqueLoadID();
            }

            if (groupWorkPriorities != null && groupWorkPriorities.Any())
            {
                groupPresetSaveable.groupWorkPrioritiesDefnames = new Dictionary<string, int>();
                foreach (KeyValuePair<WorkTypeDef, int> data in groupWorkPriorities)
                {
                    groupPresetSaveable.groupWorkPrioritiesDefnames[data.Key.defName] = data.Value;
                }
            }
            if (activeWorkTypes != null && activeWorkTypes.Any())
            {
                groupPresetSaveable.activeWorkTypes = new Dictionary<string, WorkState>();
                foreach (KeyValuePair<WorkType, WorkState> data in activeWorkTypes)
                {
                    if (data.Key.workTypeDef != null)
                    {
                        groupPresetSaveable.activeWorkTypes[data.Key.workTypeDef.defName] = data.Value;
                    }
                }
            }
            return groupPresetSaveable;
        }

        public string GetUniqueLoadID()
        {
            return "GroupPreset_" + name + id;
        }
    }
}
