using HarmonyLib;
using UnityEngine.PlayerLoop;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(EnemyHealth))]
public class EnemyHealthPatch
{
    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    private static void Update(EnemyHealth __instance)
    {
        if (!SemiFunc.IsMasterClientOrSingleplayer() || !ModConfig.EnableImmortalEnemy.Value)
            return;
        var parent = __instance.GetComponentInParent<EnemyParent>();
        if (parent != null && LevelStats.ImmortalEnemies.Contains(parent))
            Traverse.Create(__instance).Field("deadImpulse").SetValue(false);
    }
}