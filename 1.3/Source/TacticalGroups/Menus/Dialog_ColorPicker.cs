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
	public class Dialog_ColorPicker : Window
	{
		public Color color;
		public override Vector2 InitialSize
        {
            get
            {
				var x = 400;
				var colorCount = AllColors.Count;
				var rowCount = colorCount / 13f;
				var y = (rowCount * 30) + 40 + 40;
				return new Vector2(x, y);
            }
        }
		public ColonistGroup colonistGroup;
		public Dialog_ColorPicker(ColonistGroup colonistGroup)
		{
			this.colonistGroup = colonistGroup;
			this.closeOnClickedOutside = true;
		}

		private List<Color> allColors;
		private List<Color> AllColors
		{
			get
			{
				if (this.allColors == null)
				{
					this.allColors = (from x in DefDatabase<ColorDef>.AllDefsListForReading
									  where !x.hairOnly
									  select x into ic
									  select ic.color).ToList<Color>();
					this.allColors.Distinct().ToList().SortByColor((Color x) => x);
				}
				return this.allColors;
			}
		}

		private static readonly Vector2 InitialPositionShift = new Vector2(4f, 0f);
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
		public override void DoWindowContents(Rect inRect)
        {
			Widgets.ColorSelector(inRect, ref color, AllColors);
			var setGroupColorRect = new Rect(inRect.x, inRect.yMax - 24f, inRect.width / 2f - 10f, 24f);
			if (Widgets.ButtonText(setGroupColorRect, Strings.SetGroupColor))
			{
				this.colonistGroup.groupColor = color;
			}
			var removeGroupColorRect = new Rect(setGroupColorRect.xMax + 10, setGroupColorRect.y, inRect.width / 2f - 10f, 24f);
			if (Widgets.ButtonText(removeGroupColorRect, Strings.RemoveGroupColor))
			{
				this.colonistGroup.groupColor = null;
			}
		}
	}
}
