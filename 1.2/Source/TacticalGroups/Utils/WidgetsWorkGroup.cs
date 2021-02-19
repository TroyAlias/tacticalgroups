using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using static Verse.Widgets;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	public static class WidgetsWorkGroup
	{
		public const float WorkBoxSize = 25f;

		public static readonly Texture2D WorkBoxBGTex_Awful = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Awful");

		public static readonly Texture2D WorkBoxBGTex_Bad = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Bad");

		private const int AwfulBGMax = 4;

		public static readonly Texture2D WorkBoxBGTex_Mid = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Mid");

		private const int BadBGMax = 14;

		public static readonly Texture2D WorkBoxBGTex_Excellent = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Excellent");

		public static readonly Texture2D WorkBoxCheckTex = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxCheck");

		public static readonly Texture2D PassionWorkboxMinorIcon = ContentFinder<Texture2D>.Get("UI/Icons/PassionMinorGray");

		public static readonly Texture2D PassionWorkboxMajorIcon = ContentFinder<Texture2D>.Get("UI/Icons/PassionMajorGray");

		public static readonly Texture2D WorkBoxOverlay_Warning = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxOverlay_Warning");

		private const int WarnIfSelectedMax = 2;

		private const float PassionOpacity = 0.4f;

		public static Color ColorOfPriority(int prio)
		{
			switch (prio)
			{
				case 1:
					return new Color(0f, 1f, 0f);
				case 2:
					return new Color(1f, 0.9f, 0.5f);
				case 3:
					return new Color(0.8f, 0.7f, 0.5f);
				case 4:
					return new Color(0.74f, 0.74f, 0.74f);
				default:
					return Color.grey;
			}
		}

		public static bool IsCommonEnabledWork(ColonistGroup group, WorkTypeDef wType)
        {
			if (group?.groupWorkPriorities != null && group.groupWorkPriorities.TryGetValue(wType, out int priority))
			{
				return priority != 0;
			}
			return false;
		}

		public static int GetAllCommonWorkPriority(ColonistGroup group, WorkTypeDef wType)
        {
			if (group?.groupWorkPriorities != null && group.groupWorkPriorities.TryGetValue(wType, out int priority))
            {
				return priority;
            }
			return 0;
		}

		public static void DrawWorkBoxFor(float x, float y, WorkTypeDef wType, ColonistGroup group)
		{
			Rect rect = new Rect(x, y, 25f, 25f);
			GUI.color = Color.white;
			DrawWorkBoxBackground(rect, wType);
			if (Find.PlaySettings.useWorkPriorities)
			{
				int priority = GetAllCommonWorkPriority(group, wType);
				if (priority > 0)
				{
					Text.Anchor = TextAnchor.MiddleCenter;
					GUI.color = ColorOfPriority(priority);
					Widgets.Label(rect.ContractedBy(-3f), priority.ToStringCached());
					GUI.color = Color.white;
					Text.Anchor = TextAnchor.UpperLeft;
				}
				if (Event.current.type != 0 || !Mouse.IsOver(rect))
				{
					return;
				}
				if (Event.current.button == 0)
				{
					int num2 = GetAllCommonWorkPriority(group, wType) - 1;
					if (num2 < 0)
					{
						num2 = 4;
					}

					group.SetGroupWorkPriorityFor(wType, num2);

					if (ModCompatibility.BetterPawnControlIsActive)
                    {
						ModCompatibility.workManagerSaveCurrentStateMethod.Invoke(null, new object[] { group.pawns });
                    }
					SoundDefOf.DragSlider.PlayOneShotOnCamera();
				}
				if (Event.current.button == 1)
				{
					int num3 = GetAllCommonWorkPriority(group, wType) + 1;
					if (num3 > 4)
					{
						num3 = 0;
					}

					group.SetGroupWorkPriorityFor(wType, num3);

					if (ModCompatibility.BetterPawnControlIsActive)
					{
						ModCompatibility.workManagerSaveCurrentStateMethod.Invoke(null, new object[] { group.pawns });
					}
					SoundDefOf.DragSlider.PlayOneShotOnCamera();
				}
				Event.current.Use();
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab, KnowledgeAmount.SpecificInteraction);
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.ManualWorkPriorities, KnowledgeAmount.SmallInteraction);
				return;
			}

			if (IsCommonEnabledWork(group, wType))
			{
				GUI.DrawTexture(rect, WorkBoxCheckTex);
			}
			if (!Widgets.ButtonInvisible(rect))
			{
				return;
			}
			if (IsCommonEnabledWork(group, wType))
			{
				group.SetGroupWorkPriorityFor(wType, 0);

				if (ModCompatibility.BetterPawnControlIsActive)
				{
					ModCompatibility.workManagerSaveCurrentStateMethod.Invoke(null, new object[] { group.pawns });
				}
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
			}
			else
			{
				group.SetGroupWorkPriorityFor(wType, 3);

				if (ModCompatibility.BetterPawnControlIsActive)
				{
					ModCompatibility.workManagerSaveCurrentStateMethod.Invoke(null, new object[] { group.pawns });
				}
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			}
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab, KnowledgeAmount.SpecificInteraction);
		}

		private static void DrawWorkBoxBackground(Rect rect, WorkTypeDef workDef)
		{
			Texture2D image;
			Texture2D image2;
			float a;
			image = WorkBoxBGTex_Mid;
			image2 = WorkBoxBGTex_Excellent;
			a = (7 - 14f) / 6f;

			GUI.DrawTexture(rect, image);
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
			GUI.DrawTexture(rect, image2);
			GUI.color = Color.white;
		}
	}
}
