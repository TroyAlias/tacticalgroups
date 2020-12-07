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
			AddSortButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public void AddRenameButton()
        {
			var option = new TieredFloatMenuOption(Strings.Rename, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddRenameWindow(option);
			};
			option.bottomIndent = Textures.MenuButton.height + 15;
			options.Add(option);
		}

		public void AddRenameWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new Dialog_RenameColonistGroup(this, this.colonistGroup, windowRect, Textures.RenameTab, option);
			OpenNewMenu(floatMenu);
		}

		public void AddIconButton()
		{
			var option = new TieredFloatMenuOption(Strings.Icon, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddIconWindow(option);
			};
			option.bottomIndent = Textures.MenuButton.height + 23;
			options.Add(option);
		}
		public void AddIconWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new IconMenu(this, colonistGroup, windowRect, Textures.IconMenu);
			OpenNewMenu(floatMenu);
		}

		public void AddSortButton()
		{
			var option = new TieredFloatMenuOption(Strings.SortGroup, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddSortWindow(option);
			};
			option.bottomIndent = Textures.MenuButton.height + 5;
			options.Add(option);
		}
		public void AddSortWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new SkillSortMenu(this, colonistGroup, windowRect, Textures.SortMenu);
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
			Rect disbandRect = new Rect((rect.width - Textures.DisbandMenu.width) / 2f, rect.height * 0.60f, Textures.DisbandMenu.width, Textures.DisbandMenu.height);
			GUI.DrawTexture(disbandRect, Textures.DisbandMenu);

			Text.Anchor = TextAnchor.UpperCenter;

			var disbandLabelRect = new Rect(disbandRect.x, disbandRect.y + 10f, disbandRect.width, disbandRect.height - 10f);
			Widgets.Label(disbandLabelRect, Strings.Disband);

			var disbandPawn = new Rect(disbandRect.x / 2f, (disbandRect.y + disbandRect.height) - (Textures.DisbandPawn.height / 2f), Textures.DisbandPawn.width, Textures.DisbandPawn.height);
			if (Mouse.IsOver(disbandPawn))
            {
				GUI.DrawTexture(disbandPawn, Textures.DisbandPawnHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					foreach (var pawn in Find.Selector.SelectedPawns)
                    {
						this.colonistGroup.Disband(pawn);
                    }
				}
            }
			else
            {
				GUI.DrawTexture(disbandPawn, Textures.DisbandPawn);
			}

			var disbandPawnLabelRect = new Rect(disbandPawn.x, disbandPawn.y + disbandPawn.height + 3f, disbandPawn.width, disbandPawn.height - 10f);
			Widgets.Label(disbandPawnLabelRect, Strings.DisbandPawn);

			var disbandGroup = new Rect((disbandRect.x + disbandRect.width) - (Textures.DisbandGroup.width / 2f), (disbandRect.y + disbandRect.height) - (Textures.DisbandGroup.height / 2f), Textures.DisbandGroup.width, Textures.DisbandGroup.height);
			if (Mouse.IsOver(disbandGroup))
			{
				GUI.DrawTexture(disbandGroup, Textures.DisbandGroupHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticUtils.Groups.Remove(this.colonistGroup);
				}
			}
			else
			{
				GUI.DrawTexture(disbandGroup, Textures.DisbandGroup);
			}
			var disbandGroupLabelRect = new Rect(disbandGroup.x, disbandGroup.y + disbandGroup.height + 3f, disbandGroup.width, disbandGroup.height - 10f);
			Widgets.Label(disbandGroupLabelRect, Strings.DisbandGroup);


			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}