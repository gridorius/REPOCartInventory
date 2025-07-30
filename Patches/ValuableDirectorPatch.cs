using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(ValuableDirector))]
internal class ValuableDirectorPatch
{
    private static int TotalMaxAmount =>
        Traverse.Create(ValuableDirector.instance).Field("totalMaxAmount").GetValue<int>();

    private static int TinyMaxAmount =>
        Traverse.Create(ValuableDirector.instance).Field("tinyMaxAmount").GetValue<int>();

    private static int SmallMaxAmount =>
        Traverse.Create(ValuableDirector.instance).Field("smallMaxAmount").GetValue<int>();

    private static int MediumMaxAmount =>
        Traverse.Create(ValuableDirector.instance).Field("mediumMaxAmount").GetValue<int>();

    private static int BigMaxAmount => Traverse.Create(ValuableDirector.instance).Field("bigMaxAmount").GetValue<int>();

    private static int WideMaxAmount =>
        Traverse.Create(ValuableDirector.instance).Field("wideMaxAmount").GetValue<int>();

    private static int TallMaxAmount =>
        Traverse.Create(ValuableDirector.instance).Field("tallMaxAmount").GetValue<int>();

    private static int VeryTallMaxAmount =>
        Traverse.Create(ValuableDirector.instance).Field("veryTallMaxAmount").GetValue<int>();

    private static float TotalMaxValue =>
        Traverse.Create(ValuableDirector.instance).Field("totalMaxValue").GetValue<float>();

    private static float NewValueCount()
    {
        if (!ModConfig.EnableValuableScaling.Value)
            return TotalMaxValue;
        return new LevelLerpBuilder()
            .Add(5, 180)
            .Add(11, 300)
            .Add(50, 700)
            .Add(100, 1500)
            .Add(1000, 5000)
            .Add(2000, 10000)
            .GetValue(10000, 30);
    }

    private static int MultiplyItemCount(int inAmount)
    {
        if (!ModConfig.EnableValuableScaling.Value)
            return inAmount;
        var multiplier = new LevelLerpBuilder()
            .AddScalar(2, 1)
            .Add(4, 1.1f)
            .Add(6, 1.3f)
            .Add(10, 1.6f)
            .Add(15, 2)
            .Add(20, 2.3f)
            .Add(30, 2.6f)
            .Add(50, 3)
            .Add(75, 3.6f)
            .Add(100, 5f)
            .GetValue(5, 30);

        return Mathf.RoundToInt(inAmount * multiplier);
    }

    private static int NewTotalItemCount()
    {
        return MultiplyItemCount(TotalMaxAmount);
    }

    private static int NewTinyItemCount()
    {
        return MultiplyItemCount(TinyMaxAmount);
    }

    private static int NewSmallItemCount()
    {
        return MultiplyItemCount(SmallMaxAmount);
    }

    private static int NewMediumItemCount()
    {
        return MultiplyItemCount(MediumMaxAmount);
    }

    private static int NewBigItemCount()
    {
        return MultiplyItemCount(BigMaxAmount);
    }

    private static int NewWideItemCount()
    {
        return MultiplyItemCount(WideMaxAmount);
    }

    private static int NewTallItemCount()
    {
        return MultiplyItemCount(TallMaxAmount);
    }

    private static int NewVeryTallItemCount()
    {
        return MultiplyItemCount(VeryTallMaxAmount);
    }

    [HarmonyPatch("SetupHost", MethodType.Enumerator)]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SetupHostTranspiler(IEnumerable<CodeInstruction> inst)
    {
        var codeInstructionList = new List<CodeInstruction>(inst);
        if (codeInstructionList.Count <= 20)
            return codeInstructionList;

        var index1 = -1;
        for (var index2 = 1; index2 < codeInstructionList.Count - 1; ++index2)
            if (codeInstructionList[index2].opcode == OpCodes.Stfld &&
                codeInstructionList[index2 + 1].opcode == OpCodes.Call &&
                (FieldInfo)codeInstructionList[index2].operand ==
                AccessTools.Field(typeof(ValuableDirector), "veryTallMaxAmount") &&
                (MethodInfo)codeInstructionList[index2 + 1].operand ==
                AccessTools.Method(typeof(SemiFunc), "RunIsArena"))
            {
                index1 = index2;
                break;
            }

        if (index1 == -1)
            return codeInstructionList;

        var collection = new List<CodeInstruction>
        {
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewValueCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "totalMaxValue")),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewTotalItemCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "totalMaxAmount")),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewSmallItemCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "smallMaxAmount")),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewMediumItemCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "mediumMaxAmount")),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewBigItemCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "bigMaxAmount")),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewWideItemCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "wideMaxAmount")),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewVeryTallItemCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "veryTallMaxAmount")),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewTallItemCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "tallMaxAmount")),
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(ValuableDirectorPatch), "NewTinyItemCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ValuableDirector), "tinyMaxAmount"))
        };
        codeInstructionList.InsertRange(index1, collection);
        return codeInstructionList;
    }
}