using System.Collections.Generic;
using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(Map))]
internal class MapPatch
{
    [HarmonyPatch("AddRoomVolume")]
    [HarmonyPostfix]
    private static void AddRoomVolumeP(ref Map __instance)
    {
        if (!SemiFunc.IsMasterClientOrSingleplayer() || !SemiFunc.RunIsLevel())
            return;
        LevelStats.TotalModules = __instance.MapModules.Count;
    }
}