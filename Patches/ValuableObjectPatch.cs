using System.Collections.Generic;
using CartInventory.Challenges;
using CartInventory.Extensions;
using HarmonyLib;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(ValuableObject))]
public static class ValuableObjectPatch
{
    private static float Time = 0f;

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
            && !LevelStats.ValuableObjects.Contains(__instance)
           )
        {
            var traverse = __instance.GetDollarTraverse();
            var num = traverse.GetValue<float>();
            if (num <= 1000.0)
                traverse.SetValue((float)(num * 1.7));
            else if (num <= 5000.0)
                traverse.SetValue((float)(num * 1.5));
            else if (num <= 10000.0)
                traverse.SetValue((float)(num * 1.3));
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