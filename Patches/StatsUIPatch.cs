using HarmonyLib;

namespace CartInventory.Patches;


[HarmonyPatch(typeof (StatsUI))]
internal class StatsUIPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void StartP(ref StatsUI __instance) => ItemsStatsUI.Create(__instance);
}
