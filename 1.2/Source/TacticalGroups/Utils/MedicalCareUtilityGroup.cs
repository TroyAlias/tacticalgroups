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

		private static bool medicalCarePainting;

		public static void Reset()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				careTextures = new Texture2D[5];
				careTextures[0] = ContentFinder<Texture2D>.Get("UI/Icons/Medical/NoCare");
				careTextures[1] = ContentFinder<Texture2D>.Get("UI/Icons/Medical/NoMeds");
				careTextures[2] = ThingDefOf.MedicineHerbal.uiIcon;
				careTextures[3] = ThingDefOf.MedicineIndustrial.uiIcon;
				careTextures[4] = ThingDefOf.MedicineUltratech.uiIcon;
			});
		}

		public static void MedicalCareSetter(Rect rect, ref MedicalCareCategory medCare)
		{
			Rect rect2 = new Rect(rect.x, rect.y, rect.width / 5f, rect.height);
			for (int i = 0; i < 5; i++)
			{
				MedicalCareCategory mc = (MedicalCareCategory)i;
				Widgets.DrawHighlightIfMouseover(rect2);
				MouseoverSounds.DoRegion(rect2);
				GUI.DrawTexture(rect2, careTextures[i]);
				Widgets.DraggableResult draggableResult = Widgets.ButtonInvisibleDraggable(rect2);
				if (draggableResult == Widgets.DraggableResult.Dragged)
				{
					medicalCarePainting = true;
				}
				if ((medicalCarePainting && Mouse.IsOver(rect2) && medCare != mc))
				{
					medCare = mc;
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
				}
				if (medCare == mc)
				{
					Widgets.DrawBox(rect2, 3);
				}
				if (Mouse.IsOver(rect2))
				{
					TooltipHandler.TipRegion(rect2, () => mc.GetLabel(), 632165 + i * 17);
				}
				rect2.x += rect2.width;
			}
			if (!Input.GetMouseButton(0))
			{
				medicalCarePainting = false;
			}
		}

		public static string GetLabel(this MedicalCareCategory cat)
		{
			return ("MedicalCareCategory_" + cat).Translate();
		}

		public static bool AllowsMedicine(this MedicalCareCategory cat, ThingDef meds)
		{
			switch (cat)
			{
				case MedicalCareCategory.NoCare:
					return false;
				case MedicalCareCategory.NoMeds:
					return false;
				case MedicalCareCategory.HerbalOrWorse:
					return meds.GetStatValueAbstract(StatDefOf.MedicalPotency) <= ThingDefOf.MedicineHerbal.GetStatValueAbstract(StatDefOf.MedicalPotency);
				case MedicalCareCategory.NormalOrWorse:
					return meds.GetStatValueAbstract(StatDefOf.MedicalPotency) <= ThingDefOf.MedicineIndustrial.GetStatValueAbstract(StatDefOf.MedicalPotency);
				case MedicalCareCategory.Best:
					return true;
				default:
					throw new InvalidOperationException();
			}
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