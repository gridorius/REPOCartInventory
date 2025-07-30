using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof (StatsManager))]
public class StatsManagerPatch
{
    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void UpdateP() => LevelStats.Update();
}