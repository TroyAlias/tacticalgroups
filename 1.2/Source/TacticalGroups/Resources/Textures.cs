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


		public static readonly Texture2D MenuButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/MenuButton");
		public static readonly Texture2D MenuButtonHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/MenuButtonHover");
		
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

		public static readonly Texture2D GroupBanner_Default = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupIcons/GroupBlue/Default");
		public static readonly Texture2D ColonyGroupIcon_Default = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupIcons/ColonyIcons/Default");
		public static readonly Texture2D CaravanGroupIcon_Default = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupIcons/ColonyIcons/Default");
		public static readonly Texture2D PawnGroupIcon_Default = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupIcons/GroupIcons/Default");

		public static readonly Texture2D GroupIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupIconHover");
		public static readonly Texture2D GroupIconSelected = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupIconSelected");

		public static readonly Texture2D ColonistDot = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDot");
		public static readonly Texture2D ColonistDotDowned = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDotDowned");
		public static readonly Texture2D ColonistDotToxic = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDotToxic");
		public static readonly Texture2D ColonistDotMentalState = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDotMentalState");
		public static readonly Texture2D ColonistDotInspired = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/ColonistDotInspired");
		public static readonly Texture2D GroupOverlayColonistDown = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupOverlayColonistDown");

		public static readonly Texture2D TendWounded = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/TendWounded");
		public static readonly Texture2D RescueFallen = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/RescueFallen");

		public static readonly Texture2D ManageDropMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/ManageDropMenu");
		public static readonly Texture2D IconMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/IconMenu");
		public static readonly Texture2D RenameTab = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/RenameTab");
		public static readonly Texture2D SortMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/SortMenu");

		public static readonly Texture2D DisbandPawnHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandPawnHover");
		public static readonly Texture2D DisbandPawnClick = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandPawnClick");
		public static readonly Texture2D DisbandPawn = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandPawn");
		public static readonly Texture2D DisbandMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandMenu");
		public static readonly Texture2D DisbandGroupHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandGroupHover");
		public static readonly Texture2D DisbandGroupClick = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandGroupClick");
		public static readonly Texture2D DisbandGroup = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DisbandGroup");

		public static readonly Texture2D WorkButtonHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/WorkButtonHover");
		public static readonly Texture2D WardenButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/WardenButton");
		public static readonly Texture2D MineButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/MineButton");
		public static readonly Texture2D LookBusyButtonHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/LookBusyButtonHover");
		public static readonly Texture2D LookBusyButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/LookBusyButton");
		public static readonly Texture2D HuntButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/HuntButton");
		public static readonly Texture2D HaulButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/HaulButton");
		public static readonly Texture2D GetToWorkMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/GetToWorkMenu");
		public static readonly Texture2D FarmButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/FarmButton");
		public static readonly Texture2D DoctorButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/DoctorButton");
		public static readonly Texture2D CraftButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/CraftButton");
		public static readonly Texture2D CookButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/CookButton");
		public static readonly Texture2D ConstructButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/ConstructButton");
		public static readonly Texture2D ClearSnowButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/ClearSnowButton");
		public static readonly Texture2D CleanButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/CleanButton");
		public static readonly Texture2D ChopWoodButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/ChopWoodButton");

		public static readonly Texture2D TailorButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/TailorButton");
		public static readonly Texture2D SmithButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/SmithButton");
		public static readonly Texture2D HandleButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/HandleButton");
		public static readonly Texture2D FireExtinguishButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/FireExtinguishButton");
		public static readonly Texture2D ArtButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/ArtButton");

		public static readonly Texture2D Clock = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/Clock");
		public static readonly Texture2D ClockSlave = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/GetToWork/ClockSlave");
		public static readonly Texture2D ActionsMenuDrop = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/ActionsMenuDrop");
		public static readonly Texture2D OptionsMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/OptionsMenu/OptionsMenu");
		public static readonly Texture2D OptionsGearHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/OptionsMenu/OptionsGearHover");
		public static readonly Texture2D OptionsGear = ContentFinder<Texture2D>.Get("UI/ColonistBar/OptionsMenu/OptionsGear");

		public static readonly Texture2D RestFood = ContentFinder<Texture2D>.Get("UI/ColonistBar/ColonistNeedBars/RestFood");
		public static readonly Texture2D HealthBar = ContentFinder<Texture2D>.Get("UI/ColonistBar/ColonistNeedBars/HealthBar");
		public static readonly Texture2D WhiteTexture = SolidColorMaterials.NewSolidColorTexture(Color.white);

		public static readonly Texture2D SetClearButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/SetClearButton");
		public static readonly Texture2D OrdersDropMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/OrdersDropMenu");
		public static readonly Texture2D MedicalDropMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/MedicalDropMenu");
		public static readonly Texture2D AttackMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/AttackMenu");

		public static readonly Texture2D UpgradeWeaponIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/UpgradeWeaponIcon");
		public static readonly Texture2D UpgradeIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/UpgradeIconHover");
		public static readonly Texture2D UpgradeArmorIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/UpgradeArmorIcon");
		public static readonly Texture2D ShootingMeleeHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/ShootingMeleeHover");
		public static readonly Texture2D ShootingIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/ShootingIcon");

		public static readonly Texture2D Rank_13 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_13");
		public static readonly Texture2D Rank_12 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_12");
		public static readonly Texture2D Rank_11 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_11");
		public static readonly Texture2D Rank_10 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_10");
		public static readonly Texture2D Rank_9 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_9");
		public static readonly Texture2D Rank_8 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_8");
		public static readonly Texture2D Rank_7 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_7");
		public static readonly Texture2D Rank_6 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_6");
		public static readonly Texture2D Rank_5 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_5");
		public static readonly Texture2D Rank_4 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_4");
		public static readonly Texture2D Rank_3 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_3");
		public static readonly Texture2D Rank_2 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_2");
		public static readonly Texture2D Rank_1 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_1");
		public static readonly Texture2D Rank_0 = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/Rank/Rank_0");
		public static readonly Texture2D MeleeIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/MeleeIcon");
		public static readonly Texture2D ArmorIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/ArmorIcon");

		public static readonly Texture2D YellowGroupIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/YellowGroupIcon");
		public static readonly Texture2D RedGroupIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/RedGroupIcon");
		public static readonly Texture2D GreenGroupIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/GreenGroupIcon");
		public static readonly Texture2D DarkGroupIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/DarkGroupIcon");
		public static readonly Texture2D BlueGroupIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/BlueGroupIcon");

		public static readonly Texture2D StatMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/StatMenu");
		public static readonly Texture2D TiredIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/TiredIcon");
		public static readonly Texture2D StarvingIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/StarvingIcon");
		public static readonly Texture2D SadIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/SadIcon");
		public static readonly Texture2D RestedIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/RestedIcon");
		public static readonly Texture2D OkayIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/OkayIcon");
		public static readonly Texture2D HurtIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/HurtIcon");
		public static readonly Texture2D HungryIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/HungryIcon");
		public static readonly Texture2D HappyIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/HappyIcon");
		public static readonly Texture2D HealthyIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/HealthyIcon");
		public static readonly Texture2D FullIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/FullIcon");
		public static readonly Texture2D AwakeIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/AwakeIcon");
		public static readonly Texture2D AliveIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/AliveIcon");
		public static readonly Texture2D ZoneMenu = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/ZoneMenu");
		public static readonly Texture2D StatListButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/StatListButton");
		public static readonly Texture2D ManageStatButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/ManageStatButton");
		public static readonly Texture2D EditButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Manage/Management/EditButton");
		public static readonly Texture2D SocializeButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/TakeABreak/SocializeButton");
		public static readonly Texture2D LightsOutButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/TakeABreak/LightsOutButton");
		public static readonly Texture2D EntertainmentButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/TakeABreak/EntertainmentButton");
		public static readonly Texture2D ChowHallButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/TakeABreak/ChowHallButton");

		public static readonly Texture2D RedMoodBar = SolidColorMaterials.NewSolidColorTexture(new ColorInt(95, 63, 65, 255).ToColor);
		public static readonly Texture2D YellowMoodBar = SolidColorMaterials.NewSolidColorTexture(new ColorInt(94, 94, 66, 255).ToColor);

		public static readonly Texture2D PawnDrafted = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/PawnDrafted");
		public static readonly Texture2D GroupArrowRightHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupArrowRightHover");
		public static readonly Texture2D GroupArrowRight = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupArrowRight");
		public static readonly Texture2D GroupArrowLeftHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupArrowLeftHover");
		public static readonly Texture2D GroupArrowLeft = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/GroupArrowLeft");

		public static readonly Texture2D PawnArrowDown = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/PawnArrowDown");
		public static readonly Texture2D PawnArrowLeft = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/PawnArrowLeft");
		public static readonly Texture2D PawnArrowRight = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/PawnArrowRight");
		public static readonly Texture2D PawnArrowUp = ContentFinder<Texture2D>.Get("UI/ColonistBar/GroupOverlays/PawnArrowUp");


		public static readonly Texture2D TakeBuffButtonHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/TakeBuffButtonHover");
		public static readonly Texture2D TakeBuffButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Orders/Icons/TakeBuffButton");
		public static readonly Texture2D ResearchWorkButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/ResearchWorkButton");
		public static readonly Texture2D ResearchMenuButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/ResearchMenuButton");
		public static readonly Texture2D ResearchHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/ResearchHover");
		public static readonly Texture2D CaravanHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/CaravanHover");
		public static readonly Texture2D CaravanButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/Actions/CaravanButton");
	}
}
