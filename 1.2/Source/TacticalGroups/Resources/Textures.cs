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
	[StaticConstructorOnStartup]
	public static class Textures
	{
		public static readonly Texture2D DropMenuRightClick = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/DropMenuRightClick");
		public static readonly Texture2D ActionsDropMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ActionsDropMenu");

		public static readonly Texture2D RallyButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/RallyButton");
		public static readonly Texture2D RallyButtonHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/RallyButtonHover");

		public static readonly Texture2D AOMButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/AOMButton");
		public static readonly Texture2D AOMButtonHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/AOMButtonHover");
		public static readonly Texture2D AOMButtonPress = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/AOMButtonPress");

		public static readonly Texture2D MenuButtonPress = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/MenuButtonPress");

		public static readonly Texture2D AddPawnIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/AddPawnIcon");
		public static readonly Texture2D AddPawnIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/AddPawnIconHover");

		public static readonly Texture2D EyeIconOff = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/EyeIconOff");
		public static readonly Texture2D EyeIconOffHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/EyeIconOffHover");

		public static readonly Texture2D EyeIconOn = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/EyeIconOn");
		public static readonly Texture2D EyeIconOnHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/EyeIconOnHover");

		public static readonly Texture2D BackgroundColonistLayer = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ColonistUnderLayer");
		public static readonly Texture2D CreateGroupIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/CreateGroupIcon");
		public static readonly Texture2D CreateGroupIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/CreateGroupIconHover");


		public static readonly Texture2D GroupIconBox = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupIconBox");
		public static readonly Texture2D ShowHideIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/ShowHideIcon");
		public static readonly Texture2D SettingsGear = ContentFinder<Texture2D>.Get("UI/ColonistBar/SettingsGear");
		public static readonly Texture2D ExpandedGroupMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/ExpandedGroupMenu");
		public static readonly Texture2D GroupingIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupingIcon");
		public static readonly Texture2D GroupIcon_Default = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupIcons/Default");


		public static readonly Texture2D GroupIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupIconHover");
		public static readonly Texture2D GroupIconSelected = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupIconSelected");

		public static readonly Texture2D ColonistDot = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDot");
		public static readonly Texture2D ColonistDotRed = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDotRed");
		public static readonly Texture2D ColonistDotToxic = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDotToxic");
		public static readonly Texture2D ColonistDotDowned = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDotDowned");
		public static readonly Texture2D GroupOverlayColonistDown = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupOverlayColonistDown");


		public static readonly Texture2D ManageDropMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/ManageDropMenu");
		public static readonly Texture2D IconMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/IconMenu");
		public static readonly Texture2D DisbandPawnHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandPawnHover");
		public static readonly Texture2D DisbandPawnClick = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandPawnClick");
		public static readonly Texture2D DisbandPawn = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandPawn");
		public static readonly Texture2D DisbandMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandMenu");
		public static readonly Texture2D DisbandGroupHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandGroupHover");
		public static readonly Texture2D DisbandGroupClick = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandGroupClick");
		public static readonly Texture2D DisbandGroup = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandGroup");
	}
}
