using System.Collections.Generic;
using System.Linq;
using CartInventory.Extensions;
using HarmonyLib;
using UnityEngine;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(PhysGrabObjectImpactDetector))]
public class PhysGrabObjectImpactDetectorPatches
{
    [HarmonyPatch("BreakRPC")]
    [HarmonyPostfix]
    private static void StartPostFix(
        float valueLost,
        PhysGrabObjectImpactDetector? __instance,
        bool _loseValue)
    {
        if (!_loseValue)
            return;

        LevelStats.TotalLost += valueLost;
        LevelStats.UpdateLevelDollars();
        ValuableObject? component = __instance?.GetComponent<ValuableObject>();
        if (ModConfig.CarefulMode.Value && component != null)
        {
            if (LevelStats.Orbs.Contains(component) && !ModConfig.CarefulOrbDamage.Value)
                return;
            LevelStats.TotalValuablesDamage += valueLost;
            if (SemiFunc.IsMasterClientOrSingleplayer())
                CarefulHelper.RollSpawn(component);
        }
    }

    [HarmonyPatch(typeof(PhysGrabObject), "DestroyPhysGrabObjectRPC")]
    [HarmonyPostfix]
    public static void DestroyPhysGrabObjectPostfix(
        PhysGrabObject __instance)
    {
        if (!SemiFunc.RunIsLevel())
            return;
        ValuableObject component = __instance.GetComponent<ValuableObject>();
        if (component != null)
        {
            LevelStats.RemoveValuableObject(component);
            CarefulHelper.OnDestroy(component);
        }
    }
}