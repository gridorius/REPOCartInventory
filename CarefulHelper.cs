using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace CartInventory;

public class CarefulHelper
{
    public static void OnDestroy(ValuableObject component)
    {
        if (SemiFunc.IsMasterClientOrSingleplayer() && ModConfig.CarefulMode.Value &&
            SpawnHelper.SpawnedBags.Contains(component))
        {
            var roomValue = Traverse.Create(component).Field("roomVolumeCheck").GetValue<RoomVolumeCheck>();
            var inExtractionPoint = Traverse.Create(roomValue).Field("inExtractionPoint").GetValue<bool>();
            if (inExtractionPoint || TruckController.TruckItems.Any(vf => vf.Valuable == component))
                return;
            var roll = Random.Range(0, 100);
            var enemiesDifficulty2 = EnemyDirector.instance.enemiesDifficulty2;
            var enemiesDifficulty3 = EnemyDirector.instance.enemiesDifficulty3;
            var enemyCount = roll switch
            {
                >= 90 => (PickEnemy(enemiesDifficulty3), 1),
                _ => (PickEnemy(enemiesDifficulty2), 2)
            };
            CartInventory.Logger.LogWarning($"Spawn {enemyCount.Item1.name}");
            SpawnHelper.SpawnEnemy(enemyCount.Item1, component.transform.position, enemyCount.Item2);
        }
    }

    public static void RollSpawn(ValuableObject component)
    {
        var roll = Random.Range(1, 99);
        var skipChance = 100 - (ModConfig.CarefulSkipChance.Value > 100
            ? 100
            : ModConfig.CarefulSkipChance.Value < 0
                ? 0
                : ModConfig.CarefulSkipChance.Value);
        if (roll > skipChance)
            return;
        var enemiesDifficulty1 = EnemyDirector.instance.enemiesDifficulty1;
        var enemiesDifficulty2 = EnemyDirector.instance.enemiesDifficulty2;
        var enemiesDifficulty3 = EnemyDirector.instance.enemiesDifficulty3;
        while (LevelStats.TotalValuablesDamage >= ModConfig.CarefulSpawnT1Price.Value)
        {
            (EnemySetup?, float, int) spawnEnemy = (PickEnemy(enemiesDifficulty1), 3000, Random.Range(1, 4));
            if (LevelStats.TotalValuablesDamage >= ModConfig.CarefulSpawnT3Price.Value)
                spawnEnemy = (PickEnemy(enemiesDifficulty3), ModConfig.CarefulSpawnT3Price.Value, 1);
            else if (LevelStats.TotalValuablesDamage >= ModConfig.CarefulSpawnT2Price.Value)
                spawnEnemy = (PickEnemy(enemiesDifficulty2), ModConfig.CarefulSpawnT2Price.Value, Random.Range(1, 2));

            CartInventory.Logger.LogWarning($"Spawn {spawnEnemy.Item1.name}");
            SpawnHelper.SpawnEnemy(spawnEnemy.Item1, component.transform.position, spawnEnemy.Item3);
            LevelStats.TotalValuablesDamage -= spawnEnemy.Item2;
        }
    }

    public static EnemySetup PickEnemy(List<EnemySetup> enemies)
    {
        var fromIndex = Random.Range(0, enemies.Count);
        var currentIndex = 0;
        foreach (var enemy in enemies.Where(e =>
                     !e.name.Contains("Ceiling Eye")
                     && !e.name.Contains("Slow Walker")
                     && !e.name.Contains("Thin Man")
                     && !e.name.Contains("Hidden")
                 ))
        {
            if (currentIndex == fromIndex)
                return enemy;
            currentIndex++;
        }

        return enemies.First();
    }
}