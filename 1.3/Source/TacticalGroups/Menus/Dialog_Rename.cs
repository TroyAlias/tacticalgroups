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
	public abstract class Dialog_Rename : TieredFloatMenu
	{
		protected string curName;

		private bool focusedRenameField;

		private int startAcceptingInputAtFrame;

		private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

		protected virtual int MaxNameLength => 28;

		public override Vector2 InitialSize => new Vector2(280f, 175f);

		protected string confirmationText;
		public Dialog_Rename(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, string confirmationText)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.parentWindow = parentWindow;
			this.confirmationText = confirmationText;
			forcePause = true;
			doCloseX = true;
			absorbInputAroundWindow = true;
			closeOnAccept = false;
			closeOnClickedOutside = true;
		}

		public void WasOpenedByHotkey()
		{
			startAcceptingInputAtFrame = Time.frameCount + 1;
		}

		protected virtual AcceptanceReport NameIsValid(string name)
		{
			if (name.Length == 0)
			{
				return false;
			}
			return true;
		}

		public override void DoWindowContents(Rect inRect)
		{
			base.DoWindowContents(inRect);
			Text.Font = GameFont.Small;
			bool flag = false;
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				flag = true;
				Event.current.Use();
			}
			GUI.SetNextControlName("RenameField");
			string text = Widgets.TextField(new Rect(19f, 36f, inRect.width - 40f, 35f), curName);
			if (AcceptsInput && text.Length < MaxNameLength)
			{
				curName = text;
			}
			else if (!AcceptsInput)
			{
				((TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl)).SelectAll();
			}
			if (!focusedRenameField)
			{
				UI.FocusControl("RenameField", this);
				focusedRenameField = true;
			}

			var rect2 = new Rect(55f, inRect.height - 75f, Textures.MenuButton.width, Textures.MenuButton.height);
			if (Mouse.IsOver(rect2))
            {
				GUI.DrawTexture(rect2, Textures.MenuButtonHover);
            }
			else
            {
				GUI.DrawTexture(rect2, Textures.MenuButton);
            }
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect2, confirmationText);;
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			if (!(Widgets.ButtonInvisible(rect2) || flag))
			{
				return;
			}
			AcceptanceReport acceptanceReport = NameIsValid(curName);
			if (!acceptanceReport.Accepted)
			{
				if (acceptanceReport.Reason.NullOrEmpty())
				{
					Messages.Message("NameIsInvalid".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
				else
				{
					Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, historical: false);
				}
			}
			else
			{
				SetName(curName);
				Find.WindowStack.TryRemove(this);
			}
		}



		protected abstract void SetName(string name);
	}
}
