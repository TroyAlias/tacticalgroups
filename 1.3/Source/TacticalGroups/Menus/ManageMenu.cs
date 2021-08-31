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
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(27f, 60f);

		public ManageMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddRenameButton();
			AddIconButton();
			AddSortButton();
			AddManagementButton();
			if (this.colonistGroup.isColonyGroup)
            {
				AddDiplomacyButton();
            }
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
			if (this.colonistGroup.isColonyGroup && this.colonistGroup.Map != null)
            {
				this.colonistGroup.Map.wealthWatcher.ForceRecount();
			}
		}

		private OptionsSlideMenu optionsSlideMenu;
		private PawnButtonsMenu pawnButtonsMenu;
		public override void PostOpen()
        {
            base.PostOpen();
			AddManagementWindow(options[3]);

			optionsSlideMenu = new OptionsSlideMenu(this, this.colonistGroup, windowRect, Textures.OptionsSlideMenu);
			pawnButtonsMenu = new PawnButtonsMenu(this, this.colonistGroup, windowRect, Textures.PawnsButtonMenu);

			Find.WindowStack.Add(optionsSlideMenu);
			Find.WindowStack.Add(pawnButtonsMenu);
		}

		public override void PostClose()
        {
            base.PostClose();
			optionsSlideMenu?.Close();
			pawnButtonsMenu?.Close();
		}

		public void AddRenameButton()
        {
			var option = new TieredFloatMenuOption(Strings.Rename, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddRenameWindow(option);
			};
			option.bottomIndent = Textures.MenuButton.height + 10;
			options.Add(option);
		}

		public void AddRenameWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new Dialog_RenameColonistGroup(this, this.colonistGroup, windowRect, Textures.RenameTab, option, "OK".Translate());
			OpenNewMenu(floatMenu);
		}

		public void AddIconButton()
		{
			var option = new TieredFloatMenuOption(Strings.Icon, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddIconWindow(option);
			};
			option.bottomIndent = Textures.MenuButton.height + 17;
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
			option.bottomIndent = Textures.MenuButton.height + 10;
			options.Add(option);
		}
		public void AddSortWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new SortMenu(this, colonistGroup, windowRect, Textures.SortMenu);
			OpenNewMenu(floatMenu);
		}

		public void AddManagementButton()
		{
			var option = new TieredFloatMenuOption(Strings.Management, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddManagementWindow(option);
			};
			option.bottomIndent = Textures.MenuButton.height + 67;
			options.Add(option);
		}

		public void AddManagementWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new ManagementMenu(this, colonistGroup, windowRect, Textures.StatMenu);
			OpenNewMenu(floatMenu);
		}

		public void AddDiplomacyButton()
		{
			var option = new TieredFloatMenuOption(Strings.Diplomacy, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				Find.MainTabsRoot.ToggleTab(MainButtonDefOf.Factions);
			};
			option.bottomIndent = Textures.MenuButton.height + 5;
			options.Add(option);
		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Vector2 zero = Vector2.zero;
			Rect groupNameRect = new Rect(zero.x, zero.y + 20f, rect.width, 30f);
			Text.Anchor = TextAnchor.MiddleCenter;
			var font = Text.Font;
			Text.Font = GameFont.Medium;
			Widgets.Label(groupNameRect, this.colonistGroup.curGroupName);
			Text.Font = font;
			Text.Anchor = TextAnchor.UpperLeft;
			zero += InitialFloatOptionPositionShift;
			for (int i = 0; i < options.Count; i++)
			{
				TieredFloatMenuOption floatMenuOption = options[i];
				Rect rect2 = new Rect(zero.x, zero.y, (this.backgroundTexture.width - InitialFloatOptionPositionShift.x) / 1.2f, floatMenuOption.curIcon.height);
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
			if (this.colonistGroup.isColonyGroup || this.colonistGroup.isTaskForce)
			{
				Rect treasureButtonRect = new Rect(rect.x + Textures.TreasuryButton.width, rect.height - 113, Textures.TreasuryButton.width, Textures.TreasuryButton.height);
				GUI.DrawTexture(treasureButtonRect, Textures.TreasuryButton);
				if (Mouse.IsOver(treasureButtonRect))
				{
					GUI.DrawTexture(treasureButtonRect, Textures.RescueTendHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                    {
						Find.MainTabsRoot.ToggleTab(DefDatabase<MainButtonDef>.GetNamed("History"));
					}
				}
				Rect treasureLabelRect = new Rect(treasureButtonRect.x + treasureButtonRect.width + 10f, treasureButtonRect.y, 100f, 26f);
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(treasureLabelRect, this.colonistGroup.Map.wealthWatcher.WealthTotal.ToStringDecimalIfSmall());
				Text.Anchor = TextAnchor.UpperLeft;
			}
			else if (this.colonistGroup is PawnGroup pawnGroup)
			{
				Rect disbandRect = new Rect((rect.width - Textures.DisbandMenu.width) / 2f, rect.height * 0.66f, Textures.DisbandMenu.width, Textures.DisbandMenu.height);
				GUI.DrawTexture(disbandRect, Textures.DisbandMenu);

				Text.Anchor = TextAnchor.UpperCenter;
				var disbandLabelRect = new Rect(disbandRect.x, disbandRect.y + 25f, disbandRect.width, disbandRect.height - 10f);
				Widgets.Label(disbandLabelRect, Strings.Disband);

				var toggleAbleAutoDisbandRect = new Rect(disbandRect.x + (disbandRect.width / 2f) - (Textures.WorkSelectEmpty.width / 2), disbandRect.yMax + 5, Textures.WorkSelectEmpty.width, Textures.WorkSelectEmpty.height);
				if (pawnGroup.autoDisbandWithoutPawns)
				{
					GUI.DrawTexture(toggleAbleAutoDisbandRect, Textures.WorkSelectEmpty);
				}
				else
				{
					GUI.DrawTexture(toggleAbleAutoDisbandRect, Textures.WorkSelectBlue);
				}
				if (Mouse.IsOver(toggleAbleAutoDisbandRect))
                {
					TooltipHandler.TipRegion(toggleAbleAutoDisbandRect, Strings.AutoDisbandTooltip);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                    {
						pawnGroup.autoDisbandWithoutPawns = !pawnGroup.autoDisbandWithoutPawns;
					}
				}

				var disbandPawn = new Rect((disbandRect.x / 2f) + 5f, (disbandRect.y + disbandRect.height) - (Textures.DisbandPawn.height / 2f), Textures.DisbandPawn.width, Textures.DisbandPawn.height);
				MouseoverSounds.DoRegion(disbandPawn);
				if (Mouse.IsOver(disbandPawn))
				{
					GUI.DrawTexture(disbandPawn, Textures.DisbandPawnHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
						foreach (var pawn in Find.Selector.SelectedPawns)
						{
							this.colonistGroup.Disband(pawn);
						}
						Event.current.Use();
						if (!this.colonistGroup.pawns.Any())
                        {
							this.CloseAllWindows();
                        }
					}
				}
				else
				{
					GUI.DrawTexture(disbandPawn, Textures.DisbandPawn);
				}

				var disbandPawnLabelRect = new Rect(disbandPawn.x, disbandPawn.y + disbandPawn.height + 1f, disbandPawn.width, disbandPawn.height - 10f);
				Widgets.Label(disbandPawnLabelRect, Strings.DisbandPawn);

				var disbandGroup = new Rect((disbandRect.x + disbandRect.width) - (Textures.DisbandGroup.width / 2f), (disbandRect.y + disbandRect.height) - (Textures.DisbandGroup.height / 2f), Textures.DisbandGroup.width, Textures.DisbandGroup.height);
				MouseoverSounds.DoRegion(disbandGroup);
				if (Mouse.IsOver(disbandGroup))
				{
					GUI.DrawTexture(disbandGroup, Textures.DisbandGroupHover);
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
					{
						this.CloseAllWindows();
						TacticDefOf.TG_DisbandGroupSFX.PlayOneShotOnCamera();
						Event.current.Use();
						if (Find.CurrentMap != null && this.colonistGroup.pawns != null)
                        {
							for (int num = this.colonistGroup.pawns.Count - 1; num >= 0; num--)
                            {
								if (this.colonistGroup.pawns[num].Map == Find.CurrentMap)
                                {
									this.colonistGroup.Disband(this.colonistGroup.pawns[num]);
                                }
                            }
                        }
						this.colonistGroup.Disband();
					}
				}
				else
				{
					GUI.DrawTexture(disbandGroup, Textures.DisbandGroup);
				}
				var disbandGroupLabelRect = new Rect(disbandGroup.x, disbandGroup.y + disbandGroup.height + 1f, disbandGroup.width, disbandGroup.height - 10f);
				Widgets.Label(disbandGroupLabelRect, Strings.DisbandGroup);
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}
	}
}
