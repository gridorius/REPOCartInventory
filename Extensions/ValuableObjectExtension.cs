using HarmonyLib;
using Photon.Pun;

namespace CartInventory.Extensions;

public static class ValuableObjectExtension
{
    public static void SetDollarValue(this ValuableObject valuable, float value)
    {
        valuable.DollarValueSetRPC(value);
        if (SemiFunc.IsMasterClient())
            Traverse.Create(valuable).Field("photonView").GetValue<PhotonView>().RPC(
                "DollarValueSetRPC", RpcTarget.All, value);
    }

    public static float GetDollarValue(this ValuableObject valuable)
    {
        return Traverse.Create(valuable).Field("dollarValueCurrent").GetValue<float>();
    }

    public static Traverse GetDollarTraverse(this ValuableObject valuable)
    {
        return Traverse.Create(valuable).Field("dollarValueCurrent");
    }
}