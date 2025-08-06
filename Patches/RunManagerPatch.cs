#nullable disable
using CartInventory.Challenges;
using HarmonyLib;
using Photon.Pun;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(RunManager))]
internal class RunManagerPatch
{
    private static bool _goingToManiMenu = false;
    private static Level _nextLevel;

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
        if (!SemiFunc.IsMasterClientOrSingleplayer())
            return;
        if (ModConfig.SkipShopLevel.Value)
        {
            bool levelChanged = false;
            if (__instance.levelCurrent == __instance.levelShop)
            {
                __instance.levelCurrent = __instance.levelLobby;
                _nextLevel = __instance.levelLobby;
                levelChanged = true;
            }

            if (_nextLevel != null)
            {
                (__instance.levelCurrent, _nextLevel) = (_nextLevel, __instance.levelCurrent);
                levelChanged = true;
            }

            if (SemiFunc.RunIsLevel())
                _nextLevel = null;

            if (levelChanged && GameManager.Multiplayer())
            {
                var pun = Traverse
                    .Create(__instance).Field("runManagerPUN").GetValue<RunManagerPUN>();
                var photonView = Traverse.Create(pun).Field("photonView").GetValue<PhotonView>();
                photonView.RPC("UpdateLevelRPC", RpcTarget.OthersBuffered, __instance.levelCurrent.name,
                    __instance.levelsCompleted, Traverse.Create(__instance).Field("gameOver").GetValue<bool>());
            }
        }

        CartInventory.Instance.SaveManager.UpdateSaveId();
        if (SemiFunc.RunIsLevel() && LevelStats.CurrentLevel > 1)
            ChallengeManager.RollChallenge();
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
        if (SemiFunc.IsMasterClientOrSingleplayer())
            CartInventory.Instance.SaveManager.UpdateSaveId();
    }
}