using HarmonyLib;
using Photon.Pun;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(MapToolController))]
public class MapToolControllerPatch
{
    public static bool MapIsOpen = false;

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void UpdateP2(ref bool ___Active, ref PhotonView ___photonView)
    {
        if (GameManager.Multiplayer() && !___photonView.IsMine)
            return;
        MapIsOpen = ___Active;
    }
}