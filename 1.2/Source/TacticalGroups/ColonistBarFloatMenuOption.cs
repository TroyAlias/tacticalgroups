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

		public static readonly Texture2D RallyIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/RallyIcon");
		public static readonly Texture2D RallyIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/RallyIconHover");

		public static readonly Texture2D ActionsIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ActionsIcon");
		public static readonly Texture2D ActionsIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ActionsIconHover");
		public static readonly Texture2D ActionsIconPress = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ActionsIconPress");
		public static readonly Texture2D OrdersIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/OrdersIcon");
		public static readonly Texture2D OrdersIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/OrdersIconHover");
		public static readonly Texture2D OrdersIconPress = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/OrdersIconPress");
		public static readonly Texture2D ManageIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ManageIcon");
		public static readonly Texture2D ManageIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ManageIconHover");
		public static readonly Texture2D ManageIconPress = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ManageIconPress");

		public static Texture2D BackgroundColonistLayer = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ColonistUnderLayer");

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

		public ColonistBarFloatMenuOption(Action action, Texture2D icon, Texture2D hoverIcon, Texture2D selectedIcon,
			MenuOptionPriority priority = MenuOptionPriority.Default, Action mouseoverGuiAction = null, Thing revalidateClickTarget = null, 
			float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null)
		{
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
			if (Mouse.IsOver(rect))
			{
				Log.Message("2 iconHover: " + iconHover);
			}
			GUI.color = ((!Disabled) ? ColorTextActive : ColorTextDisabled) * color;
			if (sizeMode == FloatMenuSizeMode.Tiny)
			{
				rect4.y += 1f;
			}
			Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);
			Text.Anchor = TextAnchor.MiddleLeft;
			//Widgets.Label(rect4, Label);
			Text.Anchor = TextAnchor.UpperLeft;
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
