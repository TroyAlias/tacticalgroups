using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using static Verse.Widgets;

namespace TacticalGroups
{
	public static class DrugPolicyUIUtilityGroup
	{
		public const string AssigningDrugsTutorHighlightTag = "ButtonAssignDrugs";

		public static void DoAssignDrugPolicyButtons(Rect rect, ColonistGroup group)
		{
			int num = Mathf.FloorToInt((rect.width - 4f) * 0.714285731f);
			int num2 = Mathf.FloorToInt((rect.width - 4f) * 0.2857143f);
			float x = rect.x;
			Rect rect2 = new Rect(x, rect.y + 2f, num, rect.height - 4f);
			string text = group.pawns.First().drugs.CurrentPolicy.label;

			Widgets.Dropdown(rect2, group, (ColonistGroup g) => g.pawns.First().drugs.CurrentPolicy, Button_GenerateMenu, text.Truncate(rect2.width), null, 
				group.pawns.First().drugs.CurrentPolicy.label, null, delegate
			{
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.DrugPolicies, KnowledgeAmount.Total);
			}, paintable: true);
			x += (float)num;
			x += 4f;
			Rect rect3 = new Rect(x, rect.y + 2f, num2, rect.height - 4f);
			if (Widgets.ButtonText(rect3, "AssignTabEdit".Translate()))
			{
				var window = new Dialog_ManageDrugPolicies(group.pawns.First().drugs.CurrentPolicy);
				Find.WindowStack.Add(window);
			}
			UIHighlighter.HighlightOpportunity(rect2, "ButtonAssignDrugs");
			UIHighlighter.HighlightOpportunity(rect3, "ButtonAssignDrugs");
			x += (float)num2;
		}

		private static IEnumerable<Widgets.DropdownMenuElement<DrugPolicy>> Button_GenerateMenu(ColonistGroup group)
		{
			foreach (DrugPolicy assignedDrugs in Current.Game.drugPolicyDatabase.AllPolicies)
			{
				yield return new Widgets.DropdownMenuElement<DrugPolicy>
				{
					option = new FloatMenuOption(assignedDrugs.label, delegate
					{

						foreach (var pawn in group.pawns)
                        {
							pawn.drugs.CurrentPolicy = assignedDrugs;
						}
						if (group.groupDrugPolicyEnabled)
                        {
							group.groupDrugPolicy = assignedDrugs;
                        }
						if (!(ModCompatibility.assignManagerSaveCurrentStateMethod is null))
                        {
							ModCompatibility.assignManagerSaveCurrentStateMethod.Invoke(null, new object[]
							{
								group.pawns
							});
                        }
					}),
					payload = assignedDrugs
				};
			}
		}
	}
}
