using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(ChatManager))]
internal class ChatManagerPatch
{
    public static ChatManager.ChatState ChatState;

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void UpdatePatch(ref ChatManager.ChatState ___chatState)
    {
        ChatState = ___chatState;
    }
}