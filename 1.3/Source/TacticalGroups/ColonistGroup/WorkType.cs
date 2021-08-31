using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	public class WorkType : IExposable
    {
        public WorkTypeDef workTypeDef;
        public WorkTypeEnum workTypeEnum;

        public string Label
        {
            get
            {
                if (workTypeDef != null)
                {
                    return workTypeDef.label;
                }
                return workTypeEnum.ToString();
            }
        }
		public WorkType()
        {

        }

        public WorkType(WorkTypeDef workTypeDef)
        {
            this.workTypeDef = workTypeDef;
        }

        public WorkType(WorkTypeEnum workTypeEnum)
        {
            this.workTypeEnum = workTypeEnum;
        }
        public void ExposeData()
        {
			Scribe_Defs.Look(ref workTypeDef, "workTypeDef");
			Scribe_Values.Look(ref workTypeEnum, "workTypeEnum");
		}
	}
}
