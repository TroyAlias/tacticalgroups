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

		public override Vector2 InitialSize => new Vector2(Textures.DropMenuRightClick.width, Textures.DropMenuRightClick.height);

		private float MaxWindowHeight => (float)UI.screenHeight * 0.9f;
				
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
			zero.x += 3;
			zero.y += 3;
			GUI.DrawTexture(rect, Textures.DropMenuRightClick, ScaleMode.ScaleToFit);
			for (int i = 0; i < options.Count; i++)
			{
				ColonistBarFloatMenuOption floatMenuOption = options[i];
				Rect rect2 = new Rect(zero.x, zero.y, Textures.DropMenuRightClick.width, floatMenuOption.curIcon.height);
				if (floatMenuOption.DoGUI(rect2, givesColonistOrders, this))
				{
					Find.WindowStack.TryRemove(this);
					break;
				}
				zero.y += floatMenuOption.bottomIndent;
			}
			DrawSmallButtonsBottom(rect);
			if (Event.current.type == EventType.MouseDown)
			{
				Event.current.Use();
				Close();
			}
			GUI.color = Color.white;
		}

		public void DrawSmallButtonsBottom(Rect rect)
		{
			var iconRect = new Rect(rect.x + 7f, rect.y + (rect.height - Textures.EyeIconOn.height) - 7f, Textures.EyeIconOn.width, Textures.EyeIconOn.height);
			if (this.colonistGroup.entireGroupIsVisible)
            {
				if (Mouse.IsOver(iconRect))
				{
					GUI.DrawTexture(iconRect, Textures.EyeIconOffHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						this.Close();
						foreach (var pawnIcon in this.colonistGroup.pawnIcons)
						{
							pawnIcon.Value.isVisibleOnColonistBar = false;
						}
						this.colonistGroup.entireGroupIsVisible = false;
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
						Event.current.Use();
					}
				}
				else
				{
					GUI.DrawTexture(iconRect, Textures.EyeIconOff);
				}
			}
			else
            {
				if (Mouse.IsOver(iconRect))
				{
					GUI.DrawTexture(iconRect, Textures.EyeIconOnHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						this.Close();
						foreach (var pawnIcon in this.colonistGroup.pawnIcons)
						{
							pawnIcon.Value.isVisibleOnColonistBar = true;
						}
						this.colonistGroup.entireGroupIsVisible = true;
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
						Event.current.Use();
					}
				}
				else
				{
					GUI.DrawTexture(iconRect, Textures.EyeIconOn);
				}
			}


			var disbandRect = new Rect(rect.x + (rect.width - Textures.DisbandIcon.width) - 7f, rect.y + (rect.height - Textures.DisbandIcon.height) - 7f, Textures.DisbandIcon.width, Textures.DisbandIcon.height);
			if (Mouse.IsOver(disbandRect))
			{
				GUI.DrawTexture(disbandRect, Textures.DisbandIconHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					this.Close();
					TacticUtils.Groups.Remove(this.colonistGroup);
					TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					Event.current.Use();
				}
			}
			else
			{
				GUI.DrawTexture(disbandRect, Textures.DisbandIcon);
			}
		}
		public override void PostClose()
		{
			base.PostClose();
			if (onCloseCallback != null)
			{
				onCloseCallback();
			}
			colonistGroup.showPawnIconsRightClickMenu = false;
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
			Rect r = new Rect(0f, 0f, Textures.DropMenuRightClick.width, Textures.DropMenuRightClick.height).ContractedBy(-5f);
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
