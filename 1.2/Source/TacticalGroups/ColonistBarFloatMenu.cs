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
	public class ColonistBarFloatMenu : Window
	{
		public ColonistGroup colonistGroup;

		public Rect originRect;

		public bool givesColonistOrders;

		public bool vanishIfMouseDistant = true;

		public Action onCloseCallback;

		protected List<ColonistBarFloatMenuOption> options;

		private string title;

		private bool needSelection;

		private Color baseColor = Color.white;

		private Vector2 scrollPosition;

		private static readonly Vector2 TitleOffset = new Vector2(30f, -25f);

		private const float OptionSpacing = -1f;

		private const float MaxScreenHeightPercent = 0.9f;

		private const float MinimumColumnWidth = 70f;

		private static readonly Vector2 InitialPositionShift = new Vector2(4f, 0f);

		private const float FadeStartMouseDist = 5f;

		private const float FadeFinishMouseDist = 100f;

		protected override float Margin => 0f;

		public static readonly Texture2D DropMenuRightClick = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/DropMenuRightClick");
		public override Vector2 InitialSize => new Vector2(DropMenuRightClick.width, DropMenuRightClick.height);

		private float MaxWindowHeight => (float)UI.screenHeight * 0.9f;
		
		private float TotalWindowHeight => Mathf.Min(TotalViewHeight, MaxWindowHeight) + 1f;
		
		private float MaxViewHeight
		{
			get
			{
				if (UsingScrollbar)
				{
					float num = 0f;
					float num2 = 0f;
					for (int i = 0; i < options.Count; i++)
					{
						float requiredHeight = options[i].RequiredHeight;
						if (requiredHeight > num)
						{
							num = requiredHeight;
						}
						num2 += requiredHeight + -1f;
					}
					int columnCount = ColumnCount;
					num2 += (float)columnCount * num;
					return num2 / (float)columnCount;
				}
				return MaxWindowHeight;
			}
		}
		
		private float TotalViewHeight
		{
			get
			{
				float num = 0f;
				float num2 = 0f;
				float maxViewHeight = MaxViewHeight;
				for (int i = 0; i < options.Count; i++)
				{
					float requiredHeight = options[i].RequiredHeight;
					if (num2 + requiredHeight + -1f > maxViewHeight)
					{
						if (num2 > num)
						{
							num = num2;
						}
						num2 = requiredHeight;
					}
					else
					{
						num2 += requiredHeight + -1f;
					}
				}
				return Mathf.Max(num, num2);
			}
		}
		
		private float TotalWidth
		{
			get
			{
				float num = (float)ColumnCount * ColumnWidth;
				if (UsingScrollbar)
				{
					num += 16f;
				}
				return num;
			}
		}
		
		private float ColumnWidth
		{
			get
			{
				float num = 70f;
				for (int i = 0; i < options.Count; i++)
				{
					float requiredWidth = options[i].RequiredWidth;
					if (requiredWidth >= 300f)
					{
						return 300f;
					}
					if (requiredWidth > num)
					{
						num = requiredWidth;
					}
				}
				return Mathf.Round(num);
			}
		}
		
		private int MaxColumns => Mathf.FloorToInt(((float)UI.screenWidth - 16f) / ColumnWidth);
		
		private bool UsingScrollbar => ColumnCountIfNoScrollbar > MaxColumns;
		
		private int ColumnCount => Mathf.Min(ColumnCountIfNoScrollbar, MaxColumns);
		
		private int ColumnCountIfNoScrollbar
		{
			get
			{
				if (options == null)
				{
					return 1;
				}
				Text.Font = GameFont.Small;
				int num = 1;
				float num2 = 0f;
				float maxWindowHeight = MaxWindowHeight;
				for (int i = 0; i < options.Count; i++)
				{
					float requiredHeight = options[i].RequiredHeight;
					if (num2 + requiredHeight + -1f > maxWindowHeight)
					{
						num2 = requiredHeight;
						num++;
					}
					else
					{
						num2 += requiredHeight + -1f;
					}
				}
				return num;
			}
		}

		public FloatMenuSizeMode SizeMode
		{
			get
			{
				if (options.Count > 60)
				{
					return FloatMenuSizeMode.Tiny;
				}
				return FloatMenuSizeMode.Normal;
			}
		}

		public ColonistBarFloatMenu(List<ColonistBarFloatMenuOption> options, ColonistGroup colonistGroup, Rect originRect)
		{
			this.colonistGroup = colonistGroup;
			this.originRect = originRect;
			if (options.NullOrEmpty())
			{
				Log.Error("Created FloatMenu with no options. Closing.");
				Close();
			}
			this.options = options.OrderByDescending((ColonistBarFloatMenuOption op) => op.Priority).ToList();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
			layer = WindowLayer.Super;
			closeOnClickedOutside = true;
			doWindowBackground = false;
			drawShadow = false;
			preventCameraMotion = false;
			SoundDefOf.FloatMenu_Open.PlayOneShotOnCamera();
		}

		public ColonistBarFloatMenu(List<ColonistBarFloatMenuOption> options, ColonistGroup colonistGroup, Rect originRect, string title, bool needSelection = false) : this(options, colonistGroup, originRect)
		{
			this.title = title;
			this.needSelection = needSelection;
		}

		protected override void SetInitialSizeAndPosition()
		{
			Vector2 vector = new Vector2(originRect.x + originRect.width, originRect.y + originRect.height) + InitialPositionShift;

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

		//public override void ExtraOnGUI()
		//{
		//	base.ExtraOnGUI();
		//	if (!title.NullOrEmpty())
		//	{
		//		Vector2 vector = new Vector2(windowRect.x, windowRect.y);
		//		Text.Font = GameFont.Small;
		//		float width = Mathf.Max(150f, 15f + Text.CalcSize(title).x);
		//		Rect titleRect = new Rect(vector.x + TitleOffset.x, vector.y + TitleOffset.y, width, 23f);
		//		Find.WindowStack.ImmediateWindow(6830963, titleRect, WindowLayer.Super, delegate
		//		{
		//			GUI.color = baseColor;
		//			Text.Font = GameFont.Small;
		//			Rect position = titleRect.AtZero();
		//			position.width = 150f;
		//			GUI.DrawTexture(position, TexUI.TextBGBlack);
		//			Rect rect = titleRect.AtZero();
		//			rect.x += 15f;
		//			Text.Anchor = TextAnchor.MiddleLeft;
		//			Widgets.Label(rect, title);
		//			Text.Anchor = TextAnchor.UpperLeft;
		//		}, doBackground: false, absorbInputAroundWindow: false, 0f);
		//	}
		//}

		public override void DoWindowContents(Rect rect)
		{
			if (needSelection && Find.Selector.SingleSelectedThing == null)
			{
				Find.WindowStack.TryRemove(this);
				return;
			}
			UpdateBaseColor();
			GUI.color = baseColor;
			Text.Font = GameFont.Small;
			Vector2 zero = Vector2.zero;
			GUI.DrawTexture(rect, DropMenuRightClick, ScaleMode.ScaleToFit);
			for (int i = 0; i < options.Count; i++)
			{
				ColonistBarFloatMenuOption floatMenuOption = options[i];
				Rect rect2 = new Rect(zero.x, zero.y, DropMenuRightClick.width, floatMenuOption.curIcon.height);
				if (floatMenuOption.DoGUI(rect2, givesColonistOrders, this))
				{
					Find.WindowStack.TryRemove(this);
					break;
				}
				zero.y += floatMenuOption.bottomIndent;
			}
			if (Event.current.type == EventType.MouseDown)
			{
				Event.current.Use();
				Close();
			}
			GUI.color = Color.white;
		}

		public override void PostClose()
		{
			base.PostClose();
			if (onCloseCallback != null)
			{
				onCloseCallback();
			}
		}

		public void Cancel()
		{
			SoundDefOf.FloatMenu_Cancel.PlayOneShotOnCamera();
			Find.WindowStack.TryRemove(this);
		}

		public virtual void PreOptionChosen(ColonistBarFloatMenuOption opt)
		{
		}

		private void UpdateBaseColor()
		{
			baseColor = Color.white;
			if (!vanishIfMouseDistant)
			{
				return;
			}
			Rect r = new Rect(0f, 0f, TotalWidth, TotalWindowHeight).ContractedBy(-5f);
			if (!r.Contains(Event.current.mousePosition))
			{
				float num = GenUI.DistFromRect(r, Event.current.mousePosition);
				baseColor = new Color(1f, 1f, 1f, 1f - num / 95f);
				if (num > 95f)
				{
					Close(doCloseSound: false);
					Cancel();
				}
			}
		}
	}
}
