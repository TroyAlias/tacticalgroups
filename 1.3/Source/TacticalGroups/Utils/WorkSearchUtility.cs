using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using static Verse.Widgets;

namespace TacticalGroups
{
	public static class WorkSearchUtility
	{
		public static void SearchForWork(WorkType workType, List<Pawn> pawns)
		{
			switch (workType)
			{
				case WorkType.None: SearchForWorkGeneral(pawns); break;
				case WorkType.Construction: SearchForWorkConstruction(pawns); break;
				case WorkType.Cleaning: SearchForWorkCleaning(pawns); break;
				case WorkType.Cooking: SearchForWorkCooking(pawns); break;
				case WorkType.Crafting: SearchForWorkCrafting(pawns); break;
				case WorkType.ClearSnow: SearchForWorkClearSnow(pawns); break;
				case WorkType.Doctor: SearchForWorkDoctor(pawns); break;
				case WorkType.Hauling: SearchForWorkHauling(pawns); break;
				case WorkType.Hunting: SearchForWorkHunting(pawns); break;
				case WorkType.Mining: SearchForWorkMining(pawns); break;
				case WorkType.Plants: SearchForWorkPlants(pawns); break;
				case WorkType.Warden: SearchForWorkWarden(pawns); break;
				case WorkType.WoodChopping: SearchForWorkWoodChopping(pawns); break;
				case WorkType.Art: SearchForWorkArt(pawns); break;
				case WorkType.Handle: SearchForWorkHandle(pawns); break;
				case WorkType.Smith: SearchForWorkSmith(pawns); break;
				case WorkType.Tailor: SearchForWorkTailor(pawns); break;
				case WorkType.Research: SearchForWorkResearch(pawns); break;
				case WorkType.FireExtinguish: SearchForWorkFireExtinguish(pawns); break;
				case WorkType.TendWounded: SearchForWorkTendWounded(pawns); break;
				case WorkType.RescueFallen: SearchForWorkRescueFallen(pawns); break;
				case WorkType.UnloadCaravan: SearchForWorkUnloadCaravan(pawns); break;
				default: return;
			}
		}
		public static void SearchForWorkGeneral(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				if (pawn.mindState.IsIdle || pawn.mindState.lastJobTag == JobTag.SatisfyingNeeds)
				{
					ThinkResult result = ThinkResult.NoJob;
					try
					{
						result = pawn.thinker.MainThinkNodeRoot.TryIssueJobPackage(pawn, default(JobIssueParams));
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result.Job != null && result.Job.def != JobDefOf.GotoWander)
					{
						pawn.jobs.TryTakeOrderedJob(result.Job);
					}
				}
			}
		}

		public static void SearchForWorkConstruction(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = WorkTypeDefOf.Construction;
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}
		public static void SearchForWorkCleaning(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Cleaning");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkCooking(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Cooking");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkCrafting(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Crafting");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkClearSnow(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkGiverDef>.GetNamed("CleanClearSnow");
				if (pawn.CurJob?.workGiverDef != workType)
				{
					GetJobFor(pawn, new List<WorkGiverDef> { workType });
				}
			}
		}

		public static void SearchForWorkDoctor(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Doctor");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkHauling(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Hauling");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkHunting(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Hunting");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}
		public static void SearchForWorkMining(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Mining");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}
		public static void SearchForWorkPlants(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var growingWorkType = DefDatabase<WorkTypeDef>.GetNamed("Growing");
				var plantCuttingWorkType = DefDatabase<WorkTypeDef>.GetNamed("PlantCutting");

				if (pawn.CurJob?.workGiverDef?.workType != growingWorkType && pawn.CurJob?.workGiverDef?.workType != plantCuttingWorkType)
				{
					var workgivers = new List<WorkGiverDef>();
					workgivers.AddRange(growingWorkType.workGiversByPriority);
					workgivers.AddRange(plantCuttingWorkType.workGiversByPriority);
					GetJobFor(pawn, workgivers);
				}
			}
		}

		public static void SearchForWorkWarden(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Warden");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkWoodChopping(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("PlantCutting");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}
		public static void SearchForWorkArt(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Art");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkHandle(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Handling");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}
		public static void SearchForWorkSmith(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Smithing");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkTailor(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Tailoring");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkResearch(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Research");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkFireExtinguish(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Firefighter");
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}

		public static void SearchForWorkTendWounded(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var jg = new JobGiver_TendWounded();
				var result = jg.TryIssueJobPackage(pawn, default(JobIssueParams));
				if (result.Job != null)
                {
					pawn.jobs.TryTakeOrderedJob(result.Job);
				}
			}
		}

		public static void SearchForWorkRescueFallen(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var jg = new JobGiver_RescueNearby();
				var result = jg.TryIssueJobPackage(pawn, default(JobIssueParams));
				if (result.Job != null)
				{
					pawn.jobs.TryTakeOrderedJob(result.Job);
				}
			}
		}

		public static void SearchForWorkUnloadCaravan(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				var workType = DefDatabase<WorkGiverDef>.GetNamed("UnloadCarriers");
				if (pawn.CurJob?.workGiverDef != workType)
				{
					GetJobFor(pawn, new List<WorkGiverDef> { workType });
				}
			}
		}

		private static List<WorkGiverDef> AllowedWorkGiversFor(Pawn pawn, List<WorkGiverDef> workGiverDefs)
        {
			List<WorkGiverDef> allowedWorkGiverDefs = new List<WorkGiverDef>();
			foreach (var workGiver in workGiverDefs)
            {
				if (!pawn.WorkTypeIsDisabled(workGiver.workType))
                {
					allowedWorkGiverDefs.Add(workGiver);
                }
            }
			return allowedWorkGiverDefs;
		}
		public static void GetJobFor(Pawn pawn, List<WorkGiverDef> workGiverDefs)
		{
			List<WorkGiver> list = AllowedWorkGiversFor(pawn, workGiverDefs).Select(x => x.Worker).ToList();
			int num = -999;
			TargetInfo bestTargetOfLastPriority = TargetInfo.Invalid;
			WorkGiver_Scanner scannerWhoProvidedTarget = null;
			WorkGiver_Scanner scanner;
			IntVec3 pawnPosition;
			float closestDistSquared;
			float bestPriority;
			bool prioritized;
			bool allowUnreachable;
			Danger maxPathDanger;
			for (int j = 0; j < list.Count; j++)
			{
				WorkGiver workGiver = list[j];
				if (workGiver.def.priorityInType != num && bestTargetOfLastPriority.IsValid)
				{
					break;
				}
				if (!PawnCanUseWorkGiver(pawn, workGiver))
				{
					continue;
				}
				try
				{
					Job job2 = workGiver.NonScanJob(pawn);
					if (job2 != null)
					{
						GiveJob(pawn, job2, null);
						return;
					}
					scanner = (workGiver as WorkGiver_Scanner);
					if (scanner != null)
					{
						if (scanner.def.scanThings)
						{
							Predicate<Thing> validator = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
							IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
							Thing thing;
							if (scanner.Prioritized)
							{
								IEnumerable<Thing> enumerable2 = enumerable;
								if (enumerable2 == null)
								{
									enumerable2 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								}
								thing = ((!scanner.AllowUnreachable) ? GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn, scanner.MaxPathDanger(pawn)), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x)) : GenClosest.ClosestThing_Global(pawn.Position, enumerable2, 99999f, validator, (Thing x) => scanner.GetPriority(pawn, x)));
							}
							else if (scanner.AllowUnreachable)
							{
								IEnumerable<Thing> enumerable3 = enumerable;
								if (enumerable3 == null)
								{
									enumerable3 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								}
								thing = GenClosest.ClosestThing_Global(pawn.Position, enumerable3, 99999f, validator);
							}
							else
							{
								thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, scanner.MaxPathDanger(pawn)), 9999f, validator, enumerable, 0, scanner.MaxRegionsToScanBeforeGlobalSearch, enumerable != null);
							}
							if (thing != null)
							{
								bestTargetOfLastPriority = thing;
								scannerWhoProvidedTarget = scanner;
							}
						}
						if (scanner.def.scanCells)
						{
							pawnPosition = pawn.Position;
							closestDistSquared = 99999f;
							bestPriority = float.MinValue;
							prioritized = scanner.Prioritized;
							allowUnreachable = scanner.AllowUnreachable;
							maxPathDanger = scanner.MaxPathDanger(pawn);
							IEnumerable<IntVec3> enumerable4 = scanner.PotentialWorkCellsGlobal(pawn);
							IList<IntVec3> list2;
							if ((list2 = (enumerable4 as IList<IntVec3>)) != null)
							{
								for (int k = 0; k < list2.Count; k++)
								{
									ProcessCell(list2[k]);
								}
							}
							else
							{
								foreach (IntVec3 item in enumerable4)
								{
									ProcessCell(item);
								}
							}
						}
					}
					void ProcessCell(IntVec3 c)
					{
						bool flag = false;
						float num2 = (c - pawnPosition).LengthHorizontalSquared;
						float num3 = 0f;
						if (prioritized)
						{
							if (!c.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, c))
							{
								if (!allowUnreachable && !pawn.CanReach(c, scanner.PathEndMode, maxPathDanger))
								{
									return;
								}
								num3 = scanner.GetPriority(pawn, c);
								if (num3 > bestPriority || (num3 == bestPriority && num2 < closestDistSquared))
								{
									flag = true;
								}
							}
						}
						else if (num2 < closestDistSquared && !c.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, c))
						{
							if (!allowUnreachable && !pawn.CanReach(c, scanner.PathEndMode, maxPathDanger))
							{
								return;
							}
							flag = true;
						}
						if (flag)
						{
							bestTargetOfLastPriority = new TargetInfo(c, pawn.Map);
							scannerWhoProvidedTarget = scanner;
							closestDistSquared = num2;
							bestPriority = num3;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(pawn, " threw exception in WorkGiver ", workGiver.def.defName, ": ", ex.ToString()));
				}
				finally
				{
				}
				if (bestTargetOfLastPriority.IsValid)
				{
					Job job3 = (!bestTargetOfLastPriority.HasThing) ? scannerWhoProvidedTarget.JobOnCell(pawn, bestTargetOfLastPriority.Cell) : scannerWhoProvidedTarget.JobOnThing(pawn, bestTargetOfLastPriority.Thing);
					if (job3 != null)
					{
						job3.workGiverDef = scannerWhoProvidedTarget.def;
						GiveJob(pawn, job3, scannerWhoProvidedTarget);
						return;
					}
				}
				num = workGiver.def.priorityInType;
			}
		}
		public static void GiveJob(Pawn pawn, Job job, WorkGiver_Scanner localScanner)
		{
			job.workGiverDef = localScanner?.def;
			pawn.jobs.TryTakeOrderedJobPrioritizedWork(job, localScanner, pawn.Position);
		}

		private static bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
		{
			if (!giver.def.nonColonistsCanDo && !pawn.IsColonist)
			{
				return false;
			}
			if (pawn.WorkTagIsDisabled(giver.def.workTags))
			{
				return false;
			}
			if (giver.ShouldSkip(pawn))
			{
				return false;
			}
			if (giver.MissingRequiredCapacity(pawn) != null)
			{
				return false;
			}
			return true;
		}

		public static void TakeABreak(BreakType breakType, List<Pawn> pawns)
		{
			switch (breakType)
			{
				case BreakType.None: TakeFive(pawns); break;
				case BreakType.Socialize: SearchForSocialRelax(pawns); break;
				case BreakType.Entertainment: TakeFive(pawns); break;
				case BreakType.ChowHall: ChowHall(pawns); break;
				case BreakType.LightsOut: LightsOut(pawns); break;
				default: return;
			}

		}

		public static void TakeFive(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				if (!pawn.mindState.IsIdle)
				{
					ThinkResult result = ThinkResult.NoJob;
					try
					{
						var joyGiver = new JobGiver_GetJoy();
						joyGiver.ResolveReferences();
						result = joyGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result.Job != null && result.Job.def != JobDefOf.GotoWander)
					{
						pawn.jobs.TryTakeOrderedJob(result.Job);
					}
				}
			}
		}

		public static void SearchForSocialRelax(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				if (!pawn.mindState.IsIdle && pawn.CurJobDef != JobDefOf.SocialRelax)
				{
					Job result = null;
					try
					{
						var joyGiver = new JoyGiver_SocialRelax();
						result = joyGiver.TryGiveJob(pawn);
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result != null && result.def != JobDefOf.GotoWander)
					{
						pawn.jobs.TryTakeOrderedJob(result);
					}
				}
			}
		}

		public static void ChowHall(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				if (!pawn.mindState.IsIdle && pawn.CurJobDef != JobDefOf.Ingest)
				{
					ThinkResult result = ThinkResult.NoJob;
					try
					{
						var joyGiver = new JobGiver_GetFood();
						joyGiver.ResolveReferences();
						result = joyGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result.Job != null && result.Job.def != JobDefOf.GotoWander)
					{
						pawn.jobs.TryTakeOrderedJob(result.Job);
					}
				}
			}
		}
		public static void LightsOut(List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				if (!pawn.mindState.IsIdle && pawn.CurJobDef != JobDefOf.LayDown)
				{
					ThinkResult result = ThinkResult.NoJob;
					try
					{
						var joyGiver = new JobGiver_GetRest();
						joyGiver.ResolveReferences();
						result = joyGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
					}
					catch (Exception exception)
					{
						JobUtility.TryStartErrorRecoverJob(pawn, pawn.ToStringSafe() + " threw exception while determining job (main)", exception);
					}
					if (result.Job != null && result.Job.def != JobDefOf.GotoWander)
					{
						pawn.jobs.TryTakeOrderedJob(result.Job);
					}
				}
			}
		}
	}
}
