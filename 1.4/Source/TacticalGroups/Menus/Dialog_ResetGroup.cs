using RimWorld;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class Dialog_ResetGroup : TieredFloatMenu
	{
		public override void SetInitialSizeAndPosition()
		{
			windowRect = new Rect((UI.screenWidth - InitialSize.x) / 2f, (UI.screenHeight - InitialSize.y) / 2f, InitialSize.x, InitialSize.y);
			windowRect = windowRect.Rounded();
		}
		public Dialog_ResetGroup(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.originRect = new Rect(originRect.x + originRect.width, originRect.y, originRect.width, originRect.height);
			this.colonistGroup = colonistGroup;
			closeOnClickedOutside = true;
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

			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			Rect rect1 = new Rect(55f, inRect.y + 25f, Textures.MenuButton.width, Textures.MenuButton.height);
			Widgets.Label(rect1, Strings.ResetGroupTitle);

			Rect rect2 = new Rect(55f, inRect.height - 75f, Textures.MenuButton.width, Textures.MenuButton.height);
			if (Mouse.IsOver(rect2))
			{
				GUI.DrawTexture(rect2, Textures.MenuButtonHover);
			}
			else
			{
				GUI.DrawTexture(rect2, Textures.MenuButton);
			}

			Widgets.Label(rect2, "OK".Translate());
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			if (!(Widgets.ButtonInvisible(rect2) || flag))
			{
				return;
			}
			colonistGroup.ResetGroupPolicies();
			Find.WindowStack.TryRemove(this);
		}
	}
}
