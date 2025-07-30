#nullable disable
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(RunManager))]
internal class RunManagerPatch
{
    private static bool _goingToManiMenu = false;

    [HarmonyPatch("ChangeLevel")]
    [HarmonyPrefix]
    private static void ChangeLevelPrefix(
        RunManager __instance,
        RunManager.ChangeLevelType _changeLevelType)
    {
        if (!CartInventory.Instance.IsLoaded)
            return;

        SpawnHelper.SpawnedBags.Clear();
        LevelStats.Clear();
    }

    [HarmonyPatch("ChangeLevel")]
    [HarmonyPostfix]
    private static void ChangeLevelPostfix(
        RunManager __instance,
        bool _completedLevel,
        bool _levelFailed,
        RunManager.ChangeLevelType _changeLevelType)
    {
        CartInventory.Instance.SaveManager.UpdateSaveId();
    }

    [HarmonyPatch("UpdateLevel")]
    [HarmonyPostfix]
    private static void UpdateLevelPostfix(
        RunManager __instance,
        string _levelName,
        int _levelsCompleted,
        bool _gameOver)
    {
        CartInventory.Instance.SaveManager.UpdateSaveId();
    }

    [HarmonyPatch("ResetProgress")]
    [HarmonyPostfix]
    private static void ResetProgressPostfix(RunManager __instance)
    {
        CartInventory.Instance.SaveManager.UpdateSaveId();
    }
}