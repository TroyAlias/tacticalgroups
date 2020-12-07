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
	public class OrderMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 63f);
		public OrderMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddAttackButton();
			AddRegroupButton();
			AddBattleStationsButton();
			AddFormationsButton();
			AddMedicalButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public void AddAttackButton()
		{
			var option = new TieredFloatMenuOption(Strings.Attack, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				AddAttackWindow(option);
			};
			option.bottomIndent = Textures.MenuButton.height + 10;
			options.Add(option);
		}

		public void AddAttackWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new AttackMenu(this, colonistGroup, windowRect, Textures.AttackMenu);
			OpenNewMenu(floatMenu);
		}

		public void AddRegroupButton()
		{
			var option = new TieredFloatMenuOption(Strings.Regroup, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				this.colonistGroup.RemoveOldLord();
				this.colonistGroup.Draft();
				var firstPawn = this.colonistGroup.pawns.First();
				foreach (var pawn in this.colonistGroup.pawns)
                {
					if (pawn != null)
                    {
						var job = JobMaker.MakeJob(JobDefOf.Goto, firstPawn);
						job.locomotionUrgency = LocomotionUrgency.Sprint;
						pawn.jobs.TryTakeOrderedJob(job);
                    }
                }
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 25;
			options.Add(option);
		}

		public void AddBattleStationsButton()
		{
			var option = new TieredFloatMenuOption(Strings.BattleStations, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				Log.Message(" - AddBattleStationsButton - foreach (var pawn in this.colonistGroup.pawns) - 2", true);
				foreach (var pawn in this.colonistGroup.pawns)
				{
					Log.Message(" - AddBattleStationsButton - if (this.colonistGroup.formations?.ContainsKey(pawn) ?? false) - 3", true);
					if (this.colonistGroup.formations?.ContainsKey(pawn) ?? false)
					{
						Log.Message(" - AddBattleStationsButton - var job = JobMaker.MakeJob(JobDefOf.Goto, this.colonistGroup.formations[pawn]); - 4", true);
						var job = JobMaker.MakeJob(JobDefOf.Goto, this.colonistGroup.formations[pawn]);
						Log.Message(" - AddBattleStationsButton - job.locomotionUrgency = LocomotionUrgency.Sprint; - 5", true);
						job.locomotionUrgency = LocomotionUrgency.Sprint;
						Log.Message(" - AddBattleStationsButton - pawn.jobs.TryTakeOrderedJob(job); - 6", true);
						pawn.jobs.TryTakeOrderedJob(job);
					}
				}
			};
			option.bottomIndent = Textures.MenuButton.height + 72;
			options.Add(option);
		}


		public void AddFormationsButton()
		{
			var option = new TieredFloatMenuOption(Strings.Formation, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{

			};
			option.bottomIndent = Textures.MenuButton.height + 10;
			options.Add(option);
		}

		public void AddMedicalButton()
		{
			var option = new TieredFloatMenuOption(Strings.Medical, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{

			};
			option.bottomIndent = Textures.MenuButton.height + 5;
			options.Add(option);
		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Vector2 zero = Vector2.zero;
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
            base.DrawExtraGui(rect);
			Vector2 zero = Vector2.zero + InitialFloatOptionPositionShift;

			var shooterIconRect = new Rect(rect.x + 10, rect.y + 25f, Textures.ShootingIcon.width, Textures.ShootingIcon.height);
			GUI.DrawTexture(shooterIconRect, Textures.ShootingIcon);
			if (Mouse.IsOver(shooterIconRect))
			{
				GUI.DrawTexture(shooterIconRect, Textures.ShootingMeleeHover);
			}
			var shooters = this.colonistGroup.pawns.Where(x => x.equipment?.Primary?.def.IsRangedWeapon ?? false);
			var shooterCountRect = new Rect(shooterIconRect.x + shooterIconRect.width + 2f, shooterIconRect.y, 30, shooterIconRect.height);

			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(shooterCountRect, shooters.Count().ToString());
			var totalShooterRect = new Rect(shooterIconRect.x, shooterIconRect.y, Textures.ShootingIcon.width + 25, shooterIconRect.height).ExpandedBy(5f);
			if (Mouse.IsOver(totalShooterRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					Find.Selector.ClearSelection();
					foreach (var shooter in shooters)
                    {
						Find.Selector.Select(shooter);
                    }
					Event.current.Use();
				}
			}
			Text.Anchor = TextAnchor.MiddleCenter;
			var middlePawnCountRect = new Rect(totalShooterRect.x + totalShooterRect.width, totalShooterRect.y, 70, totalShooterRect.height);
			var capablePawns = this.colonistGroup.pawns.Where(x => !x.IsDownedOrIncapable());
			Widgets.Label(middlePawnCountRect, capablePawns.Count() + " / " + this.colonistGroup.pawns.Count);
			if (Mouse.IsOver(middlePawnCountRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					Find.Selector.ClearSelection();
					foreach (var pawn in capablePawns)
					{
						Find.Selector.Select(pawn);
					}
					Event.current.Use();
				}
			}

			var meleeIconRect = new Rect((rect.x + rect.width) - 35, rect.y + 25f, Textures.MeleeIcon.width, Textures.MeleeIcon.height);
			GUI.DrawTexture(meleeIconRect, Textures.MeleeIcon);
			if (Mouse.IsOver(meleeIconRect))
            {
				GUI.DrawTexture(meleeIconRect, Textures.ShootingMeleeHover);
			}

			var melees = this.colonistGroup.pawns.Where(x => x.equipment?.Primary?.def.IsMeleeWeapon ?? false);
			var meleeCountRect = new Rect(meleeIconRect.x - (Textures.MeleeIcon.width + 5f), meleeIconRect.y, 30, meleeIconRect.height);

			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(meleeCountRect, melees.Count().ToString());

			var totalMeleeRect = new Rect(meleeCountRect.x, meleeIconRect.y, meleeIconRect.width + meleeCountRect.width, meleeIconRect.height).ExpandedBy(5f);
			if (Mouse.IsOver(totalMeleeRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					Find.Selector.ClearSelection();
					foreach (var melee in melees)
					{
						Find.Selector.Select(melee);
					}
					Event.current.Use();
				}
			}
			Text.Anchor = TextAnchor.MiddleCenter;

			var rectY = (rect.height / 2f) - 30f;
			var setRect = new Rect(zero.x, rectY, Textures.SetClearButton.width, Textures.SetClearButton.height);
			GUI.DrawTexture(setRect, Textures.SetClearButton);
			Widgets.Label(setRect, Strings.Set);

			if (Mouse.IsOver(setRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
                {
					this.colonistGroup.SetBattleStations();
					Event.current.Use();
					CloseAllWindows();
				}
			}
			var clearRect = new Rect(zero.x + (Textures.MenuButton.width - Textures.SetClearButton.width - 3f), rectY, Textures.SetClearButton.width, Textures.SetClearButton.height);
			GUI.DrawTexture(clearRect, Textures.SetClearButton);
			Widgets.Label(clearRect, Strings.Clear);
			if (Mouse.IsOver(clearRect))
			{
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 1)
				{
					this.colonistGroup.ClearBattleStations();
					Event.current.Use();
					CloseAllWindows();
				}
			}

			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
