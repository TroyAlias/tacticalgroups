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
	public class MainFloatMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(3f, 3f);

		public MainFloatMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddRallyButton();
			AddActionButton();
			AddOrderButton();
			AddManageButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public void AddRallyButton()
		{
			var option = new TieredFloatMenuOption(Strings.Rally, null, Textures.RallyButton, Textures.RallyButtonHover, null, TextAnchor.MiddleCenter, MenuOptionPriority.High, 10f);
			option.bottomIndent = 41;
			options.Add(option);
		}

		public void AddActionButton()
		{
			var option = new TieredFloatMenuOption(Strings.Actions, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleLeft, MenuOptionPriority.High, 5f);
			option.action = delegate
			{
				AddActionWindow(option);
			};
			option.bottomIndent = Textures.AOMButton.height;
			options.Add(option);
		}

		public void AddActionWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			var rect = new Rect(windowRect.x, windowRect.y + 30, windowRect.width, windowRect.height);
			TieredFloatMenu floatMenu = new ActionsMenu(this, colonistGroup, rect, Textures.ActionsDropMenu);
			this.childWindow = floatMenu;
			Find.WindowStack.Add(floatMenu);
		}

        public void AddOrderButton()
		{
			var option = new TieredFloatMenuOption(Strings.Orders, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleLeft, MenuOptionPriority.High, 5f);
			option.bottomIndent = Textures.AOMButton.height;
			options.Add(option);
		}

		public void AddManageButton()
		{
			var option = new TieredFloatMenuOption(Strings.Manage, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleLeft, MenuOptionPriority.High, 5f);
			option.action = delegate
			{
				AddManageWindow(option);
			};
			options.Add(option);
		}
		public void AddManageWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			var rect = new Rect(windowRect.x, windowRect.y + 30, windowRect.width, windowRect.height);
			TieredFloatMenu floatMenu = new ManageMenu(this, colonistGroup, rect, Textures.ManageDropMenu);
			this.childWindow = floatMenu;
			Find.WindowStack.Add(floatMenu);
		}

		public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);
			Vector2 zero = Vector2.zero;
			zero += InitialFloatOptionPositionShift;
			for (int i = 0; i < options.Count; i++)
			{
				TieredFloatMenuOption floatMenuOption = options[i];
				Rect rect2 = new Rect(zero.x, zero.y, floatMenuOption.curIcon.width, floatMenuOption.curIcon.height);
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
	}
}
