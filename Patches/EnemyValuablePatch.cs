using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(EnemyValuable))]
public class EnemyValuablePatch
{
    [HarmonyPatch("Start")]
    [HarmonyPrefix]
    private static void Start(EnemyValuable __instance)
    {
        LevelStats.EnemyKills++;
        var valuable = __instance.GetComponent<ValuableObject>();
        if (valuable != null)
            LevelStats.Orbs.Add(valuable);
    }
}