using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(EnemyParent))]
public class EnemyParentPatch
{
    private static List<string> ImmortalEnemies = new()
    {
        // "Duck",
        "Head",
        "Robe",
        "Hunter",
        "Beamer",
    };

    [HarmonyPatch("Awake")]
    [HarmonyPrefix]
    private static void Awake(EnemyParent __instance)
    {
        LevelStats.EnemyTotal++;
    }

    [HarmonyPatch("SpawnRPC")]
    [HarmonyPostfix]
    private static void SpawnRPC(EnemyParent __instance)
    {
        LevelStats.EnemyTotal++;
        if (SemiFunc.IsMasterClientOrSingleplayer()
            && ModConfig.EnableImmortalEnemy.Value
            && ImmortalEnemies.Any(name =>
                __instance.gameObject.name.Contains(name))
            && Helpers.Chance(ModConfig.ImmortalEnemyChance.Value)
           )
            LevelStats.ImmortalEnemies.Add(__instance);
    }

    [HarmonyPatch("DespawnRPC")]
    [HarmonyPostfix]
    private static void DespawnRPC(EnemyParent __instance)
    {
        LevelStats.EnemyTotal--;
        if (SemiFunc.IsMasterClientOrSingleplayer() && LevelStats.ImmortalEnemies.Contains(__instance))
            LevelStats.ImmortalEnemies.Remove(__instance);
    }
}