using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(TruckScreenText))]
internal class TruckScreenTextPatch
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    private static void TruckScreenText_Awake()
    {
        LevelStats.TruckScreenState = TruckScreenText.PlayerChatBoxState.Idle;
    }

    [HarmonyPatch("PlayerChatBoxStateUpdateRPC")]
    [HarmonyPostfix]
    private static void TruckScreenText_PlayerChatBoxStateUpdateRPC(
        ref TruckScreenText.PlayerChatBoxState _state)
    {
        LevelStats.TruckScreenState = _state;
    }
}