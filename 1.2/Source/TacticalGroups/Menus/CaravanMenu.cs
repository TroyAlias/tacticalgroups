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
					foreach (var pawn in this.colonistGroup.ActivePawns)
					{
						foreach (var trad in window.transferables)
						{
							if (trad.AnyThing is Pawn pawn2 && this.colonistGroup.ActivePawns.Contains(pawn2))
							{
								trad.AdjustTo(1);
							}
						}
					}
					Traverse.Create(window).Method("CountToTransferChanged").GetValue();
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
	}
}
