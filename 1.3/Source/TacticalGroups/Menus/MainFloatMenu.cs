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
			AddWorkButton();
			AddOrderButton();
			AddManageButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public void AddRallyButton()
		{
			var option = new TieredFloatMenuOption(Strings.Rally, null, Textures.RallyButton, Textures.RallyButtonHover, null, TextAnchor.MiddleCenter, MenuOptionPriority.High, 10f, -1f, Strings.RallyToolTip);
			option.bottomIndent = 41;
			option.action = delegate
			{
				TacticDefOf.TG_RallySFX.PlayOneShotOnCamera();
				Find.Selector.ClearSelection();
				foreach (var pawn in this.colonistGroup.ActivePawns)
                {
					Find.Selector.Select(pawn);
					pawn.drafter.Drafted = true;
				}
				this.CloseAllWindows();

			};
			options.Add(option);
		}

		public void AddWorkButton()
		{
			var option = new TieredFloatMenuOption(Strings.Work, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleLeft, MenuOptionPriority.High, 5f);
			option.action = delegate
			{
				AddWorkWindow(option);
			};
			option.bottomIndent = Textures.AOMButton.height;
			options.Add(option);
		}

		public void AddWorkWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			if (this.colonistGroup.pawns.Any())
            {
				var rect = new Rect(windowRect.x, windowRect.y + 30, windowRect.width, windowRect.height);
				TieredFloatMenu floatMenu = new WorkMenu(this, colonistGroup, rect, Textures.ActionsMenuDrop);
				OpenNewMenu(floatMenu);
			}
		}

        public void AddOrderButton()
		{
			var option = new TieredFloatMenuOption(Strings.Orders, null, Textures.AOMButton, Textures.AOMButtonHover, Textures.AOMButtonPress, TextAnchor.MiddleLeft, MenuOptionPriority.High, 5f);
			option.action = delegate
			{
				AddOrderWindow(option);
			};
			option.bottomIndent = Textures.AOMButton.height;
			options.Add(option);
		}

		public void AddOrderWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			if (this.colonistGroup.pawns.Any())
            {
				var rect = new Rect(windowRect.x, windowRect.y + 30, windowRect.width, windowRect.height);
				TieredFloatMenu floatMenu = new OrderMenu(this, colonistGroup, rect, Textures.OrdersDropMenu);
				OpenNewMenu(floatMenu);
			}
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
			if (this.colonistGroup.pawns.Any())
            {
				var rect = new Rect(windowRect.x, windowRect.y + 30, windowRect.width, windowRect.height);
				TieredFloatMenu floatMenu = new ManageMenu(this, colonistGroup, rect, (this.colonistGroup.isColonyGroup || this.colonistGroup.isTaskForce) ? Textures.ColonyManageDropMenu : Textures.ManageDropMenu);
				OpenNewMenu(floatMenu);
			}
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
				if (floatMenuOption.DoGUI(rect2, this))
				{
					Find.WindowStack.TryRemove(this);
					break;
				}
				zero.y += floatMenuOption.bottomIndent;
			}
			DrawExtraGui(rect);
			GUI.color = Color.white;
		}

		public override void DrawExtraGui(Rect rect)
		{
			var iconRect = new Rect(rect.x + 7f, rect.y + (rect.height - Textures.EyeIconOn.height) - 7f, Textures.EyeIconOn.width, Textures.EyeIconOn.height);
			if (this.colonistGroup.entireGroupIsVisible || !this.colonistGroup.pawnIcons.Where(x => !x.Value.isVisibleOnColonistBar).Any())
			{
				if (Mouse.IsOver(iconRect))
				{
					GUI.DrawTexture(iconRect, Textures.EyeIconOffHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
						foreach (var pawnIcon in this.colonistGroup.pawnIcons)
						{
							TacticUtils.TacticalGroups.visiblePawns.Remove(pawnIcon.Key);
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
						TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
						foreach (var pawnIcon in this.colonistGroup.pawnIcons)
						{
							pawnIcon.Value.isVisibleOnColonistBar = true;
							TacticUtils.TacticalGroups.visiblePawns.Add(pawnIcon.Key);
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
			TooltipHandler.TipRegion(iconRect, Strings.ShowHideTooltip);

			var disbandPawnRect = new Rect(rect.x + (rect.width - Textures.AddPawnIcon.width) - 40f, rect.y + (rect.height - Textures.AddPawnIcon.height) - 7f, Textures.DisbandPawnIcon.width, Textures.DisbandPawnIcon.height);
			if (!this.colonistGroup.isColonyGroup)
			{
				TooltipHandler.TipRegion(disbandPawnRect, Strings.DisbandSelectedPawns);
				if (Mouse.IsOver(disbandPawnRect))
				{
					GUI.DrawTexture(disbandPawnRect, Textures.DisbandPawnIconHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
						var pawns = Find.Selector.SelectedPawns;
						foreach (var pawn in pawns)
						{
							if (this.colonistGroup.pawns.Contains(pawn))
							{
								this.colonistGroup.Disband(pawn);
							}
						}
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
						Event.current.Use();
					}
				}
				else
				{
					GUI.DrawTexture(disbandPawnRect, Textures.DisbandPawnIcon);
				}
			}

			var addPawnRect = new Rect(disbandPawnRect.xMax + 15, disbandPawnRect.y, Textures.AddPawnIcon.width, Textures.AddPawnIcon.height);
			if (Mouse.IsOver(addPawnRect))
			{
				GUI.DrawTexture(addPawnRect, Textures.AddPawnIconHover);
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
					var pawns = Find.Selector.SelectedPawns;
					if (pawns.Count > 0)
                    {
						this.colonistGroup.Add(pawns);
						TacticUtils.TacticalColonistBar.MarkColonistsDirty();
					}
					Event.current.Use();
				}
			}
			else
			{
				GUI.DrawTexture(addPawnRect, Textures.AddPawnIcon);
			}
			TooltipHandler.TipRegion(addPawnRect, Strings.AddColonistTooltip);
		}

		public override void PostClose()
        {
            base.PostClose();
			this.colonistGroup.Notify_WindowsClosed();
			this.CloseAllWindows();
		}
    }
}
