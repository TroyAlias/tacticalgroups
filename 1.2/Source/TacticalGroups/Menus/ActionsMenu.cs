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
	public class ActionsMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 25f);
		public ActionsMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddGetToWork();
			AddTakeABreak();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}
		public override void PostOpen()
		{
			base.PostOpen();
			AddGetToWorkWindow(options[0]);
		}
		public void AddGetToWork()
		{
			var option = new TieredFloatMenuOption(Strings.GetToWork, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
				AddGetToWorkWindow(option);
			};
			option.bottomIndent = Textures.AOMButton.height + 5;
			options.Add(option);
		}

		public void AddGetToWorkWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new GetToWorkMenu(this, colonistGroup, windowRect, Textures.GetToWorkMenu);
			OpenNewMenu(floatMenu);
		}

		public void AddTakeABreak()
		{
			var option = new TieredFloatMenuOption(Strings.TakeABreak, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				TacticDefOf.TG_ClickSFX.PlayOneShotOnCamera();
				AddTakeABreakWindow(option);
			};
			option.bottomIndent = Textures.AOMButton.height + 5;
			options.Add(option);
		}

		public void AddTakeABreakWindow(TieredFloatMenuOption option)
		{
			MarkOptionAsSelected(option);
			TieredFloatMenu floatMenu = new TakeABreakMenu(this, colonistGroup, windowRect, Textures.GetToWorkMenu);
			OpenNewMenu(floatMenu);
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
