using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	public class GroupPresetSaveable : IExposable
	{
		public int id;
		public string name;
		public string groupApparelPolicyIndex;
		public string groupAreaIndex;
		public string groupDrugPolicyIndex;
		public string groupFoodPolicyIndex;
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
			Scribe_Values.Look(ref groupFoodPolicyIndex, "groupFoodPolicy");
			Scribe_Values.Look(ref groupApparelPolicyIndex, "groupApparelPolicy");
		}

		private List<string> workTypesKeys;
		private List<WorkState> workStateValues;

		private List<string> workTypesDefKeys;
		private List<int> intValues;

		public GroupPreset LoadFromSaveable()
        {
			var groupPreset = new GroupPreset();
			groupPreset.name = this.name;
			groupPreset.id = this.id;
			if (!this.groupApparelPolicyIndex.NullOrEmpty())
            {
				var outfitPolicy = Current.Game.outfitDatabase.AllOutfits.FirstOrDefault(x => x.GetUniqueLoadID() == this.groupApparelPolicyIndex);
				if (outfitPolicy != null)
				{
					groupPreset.groupApparelPolicy = outfitPolicy;
				}
			}

			if (!this.groupAreaIndex.NullOrEmpty())
            {
				var area = GetArea();
				if (area != null)
				{
					groupPreset.groupArea = area;
				}
			}

			if (!this.groupDrugPolicyIndex.NullOrEmpty())
            {
				var drugPolicy = Current.Game.drugPolicyDatabase.AllPolicies.FirstOrDefault(x => x.GetUniqueLoadID() == this.groupDrugPolicyIndex);
				if (drugPolicy != null)
				{
					groupPreset.groupDrugPolicy = drugPolicy;
				}
			}

			if (!this.groupFoodPolicyIndex.NullOrEmpty())
            {
				var foodRestriction = Current.Game.foodRestrictionDatabase.AllFoodRestrictions.FirstOrDefault(x => x.GetUniqueLoadID() == this.groupFoodPolicyIndex);
				if (foodRestriction != null)
				{
					groupPreset.groupFoodPolicy = foodRestriction;
				}
			}

			if (this.groupWorkPrioritiesDefnames != null && this.groupWorkPrioritiesDefnames.Any())
            {
				groupPreset.groupWorkPriorities = new Dictionary<WorkTypeDef, int>();
				foreach (var data in groupWorkPrioritiesDefnames)
                {
					var workDef = DefDatabase<WorkTypeDef>.GetNamedSilentFail(data.Key);
					if (workDef != null)
                    {
						groupPreset.groupWorkPriorities[workDef] = data.Value;
					}
				}
			}
			if (this.activeWorkTypes != null && this.activeWorkTypes.Any())
            {
				if (groupPreset.activeWorkTypes is null)
                {
					groupPreset.activeWorkTypes = new Dictionary<WorkType, WorkState>();
                }
				foreach (var workTypeData in this.activeWorkTypes)
                {
					var def = DefDatabase<WorkTypeDef>.GetNamedSilentFail(workTypeData.Key);
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
			foreach (var map in Find.Maps)
            {
				foreach (var area in map.areaManager.AllAreas)
                {
					if (area.GetUniqueLoadID() == this.groupAreaIndex)
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

		public ApparelPolicy groupApparelPolicy;
		public Area groupArea;
		public DrugPolicy groupDrugPolicy;
		public FoodPolicy groupFoodPolicy;
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
			this.activeWorkTypes = null;
			this.groupWorkPriorities = null;
			this.groupArea = null;
			this.groupDrugPolicy = null;
			this.groupFoodPolicy = null;
			this.groupApparelPolicy = null;
		}
		public void CopySettingsFrom(ColonistGroup colonistGroup)
		{
			if (colonistGroup.activeWorkTypes?.Count > 0)
			{
				this.activeWorkTypes = colonistGroup.activeWorkTypes;
			}
			if (colonistGroup.groupWorkPriorities?.Count > 0)
			{
				this.groupWorkPriorities = colonistGroup.groupWorkPriorities;
			}
			if (colonistGroup.groupAreaEnabled && colonistGroup.groupArea != null)
			{
				this.groupArea = colonistGroup.groupArea;
			}
			if (colonistGroup.groupDrugPolicyEnabled && colonistGroup.groupDrugPolicy != null)
			{
				this.groupDrugPolicy = colonistGroup.groupDrugPolicy;
			}
			if (colonistGroup.groupFoodPolicyEnabled && colonistGroup.groupFoodPolicy != null)
			{
				this.groupFoodPolicy = colonistGroup.groupFoodPolicy;
			}
			if (colonistGroup.groupApparelPolicyEnabled && colonistGroup.groupApparelPolicy != null)
			{
				this.groupApparelPolicy = colonistGroup.groupApparelPolicy;
			}
		}

		public GroupPresetSaveable SaveToSaveable()
        {
			var groupPresetSaveable = new GroupPresetSaveable();
			groupPresetSaveable.id = this.id;
			groupPresetSaveable.name = this.name;
			if (this.groupApparelPolicy != null)
			{
				groupPresetSaveable.groupApparelPolicyIndex = groupApparelPolicy.GetUniqueLoadID();
			}

			if (this.groupArea != null)
			{
				groupPresetSaveable.groupAreaIndex = this.groupArea.GetUniqueLoadID();
			}

			if (this.groupDrugPolicy != null)
			{
				groupPresetSaveable.groupDrugPolicyIndex = this.groupDrugPolicy.GetUniqueLoadID();
			}

			if (this.groupFoodPolicy != null)
			{
				groupPresetSaveable.groupDrugPolicyIndex = this.groupFoodPolicy.GetUniqueLoadID();
			}

			if (this.groupWorkPriorities != null && this.groupWorkPriorities.Any())
			{
				groupPresetSaveable.groupWorkPrioritiesDefnames = new Dictionary<string, int>();
				foreach (var data in groupWorkPriorities)
				{
					groupPresetSaveable.groupWorkPrioritiesDefnames[data.Key.defName] = data.Value;
				}
			}
			if (this.activeWorkTypes != null && this.activeWorkTypes.Any())
			{
				groupPresetSaveable.activeWorkTypes = new Dictionary<string, WorkState>();
				foreach (var data in this.activeWorkTypes)
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
