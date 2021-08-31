using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	public class ColonistBarColonistDrawer
	{
		private struct IconDrawCall
		{
			public Texture2D texture;

			public string tooltip;

			public Color? color;

			public IconDrawCall(Texture2D texture, string tooltip = null, Color? color = null)
			{
				this.texture = texture;
				this.tooltip = tooltip;
				this.color = color;
			}
		}

		public Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

		public static readonly Texture2D DeadColonistTex = ContentFinder<Texture2D>.Get("UI/Misc/DeadColonist");

		public static readonly Texture2D Icon_FormingCaravan = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/FormingCaravan");

		public static readonly Texture2D Icon_MentalStateNonAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateNonAggro");

		public static readonly Texture2D Icon_MentalStateAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateAggro");

		public static readonly Texture2D Icon_MedicalRest = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MedicalRest");

		public static readonly Texture2D Icon_Sleeping = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Sleeping");

		public static readonly Texture2D Icon_Fleeing = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Fleeing");

		public static readonly Texture2D Icon_Attacking = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Attacking");

		public static readonly Texture2D Icon_Idle = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Idle");

		public static readonly Texture2D Icon_Burning = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Burning");

		public static readonly Texture2D Icon_Inspired = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Inspired");

		public static readonly Vector2 DefaultPawnTextureSize = new Vector2(TacticalColonistBar.BaseSize.x - 2f, 75f);

		public static Vector2 PawnTextureSize = new Vector2(TacticalColonistBar.BaseSize.x - 2f, 75f);

		public static Vector3 PawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);

		public static float PawnTextureCameraZoom = 1.28205f;

		private static Vector2[] bracketLocs = new Vector2[4];

		public static readonly Texture2D MoodBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.4f, 0.47f, 0.53f, 0.44f));
		public void DrawColonist(Rect rect, Pawn colonist, Map pawnMap, bool highlight, bool reordering)
		{
			if (!(ModCompatibility.alteredCarbonDrawColonist_PatchMethod is null))
            {
				bool prefixValue = (bool)ModCompatibility.alteredCarbonDrawColonist_PatchMethod.Invoke(this, new object[]
				{
					rect, colonist, pawnMap, highlight, reordering, pawnLabelsCache, PawnTextureSize, MoodBGTex, bracketLocs
				});
				if (!prefixValue)
				{
					return;
				}
            }
			float alpha = TacticUtils.TacticalColonistBar.GetEntryRectAlpha(rect);
			ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref alpha);
			if (reordering)
			{
				alpha *= 0.5f;
			}
			Color color2 = GUI.color = new Color(1f, 1f, 1f, alpha);
			GUI.DrawTexture(rect, TacticalColonistBar.BGTex);

			if (colonist.needs != null && colonist.needs.mood != null)
			{
				Rect position = rect.ContractedBy(2f);
				float num = position.height * colonist.needs.mood.CurLevelPercentage;
				position.yMin = position.yMax - num;
				position.height = num;
				if (TacticalGroupsSettings.DisplayColorBars)
			    {
					GUI.DrawTexture(position, GetMoodBarTexture(colonist));
					if (!TacticalGroupsSettings.DisplayBreakRiskOverlay)
					{
						GUI.DrawTexture(position, Textures.ColorMoodBarOverlay);
					}
				}
				else
			    {
					GUI.DrawTexture(position, MoodBGTex);
			    }

				if (TacticalGroupsSettings.DisplayBreakRiskOverlay)
                {
					if (colonist.needs.mood.CurLevel < colonist.mindState.mentalBreaker.BreakThresholdMajor)
					{
						GUI.DrawTexture(rect.ContractedBy(2f), Textures.ColorMoodBarOverlayRedMajor);
					}
					else if (colonist.needs.mood.CurLevel < colonist.mindState.mentalBreaker.BreakThresholdMinor)
					{
						GUI.DrawTexture(rect.ContractedBy(2f), Textures.ColorMoodBarOverlayRedMinor);
					}
					else if (TacticalGroupsSettings.DisplayColorBars)
					{
						GUI.DrawTexture(position, Textures.ColorMoodBarOverlay);
					}
				}
			}

			if (highlight)
			{
				int thickness = (rect.width <= 22f) ? 2 : 3;
				GUI.color = Color.white;
				Widgets.DrawBox(rect, thickness);
				GUI.color = color2;
			}
			Rect rect2 = rect.ContractedBy(-2f * TacticUtils.TacticalColonistBar.Scale);
			if ((colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist.Corpse) : Find.Selector.SelectedObjects.Contains(colonist)) && !WorldRendererUtility.WorldRenderedNow)
			{
				DrawSelectionOverlayOnGUI(colonist, rect2);
			}
			else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
			{
				DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
			}

			var pawnIconRect = GetPawnTextureRect(rect.position);
			var pawnTextureSize = PawnTextureSize * TacticalGroupsSettings.PawnScale;
			pawnTextureSize *= pawnIconRect.size.magnitude / pawnTextureSize.magnitude;
			GUI.DrawTexture(pawnIconRect, PortraitsCache.Get(colonist, pawnTextureSize, Rot4.South, PawnTextureCameraOffset, PawnTextureCameraZoom * TacticalGroupsSettings.PawnScale));

			if (colonist.Drafted)
            {
				GUI.DrawTexture(rect, Textures.PawnDrafted);
			}

			GUI.color = new Color(1f, 1f, 1f, alpha * 0.8f);
			DrawIcons(rect, colonist);
			GUI.color = color2;
			if (colonist.Dead)
			{
				GUI.DrawTexture(rect, DeadColonistTex);
			}
			else if (colonist.IsPrisoner)
            {
				GUI.DrawTexture(rect, Textures.PawnPrisoner);
			}
			
			var pawnStates = PawnStateUtility.GetAllPawnStatesCache(colonist);
			if (pawnStates.Contains(PawnState.IsBleeding))
            {
				GUI.DrawTexture(rect, Textures.PawnBleeding);
			}
			
			float num2 = 4f * TacticUtils.TacticalColonistBar.Scale;
			Vector2 pos = new Vector2(rect.center.x, rect.yMax - num2);
			GenMapUIOptimized.DrawPawnLabel(colonist, pos, alpha, rect.width + TacticUtils.TacticalColonistBar.SpaceBetweenColonistsHorizontal - 2f, pawnLabelsCache);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			
			if (!(ModCompatibility.pawnBadgesDrawMethod is null))
            {
				ModCompatibility.pawnBadgesDrawMethod.Invoke(this, new object[]
				{
					rect, colonist, pawnMap, highlight, reordering
				});
			}
			if (!(ModCompatibility.jobInBarDrawMethod is null))
			{
				ModCompatibility.jobInBarDrawMethod.Invoke(this, new object[]
				{
					rect, colonist, pawnMap, highlight, reordering
				});
			}
		}
		public static void DrawHealthBar(Rect rect, Pawn p, float healthBarWidth)
		{
			if (TacticalGroupsSettings.DisplayHealth)
			{
				Rect healthBar = new Rect(rect.x - healthBarWidth, rect.y, healthBarWidth, rect.height);
				float num = Mathf.Clamp(p.health.summaryHealth.SummaryHealthPercent, 0f, 1f);
				Rect rect3 = GenUI.ContractedBy(healthBar, 1f);
				float num5 = rect3.height * num;
				rect3.yMin = rect3.yMax - num5;
				rect3.height = num5;

				GUI.DrawTexture(rect3, TacticalGroupsSettings.HealthNeedBar, ScaleMode.ScaleAndCrop);
				GUI.DrawTexture(healthBar, Textures.HealthBar, ScaleMode.StretchToFill);
			}
		}

		public static void DrawRestAndFoodBars(Rect rect, Pawn p, float needWidth)
		{
			Rect needBar = new Rect(rect.x + rect.width, rect.y, needWidth, rect.height);
			if (TacticalGroupsSettings.DisplayFood && p.needs?.food != null)
			{
				float num = Mathf.Clamp(p.needs.food.CurLevelPercentage, 0f, 1f);
				Rect rect3 = GenUI.ContractedBy(needBar, 1f);
				float num5 = rect3.height * num;
				rect3.yMin = rect3.yMax - num5;
				rect3.height = num5;

				GUI.DrawTexture(rect3, TacticalGroupsSettings.FoodNeedBar, ScaleMode.ScaleAndCrop);
				GUI.DrawTexture(needBar, Textures.RestFood, ScaleMode.StretchToFill);
				needBar.x += needWidth;
			}
			if (TacticalGroupsSettings.DisplayRest && p.needs?.rest != null)
            {
				float num = Mathf.Clamp(p.needs.rest.CurLevelPercentage, 0f, 1f);
				Rect rect3 = GenUI.ContractedBy(needBar, 1f);
				float num5 = rect3.height * num;
				rect3.yMin = rect3.yMax - num5;
				rect3.height = num5;

				GUI.DrawTexture(rect3, TacticalGroupsSettings.RestNeedBar, ScaleMode.ScaleAndCrop);
				GUI.DrawTexture(needBar, Textures.RestFood, ScaleMode.StretchToFill);
			}
		}

		public static Texture2D GetMoodBarTexture(Pawn colonist)
		{
			float curLevel = colonist.needs.mood.CurLevel;
			Texture2D result;
			if (TacticalGroupsSettings.ColorBarMode == ColorBarMode.Default)
            {
				if (curLevel <= 0.35f)
				{
					result = TacticalGroupsSettings.DefaultMoodBarLowerBar;
				}
				else if (curLevel <= 0.69f)
				{
					result = TacticalGroupsSettings.DefaultMoodBarMiddleBar;
				}
				else
				{
					result = TacticalGroupsSettings.DefaultMoodBarUpperBar;
				}
			}
			else
            {
				if (curLevel <= 0.19f)
				{
					result = TacticalGroupsSettings.ExtendedMoodBarLowerIIBar;
				}
				else if (curLevel <= 0.27f)
				{
					result = TacticalGroupsSettings.ExtendedMoodBarLowerBar;
				}
				else if (curLevel <= 0.56f)
				{
					result = TacticalGroupsSettings.ExtendedMoodBarMiddleBar;
				}
				else if (curLevel <= 0.79f)
                {
					result = TacticalGroupsSettings.ExtendedMoodBarUpperBar;
                }
				else
                {
					result = TacticalGroupsSettings.ExtendedMoodBarUpperIIBar;
                }
			}
			return result;
		}
		private Rect GroupFrameRect(int group)
		{
			float num = 99999f;
			float num2 = 0f;
			float num3 = 0f;
			List<TacticalColonistBar.Entry> entries = TacticUtils.TacticalColonistBar.Entries;
			List<Rect> drawLocs = TacticUtils.TacticalColonistBar.DrawLocs;
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].group == group)
				{
					num = Mathf.Min(num, drawLocs[i].x);
					num2 = Mathf.Max(num2, drawLocs[i].x + TacticUtils.TacticalColonistBar.Size.x);
					num3 = Mathf.Max(num3, drawLocs[i].y + TacticUtils.TacticalColonistBar.Size.y);
				}
			}
			return new Rect(num, 0f, num2 - num, num3 - 0f).ContractedBy(-12f * TacticUtils.TacticalColonistBar.Scale);
		}

		public void DrawGroupFrame(int group)
		{
			Rect position = GroupFrameRect(group);
			Map map = TacticUtils.TacticalColonistBar.Entries.Find((TacticalColonistBar.Entry x) => x.group == group).map;
			float num = (map == null) ? ((!WorldRendererUtility.WorldRenderedNow) ? 0.75f : 1f) : ((map == Find.CurrentMap && !WorldRendererUtility.WorldRenderedNow) ? 1f : 0.75f);
			Widgets.DrawRectFast(position, new Color(0.5f, 0.5f, 0.5f, 0.4f * num));
		}

		public void ApplyEntryInAnotherMapAlphaFactor(Map map, ref float alpha)
		{
			if (map == null)
			{
				if (!WorldRendererUtility.WorldRenderedNow)
				{
					alpha = Mathf.Min(alpha, 0.4f);
				}
			}
			else if (map != Find.CurrentMap || WorldRendererUtility.WorldRenderedNow)
			{
				alpha = Mathf.Min(alpha, 0.4f);
			}
		}

		public void HandleClicks(Rect rect, Pawn colonist, int reorderableGroup, out bool reordering)
		{
			if (!(ModCompatibility.alteredCarbonHandleClicks_PatchMethod is null))
			{
				reordering = false;
				bool prefixValue = (bool)ModCompatibility.alteredCarbonHandleClicks_PatchMethod.Invoke(this, new object[]
				{
					rect, colonist, reorderableGroup, reordering
				});
				if (!prefixValue)
				{
					return;
				}
			}
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.clickCount == 2 && Mouse.IsOver(rect))
			{
				Event.current.Use();
				CameraJumper.TryJump(colonist);
			}
			reordering = ReorderableWidget.Reorderable(reorderableGroup, rect, useRightButton: true);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.IsOver(rect))
			{
				HarmonyPatches.curClickedColonist = colonist;
				Event.current.Use();
			}
		}

		public void HandleGroupFrameClicks(int group)
		{
			Rect rect = GroupFrameRect(group);
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Mouse.IsOver(rect) && !TacticUtils.TacticalColonistBar.AnyColonistOrCorpseAt(UI.MousePositionOnUIInverted))
			{
				bool worldRenderedNow = WorldRendererUtility.WorldRenderedNow;
				if ((!worldRenderedNow && !Find.Selector.dragBox.IsValidAndActive) || (worldRenderedNow && !Find.WorldSelector.dragBox.IsValidAndActive))
				{
					Find.Selector.dragBox.active = false;
					Find.WorldSelector.dragBox.active = false;
					TacticalColonistBar.Entry entry = TacticUtils.TacticalColonistBar.Entries.Find((TacticalColonistBar.Entry x) => x.group == group);
					Map map = entry.map;
					if (map == null)
					{
						if (WorldRendererUtility.WorldRenderedNow)
						{
							CameraJumper.TrySelect(entry.pawn);
						}
						else
						{
							CameraJumper.TryJumpAndSelect(entry.pawn);
						}
					}
					else
					{
						if (!CameraJumper.TryHideWorld() && Find.CurrentMap != map)
						{
							SoundDefOf.MapSelected.PlayOneShotOnCamera();
						}
						Current.Game.CurrentMap = map;
					}
				}
			}
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.IsOver(rect))
			{
				Event.current.Use();
			}
		}

		public void Notify_RecachedEntries()
		{
			pawnLabelsCache.Clear();
		}

		public Rect GetPawnTextureRect(Vector2 pos)
		{
			float x = pos.x;
			float y = pos.y;

			float pawnWidth = TacticalGroupsSettings.PawnBoxWidth;
			float pawnHeight = TacticalGroupsSettings.PawnBoxWidth * 1.630434782608696f;

			Vector2 vector = new Vector2(pawnWidth, pawnHeight) * TacticUtils.TacticalColonistBar.Scale;
			Rect pawnTexture = new Rect(x + 1f, y - (vector.y - TacticUtils.TacticalColonistBar.Size.y) - 1f, vector.x, vector.y).ContractedBy(1f);
			if (TacticalGroupsSettings.PawnCameraOffsetZ < 0 && pawnHeight < TacticUtils.TacticalColonistBar.Size.y)
			{
				PawnTextureCameraOffset.z = 0f;
				pawnTexture.y -= (pawnTexture.height * Mathf.Abs(TacticalGroupsSettings.PawnCameraOffsetZ)) * TacticalGroupsSettings.PawnScale;
			}
			return pawnTexture;
		}

		private static readonly float BaseIconAreaWidth = PawnTextureSize.x;
		private static readonly float BaseIconMaxSize = 20f;

		private static List<IconDrawCall> tmpIconsToDraw = new List<IconDrawCall>();
		public void DrawIcons(Rect rect, Pawn colonist)
		{
			if (colonist.Dead)
			{
				return;
			}
			tmpIconsToDraw.Clear();
			bool flag = false;
			if (colonist.CurJob != null)
			{
				JobDef def = colonist.CurJob.def;
				if (def == JobDefOf.AttackMelee || def == JobDefOf.AttackStatic)
				{
					flag = true;
				}
				else if (def == JobDefOf.Wait_Combat)
				{
					Stance_Busy stance_Busy = colonist.stances.curStance as Stance_Busy;
					if (stance_Busy != null && stance_Busy.focusTarg.IsValid)
					{
						flag = true;
					}
				}
			}
			
			if (colonist.IsFormingCaravan())
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_FormingCaravan, Strings.ActivityIconFormingCaravan));
			}
			if (colonist.InAggroMentalState)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_MentalStateAggro, colonist.MentalStateDef.LabelCap));
			}
			else if (colonist.InMentalState)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_MentalStateNonAggro, colonist.MentalStateDef.LabelCap));
			}
			else if (colonist.InBed() && colonist.CurrentBed().Medical)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_MedicalRest, Strings.ActivityIconMedicalRest));
			}
			else if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Sleeping, Strings.ActivityIconSleeping));
			}
			else if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Fleeing, Strings.ActivityIconFleeing));
			}
			else if (flag)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Attacking, Strings.ActivityIconAttacking));
			}
			else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 1)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Idle, Strings.ActivityIconIdle));
			}
			if (colonist.IsBurning())
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Burning, Strings.ActivityIconBurning));
			}
			
			if (colonist.Inspired)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Inspired, colonist.InspirationDef.LabelCap));
			}
			
			if (colonist.IsSlaveOfColony)
			{
				tmpIconsToDraw.Add(new IconDrawCall(colonist.guest.GetIcon()));
			}
			else
			{
				bool flag2 = false;
				if (colonist.Ideo != null)
				{
					Ideo ideo = colonist.Ideo;
					Precept_Role role = ideo.GetRole(colonist);
					if (role != null)
					{
						tmpIconsToDraw.Add(new IconDrawCall(role.Icon, null, ideo.Color));
						flag2 = true;
					}
				}
				if (!flag2)
				{
					Faction faction = null;
					if (colonist.HasExtraMiniFaction())
					{
						faction = colonist.GetExtraMiniFaction();
					}
					else if (colonist.HasExtraHomeFaction())
					{
						faction = colonist.GetExtraHomeFaction();
					}
					if (faction != null)
					{
						tmpIconsToDraw.Add(new IconDrawCall(faction.def.FactionIcon, null, faction.Color));
					}
				}
			}
			
			if (!(ModCompatibility.rimworldOfMagicDrawMethod is null))
			{
				ModCompatibility.rimworldOfMagicDrawMethod.Invoke(this, new object[]
				{
					null, rect, colonist
				});
			}
			
			float num = Mathf.Min(BaseIconAreaWidth / (float)tmpIconsToDraw.Count, BaseIconMaxSize) * TacticUtils.TacticalColonistBar.Scale;
			Vector2 pos = new Vector2(rect.x + 1f, rect.yMax - num - 1f);
			foreach (IconDrawCall item in tmpIconsToDraw)
			{
				GUI.color = item.color ?? Color.white;
				DrawIcon(item.texture, ref pos, num, item.tooltip);
				GUI.color = Color.white;
			}
		}

		private void DrawIcon(Texture2D icon, ref Vector2 pos, float iconSize, string tooltip = null)
		{
			Rect rect = new Rect(pos.x, pos.y, iconSize, iconSize);
			GUI.DrawTexture(rect, icon);
			if (tooltip != null)
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			pos.x += iconSize;
		}
		public void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
		{
			Thing obj = colonist;
			if (colonist.Dead)
			{
				obj = colonist.Corpse;
			}
			float num = 0.4f * TacticUtils.TacticalColonistBar.Scale;
			SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<object>(textureSize: new Vector2((float)SelectionDrawerUtility.SelectedTexGUI.width * num, (float)SelectionDrawerUtility.SelectedTexGUI.height * num), bracketLocs: bracketLocs, obj: (object)obj, rect: rect, selectTimes: SelectionDrawer.SelectTimes, jumpDistanceFactor: 20f * TacticUtils.TacticalColonistBar.Scale);
			DrawSelectionOverlayOnGUI(bracketLocs, num);
		}

		public void DrawCaravanSelectionOverlayOnGUI(Caravan caravan, Rect rect)
		{
			float num = 0.4f * TacticUtils.TacticalColonistBar.Scale;
			SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<WorldObject>(textureSize: new Vector2((float)SelectionDrawerUtility.SelectedTexGUI.width * num, (float)SelectionDrawerUtility.SelectedTexGUI.height * num), bracketLocs: bracketLocs, obj: (WorldObject)caravan, rect: rect, selectTimes: WorldSelectionDrawer.SelectTimes, jumpDistanceFactor: 20f * TacticUtils.TacticalColonistBar.Scale);
			DrawSelectionOverlayOnGUI(bracketLocs, num);
		}

		public void DrawSelectionOverlayOnGUI(Vector2[] bracketLocs, float selectedTexScale)
		{
			int num = 90;
			for (int i = 0; i < 4; i++)
			{
				Widgets.DrawTextureRotated(bracketLocs[i], SelectionDrawerUtility.SelectedTexGUI, num, selectedTexScale);
				num += 90;
			}
		}

		public static void ShowDrafteesWeapon(Rect rect, Pawn colonist, int weaponPlacementYOffset)
		{
			if (!TacticalGroupsSettings.DisplayWeapons)
            {
				return;
            }
			if (colonist == null)
			{
				return;
			}
			if (colonist.Dead)
			{
				return;
			}
			if (colonist.Downed)
			{
				return;
			}
			var container = colonist.equipment.GetDirectlyHeldThings();
			if (container == null || !container.Any())
			{
				return;
			}
			if (TacticalGroupsSettings.WeaponShowMode == WeaponShowMode.Drafted && !colonist.Drafted)
            {
				return;
            }
			if (colonist.TryGetGroups(out HashSet<ColonistGroup> groups) && groups.Any(x => x.hideWeaponOverlay))
			{
				return;
			}
			float alpha = TacticUtils.TacticalColonistBar.GetEntryRectAlpha(rect);
			if (colonist.Map != Find.CurrentMap || WorldRendererUtility.WorldRenderedNow)
			{
				alpha = 0.4f;
			}

			var size = rect.width * TacticalGroupsSettings.WeaponShowScale;
			var diff = size - rect.width;
			var xPos = rect.x - (diff / 2f);
			DrawColonistsBarWeaponIcon(new Rect(xPos, rect.yMax + weaponPlacementYOffset, size, size), container[0], alpha);
		}
		private static void DrawColonistsBarWeaponIcon(Rect rect, Thing thingWeapon, float alpha = 1f)
		{
			if (thingWeapon?.def == null)
			{
				return;
			}
			if (!thingWeapon.def.IsWeapon)
			{
				return;
			}
			if (thingWeapon.def.uiIcon == null)
			{
				return;
			}
			Texture2D weaponIcon = ((Texture2D)thingWeapon.Graphic.ExtractInnerGraphicFor(thingWeapon).MatSingleFor(thingWeapon).mainTexture) ?? thingWeapon.def.uiIcon;
			Color color = new Color(thingWeapon.DrawColor.r, thingWeapon.DrawColor.g, thingWeapon.DrawColor.b, alpha);
			DrawColonistBarWeaponIcon(rect, weaponIcon, color);
		}
		private static void DrawColonistBarWeaponIcon(Rect rect, Texture weaponIcon, Color color, float rotationAngle = 0f)
		{
			if (weaponIcon == null)
			{
				return;
			}
			Matrix4x4 matrix = GUI.matrix;
			if (!rotationAngle.Equals(0f))
			{
				Vector2 pivotPoint = new Vector2((rect.xMin + rect.width / 2f) * Prefs.UIScale, (rect.yMin + rect.height / 2f) * Prefs.UIScale);
				GUIUtility.RotateAroundPivot(rotationAngle, pivotPoint);
			}
			Color color2 = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(rect, weaponIcon);
			GUI.color = color2;
			GUI.matrix = matrix;
		}
	}
}
