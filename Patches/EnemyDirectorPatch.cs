using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(EnemyDirector))]
internal class EnemyDirectorPatch
{
    [HarmonyPatch("AmountSetup")]
    [HarmonyPrefix]
    private static bool AmountSetupPrefix(
        ref EnemyDirector __instance,
        ref int ___amountCurve1Value,
        ref int ___amountCurve2Value,
        ref int ___amountCurve3Value,
        ref int ___totalAmount,
        ref List<EnemySetup> ___enemiesDifficulty1,
        ref List<EnemySetup> ___enemiesDifficulty2,
        ref List<EnemySetup> ___enemiesDifficulty3,
        ref List<EnemySetup> ___enemyListCurrent,
        ref float ___despawnedTimeMultiplier)
    {
        if (!ModConfig.EnableEnemyScaling.Value ||
            LevelStats.CurrentLevel < ModConfig.EnableEnemyScalingSkipLevels.Value + 1)
            return true;
        var difficultyMultiplier1 = SemiFunc.RunGetDifficultyMultiplier1();
        var difficultyMultiplier2 = SemiFunc.RunGetDifficultyMultiplier2();
        var difficultyMultiplier3 = SemiFunc.RunGetDifficultyMultiplier3();

        if (difficultyMultiplier2 > 0.0)
        {
            ___amountCurve3Value =
                Mathf.CeilToInt(__instance.amountCurve3_2.Evaluate(difficultyMultiplier2) *
                                ModConfig.EnemyTier3Multiplier.Value) + 2;
            ___amountCurve2Value =
                Mathf.CeilToInt(__instance.amountCurve2_2.Evaluate(difficultyMultiplier2) *
                                ModConfig.EnemyTier2Multiplier.Value) + 2;
            ___amountCurve1Value =
                Mathf.CeilToInt(__instance.amountCurve1_2.Evaluate(difficultyMultiplier2) *
                                ModConfig.EnemyTier1Multiplier.Value) + 2;
        }
        else
        {
            ___amountCurve3Value =
                Mathf.CeilToInt(__instance.amountCurve3_1.Evaluate(difficultyMultiplier1) *
                                ModConfig.EnemyTier3Multiplier.Value) + 1;
            ___amountCurve2Value =
                Mathf.CeilToInt(__instance.amountCurve2_1.Evaluate(difficultyMultiplier1) *
                                ModConfig.EnemyTier2Multiplier.Value) + 1;
            ___amountCurve1Value =
                Mathf.CeilToInt(__instance.amountCurve1_1.Evaluate(difficultyMultiplier1) *
                                ModConfig.EnemyTier1Multiplier.Value) + 1;
        }

        var traverse = Traverse.Create(EnemyDirector.instance);
        ___enemyListCurrent.Clear();
        for (var index = 0; index < ___amountCurve1Value; ++index)
            traverse.Method("PickEnemies", ___enemiesDifficulty1).GetValue();
        for (var index = 0; index < ___amountCurve2Value; ++index)
            traverse.Method("PickEnemies", ___enemiesDifficulty2).GetValue();
        for (var index = 0; index < ___amountCurve3Value; ++index)
            traverse.Method("PickEnemies", ___enemiesDifficulty3).GetValue();
        ___despawnedTimeMultiplier = 1f;
        if (difficultyMultiplier3 > 0.0)
            ___despawnedTimeMultiplier = __instance.despawnTimeCurve_2.Evaluate(difficultyMultiplier3);
        else if (difficultyMultiplier2 > 0.0)
            ___despawnedTimeMultiplier = __instance.despawnTimeCurve_1.Evaluate(difficultyMultiplier2);
        ___totalAmount = ___amountCurve1Value + ___amountCurve2Value + ___amountCurve3Value;
        return false;
    }
}