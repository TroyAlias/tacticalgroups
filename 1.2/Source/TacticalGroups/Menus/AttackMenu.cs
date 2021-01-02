using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace TacticalGroups
{
	public class AttackMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 30f);
		public AttackMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddFireAtWillButton();
			AddStrongestButton();
			AddWeakestButton();
			AddPursueFleeingButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}

		public void AddFireAtWillButton()
        {
			var option = new TieredFloatMenuOption(Strings.FireAtWill, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter,
				MenuOptionPriority.High, 0f, -1f, Strings.FireAtWillTooltip);
			option.action = delegate
			{
				this.colonistGroup.Draft();
				foreach (var pawn in this.colonistGroup.pawns)
                {
					if (pawn.drafter != null)
                    {
						pawn.drafter.FireAtWill = true;
					}
				}
				this.colonistGroup.SelectAll();
				this.colonistGroup.RemoveOldLord();
				TacticDefOf.TG_AttackOrdersSFX.PlayOneShotOnCamera();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 30;
			options.Add(option);
		}

		public void AddStrongestButton()
		{
			var option = new TieredFloatMenuOption(Strings.Strongest, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, 
				MenuOptionPriority.High, 0f, -1f, Strings.StrongestTooltip);
			option.action = delegate
			{
				TacticDefOf.TG_AttackOrdersSFX.PlayOneShotOnCamera();
				this.colonistGroup.Draft();
				this.colonistGroup.SelectAll();
				this.colonistGroup.SwitchToAttackMode();
				this.colonistGroup.RemoveOldLord();
				var targets = this.colonistGroup.Map.attackTargetsCache.GetPotentialTargetsFor(this.colonistGroup.pawns.First()).Where(x => !x.Thing.Fogged()).Select(x => x.Thing).ToList();
				LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_AssaultThingsStrongest(Faction.OfPlayer, targets, 0, true), this.colonistGroup.Map, this.colonistGroup.pawns);
				this.colonistGroup.SearchForJob();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 10;
			options.Add(option);
		}

		public void AddWeakestButton()
		{
			var option = new TieredFloatMenuOption(Strings.Weakest, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, 
				MenuOptionPriority.High, 0f, -1f, Strings.WeakestTooltip);
			option.action = delegate
			{
				TacticDefOf.TG_AttackOrdersSFX.PlayOneShotOnCamera();
				this.colonistGroup.Draft();
				this.colonistGroup.SelectAll();
				this.colonistGroup.SwitchToAttackMode();
				this.colonistGroup.RemoveOldLord();
				var targets = this.colonistGroup.Map.attackTargetsCache.GetPotentialTargetsFor(this.colonistGroup.pawns.First()).Where(x => !x.Thing.Fogged()).Select(x => x.Thing).ToList();
				LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_AssaultThingsWeakest(Faction.OfPlayer, targets, 0, true), this.colonistGroup.Map, this.colonistGroup.pawns);
				this.colonistGroup.SearchForJob();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 35;
			options.Add(option);
		}

		public void AddPursueFleeingButton()
		{
			var option = new TieredFloatMenuOption(Strings.PursueFleeing, null, Textures.LookBusyButton, Textures.LookBusyButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, 
				MenuOptionPriority.High, 0f, -1f, Strings.PursueFleeingTooltip);
			option.action = delegate
			{
				TacticDefOf.TG_AttackOrdersSFX.PlayOneShotOnCamera();
				this.colonistGroup.Draft();
				this.colonistGroup.SelectAll();
				this.colonistGroup.SwitchToAttackMode();
				this.colonistGroup.RemoveOldLord();
				var targets = this.colonistGroup.Map.attackTargetsCache.GetPotentialTargetsFor(this.colonistGroup.pawns.First()).Where(x => !x.Thing.Fogged()).Select(x => x.Thing).ToList();
				LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_AssaultThingsPursueFleeing(Faction.OfPlayer, targets, 0, true), this.colonistGroup.Map, this.colonistGroup.pawns);
				this.colonistGroup.SearchForJob();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 15;
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
	}
}
