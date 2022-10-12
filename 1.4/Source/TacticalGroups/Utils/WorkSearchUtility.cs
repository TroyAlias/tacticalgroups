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
			if (workType.workTypeDef != null)
            {
				SearchForWork(workType.workTypeDef, pawns);

			}
			else
            {
				SearchForWork(workType.workTypeEnum, pawns);
			}
		}

		public static void SearchForWork(WorkTypeDef workType, List<Pawn> pawns)
		{
			foreach (var pawn in pawns)
			{
				if (pawn.CurJob?.workGiverDef?.workType != workType)
				{
					GetJobFor(pawn, workType.workGiversByPriority);
				}
			}
		}
		public static void SearchForWork(WorkTypeEnum workTypeEnum, List<Pawn> pawns)
		{
			switch (workTypeEnum)
			{
				case WorkTypeEnum.None: SearchForWorkGeneral(pawns); break;
				case WorkTypeEnum.TendWounded: SearchForWorkTendWounded(pawns); break;
				case WorkTypeEnum.RescueFallen: SearchForWorkRescueFallen(pawns); break;
				case WorkTypeEnum.UnloadCaravan: SearchForWorkUnloadCaravan(pawns); break;
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
			if (pawn.Dead || !pawn.Spawned)
            {
				return;
            }
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
						GiveJob(pawn, job2, workGiver);
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
		public static void GiveJob(Pawn pawn, Job job, WorkGiver giver)
		{
			job.workGiverDef = giver.def;
			pawn.jobs.TryTakeOrderedJobPrioritizedWork(job, giver, pawn.Position);
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
			try
            {
				if (giver.ShouldSkip(pawn))
				{
					return false;
				}
			}
            catch
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
