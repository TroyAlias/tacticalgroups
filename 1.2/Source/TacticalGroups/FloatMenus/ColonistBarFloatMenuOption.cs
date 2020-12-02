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
	[StaticConstructorOnStartup]
	public class ColonistBarFloatMenuOption
	{
		private string labelInt;

		public float bottomIndent;

		public Action action;

		private MenuOptionPriority priorityInt = MenuOptionPriority.Default;

		public bool autoTakeable;

		public float autoTakeablePriority;

		public Action mouseoverGuiAction;

		public Thing revalidateClickTarget;

		public WorldObject revalidateWorldClickTarget;

		public float extraPartWidth;

		public Func<Rect, bool> extraPartOnGUI;

		public string tutorTag;

		private FloatMenuSizeMode sizeMode;

		private bool drawPlaceHolderIcon;

		private ThingDef shownItem;

		private Texture2D itemIcon;

		private Color iconColor = Color.white;

		public const float MaxWidth = 300f;

		private static readonly Color ColorTextActive = Color.white;

		private static readonly Color ColorTextDisabled = new Color(0.9f, 0.9f, 0.9f);

		public const float ExtraPartHeight = 30f;

		public Texture2D curIcon;
		public Texture2D iconHover;
		public Texture2D iconSelected;
		public string Label
		{
			get
			{
				return labelInt;
			}
			set
			{
				if (value.NullOrEmpty())
				{
					value = "(missing label)";
				}
				labelInt = value.TrimEnd();
				SetSizeMode(sizeMode);
			}
		}

		private float VerticalMargin
		{
			get
			{
				if (sizeMode != FloatMenuSizeMode.Normal)
				{
					return 1f;
				}
				return 4f;
			}
		}

		private float HorizontalMargin
		{
			get
			{
				if (sizeMode != FloatMenuSizeMode.Normal)
				{
					return 3f;
				}
				return 6f;
			}
		}

		private float IconOffset
		{
			get
			{
				if (shownItem == null && !drawPlaceHolderIcon && !(itemIcon != null))
				{
					return 0f;
				}
				return 27f;
			}
		}

		private GameFont CurrentFont
		{
			get
			{
				if (sizeMode != FloatMenuSizeMode.Normal)
				{
					return GameFont.Tiny;
				}
				return GameFont.Small;
			}
		}

		public bool Disabled
		{
			get
			{
				return action == null;
			}
			set
			{
				if (value)
				{
					action = null;
				}
			}
		}
		public MenuOptionPriority Priority
		{
			get
			{
				if (Disabled)
				{
					return MenuOptionPriority.DisabledOption;
				}
				return priorityInt;
			}
			set
			{
				if (Disabled)
				{
					Log.Error("Setting priority on disabled FloatMenuOption: " + Label);
				}
				priorityInt = value;
			}
		}

		private TextAnchor textAnchor;
		private float leftTextIndent;
		public ColonistBarFloatMenuOption(string label, Action action, Texture2D icon, Texture2D hoverIcon, Texture2D selectedIcon, TextAnchor textAnchor = TextAnchor.MiddleCenter,
			MenuOptionPriority priority = MenuOptionPriority.Default, float leftTextIndent = 0f, Action mouseoverGuiAction = null, Thing revalidateClickTarget = null, 
			float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null)
		{
			this.labelInt = label;
			this.textAnchor = textAnchor;
			this.leftTextIndent = leftTextIndent;
			this.curIcon = icon;
			this.iconHover = hoverIcon;
			this.iconSelected = selectedIcon;
			this.action = action;
			priorityInt = priority;
			this.revalidateClickTarget = revalidateClickTarget;
			this.mouseoverGuiAction = mouseoverGuiAction;
			this.extraPartWidth = extraPartWidth;
			this.extraPartOnGUI = extraPartOnGUI;
			this.revalidateWorldClickTarget = revalidateWorldClickTarget;
		}

		public void SetSizeMode(FloatMenuSizeMode newSizeMode)
		{
			sizeMode = newSizeMode;
		}

		public void Chosen(bool colonistOrdering, ColonistBarFloatMenu floatMenu)
		{
			floatMenu?.PreOptionChosen(this);
			if (!Disabled)
			{
				if (action != null)
				{
					if (colonistOrdering)
					{
						SoundDefOf.ColonistOrdered.PlayOneShotOnCamera();
					}
					action();
				}
			}
			else
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
			}
		}


		public virtual bool DoGUI(Rect rect, bool colonistOrdering, ColonistBarFloatMenu floatMenu)
		{
			if (curIcon != null)
			{
				rect = new Rect(rect.x, rect.y, curIcon.width, curIcon.height);
			}
			Rect rect2 = rect;
			rect2.height--;
			bool flag = !Disabled && Mouse.IsOver(rect2);
			bool flag2 = false;
			Text.Font = CurrentFont;
			Rect rect3 = rect;
			rect3.xMin += 4f;
			rect3.xMax = rect.x + 27f;
			rect3.yMin += 4f;
			rect3.yMax = rect.y + 27f;
			if (flag)
			{
				rect3.x += 4f;
			}
			Rect rect4 = rect;
			rect4.xMin += HorizontalMargin;
			rect4.xMax -= HorizontalMargin;
			rect4.xMax -= 4f;
			rect4.xMax -= extraPartWidth + IconOffset;
			rect4.x += IconOffset;
			if (flag)
			{
				rect4.x += 4f;
			}
			Rect rect5 = default(Rect);
			if (extraPartWidth != 0f)
			{
				float num = Mathf.Min(Text.CalcSize(Label).x, rect4.width - 4f);
				rect5 = new Rect(rect4.xMin + num, rect4.yMin, extraPartWidth, 30f);
				flag2 = Mouse.IsOver(rect5);
			}
			if (!Disabled)
			{
				MouseoverSounds.DoRegion(rect2);
			}
			Color color = GUI.color;
			//if (Disabled)
			//{
			//	GUI.color = ColorBGDisabled * color;
			//}
			//else if (flag && !flag2)
			//{
			//	GUI.color = ColorBGActiveMouseover * color;
			//}
			//else
			//{
			//	GUI.color = ColorBGActive * color;
			//}
			//GUI.DrawTexture(rect, BaseContent.WhiteTex);
			if (Mouse.IsOver(rect) && iconHover != null)
            {
				GUI.DrawTexture(rect, iconHover);
				Log.Message("iconHover: " + iconHover);
			}
			else if (curIcon != null)
            {
				GUI.DrawTexture(rect, curIcon);
            }
			if (labelInt != null)
            {
				Text.Anchor = this.textAnchor;
				var textRect = new Rect(rect);
				textRect.x += this.leftTextIndent;
				Widgets.Label(textRect, Label);
				Text.Anchor = TextAnchor.UpperLeft;
			}
			GUI.color = ((!Disabled) ? ColorTextActive : ColorTextDisabled) * color;
			if (sizeMode == FloatMenuSizeMode.Tiny)
			{
				rect4.y += 1f;
			}
			Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);

			GUI.color = iconColor;
			if (shownItem != null || drawPlaceHolderIcon)
			{
				Widgets.DefIcon(rect3, shownItem, null, 1f, drawPlaceHolderIcon);
			}
			else if ((bool)itemIcon)
			{
				GUI.DrawTexture(rect3, itemIcon);
			}
			GUI.color = color;
			if (extraPartOnGUI != null)
			{
				bool num2 = extraPartOnGUI(rect5);
				GUI.color = color;
				if (num2)
				{
					return true;
				}
			}
			if (flag && mouseoverGuiAction != null)
			{
				mouseoverGuiAction();
			}
			if (tutorTag != null)
			{
				UIHighlighter.HighlightOpportunity(rect, tutorTag);
			}
			if (Widgets.ButtonInvisible(rect2))
			{
				if (tutorTag != null && !TutorSystem.AllowAction(tutorTag))
				{
					return false;
				}
				Chosen(colonistOrdering, floatMenu);
				if (tutorTag != null)
				{
					TutorSystem.Notify_Event(tutorTag);
				}
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return "FloatMenuOption(" + Label + ", " + (Disabled ? "disabled" : "enabled") + ")";
		}
	}
}