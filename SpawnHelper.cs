using System;
using System.Collections;
using System.Collections.Generic;
using CartInventory.Extensions;
using HarmonyLib;
using Photon.Pun;
using REPOLib.Modules;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CartInventory;

public static class SpawnHelper
{
    public static List<ValuableObject> SpawnedBags { get; } = new();

    public static void SpawnTaxBagInCart(PhysGrabCart cart, int dollars)
    {
        var cartPosition = cart.transform.position;
        var spawnPosition = new Vector3(cartPosition.x, cartPosition.y + 0.1f, cartPosition.z);
        SpawnTaxBag(spawnPosition, dollars);
    }

    public static void SpawnTaxBag(Vector3 spawnPosition, int dollars)
    {
        var original = AssetManager.instance.surplusValuableSmall;
        var gameObject = SemiFunc.IsMultiplayer()
            ? PhotonNetwork.InstantiateRoomObject("Valuables/" + original.name, spawnPosition,
                Quaternion.identity)
            : Object.Instantiate(original, spawnPosition, Quaternion.identity);
        var valuableComponent = gameObject.GetComponent<ValuableObject>();
        var physGrabObject = gameObject.GetComponent<PhysGrabObject>();
        Traverse.Create(physGrabObject).Field("spawnTorque").SetValue(Random.insideUnitSphere * 0.05f);
        valuableComponent.SetDollarValue(dollars);
        SpawnedBags.Add(valuableComponent);
    }

    public static void SpawnEnemy(string name, Vector3 spawnPosition, int count = 1)
    {
        if (EnemyDirector.instance.TryGetEnemyThatContainsName(name, out var enemySetup))
            SpawnEnemy(enemySetup, spawnPosition, count);
    }

    public static void SpawnEnemy(EnemySetup setup, Vector3 spawnPosition, int count = 1)
    {
        LevelPoint? levelPoint = null;
        var num = 500f;
        foreach (var levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
            if (Vector3.Distance(levelPathPoint.transform.position, spawnPosition) < (double)num)
            {
                levelPoint = levelPathPoint;
                num = Vector3.Distance(levelPathPoint.transform.position, spawnPosition);
            }

        if (levelPoint == null)
            return;
        for (var i = 0; i < count; i++)
            Enemies.SpawnEnemy(setup, levelPoint.transform.position, Quaternion.identity, false);
    }

    private static IEnumerator SpawnEnemyAfterTime(
        EnemySetup enemySetup,
        Vector3 position,
        TimeSpan timeSpan)
    {
        yield return new WaitForSeconds((float)timeSpan.TotalSeconds);
        Enemies.SpawnEnemy(enemySetup, position, Quaternion.identity, false);
    }
}