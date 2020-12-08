using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	public class TieredFloatMenu : Window
	{
		public ColonistGroup colonistGroup;

		public TieredFloatMenu childWindow;
		public bool Selected => childWindow != null;

		public TieredFloatMenu parentWindow;
		public bool HasActiveParent => parentWindow != null; 

		public Rect originRect;

		public bool vanishIfMouseDistant = true;

		public Action onCloseCallback;

		protected List<TieredFloatMenuOption> options;

		private Color baseColor = Color.white;
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
			layer = WindowLayer.Super;
			closeOnClickedOutside = true;
			doWindowBackground = false;
			drawShadow = false;
			preventCameraMotion = false;
		}

		protected override void SetInitialSizeAndPosition()
		{
			Vector2 vector = new Vector2(originRect.x + originRect.width, originRect.y) + InitialPositionShift;

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
			if (this.childWindow != null)
			{
				this.childWindow.Close();
			}
			this.childWindow = floatMenu;
			Find.WindowStack.Add(floatMenu);
		}

        public override void PostOpen()
        {
            base.PostOpen();
        }
        public void TryCloseChildWindow()
        {
			if (childWindow != null)
			{
				childWindow.Close();
				this.childWindow = null;
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
			if (colonistGroup != null)
            {
				colonistGroup.showPawnIconsRightClickMenu = false;
			}
		}

		public void Cancel()
		{
			TryCloseChildWindow();
			Find.WindowStack.TryRemove(this);
			Log.Message("Cancel");
		}

        public override void Close(bool doCloseSound = true)
        {
			TryCloseChildWindow();
			Log.Message("Close");
			base.Close(doCloseSound);
        }

		public void CloseAllWindows()
        {
			this.Close();
			this.parentWindow?.CloseAllWindows();
        }

        public virtual void PreOptionChosen(TieredFloatMenuOption opt)
		{
		}

		private void UpdateBaseColor()
		{
			if (!Selected && !HasActiveParent)
			{
				baseColor = Color.white;
				if (!vanishIfMouseDistant)
				{
					return;
				}
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
