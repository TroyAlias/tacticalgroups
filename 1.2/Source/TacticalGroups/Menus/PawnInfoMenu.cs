using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public class PawnInfoMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(25f, 30f);

		private Pawn pawn;
		public PawnInfoMenu(Pawn pawn, TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			this.pawn = pawn;
			this.layer = WindowLayer.GameUI;
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		protected override void SetInitialSizeAndPosition()
        {
			Vector2 vector = new Vector2(originRect.x - 201, originRect.y + (originRect.height - 84)) + InitialPositionShift;
			windowRect = new Rect(vector.x, vector.y, InitialSize.x, InitialSize.y);
			if (vector.x + InitialSize.x > (float)UI.screenWidth)
			{
				var toShift = (vector.x + InitialSize.x) - (float)UI.screenWidth;
				this.windowRect.x -= toShift;
				ShiftParentWindowsX(toShift);
			}
			if (vector.y + InitialSize.y > (float)UI.screenHeight)
			{
				var toShift = (vector.x + InitialSize.y) - (float)UI.screenHeight;
				this.windowRect.y -= toShift;
				ShiftParentWindowsY(toShift);
			}
		}

		public static readonly Vector3 PawnTextureCameraOffset = new Vector3(0f, 0f, 0.10f);

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Vector2 zero = Vector2.zero;
			zero += InitialFloatOptionPositionShift;
			for (int i = 0; i < options.Count; i++)
			{
				TieredFloatMenuOption floatMenuOption = options[i];
				Rect rect2 = new Rect(zero.x, zero.y, (this.backgroundTexture.width - InitialFloatOptionPositionShift.x) / 1.2f, floatMenuOption.curIcon.height);
				if (floatMenuOption.DoGUI(rect2, this))
				{
					Find.WindowStack.TryRemove(this);
					break;
				}
				zero.y += floatMenuOption.bottomIndent;
			}

			var pawnBox = new Rect(rect.x + 10f, rect.y + 25f, 130f, 180f);
			GUI.DrawTexture(pawnBox, PortraitsCache.Get(pawn, pawnBox.size, PawnTextureCameraOffset, 1.28f));
			Widgets.InfoCardButton(pawnBox.x + pawnBox.width - 18f, pawnBox.x + pawnBox.height - 23f, pawn);

			Text.Anchor = TextAnchor.MiddleLeft;

			var armorValue = pawn.apparel.WornApparel != null ? TacticUtils.OverallArmorValue(pawn) : 0f;
			var armorValueLabel = new Rect(pawnBox.xMax + 5, pawnBox.y, 27, 26);
			Widgets.Label(armorValueLabel, armorValue.ToStringDecimalIfSmall());
			var armorValueRect = new Rect(armorValueLabel.xMax, pawnBox.y, Textures.CrossHairs.width, Textures.CrossHairs.height);
			GUI.DrawTexture(armorValueRect, Textures.ArmorIcon);
			
			
			var dpsValue = pawn.equipment.Primary != null ? TacticUtils.WeaponScoreGain(pawn.equipment.Primary) : pawn.GetStatValue(StatDefOf.MeleeDPS);
			var dpsValueLabel = new Rect(pawnBox.xMax + 5, armorValueLabel.yMax, 27, 26);
			Widgets.Label(dpsValueLabel, dpsValue.ToStringDecimalIfSmall());
			var dpsValueRect = new Rect(dpsValueLabel.xMax, dpsValueLabel.y, Textures.CrossHairs.width, Textures.CrossHairs.height);
			GUI.DrawTexture(dpsValueRect, Textures.CrossHairs);
			var pawnInfoRect = new Rect(dpsValueRect.xMax - 30, rect.y + 90f, rect.width - 170f, rect.height - 110f);
			//Widgets.DrawBox(pawnInfoRect);
			TacticCharacterCardUtility.DrawCharacterCard(pawnInfoRect, pawn, null, rect);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
