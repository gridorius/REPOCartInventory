using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(StatsManager), "SaveFileDelete")]
public class SaveFileDeletionPatch
{
    [HarmonyPrefix]
    private static bool Prefix(string saveFileName)
    {
        CartInventory.Instance.SaveManager.Delete();
        return true;
    }
}