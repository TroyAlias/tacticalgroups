using RimWorld;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
	public class Dialog_PresetRemove : TieredFloatMenu
	{
		private readonly GroupPreset groupPreset;
		public override void SetInitialSizeAndPosition()
		{
			//windowRect = new Rect(originRect.x, originRect.y, InitialSize.x, InitialSize.y);
			//windowRect = windowRect.Rounded();
			windowRect = new Rect((UI.screenWidth - InitialSize.x) / 2f, (UI.screenHeight - InitialSize.y) / 2f, InitialSize.x, InitialSize.y);
			windowRect = windowRect.Rounded();
		}
		public Dialog_PresetRemove(TieredFloatMenu parentWindow, GroupPreset groupPreset, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.originRect = new Rect(originRect.x + originRect.width, originRect.y, originRect.width, originRect.height);
			this.colonistGroup = colonistGroup;
			this.groupPreset = groupPreset;
			closeOnClickedOutside = true;
		}

		public override void DoWindowContents(Rect inRect)
		{
			base.DoWindowContents(inRect);
			bool flag = false;
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				flag = true;
				Event.current.Use();
			}

			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			Rect rect1 = new Rect(55f, inRect.y + 25f, Textures.MenuButton.width, Textures.MenuButton.height);
			Widgets.Label(rect1, Strings.RemovePreset);

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
			TacticalGroupsSettings.RemoveGroupPreset(groupPreset);
			Find.WindowStack.TryRemove(this);
		}
	}
}
