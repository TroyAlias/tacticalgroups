using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TacticalGroups
{
	public class Dialog_ColorPicker : TieredFloatMenu
	{
		public Color curColor;

		private Dictionary<BodyColor, Rect> colorRects;
		private Dictionary<string, Rect> stringRects;
		public Dialog_ColorPicker(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.closeOnClickedOutside = true;
			colorRects = new Dictionary<BodyColor, Rect>();
			stringRects = new Dictionary<string, Rect>();

			var torsoRect = new Rect(220, 30, 24, 24);
			colorRects[BodyColor.Torso] = torsoRect;
			stringRects[Strings.Torso] = new Rect(torsoRect.xMax + 10, torsoRect.y + 3, 40, 24);

			var handsRect = new Rect(torsoRect.x, torsoRect.yMax + 10, torsoRect.width, torsoRect.height);
			colorRects[BodyColor.Hands] = handsRect;
			stringRects[Strings.Hands] = new Rect(handsRect.xMax + 10, handsRect.y + 3, 40, 24);

			var allRect = new Rect(torsoRect.xMax + 60, torsoRect.y + 5, torsoRect.width * 2, torsoRect.height * 2);
			colorRects[BodyColor.All] = allRect;

			var legsRect = new Rect(allRect.xMax + 15, torsoRect.y + 3, 40, 24);
			stringRects[Strings.Legs] = legsRect;
			colorRects[BodyColor.Legs] = new Rect(legsRect.xMax, torsoRect.y, 24, 24);

			var feetRect = new Rect(legsRect.x, legsRect.yMax + 13, 40, 24);
			stringRects[Strings.Feet] = feetRect;
			colorRects[BodyColor.Feet] = new Rect(feetRect.xMax, feetRect.y - 3, 24, 24);

			var headRect = new Rect(colorRects[BodyColor.Legs].xMax + 10, torsoRect.y + 3, 40, 24);
			stringRects[Strings.Head] = headRect;
			colorRects[BodyColor.Head] = new Rect(headRect.xMax, torsoRect.y, 24, 24);

			var hairRect = new Rect(headRect.x, headRect.yMax + 13, 40, 24);
			stringRects[Strings.Hair] = hairRect;
			colorRects[BodyColor.Hair] = new Rect(hairRect.xMax, hairRect.y - 3, 24, 24);
		}
        protected override Vector2 InitialPositionShift => new Vector2(4f, 0f);
		protected override void SetInitialSizeAndPosition()
		{
			Vector2 vector = UI.MousePositionOnUIInverted + InitialPositionShift;
			if (vector.x + InitialSize.x > (float)UI.screenWidth)
			{
				vector.x = (float)UI.screenWidth - InitialSize.x;
			}
			if (vector.y + InitialSize.y > (float)UI.screenHeight)
			{
				vector.y = (float)UI.screenHeight - InitialSize.y;
			}
			windowRect = new Rect(vector.x, vector.y, InitialSize.x, InitialSize.y);
		}

		private BodyColor activeMode;
		public override void DoWindowContents(Rect inRect)
        {
			base.DoWindowContents(inRect);

			foreach (var data in colorRects)
            {
				var colorOption = GetColorOptionFor(data.Key);
				if (colorOption != null)
                {
					if (colorOption.pawnFavoriteOnly)
                    {
						GUI.DrawTexture(data.Value, Textures.PawnFavoriteIcon);
					}
                    else
                    {
						Widgets.DrawBoxSolidWithOutline(data.Value, colorOption.color, Color.white);
					}
				}
				else
                {
					Widgets.DrawBoxSolidWithOutline(data.Value, Color.clear, Color.white);
				}

				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1 && Mouse.IsOver(data.Value))
				{
					activeMode = data.Key;
				}
				else if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Event.current.clickCount == 1 && Mouse.IsOver(data.Value))
                {
					RemoveColor();
				}
			}
			if (colorRects.ContainsKey(activeMode))
            {
				GUI.DrawTexture(colorRects[activeMode], Textures.GreenSelectionBox);
            }

			foreach (var data in stringRects)
            {
				Widgets.Label(data.Value, data.Key);
            }

			var colorSelectorRect = new Rect(37, 142, 570, 195);
			var oldColor = curColor;
			Widgets.ColorSelector(colorSelectorRect, ref curColor, ColorUtils.AllColors);
			if (oldColor != curColor)
            {
				SetColor();
            }
			var setIdeoColorRect = new Rect(40, inRect.yMax - 58, 174, 24f);
			if (Widgets.ButtonText(setIdeoColorRect, Strings.SetIdeologyColor))
			{
				curColor = Faction.OfPlayer.ideos.PrimaryIdeo.ApparelColor;
				SetColor();
			}

			var setPawnFavoritesColor = new Rect(setIdeoColorRect.xMax + 15, setIdeoColorRect.y, setIdeoColorRect.width, 24f);
			if (Widgets.ButtonText(setPawnFavoritesColor, Strings.SetPawnFavoritesColor))
			{
				if (this.colonistGroup.groupColor is null)
                {
					this.colonistGroup.groupColor = new GroupColor();
				}
				if (this.colonistGroup.groupColor.bodyColors is null)
				{
					this.colonistGroup.groupColor.bodyColors = new Dictionary<BodyColor, ColorOption>();
				}
				this.colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(true);
				curColor = Color.clear;
			}

			var clearGroupColor = new Rect(setPawnFavoritesColor.xMax + 15, setIdeoColorRect.y, setIdeoColorRect.width, 24f);
			if (Widgets.ButtonText(clearGroupColor, Strings.ClearGroupColor))
			{
				RemoveColor();
			}
		}

		private void RemoveColor()
        {
			if (this.colonistGroup.groupColor?.bodyColors != null && this.colonistGroup.groupColor.bodyColors.TryGetValue(activeMode, out var colorOption))
			{
				foreach (var pawn in this.colonistGroup.pawns)
                {
					if (activeMode == BodyColor.Hair)
					{
						if (pawn.style != null && pawn.style.nextHairColor.HasValue && pawn.style.nextHairColor.Value == colorOption.GetColor(pawn))
                        {
							pawn.style.nextHairColor = null;
							if (pawn.CurJobDef == JobDefOf.DyeHair)
                            {
								pawn.jobs.StopAll();
                            }
						}
					}
                    else if (pawn.apparel?.WornApparel != null)
                    {
						foreach (var apparel in pawn.apparel.WornApparel)
                        {
							if (apparel.def.IsHeadgear() && activeMode == BodyColor.Head)
							{
								TryClearDesiredColor(pawn, apparel, colorOption);
							}
							else if (apparel.def.IsFeetGear() && activeMode == BodyColor.Feet)
							{
								TryClearDesiredColor(pawn, apparel, colorOption);
							}
							else if (apparel.def.IsLegsGear() && activeMode == BodyColor.Legs)
							{
								TryClearDesiredColor(pawn, apparel, colorOption);
							}
							else if (apparel.def.IsTorsoGear() && activeMode == BodyColor.Torso)
							{
								TryClearDesiredColor(pawn, apparel, colorOption);
							}
							else if (apparel.def.IsArmsGear() && activeMode == BodyColor.Hands)
							{
								TryClearDesiredColor(pawn, apparel, colorOption);
							}
							else if (activeMode == BodyColor.All)
							{
								TryClearDesiredColor(pawn, apparel, colorOption);
							}
						}
                    }
				}

				this.colonistGroup.groupColor.bodyColors.Remove(activeMode);
			}
		}

		private void TryClearDesiredColor(Pawn pawn, Apparel apparel, ColorOption colorOption)
        {
			if (apparel.DesiredColor.HasValue && apparel.DesiredColor.Value == colorOption.GetColor(pawn))
            {
				apparel.DesiredColor = null;
				if (pawn.CurJobDef == JobDefOf.RecolorApparel)
				{
					pawn.jobs.StopAll();
				}
			}
        }
		private ColorOption GetColorOptionFor(BodyColor bodyColor)
        {
			if (colonistGroup.groupColor?.bodyColors != null)
            {
				if (colonistGroup.groupColor.bodyColors.TryGetValue(bodyColor, out var color))
                {
					return color;
                }
			}
			return null;
        }

		private void SetColor()
		{
			if (this.colonistGroup.groupColor is null)
			{
				this.colonistGroup.groupColor = new GroupColor();
			}
			if (this.colonistGroup.groupColor.bodyColors is null)
			{
				this.colonistGroup.groupColor.bodyColors = new Dictionary<BodyColor, ColorOption>();
			}
			switch (activeMode)
			{
				case BodyColor.All: this.colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Head: this.colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Torso: this.colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Feet: this.colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Hands: this.colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Legs: this.colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Hair: this.colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
			}
		}
	}
}
