using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class Dialog_ColorPicker : TieredFloatMenu
	{
		public Color curColor;

		private readonly Dictionary<BodyColor, Rect> colorRects;
		private readonly Dictionary<string, Rect> stringRects;
		public Dialog_ColorPicker(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			closeOnClickedOutside = true;
			colorRects = new Dictionary<BodyColor, Rect>();
			stringRects = new Dictionary<string, Rect>();

			Rect torsoRect = new Rect(220, 30, 24, 24);
			colorRects[BodyColor.Torso] = torsoRect;
			stringRects[Strings.Torso] = new Rect(torsoRect.xMax + 10, torsoRect.y + 3, 40, 24);

			Rect handsRect = new Rect(torsoRect.x, torsoRect.yMax + 10, torsoRect.width, torsoRect.height);
			colorRects[BodyColor.Hands] = handsRect;
			stringRects[Strings.Hands] = new Rect(handsRect.xMax + 10, handsRect.y + 3, 40, 24);

			Rect allRect = new Rect(torsoRect.xMax + 60, torsoRect.y + 5, torsoRect.width * 2, torsoRect.height * 2);
			colorRects[BodyColor.All] = allRect;

			Rect legsRect = new Rect(allRect.xMax + 15, torsoRect.y + 3, 40, 24);
			stringRects[Strings.Legs] = legsRect;
			colorRects[BodyColor.Legs] = new Rect(legsRect.xMax, torsoRect.y, 24, 24);

			Rect feetRect = new Rect(legsRect.x, legsRect.yMax + 13, 40, 24);
			stringRects[Strings.Feet] = feetRect;
			colorRects[BodyColor.Feet] = new Rect(feetRect.xMax, feetRect.y - 3, 24, 24);

			Rect headRect = new Rect(colorRects[BodyColor.Legs].xMax + 10, torsoRect.y + 3, 40, 24);
			stringRects[Strings.Head] = headRect;
			colorRects[BodyColor.Head] = new Rect(headRect.xMax, torsoRect.y, 24, 24);

			Rect hairRect = new Rect(headRect.x, headRect.yMax + 13, 40, 24);
			stringRects[Strings.Hair] = hairRect;
			colorRects[BodyColor.Hair] = new Rect(hairRect.xMax, hairRect.y - 3, 24, 24);
		}
		protected override Vector2 InitialPositionShift => new Vector2(4f, 0f);
		public override void SetInitialSizeAndPosition()
		{
			Vector2 vector = UI.MousePositionOnUIInverted + InitialPositionShift;
			if (vector.x + InitialSize.x > UI.screenWidth)
			{
				vector.x = UI.screenWidth - InitialSize.x;
			}
			if (vector.y + InitialSize.y > UI.screenHeight)
			{
				vector.y = UI.screenHeight - InitialSize.y;
			}
			windowRect = new Rect(vector.x, vector.y, InitialSize.x, InitialSize.y);
		}

		private BodyColor activeMode;
		public override void DoWindowContents(Rect inRect)
		{
			base.DoWindowContents(inRect);

			foreach (KeyValuePair<BodyColor, Rect> data in colorRects)
			{
				ColorOption colorOption = GetColorOptionFor(data.Key);
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

			foreach (KeyValuePair<string, Rect> data in stringRects)
			{
				Widgets.Label(data.Value, data.Key);
			}

			Rect colorSelectorRect = new Rect(37, 142, 570, 195);
			Color oldColor = curColor;
			Widgets.ColorSelector(colorSelectorRect, ref curColor, ColorUtils.AllColors, out _);
			if (oldColor != curColor)
			{
				SetColor();
			}
			Rect setIdeoColorRect = new Rect(40, inRect.yMax - 58, 174, 24f);
			if (Widgets.ButtonText(setIdeoColorRect, Strings.SetIdeologyColor))
			{
				curColor = Faction.OfPlayer.ideos.PrimaryIdeo.ApparelColor;
				SetColor();
			}

			Rect setPawnFavoritesColor = new Rect(setIdeoColorRect.xMax + 15, setIdeoColorRect.y, setIdeoColorRect.width, 24f);
			if (Widgets.ButtonText(setPawnFavoritesColor, Strings.SetPawnFavoritesColor))
			{
				if (colonistGroup.groupColor is null)
				{
					colonistGroup.groupColor = new GroupColor();
				}
				if (colonistGroup.groupColor.bodyColors is null)
				{
					colonistGroup.groupColor.bodyColors = new Dictionary<BodyColor, ColorOption>();
				}
				colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(true);
				curColor = Color.clear;
			}

			Rect clearGroupColor = new Rect(setPawnFavoritesColor.xMax + 15, setIdeoColorRect.y, setIdeoColorRect.width, 24f);
			if (Widgets.ButtonText(clearGroupColor, Strings.ClearGroupColor))
			{
				RemoveColor();
			}
		}

		private void RemoveColor()
		{
			if (colonistGroup.groupColor?.bodyColors != null && colonistGroup.groupColor.bodyColors.TryGetValue(activeMode, out ColorOption colorOption))
			{
				foreach (Pawn pawn in colonistGroup.pawns)
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
						foreach (Apparel apparel in pawn.apparel.WornApparel)
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

				colonistGroup.groupColor.bodyColors.Remove(activeMode);
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
				if (colonistGroup.groupColor.bodyColors.TryGetValue(bodyColor, out ColorOption color))
				{
					return color;
				}
			}
			return null;
		}

		private void SetColor()
		{
			if (colonistGroup.groupColor is null)
			{
				colonistGroup.groupColor = new GroupColor();
			}
			if (colonistGroup.groupColor.bodyColors is null)
			{
				colonistGroup.groupColor.bodyColors = new Dictionary<BodyColor, ColorOption>();
			}
			switch (activeMode)
			{
				case BodyColor.All: colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Head: colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Torso: colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Feet: colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Hands: colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Legs: colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
				case BodyColor.Hair: colonistGroup.groupColor.bodyColors[activeMode] = new ColorOption(curColor); break;
			}
		}
	}
}
