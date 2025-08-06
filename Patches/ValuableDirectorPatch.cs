using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CartInventory.Challenges;
using CartInventory.Extensions;
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

    public static float NewValueCount()
    {
        if (!ModConfig.EnableValuableScaling.Value)
            return TotalMaxValue;
        return new LevelLerpBuilder()
            .Add(7, 180)
            .Add(11, 250)
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
            .AddScalar(2, 1f)
            .Add(4, 1.4f)
            .Add(6, 1.8f)
            .Add(10, 2f)
            .Add(15, 2.4f)
            .Add(20, 2.7f)
            .Add(30, 3f)
            .Add(50, 3.6f)
            .Add(75, 4f)
            .Add(100, 4.7f)
            .GetValue(5);

        return Mathf.RoundToInt(inAmount * multiplier);
    }

    private static int NewTotalItemCount()
    {
        return MultiplyItemCount(TotalMaxAmount);
    }

    private static int NewTinyItemCount()
    {
        if (ChallengeManager.CurrentChallengeIs(Challenges.Challenges.BigItems))
            return 0;
        return MultiplyItemCount(TinyMaxAmount);
    }

    private static int NewSmallItemCount()
    {
        if (ChallengeManager.CurrentChallengeIs(Challenges.Challenges.BigItems))
            return 0;
        return MultiplyItemCount(SmallMaxAmount);
    }

    private static int NewMediumItemCount()
    {
        if (ChallengeManager.CurrentChallengeIs(Challenges.Challenges.BigItems))
            return 0;
        return MultiplyItemCount(MediumMaxAmount);
    }

    private static int NewBigItemCount()
    {
        return MultiplyItemCount(BigMaxAmount);
    }

    private static int NewWideItemCount()
    {
        if (ChallengeManager.CurrentChallengeIs(Challenges.Challenges.BigItems))
            return 0;
        return MultiplyItemCount(WideMaxAmount);
    }

    private static int NewTallItemCount()
    {
        if (ChallengeManager.CurrentChallengeIs(Challenges.Challenges.BigItems))
            return 0;
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

        var collection = new List<CodeInstruction>();
        collection.SetField(typeof(ValuableDirectorPatch), "NewValueCount",
                typeof(ValuableDirector), "totalMaxValue")
            .SetField(typeof(ValuableDirectorPatch), "NewTotalItemCount",
                typeof(ValuableDirector),
                "totalMaxAmount")
            .SetField(typeof(ValuableDirectorPatch), "NewSmallItemCount",
                typeof(ValuableDirector),
                "smallMaxAmount")
            .SetField(typeof(ValuableDirectorPatch), "NewMediumItemCount",
                typeof(ValuableDirector),
                "mediumMaxAmount")
            .SetField(typeof(ValuableDirectorPatch), "NewBigItemCount",
                typeof(ValuableDirector), "bigMaxAmount")
            .SetField(typeof(ValuableDirectorPatch), "NewWideItemCount",
                typeof(ValuableDirector),
                "wideMaxAmount")
            .SetField(typeof(ValuableDirectorPatch), "NewVeryTallItemCount",
                typeof(ValuableDirector),
                "tallMaxAmount")
            .SetField(typeof(ValuableDirectorPatch), "NewTallItemCount",
                typeof(ValuableDirector),
                "tallMaxAmount")
            .SetField(typeof(ValuableDirectorPatch), "NewTinyItemCount",
                typeof(ValuableDirector),
                "tinyMaxAmount");
        codeInstructionList.InsertRange(index1, collection);
        return codeInstructionList;
    }
}