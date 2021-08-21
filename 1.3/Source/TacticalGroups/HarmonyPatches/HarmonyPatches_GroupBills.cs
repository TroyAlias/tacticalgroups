using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace TacticalGroups
{
    public static class HarmonyPatches_GroupBills
    {
        private static Dictionary<Bill_Production, PawnGroup> billsSelectedGroup;

        public static void ExposeData()
        {
            try
            {
                Scribe_Collections.Look(ref billsSelectedGroup, "billsSelectedGroup", LookMode.Reference, LookMode.Reference);
            }
            catch { }
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                try
                {
                    billsSelectedGroup.RemoveAll((s) => s.Key is null || s.Value is null);
                }
                catch { }
            }
        }

        public static Dictionary<Bill_Production, PawnGroup> BillsSelectedGroup
        {
            get
            {
                if (billsSelectedGroup is null)
                {
                    billsSelectedGroup = new Dictionary<Bill_Production, PawnGroup>();
                }
                return billsSelectedGroup;
            }
        }

        public static string DoWindowContents_GetBillSelectedGroup(Bill_Production bill)
        {
            PawnGroup billSelectedGroup = (bill is null) ? null : BillsSelectedGroup.TryGetValue(bill);
            if (!(billSelectedGroup is null))
            {
                return "AnyWorker".Translate() + " of " + billSelectedGroup.curGroupName;
            }
            else
            {
                return "AnyWorker".Translate();
            }
        }

        [HarmonyDebug]
        public static IEnumerable<CodeInstruction> DoWindowContents_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var methodToCall = AccessTools.Method(typeof(HarmonyPatches_GroupBills), nameof(DoWindowContents_GetBillSelectedGroup));
            var billField = AccessTools.Field(typeof(Dialog_BillConfig), "bill");
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instruction = codes[i];
                if (!found && instruction.opcode == OpCodes.Ldstr && (instruction.operand as string) == "AnyWorker")
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(instruction);
                    yield return new CodeInstruction(OpCodes.Ldfld, billField);
                    yield return new CodeInstruction(OpCodes.Call, methodToCall);
                    i += 2;
                    found = true;
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
            List<Widgets.DropdownMenuElement<Pawn>> dropdownMenuElements = __result.ToList();
            foreach (PawnGroup pawnGroup in pawnGroups)
            {
                if (!(pawnGroup.pawns is null) && pawnGroup.pawns.Count > 0)
                {
                    foreach (Pawn pawn in pawnGroup.pawns)
                    {
                        bool allowed = true;
                        if (!(___bill.recipe.workSkill is null))
                        {
                            int level = pawn.skills.GetSkill(___bill.recipe.workSkill).Level;
                            allowed = level >= ___bill.allowedSkillRange.min && level <= ___bill.allowedSkillRange.max;
                        }
                        if (allowed)
                        {
                            Widgets.DropdownMenuElement<Pawn> dropdownMenuElement = new Widgets.DropdownMenuElement<Pawn>();
                            dropdownMenuElement.option = new FloatMenuOption("Any pawn of " + pawnGroup.curGroupName, delegate ()
                            {
                                ___bill.SetAnyPawnRestriction();
                                BillsSelectedGroup.SetOrAdd(___bill, pawnGroup);
                            });
                            dropdownMenuElement.payload = null;
                            int atIndex = 1;
                            if (ModsConfig.IdeologyActive)
                                atIndex++;
                            dropdownMenuElements.Insert(atIndex, dropdownMenuElement);
                            break;
                        }
                    }
                }
            }
            __result = dropdownMenuElements.AsEnumerable();
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
                    if (__instance.recipe.workSkill is null)
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

        public static void SetPawnRestriction(Bill __instance)
        {
            if (BillsSelectedGroup.ContainsKey(__instance as Bill_Production))
            {
                BillsSelectedGroup.Remove(__instance as Bill_Production);
            }
        }
    }
}
