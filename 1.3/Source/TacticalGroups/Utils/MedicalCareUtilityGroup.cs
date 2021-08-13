using HarmonyLib;
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
	[StaticConstructorOnStartup]
	public static class MedicalCareUtilityGroup
	{
		private static Texture2D[] careTextures;

		public const float CareSetterHeight = 28f;

		public const float CareSetterWidth = 140f;
		public static void Reset()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				careTextures = AccessTools.Field(typeof(MedicalCareUtility), "careTextures").GetValue(null) as Texture2D[];
			});
		}
		public static string GetLabel(this MedicalCareCategory cat)
		{
			return ("MedicalCareCategory_" + cat).Translate();
		}

		public static MedicalCareCategory GetMedicalCare(ColonistGroup group)
        {
			foreach (var pawn in group.pawns)
            {
				if (pawn.playerSettings?.medCare != null)
                {
					return pawn.playerSettings.medCare;
				}
            }
			return MedicalCareCategory.NormalOrWorse;
		}
		public static void MedicalCareSelectButton(Rect rect, ColonistGroup group)
		{
			Widgets.Dropdown(rect, group, MedicalCareSelectButton_GetMedicalCare, MedicalCareSelectButton_GenerateMenu, null, careTextures[(uint)GetMedicalCare(group)], null, null, null, paintable: true);
		}

		private static MedicalCareCategory MedicalCareSelectButton_GetMedicalCare(ColonistGroup group)
		{
			return GetMedicalCare(group);
		}

		private static IEnumerable<Widgets.DropdownMenuElement<MedicalCareCategory>> MedicalCareSelectButton_GenerateMenu(ColonistGroup group)
		{
			for (int i = 0; i < 5; i++)
			{
				MedicalCareCategory mc = (MedicalCareCategory)i;
				yield return new Widgets.DropdownMenuElement<MedicalCareCategory>
				{
					option = new FloatMenuOption(mc.GetLabel(), delegate
					{
						foreach (var p in group.pawns)
                        {
							p.playerSettings.medCare = mc;
						}
					}),
					payload = mc
				};
			}
		}
	}
}