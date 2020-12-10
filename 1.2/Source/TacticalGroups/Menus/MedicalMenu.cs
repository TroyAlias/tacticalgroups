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
	public class MedicalMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(0f, 0f);
		protected override Vector2 InitialFloatOptionPositionShift => new Vector2(this.backgroundTexture.width / 10, 30f);
		public MedicalMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.options = new List<TieredFloatMenuOption>();
			AddRescueFallenButton();
			AddTendWoundedButton();
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetSizeMode(SizeMode);
			}
		}
		public void AddRescueFallenButton()
		{
			var option = new TieredFloatMenuOption(Strings.RescueFallen, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				this.colonistGroup.RemoveOldLord();
				LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_RescueFallen(Faction.OfPlayer), this.colonistGroup.Map, this.colonistGroup.pawns);
				this.colonistGroup.SearchForJob();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 10;
			options.Add(option);
		}

		public void AddTendWoundedButton()
		{
			var option = new TieredFloatMenuOption(Strings.TendWounded, null, Textures.MenuButton, Textures.MenuButtonHover, Textures.MenuButtonPress, TextAnchor.MiddleCenter, MenuOptionPriority.High, 0f);
			option.action = delegate
			{
				this.colonistGroup.Draft();
				this.colonistGroup.RemoveOldLord();
				LordMaker.MakeNewLord(Faction.OfPlayer, new LordJob_TendWounded(Faction.OfPlayer), this.colonistGroup.Map, this.colonistGroup.pawns);
				this.colonistGroup.SearchForJob();
				this.CloseAllWindows();
			};
			option.bottomIndent = Textures.MenuButton.height + 35;
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
