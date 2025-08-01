using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(LevelGenerator))]
internal class LevelGeneratorPatch
{
    private static int GetNewModuleCount()
    {
        if (!SemiFunc.RunIsLevel() || !ModConfig.EnableModuleScaling.Value)
            return LevelStats.CurrentModCount;

        return new LevelLerpBuilder()
            .AddScalar(2, 2 + LevelStats.CurrentLevel)
            .Add(6, 6)
            .Add(10, 10)
            .Add(20, 16)
            .Add(50, 20)
            .Add(100, 25)
            .GetIntValue(30, 6);
    }

    public static int GetNewExtractionCount()
    {
        if (!SemiFunc.RunIsLevel())
            return 0;
        if (!ModConfig.EnableExtractionScaling.Value)
            return LevelStats.CurrentExtractionCount;
        return LevelStats.CurrentLevel switch
        {
            <= 2 => 0,
            <= 3 => 1,
            <= 5 => 2,
            <= 10 => 3,
            <= 15 => 4,
            <= 20 => 5,
            <= 30 => 6,
            <= 50 => 7,
            <= 100 => 8,
            _ => 9
        };
    }

    [HarmonyPatch("TileGeneration", MethodType.Enumerator)]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TileGenerationTranspiler(
        IEnumerable<CodeInstruction> inst)
    {
        var codeInstructionList = new List<CodeInstruction>(inst);
        if (codeInstructionList.Count <= 20)
            return codeInstructionList;

        var index1 = -1;
        for (var index2 = 3; index2 < codeInstructionList.Count - 1; ++index2)
            if (codeInstructionList[index2].opcode == OpCodes.Ldfld &&
                (FieldInfo)codeInstructionList[index2].operand ==
                AccessTools.Field(typeof(LevelGenerator), "ModuleAmount") &&
                codeInstructionList[index2 + 1].opcode == OpCodes.Ldc_I4_3)
            {
                index1 = index2 - 3;
                break;
            }

        if (index1 == -1)
            return codeInstructionList;

        var index3 = -1;
        for (var index4 = 1; index4 < codeInstructionList.Count - 1; ++index4)
            if (codeInstructionList[index4].opcode == OpCodes.Ldarg_0 &&
                codeInstructionList[index4 + 1].opcode == OpCodes.Ldc_I4_M1 &&
                codeInstructionList[index4 + 4].opcode == OpCodes.Ldfld &&
                (FieldInfo)codeInstructionList[index4 + 4].operand ==
                AccessTools.Field(typeof(LevelGenerator), "ExtractionAmount"))
            {
                index3 = index4 - 1;
                break;
            }

        if (index3 == -1)
            return codeInstructionList;

        var collection1 = new List<CodeInstruction>
        {
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(LevelGeneratorPatch), "GetNewModuleCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(LevelGenerator), "ModuleAmount"))
        };
        var collection2 = new List<CodeInstruction>
        {
            new(OpCodes.Ldloc_1),
            new(OpCodes.Call, AccessTools.Method(typeof(LevelGeneratorPatch), "GetNewExtractionCount")),
            new(OpCodes.Stfld, AccessTools.Field(typeof(LevelGenerator), "ExtractionAmount"))
        };
        codeInstructionList.InsertRange(index3, collection2);
        codeInstructionList.InsertRange(index1, collection1);
        return codeInstructionList;
    }

    [HarmonyPatch("GenerateDone")]
    [HarmonyPostfix]
    private static void GenerateDonePostfix(ref int ___ModuleAmount, ref int ___ExtractionAmount)
    {
        LevelStats.Carts = Object.FindObjectsOfType<PhysGrabCart>().Where(cart => cart != null).ToList();
        CartInventory.Instance.SaveManager.UpdateSaveId();
        CartInventory.Instance.SaveManager.Load();
    }
}