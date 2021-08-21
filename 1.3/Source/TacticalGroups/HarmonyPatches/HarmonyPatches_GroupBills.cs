using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;
using Verse.AI;
using System;
using System.Linq.Expressions;

namespace TacticalGroups
{
    public static class HarmonyPatches_GroupBills
    {
        public static Dictionary<Bill_Production, PawnGroup> billsSelectedGroup = new Dictionary<Bill_Production, PawnGroup>();
        public static string DoWindowContents_GetBillSelectedGroup(Bill_Production ___bill)
        {
            PawnGroup billSelectedGroup = (___bill is null) ? null : billsSelectedGroup.TryGetValue(___bill);
            if (!(billSelectedGroup is null))
            {
                return billSelectedGroup.curGroupName;
            }
            else
            {
                return "AnyWorker".Translate();
            }
        }
        public static IEnumerable<CodeInstruction> DoWindowContents_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instruction = codes[i];
                if (instruction.opcode == OpCodes.Ldstr && (instruction.operand as string) == "AnyWorker")
                {
                    yield return CodeInstruction.LoadField(typeof(Dialog_BillConfig), "bill");
                    //yield return CodeInstruction.Call(typeof(HarmonyPatches_GroupBills), "DoWindowContents_GetBillSelectedGroup");
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(HarmonyPatches_GroupBills), nameof(DoWindowContents_GetBillSelectedGroup)));
                    yield return new CodeInstruction(OpCodes.Nop);
                    i += 2; // skip Translate
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static void GeneratePawnRestrictionOptions(Bill_Production ___bill, ref IEnumerable<Widgets.DropdownMenuElement<Pawn>> __result)
        {
            List<PawnGroup> pawnGroups = TacticUtils.AllPawnGroups;
            pawnGroups.Reverse(); // Reverse the list because it iterates in the wrong direction

            List<Widgets.DropdownMenuElement<Pawn>> dropdownMenuElements = new List<Widgets.DropdownMenuElement<Pawn>>();

            foreach (PawnGroup pawnGroup in pawnGroups)
            {
                Log.Message("checking group '" + pawnGroup.curGroupName + "'");
                if (!(pawnGroup.pawns is null) && pawnGroup.pawns.Count > 0)
                {
                    foreach (Pawn pawn in pawnGroup.pawns)
                    {
                        Log.Message("    checking pawn '" + pawn.LabelShort + "' of group '" + pawnGroup.curGroupName + "'");
                        bool allowed = true;
                        if (!(___bill.recipe.workSkill is null))
                        {
                            int level = pawn.skills.GetSkill(___bill.recipe.workSkill).Level;
                            allowed = level >= ___bill.allowedSkillRange.min && level <= ___bill.allowedSkillRange.max;
                        }
                        Log.Message("        allowed = " + allowed);
                        if (allowed)
                        {
                            Widgets.DropdownMenuElement<Pawn> dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>();
                            dropdownMenuElement.option = new FloatMenuOption(pawnGroup.curGroupName ?? "Pawn group of " + pawn, delegate ()
                            {
                                ___bill.SetAnyPawnRestriction();
                                billsSelectedGroup.SetOrAdd(___bill, pawnGroup);
                            });
                            dropdownMenuElement.payload = null;
                            dropdownMenuElements.Add(dropdownMenuElement);
                            break;
                        }
                    }
                }
            }

            __result = __result.Concat(dropdownMenuElements);
        }

        public static bool PawnAllowedToStartAnew(Pawn p, Bill_Production __instance, ref bool __result)
        {
            PawnGroup billSelectedGroup = billsSelectedGroup.TryGetValue(__instance);
            if (!(billSelectedGroup is null) && __instance.PawnRestriction is null && !__instance.SlavesOnly)
            {
                bool pawnInGroup = billSelectedGroup.pawns.Contains(p);
                WorkGiverDef workGiver = __instance.billStack.billGiver.GetWorkgiver();
                if (pawnInGroup && !p.WorkTypeIsDisabled(workGiver.workType))
                {
                    if (!(__instance.recipe.workSkill is SkillDef))
                    {
                        __result = true;
                    }
                    else
                    {
                        int level = p.skills.GetSkill(__instance.recipe.workSkill).Level;
                        if (level < __instance.allowedSkillRange.min)
                        {
                            JobFailReason.Is("UnderAllowedSkill".Translate(__instance.allowedSkillRange.min), __instance.Label);
                            __result = false;
                        }
                        else if (level > __instance.allowedSkillRange.max)
                        {
                            JobFailReason.Is("AboveAllowedSkill".Translate(__instance.allowedSkillRange.max), __instance.Label);
                            __result = false;
                        }
                        else
                        {
                            __result = true;
                        }
                    }
                }
                else
                {
                    __result = false;
                }
                return false;
            }
            return true;
        }
    }
}
