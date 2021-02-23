using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	public class GroupPreset : IExposable
    {
		public Outfit groupOutfit;
		public Area groupArea;
		public DrugPolicy groupDrugPolicy;
		public FoodRestriction groupFoodRestriction;
		public Dictionary<WorkType, WorkState> activeWorkTypes = new Dictionary<WorkType, WorkState>();
		public Dictionary<WorkTypeDef, int> groupWorkPriorities = new Dictionary<WorkTypeDef, int>();

		public string name;
		public GroupPreset()
        {

        }

		public void SetName(string name)
        {
			this.name = name;
        }
        public void ExposeData()
        {
			Scribe_Values.Look(ref name, "name");

			Scribe_Collections.Look(ref activeWorkTypes, "activeWorkTypes", LookMode.Value, LookMode.Value, ref workTypesKeys, ref workStateValues);
			Scribe_Collections.Look(ref groupWorkPriorities, "groupWorkPriorities", LookMode.Def, LookMode.Value, ref workTypesDefKeys, ref intValues);
			try
			{
				Scribe_Deep.Look(ref groupArea, "groupArea");
			}
			catch { }
			try
			{
				Scribe_Deep.Look(ref groupDrugPolicy, "groupDrugPolicy");
			}
			catch { }

			try
			{
				Scribe_Deep.Look(ref groupFoodRestriction, "groupFoodRestriction");
			}
			catch { }
			try
			{
				Scribe_Deep.Look(ref groupOutfit, "groupOutfit");
			}
			catch { }
		}

		public void ClearSettings()
        {
			this.activeWorkTypes = null;
			this.groupWorkPriorities = null;
			this.groupArea = null;
			this.groupDrugPolicy = null;
			this.groupFoodRestriction = null;
			this.groupOutfit = null;
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
			if (colonistGroup.groupFoodRestrictionEnabled && colonistGroup.groupFoodRestriction != null)
			{
				this.groupFoodRestriction = colonistGroup.groupFoodRestriction;
			}
			if (colonistGroup.groupOutfitEnabled && colonistGroup.groupOutfit != null)
			{
				this.groupOutfit = colonistGroup.groupOutfit;
			}
		}

		private List<WorkType> workTypesKeys;
		private List<WorkState> workStateValues;

		private List<WorkTypeDef> workTypesDefKeys;
		private List<int> intValues;
	}
}
