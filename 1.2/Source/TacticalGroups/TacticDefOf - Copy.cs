using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
	[StaticConstructorOnStartup]
	public static class OverlayDrawer
	{
		public static readonly Material downedMat;

		private static readonly float BaseAlt;

		static OverlayDrawer()
		{
			downedMat = MaterialPool.MatFrom("UI/ColonistBar/GroupOverlays/ColonistDotDowned", ShaderDatabase.MetaOverlay);
			BaseAlt = AltitudeLayer.MetaOverlays.AltitudeFor();
		}

		public static void DrawDownedOverlay(Vector3 drawPos)
		{
			RenderPulsingOverlay(drawPos, downedMat, 2);
		}
		private static void RenderPulsingOverlay(Vector3 drawPos, Material mat, int altInd)
		{
			Mesh plane = MeshPool.plane08;
			drawPos.y = BaseAlt + 3f / 70f * (float)altInd;
			RenderPulsingOverlayInternal(drawPos, mat, plane);
		}
		private static void RenderPulsingOverlayInternal(Vector3 drawPos, Material mat, Mesh mesh)
		{
			float num = ((float)Math.Sin((Time.realtimeSinceStartup + 397f) * 4f) + 1f) * 0.5f;
			num = 0.3f + num * 0.7f;
			Material material = FadedMaterialPool.FadedVersionOf(mat, num);
			Graphics.DrawMesh(mesh, drawPos, Quaternion.identity, material, 0);
			Log.Message("Drawing: " + mesh + " - " + drawPos + " - " + mat + " - " + UI.MouseMapPosition());
		}
	}
}
