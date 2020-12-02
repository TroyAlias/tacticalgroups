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
	public class ManageMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 55f);
		public ManageMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddRenameButton();
			AddIconButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public void AddRenameButton()
        {
			var option = new TieredFloatMenuOption(Strings.Rename, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				this.childWindow?.Close();
				MarkOptionAsSelected(option);
				Find.WindowStack.Add(new Dialog_RenameColonistGroup(this.colonistGroup, windowRect, option));
			};
			option.bottomIndent = Textures.AOMButton.height + 5;
			options.Add(option);
		}

		public void AddIconButton()
		{
			var option = new TieredFloatMenuOption(Strings.Icon, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddIconWindow(option);
			};
			option.bottomIndent = Textures.AOMButton.height + 5;
			options.Add(option);
		}
		public void AddIconWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			var rect = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height);
			TieredFloatMenu floatMenu = new IconMenu(this, colonistGroup, rect, Textures.IconMenu);
			OpenNewMenu(floatMenu);
		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Vector2 zero = Vector2.zero;
			Rect groupNameRect = new Rect(zero.x, zero.y + 20f, rect.width, 30f);
			Text.Anchor = TextAnchor.MiddleCenter;
			var font = Text.Font;
			Text.Font = GameFont.Medium;
			Widgets.Label(groupNameRect, this.colonistGroup.GetGroupName());
			Text.Font = font;
			Text.Anchor = TextAnchor.UpperLeft;
			zero += InitialFloatOptionPositionShift;
			for (int i = 0; i < options.Count; i++)
			{
				TieredFloatMenuOption floatMenuOption = options[i];
				Rect rect2 = new Rect(zero.x, zero.y, (this.backgroundTexture.width - InitialFloatOptionPositionShift.x) / 1.2f, floatMenuOption.curIcon.height);
				if (floatMenuOption.DoGUI(rect2, givesColonistOrders, this))
				{
					Find.WindowStack.TryRemove(this);
					break;
				}
				zero.y += floatMenuOption.bottomIndent;
			}
			DrawExtraGui(rect);
			if (Event.current.type == EventType.MouseDown)
			{
				Event.current.Use();
				Close();
			}
			GUI.color = Color.white;
		}

		public override void DrawExtraGui(Rect rect)
		{

		}
	}
}
