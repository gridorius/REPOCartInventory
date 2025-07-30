using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(ExtractionPoint))]
public class ExtractionPointPatch
{
    [HarmonyPatch("ActivateTheFirstExtractionPointAutomaticallyWhenAPlayerLeaveTruck")]
    [HarmonyPostfix]
    private static void ActivateTheFirstExtractionPointAutomaticallyWhenAPlayerLeaveTruck()
    {
        LevelStats.FirstExtractionPointOpened = true;
    }
}