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
	public class TieredFloatMenu : Window
	{
		public ColonistGroup colonistGroup;

		public List<TieredFloatMenu> childWindows;
		public bool Selected => childWindows != null && childWindows.Any();

		public TieredFloatMenu parentWindow;
		public bool HasActiveParent => parentWindow != null; 

		public Rect originRect;

		public Action onCloseCallback;

		protected List<TieredFloatMenuOption> options;

		protected Color baseColor = Color.white;
		protected virtual Vector2 InitialPositionShift => new Vector2(4f, 0f);
		protected virtual Vector2 InitialFloatOptionPositionShift => new Vector2(0f, 0f);
		protected override float Margin => 0f;
		public override Vector2 InitialSize => new Vector2(this.backgroundTexture.width, this.backgroundTexture.height);

		public Texture2D backgroundTexture;
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

		public TieredFloatMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
		{
			this.soundAmbient = null;
			this.soundAppear = null;
			this.soundClose = null;
			this.parentWindow = parentWindow;
			this.colonistGroup = colonistGroup;
			this.originRect = originRect;
			this.backgroundTexture = backgroundTexture;
			this.layer = WindowLayer.GameUI;
			closeOnClickedOutside = false;
			doWindowBackground = false;
			drawShadow = false;
			preventCameraMotion = false;
		}

		protected override void SetInitialSizeAndPosition()
		{
			Vector2 vector = new Vector2(originRect.x + originRect.width, originRect.y) + InitialPositionShift;
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

		protected void ShiftParentWindowsX(float toShift)
		{
			if (this.parentWindow != null)
			{
				this.parentWindow.windowRect.x -= toShift;
				this.parentWindow.ShiftParentWindowsX(toShift);
			}
		}

		protected void ShiftParentWindowsY(float toShift)
		{
			if (this.parentWindow != null)
			{
				this.parentWindow.windowRect.y -= toShift;
				this.parentWindow.ShiftParentWindowsY(toShift);
			}
		}
		public override void DoWindowContents(Rect rect)
		{
			UpdateBaseColor();
			GUI.color = baseColor;
			GUI.DrawTexture(rect, backgroundTexture, ScaleMode.ScaleToFit);
		}

		public virtual void DrawExtraGui(Rect rect)
		{

		}

		public void MarkOptionAsSelected(TieredFloatMenuOption option)
        {
			option.selected = true;
			foreach (var otherOption in options)
			{
				if (otherOption != option)
				{
					otherOption.selected = false;
				}
			}
		}

		public void OpenNewMenu(TieredFloatMenu floatMenu)
		{
			if (this.childWindows != null)
            {
				for (int num = childWindows.Count - 1; num >= 0; num--)
                {
					childWindows[num].Close();
				}
				this.childWindows.Clear();
			}
			else
            {
				this.childWindows = new List<TieredFloatMenu>();
            }
			this.childWindows.Add(floatMenu);
			Find.WindowStack.Add(floatMenu);
		}

        public override void PostOpen()
        {
            base.PostOpen();
			TacticDefOf.TG_MenuButtonOpenMenus.PlayOneShotOnCamera();
		}
		public void TryCloseChildWindow()
        {
			if (childWindows != null)
			{
				for (int num = childWindows.Count - 1; num >= 0; num--)
                {
					childWindows[num].Close();
                }
				this.childWindows = null;
			}
		}
		public override void PostClose()
		{
			TryCloseChildWindow();
			base.PostClose();
			if (onCloseCallback != null)
			{
				onCloseCallback();
			}
			//Log.Message("PostClose");
		}

		public void Cancel()
		{
			TryCloseChildWindow();
			Find.WindowStack.TryRemove(this);
			//Log.Message("Cancel");
		}

		public override void Close(bool doCloseSound = true)
        {
			parentWindow?.childWindows?.Remove(this);
			TryCloseChildWindow();
			base.Close(doCloseSound);
			//Log.Message("Close");
        }

		public void CloseAllWindows()
        {
			this.Close();
			this.parentWindow?.CloseAllWindows();
			if (this.childWindows != null)
            {
				foreach (var childWindow in this.childWindows)
				{
					childWindow.CloseAllWindows();
				}
			}
        }

        public virtual void PreOptionChosen(TieredFloatMenuOption opt)
		{
		}

		protected virtual void UpdateBaseColor()
		{
			if (!Selected && !HasActiveParent)
			{
				baseColor = Color.white;
				Rect r = new Rect(0f, 0f, backgroundTexture.width, backgroundTexture.height).ContractedBy(-5f);
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
}
