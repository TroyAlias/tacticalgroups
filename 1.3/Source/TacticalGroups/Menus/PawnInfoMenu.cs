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
			this.parentWindow.layer = WindowLayer.Dialog;
			this.layer = WindowLayer.GameUI;
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
			TacticSkillUI.Reset();
		}

        public override void PostOpen()
        {

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

			var pawnBox = new Rect(rect.x + 10f, rect.y + 23f, 130f, 180f);
			GUI.DrawTexture(pawnBox, PortraitsCache.Get(pawn, pawnBox.size, Rot4.South, PawnTextureCameraOffset, 1.15f));
			Widgets.InfoCardButton(pawnBox.x + pawnBox.width - 18f, pawnBox.x + pawnBox.height - 23f, pawn);
			Text.Anchor = TextAnchor.MiddleLeft;

			//var pawnTabsRect = new Rect(pawnBox.x, pawnBox.yMax + 10, 120, 120);
			//Widgets.DrawBox(pawnTabsRect);

			//MainTabWindow_Inspect mainTabWindow_Inspect = (MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow;
			//
			//TacticInspectPaneUtility.UpdateTabs(mainTabWindow_Inspect);

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
			TacticCharacterCardUtility.DrawCharacterCard(pawnInfoRect, pawn, null, rect);

			Text.Anchor = TextAnchor.MiddleCenter;

			var moodTexture = GetMoodTexture(out string moodLabel);
			var moodRect = new Rect(rect.x + 425f, rect.y + 90, moodTexture.width, moodTexture.height);
			GUI.DrawTexture(moodRect, moodTexture);
			var moodLabelRect = new Rect(moodRect.x, moodRect.y + moodTexture.height, 45, 24);
			Widgets.Label(moodLabelRect, moodLabel);
			TooltipHandler.TipRegion(moodRect, Strings.MoodIconTooltip);
			
			var healthTexture = GetHealthTexture(out string healthPercent);
			var healthRect = new Rect(moodRect.x + 45f, moodRect.y, healthTexture.width, healthTexture.height);
			GUI.DrawTexture(healthRect, healthTexture);
			var healthLabelRect = new Rect(healthRect.x, healthRect.y + healthRect.height, 40, 24);
			Widgets.Label(healthLabelRect, healthPercent);
			TooltipHandler.TipRegion(healthRect, Strings.HealthIconTooltip);
			
			var restTexture = GetRestTexture(out string restPercent);
			var restRect = new Rect(healthRect.x + 45f, healthRect.y, restTexture.width, restTexture.height);
			GUI.DrawTexture(restRect, restTexture);
			var restLabelRect = new Rect(restRect.x, restRect.y + restRect.height, 40, 24);
			Widgets.Label(restLabelRect, restPercent);
			TooltipHandler.TipRegion(restRect, Strings.RestIconTooltip);
			
			var foodTexture = GetFoodTexture(out string foodPercent);
			var foodStatRect = new Rect(restRect.x + 45f, restRect.y, foodTexture.width, foodTexture.height);
			GUI.DrawTexture(foodStatRect, foodTexture);
			var foodLabelRect = new Rect(foodStatRect.x, foodStatRect.y + foodStatRect.height, 40, 24);
			Widgets.Label(foodLabelRect, foodPercent);
			TooltipHandler.TipRegion(foodStatRect, Strings.HungerIconTooltip);

			var needRect = new Rect(moodRect.x - 5f, moodLabelRect.yMax, 180f, rect.height - 160);
			TacticNeedsCardUtility.DoNeeds(needRect, pawn);

			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;

			var skillRect = new Rect(needRect.xMax + 10, moodRect.y - 1f, 164, rect.height - 110);
			TacticSkillUI.DrawSkillsOf(pawn, new Vector2(skillRect.x, skillRect.y), TacticSkillUI.SkillDrawMode.Gameplay);
		}

		public Texture2D GetMoodTexture(out string moodString)
		{
			var averageValue = pawn.needs.mood.CurLevelPercentage;
			if (averageValue < 0.33)
			{
				moodString = Strings.Sad;
				return Textures.SadIcon;
			}
			else if (averageValue < 0.66)
			{
				moodString = Strings.Okay;
				return Textures.OkayIcon;
			}
			moodString = Strings.Happy;
			return Textures.HappyIcon;
		}

		public Texture2D GetHealthTexture(out string healthPercent)
		{
			var averageValue = pawn.health.summaryHealth.SummaryHealthPercent;
			healthPercent = (averageValue * 100f).ToStringDecimalIfSmall() + "%";
			if (averageValue < 0.33)
			{
				return Textures.HurtIcon;
			}
			else if (averageValue < 0.66)
			{
				return Textures.AliveIcon;
			}
			return Textures.HealthyIcon;
		}

		public Texture2D GetRestTexture(out string restPercent)
		{
			var averageValue = pawn.needs.rest.CurLevelPercentage;
			restPercent = (averageValue * 100f).ToStringDecimalIfSmall() + "%";
			if (averageValue < 0.33)
			{
				return Textures.TiredIcon;
			}
			else if (averageValue < 0.66)
			{
				return Textures.AwakeIcon;
			}
			return Textures.RestedIcon;
		}

		public Texture2D GetFoodTexture(out string foodPercent)
		{
			var averageValue = pawn.needs.food.CurLevelPercentage;
			foodPercent = (averageValue * 100f).ToStringDecimalIfSmall() + "%";
			if (averageValue < 0.33f)
			{
				return Textures.StarvingIcon;
			}
			else if (averageValue < 0.66f)
			{
				return Textures.HungryIcon;
			}
			return Textures.FullIcon;
		}
	}
}
