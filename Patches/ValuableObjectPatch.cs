using System.Collections.Generic;
using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(ValuableObject))]
public static class ValuableObjectPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start()
    {
    }

    [HarmonyPatch("DollarValueSetLogic")]
    [HarmonyPostfix]
    [HarmonyPriority(1000)]
    private static void DollarValueSetLogic(ref ValuableObject __instance)
    {
        if (SemiFunc.IsMasterClientOrSingleplayer()
            && ModConfig.EnableValuableScaling.Value
            && !SpawnHelper.SpawnedBags.Contains(__instance)
           )
        {
            var traverse = Traverse.Create(__instance).Field("dollarValueCurrent");
            var num = traverse.GetValue<float>();
            if (num <= 1000.0)
                traverse.SetValue((float)(num * 1.5));
            else if (num <= 5000.0)
                traverse.SetValue((float)(num * 1.3));
            else if (num <= 10000.0)
                traverse.SetValue((float)(num * 1.2));
            else
                traverse.SetValue((float)(num * 1.1));
        }

        LevelStats.RegisterValuableObject(__instance);
    }

    [HarmonyPatch("DollarValueSetRPC")]
    [HarmonyPostfix]
    private static void DollarValueSetRPC(ValuableObject __instance)
    {
        LevelStats.RegisterValuableObject(__instance);
    }
}