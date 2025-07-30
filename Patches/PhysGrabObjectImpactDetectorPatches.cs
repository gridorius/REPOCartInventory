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
        ValuableObject? component = __instance?.GetComponent<ValuableObject>();
        if (ModConfig.CarefulMode.Value && component != null)
        {
            LevelStats.TotalValuablesDamage += valueLost;
            if (SemiFunc.IsMasterClientOrSingleplayer())
                RollSpawn(component);
        }

        LevelStats.UpdateLevelDollars();
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
            if (!SemiFunc.IsMasterClientOrSingleplayer())
                return;
            if (ModConfig.CarefulMode.Value && SpawnHelper.SpawnedBags.Contains(component))
            {
                var roomValue = Traverse.Create(component).Field("roomVolumeCheck").GetValue<RoomVolumeCheck>();
                var inExtractionPoint = Traverse.Create(roomValue).Field("inExtractionPoint").GetValue<bool>();
                if (inExtractionPoint || TruckController.TruckItems.Any(vf => vf.Valuable == component))
                    return;
                var roll = Random.Range(0, 100);
                var enemiesDifficulty2 = EnemyDirector.instance.enemiesDifficulty2;
                var enemiesDifficulty3 = EnemyDirector.instance.enemiesDifficulty3;
                (EnemySetup, int) enemyCount = roll switch
                {
                    >= 90 => (PickEnemy(enemiesDifficulty3), 1),
                    _ => (PickEnemy(enemiesDifficulty2), 2),
                };
                CartInventory.Logger.LogWarning($"Spawn {enemyCount.Item1.name}");
                SpawnHelper.SpawnEnemy(enemyCount.Item1, component.transform.position, enemyCount.Item2);
            }
        }
    }

    private static void RollSpawn(ValuableObject component)
    {
        var roll = Random.Range(0, 100);
        if (roll > 60)
            return;
        while (LevelStats.TotalValuablesDamage >= 3000)
        {
            var enemiesDifficulty1 = EnemyDirector.instance.enemiesDifficulty1;
            var enemiesDifficulty2 = EnemyDirector.instance.enemiesDifficulty2;
            var enemiesDifficulty3 = EnemyDirector.instance.enemiesDifficulty3;
            (EnemySetup?, float, int) enemyPrice = LevelStats.TotalValuablesDamage switch
            {
                >= 9000 => (PickEnemy(enemiesDifficulty3), 9000, 1),
                >= 6000 => (PickEnemy(enemiesDifficulty2), 6000, Random.Range(1, 2)),
                _ => (PickEnemy(enemiesDifficulty1), 3000, Random.Range(1, 4)),
            };

            CartInventory.Logger.LogWarning($"Spawn {enemyPrice.Item1.name}");
            SpawnHelper.SpawnEnemy(enemyPrice.Item1, component.transform.position, enemyPrice.Item3);
            LevelStats.TotalValuablesDamage -= enemyPrice.Item2;
        }
    }

    private static EnemySetup PickEnemy(List<EnemySetup> enemies)
    {
        var fromIndex = Random.Range(0, enemies.Count);
        var currentIndex = 0;
        foreach (var enemy in enemies.Where(e =>
                     !e.name.Contains("Ceiling Eye")
                     && !e.name.Contains("Slow Walker")
                     && !e.name.Contains("Thin Man")
                     && !e.name.Contains("Hidden")
                     && !e.name.Contains("Thin Man")
                 ))
        {
            if (currentIndex == fromIndex)
                return enemy;
            currentIndex++;
        }

        return enemies.First();
    }
}