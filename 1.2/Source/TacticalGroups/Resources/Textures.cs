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

		public static readonly Texture2D RallyButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/RallyButton");
		public static readonly Texture2D RallyButtonHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/RallyButtonHover");

		public static readonly Texture2D AOMButton = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/AOMButton");
		public static readonly Texture2D AOMButtonHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/AOMButtonHover");
		public static readonly Texture2D AOMButtonPress = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/AOMButtonPress");

		public static readonly Texture2D DisbandIcon = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/DisbandIcon");
		public static readonly Texture2D DisbandIconHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/DisbandIconHover");

		public static readonly Texture2D EyeIconOff = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/EyeIconOff");
		public static readonly Texture2D EyeIconOffHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/EyeIconOffHover");

		public static readonly Texture2D EyeIconOn = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/EyeIconOn");
		public static readonly Texture2D EyeIconOnHover = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/EyeIconOnHover");

		public static Texture2D BackgroundColonistLayer = ContentFinder<Texture2D>.Get("UI/ColonistBar/RightClickGroupIcons/ColonistUnderLayer");
	}
}
