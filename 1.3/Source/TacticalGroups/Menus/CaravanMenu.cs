using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace TacticalGroups
{
	public class CaravanMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(30, 63f);

		public CaravanMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{

		}

		private CaravanOptionsMenu caravanOptionsMenu;
		public override void PostOpen()
		{
			base.PostOpen();
			caravanOptionsMenu = new CaravanOptionsMenu(this, this.colonistGroup, windowRect, Textures.LoadoutMenu);
			Find.WindowStack.Add(caravanOptionsMenu);
		}

		public override void PostClose()
		{
			base.PostClose();
			caravanOptionsMenu?.Close();
		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Text.Anchor = TextAnchor.MiddleCenter;
			var sendRect = new Rect(rect.x + 20, rect.y + 25, Textures.SetClearButton.width, Textures.SetClearButton.height);
			GUI.DrawTexture(sendRect, Textures.SetClearButton);
			Widgets.Label(sendRect, Strings.Send);
			if (Mouse.IsOver(sendRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					var window = new Dialog_FormCaravan(this.colonistGroup.Map);
					Find.WindowStack.Add(window);
					Traverse.Create(window).Field("autoSelectFoodAndMedicine").SetValue(this.colonistGroup.travelSuppliesEnabled);
					List<Pawn> selectedPawns = new List<Pawn>();
					foreach (var trad in window.transferables)
					{
						if (trad.AnyThing is Pawn pawn2 && this.colonistGroup.ActivePawns.Contains(pawn2))
						{
							trad.AdjustTo(1);
							selectedPawns.Add(pawn2);
						}
					}
					Traverse.Create(window).Method("CountToTransferChanged").GetValue();
					if (!this.colonistGroup.travelSuppliesEnabled && this.colonistGroup.bedrollsEnabled)
					{
						this.SelectBedrolls(window, window.transferables, selectedPawns);
					}
					this.CloseAllWindows();
					Event.current.Use();
				}
				GUI.DrawTexture(sendRect, Textures.WorkButtonHover);
			}

			var unloadRect = new Rect(sendRect.xMax - 15, sendRect.yMax + 20, Textures.SetClearButton.width, Textures.SetClearButton.height);
			GUI.DrawTexture(unloadRect, Textures.SetClearButton);
			Widgets.Label(unloadRect, Strings.Unload);
			if (Mouse.IsOver(unloadRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					this.colonistGroup.AssignTemporaryWorkers(WorkType.UnloadCaravan);
					WorkSearchUtility.SearchForWork(WorkType.UnloadCaravan, this.colonistGroup.ActivePawns);
					Event.current.Use();
				}
				GUI.DrawTexture(unloadRect, Textures.WorkButtonHover);
			}

			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
		}


		private static List<TransferableOneWay> tmpBeds = new List<TransferableOneWay>();
        private void SelectBedrolls(Dialog_FormCaravan window, List<TransferableOneWay> transferables, List<Pawn> pawnsFromTransferables)
        {
            IEnumerable<TransferableOneWay> enumerable = transferables.Where((TransferableOneWay x) => x.ThingDef.category != ThingCategory.Pawn && !x.ThingDef.thingCategories.NullOrEmpty() && x.ThingDef.thingCategories.Contains(ThingCategoryDefOf.Medicine));
            IEnumerable<TransferableOneWay> enumerable2 = transferables.Where((TransferableOneWay x) => x.ThingDef.IsIngestible && !x.ThingDef.IsDrug && !x.ThingDef.IsCorpse);
            tmpBeds.Clear();
            for (int i = 0; i < transferables.Count; i++)
            {
                for (int j = 0; j < transferables[i].things.Count; j++)
                {
                    Thing thing = transferables[i].things[j];
                    for (int k = 0; k < thing.stackCount; k++)
                    {
                        Building_Bed building_Bed;
                        if ((building_Bed = (thing.GetInnerIfMinified() as Building_Bed)) != null && building_Bed.def.building.bed_caravansCanUse)
                        {
                            for (int l = 0; l < building_Bed.SleepingSlotsCount; l++)
                            {
                                tmpBeds.Add(transferables[i]);
                            }
                        }
                    }
                }
            }
            tmpBeds.SortByDescending((TransferableOneWay x) => x.AnyThing.GetStatValue(StatDefOf.BedRestEffectiveness));
            foreach (TransferableOneWay item in enumerable)
            {
                item.AdjustTo(0);
            }
            foreach (TransferableOneWay item2 in enumerable2)
            {
                item2.AdjustTo(0);
            }
            foreach (TransferableOneWay tmpBed in tmpBeds)
            {
                tmpBed.AdjustTo(0);
            }
            if (pawnsFromTransferables.Any())
            {
                foreach (Pawn item3 in pawnsFromTransferables)
                {
                    TransferableOneWay transferableOneWay = BestBedFor(item3);
                    if (transferableOneWay != null)
                    {
                        tmpBeds.Remove(transferableOneWay);
                        if (transferableOneWay.CanAdjustBy(1).Accepted)
                        {
                            AddOneIfMassAllows(window, transferableOneWay);
                        }
                    }
                }
            }
        }

        private bool AddOneIfMassAllows(Dialog_FormCaravan window, Transferable transferable)
        {
			if (transferable.CanAdjustBy(1).Accepted && window.MassUsage + transferable.ThingDef.BaseMass < window.MassCapacity)
            {
                transferable.AdjustBy(1);
                Traverse.Create(window).Field("massUsageDirty").SetValue(true);
                return true;
            }
            return false;
        }

        private TransferableOneWay BestBedFor(Pawn pawn)
        {
            for (int i = 0; i < tmpBeds.Count; i++)
            {
                Thing innerIfMinified = tmpBeds[i].AnyThing.GetInnerIfMinified();
                if (RestUtility.CanUseBedEver(pawn, innerIfMinified.def))
                {
                    return tmpBeds[i];
                }
            }
            return null;
        }
	}
}
