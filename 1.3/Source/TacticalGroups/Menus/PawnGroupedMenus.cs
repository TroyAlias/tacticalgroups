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
	public class PawnGroupedMenu : TieredFloatMenu
	{
		protected override Vector2 InitialPositionShift => new Vector2(-(originRect.width - 25), 25);

		private string menuTitle;
		public PawnGroupedMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, string menuTitle)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture)
		{
			this.menuTitle = menuTitle;
		}

		protected Vector2 scrollPosition;

		protected float lastDrawnHeight;
		protected virtual void DoPawnsListing(Rect rect)
		{

		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			Text.Anchor = TextAnchor.MiddleCenter;
			var pawnsTitleBox = new Rect(rect.x + 75, rect.y + 35, 150, 30);
			Text.Font = GameFont.Medium;
			Widgets.Label(pawnsTitleBox, this.menuTitle);

			var pawnListingBox = new Rect(rect.x, 75, rect.width, rect.height - 95).ContractedBy(10);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;

			Rect rect2 = new Rect(0f, 0f, rect.width, lastDrawnHeight);
			bool num = rect2.height > pawnListingBox.height;
			if (num)
			{
				Widgets.BeginScrollView(pawnListingBox, ref scrollPosition, rect2, showScrollbars: false);
			}
			else
			{
				scrollPosition = Vector2.zero;
				GUI.BeginGroup(pawnListingBox);
			}
			DoPawnsListing(rect2);
			if (num)
			{
				Widgets.EndScrollView();
			}
			else
			{
				GUI.EndGroup();
			}
		}
	}

	public class AnimalMenu : PawnGroupedMenu
	{
		private readonly Dictionary<ThingDef, TreeNode_Pawns> animalNodes;
		public AnimalMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, string menuTitle) 
			: base(parentWindow, colonistGroup, originRect, backgroundTexture, menuTitle)
		{
			animalNodes = new Dictionary<ThingDef, TreeNode_Pawns>();
		}

		protected override void DoPawnsListing(Rect rect)
		{
			Listing_PawnsMenu listing_AnimalMenu = new Listing_PawnsMenu();
			listing_AnimalMenu.Begin(rect);
			listing_AnimalMenu.nestIndentWidth = 7f;
			listing_AnimalMenu.lineHeight = 25;
			listing_AnimalMenu.verticalSpacing = 0f;

			var animals = this.colonistGroup.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer).Where(x => x.RaceProps.Animal);

			foreach (var data in animalNodes)
            {
				data.Value.pawns?.Clear();
            }

			foreach (var pawn in animals)
            {
				if (animalNodes.ContainsKey(pawn.def)) 
				{
					if (animalNodes[pawn.def].pawns is null)
                    {
						animalNodes[pawn.def].pawns = new List<Pawn>();
					}
					animalNodes[pawn.def].pawns.Add(pawn);
				}
				else
                {
					animalNodes[pawn.def] = new TreeNode_Pawns(new List<Pawn> { pawn }, pawn.def.LabelCap);
				}
			}

			foreach (var listing in animalNodes)
            {
				listing_AnimalMenu.DoCategory(listing.Value, 0, 32);
			}
			listing_AnimalMenu.End();
			lastDrawnHeight = listing_AnimalMenu.CurHeight;
		}
	}

	public class PrisonerMenu : PawnGroupedMenu
	{

		private readonly Dictionary<PrisonerInteractionModeDef, TreeNode_Pawns> prisonerNodes;
		public PrisonerMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, string menuTitle)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture, menuTitle)
		{
			prisonerNodes = new Dictionary<PrisonerInteractionModeDef, TreeNode_Pawns>();

			foreach (PrisonerInteractionModeDef item in DefDatabase<PrisonerInteractionModeDef>.AllDefs.OrderBy((PrisonerInteractionModeDef pim) => pim.listOrder))
			{
				prisonerNodes[item] = new TreeNode_Pawns(null, item.LabelCap);
				prisonerNodes[item].SetOpen(32, true);
			}
		}

		protected override void DoPawnsListing(Rect rect)
		{
			Listing_PawnsMenu listing_PrisonerMenu = new Listing_PawnsMenu();
			listing_PrisonerMenu.Begin(rect);
			listing_PrisonerMenu.nestIndentWidth = 7f;
			listing_PrisonerMenu.lineHeight = 25;
			listing_PrisonerMenu.verticalSpacing = 0f;

			var prisoners = this.colonistGroup.Map.mapPawns.PrisonersOfColony;

			foreach (var data in prisonerNodes)
			{
				data.Value.pawns?.Clear();
			}

			foreach (var pawn in prisoners)
			{
				if (prisonerNodes.ContainsKey(pawn.guest.interactionMode))
				{
					if (prisonerNodes[pawn.guest.interactionMode].pawns is null)
					{
						prisonerNodes[pawn.guest.interactionMode].pawns = new List<Pawn>();
					}
					prisonerNodes[pawn.guest.interactionMode].pawns.Add(pawn);
				}
				else
				{
					prisonerNodes[pawn.guest.interactionMode] = new TreeNode_Pawns(new List<Pawn> { pawn }, pawn.guest.interactionMode.LabelCap);
				}
			}

			foreach (var listing in prisonerNodes)
			{
				listing_PrisonerMenu.DoCategory(listing.Value, 0, 32);
			}
			listing_PrisonerMenu.End();
			lastDrawnHeight = listing_PrisonerMenu.CurHeight;
		}
	}

	public class GuestMenu : PawnGroupedMenu
	{
		private readonly Dictionary<FactionDef, TreeNode_Pawns> guestNodes;
		public GuestMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, string menuTitle)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture, menuTitle)
		{
			guestNodes = new Dictionary<FactionDef, TreeNode_Pawns>();
		}

		protected override void DoPawnsListing(Rect rect)
		{
			Listing_PawnsMenu listing_GuestMenu = new Listing_PawnsMenu();
			listing_GuestMenu.Begin(rect);
			listing_GuestMenu.nestIndentWidth = 7f;
			listing_GuestMenu.lineHeight = 25;
			listing_GuestMenu.verticalSpacing = 0f;

			var guests = this.colonistGroup.Map.mapPawns.AllPawnsSpawned.Where(x => x.RaceProps.Humanlike && !x.IsPrisoner && !x.Fogged() && x.Faction != null && 
			x.Faction != Faction.OfPlayer && !x.Faction.HostileTo(Faction.OfPlayer));

			foreach (var data in guestNodes)
			{
				data.Value.pawns?.Clear();
			}

			foreach (var pawn in guests)
			{
				if (guestNodes.ContainsKey(pawn.Faction.def))
				{
					if (guestNodes[pawn.Faction.def].pawns is null)
					{
						guestNodes[pawn.Faction.def].pawns = new List<Pawn>();
					}
					guestNodes[pawn.Faction.def].pawns.Add(pawn);
				}
				else
				{
					guestNodes[pawn.Faction.def] = new TreeNode_Pawns(new List<Pawn> { pawn }, pawn.Faction.def.LabelCap);
				}
			}
			foreach (var listing in guestNodes)
			{
				listing_GuestMenu.DoCategory(listing.Value, 0, 32);
			}
			listing_GuestMenu.End();
			lastDrawnHeight = listing_GuestMenu.CurHeight;
		}
	}
	public class SlaveMenu : PawnGroupedMenu
	{
		private readonly TreeNode_Pawns slaveNodes;
		public SlaveMenu(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, string menuTitle)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture, menuTitle)
		{
			slaveNodes = new TreeNode_Pawns(new List<Pawn>(), Strings.Slaves);
			slaveNodes.SetOpen(32, true);
		}

		protected override void DoPawnsListing(Rect rect)
		{
			Listing_PawnsMenu listing_SlaveMenu = new Listing_PawnsMenu();
			listing_SlaveMenu.Begin(rect);
			listing_SlaveMenu.nestIndentWidth = 7f;
			listing_SlaveMenu.lineHeight = 25;
			listing_SlaveMenu.verticalSpacing = 0f;
			slaveNodes.pawns.Clear();
			var slaves = this.colonistGroup.Map.mapPawns.AllPawnsSpawned.Where(x => x.RaceProps.Humanlike && x.IsSlaveOfColony && !x.Fogged());
			foreach (var pawn in slaves)
			{
				slaveNodes.pawns.Add(pawn);
			}
			listing_SlaveMenu.DoCategory(slaveNodes, 0, 32, true);
			listing_SlaveMenu.End();
			lastDrawnHeight = listing_SlaveMenu.CurHeight;
		}
	}

}
