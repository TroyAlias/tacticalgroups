using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TacticalGroups
{
    [StaticConstructorOnStartup]
    public static class HostilityResponseModeUtilityGroup
    {
        private static readonly Texture2D IgnoreIcon = ContentFinder<Texture2D>.Get("UI/Icons/HostilityResponse/Ignore");

        private static readonly Texture2D AttackIcon = ContentFinder<Texture2D>.Get("UI/Icons/HostilityResponse/Attack");

        private static readonly Texture2D FleeIcon = ContentFinder<Texture2D>.Get("UI/Icons/HostilityResponse/Flee");

        private static readonly Color IconColor = new Color(0.84f, 0.84f, 0.84f);

        public static Texture2D GetIcon(this HostilityResponseMode response)
        {
            switch (response)
            {
                case HostilityResponseMode.Ignore:
                    return IgnoreIcon;
                case HostilityResponseMode.Attack:
                    return AttackIcon;
                case HostilityResponseMode.Flee:
                    return FleeIcon;
                default:
                    return BaseContent.BadTex;
            }
        }

        public static HostilityResponseMode GetNextResponse(Pawn pawn)
        {
            switch (pawn.playerSettings.hostilityResponse)
            {
                case HostilityResponseMode.Ignore:
                    if (pawn.WorkTagIsDisabled(WorkTags.Violent))
                    {
                        return HostilityResponseMode.Flee;
                    }
                    return HostilityResponseMode.Attack;
                case HostilityResponseMode.Attack:
                    return HostilityResponseMode.Flee;
                case HostilityResponseMode.Flee:
                    return HostilityResponseMode.Ignore;
                default:
                    return HostilityResponseMode.Ignore;
            }
        }

        public static string GetLabel(this HostilityResponseMode response)
        {
            return ("HostilityResponseMode_" + response).Translate();
        }

        public static void DrawResponseButton(Rect rect, ColonistGroup group, bool paintable)
        {
            Widgets.Dropdown(rect, group, IconColor, DrawResponseButton_GetResponse, DrawResponseButton_GenerateMenu, null, group.pawns.First().playerSettings.hostilityResponse.GetIcon(), null, null, delegate
            {

            }, paintable);
        }

        private static HostilityResponseMode DrawResponseButton_GetResponse(ColonistGroup group)
        {
            return group.pawns.First().playerSettings.hostilityResponse;
        }

        private static IEnumerable<Widgets.DropdownMenuElement<HostilityResponseMode>> DrawResponseButton_GenerateMenu(ColonistGroup group)
        {
            foreach (HostilityResponseMode response in Enum.GetValues(typeof(HostilityResponseMode)))
            {
                yield return new Widgets.DropdownMenuElement<HostilityResponseMode>
                {
                    option = new FloatMenuOption(response.GetLabel(), delegate
                    {
                        foreach (Pawn p in group.pawns)
                        {
                            p.playerSettings.hostilityResponse = response;
                        }
                    }, response.GetIcon(), Color.white),
                    payload = response
                };
            }
        }
    }
}
