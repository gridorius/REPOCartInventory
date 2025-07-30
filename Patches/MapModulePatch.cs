using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof (MapModule))]
internal class MapModulePatch
{
    [HarmonyPatch("Hide")]
    [HarmonyPrefix]
    private static void HideP(ref bool ___animating)
    {
        if (___animating)
            return;
        LevelStats.AddModuleExplored();
    }
}
