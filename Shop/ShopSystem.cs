using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace CartInventory.Shop;

public class ShopSystem
{
    public static Dictionary<Item, int> PurchasedItems = new();
    public static int PurchasePrice;
    private static readonly Dictionary<Item, int> Items = new();

    public static void UpdateShopItems()
    {
        Items.Clear();
        PurchasedItems.Clear();
        var items = StatsManager.instance.itemDictionary.Values;
        foreach (var item in items)
        {
            item.maxAmountInShop = 20;
            item.maxPurchaseAmount = 20;
            item.maxPurchase = true;
            var price = CalculateValue(item);
            Items.Add(item, price);
            PurchasedItems.Add(item, 0);
        }
    }

    public static bool Buy()
    {
        if (PurchasePrice > SemiFunc.StatGetRunCurrency())
            return false;

        var spawnItems = new List<Item>();
        foreach (var item in PurchasedItems)
            if (item.Value > 0)
                for (var i = 0; i < item.Value; i++)
                {
                    StatsManager.instance.ItemPurchase(item.Key.itemAssetName);
                    if (item.Key.itemType == SemiFunc.itemType.item_upgrade)
                        StatsManager.instance.AddItemsUpgradesPurchased(item.Key.itemAssetName);
                    if (item.Key.itemType == SemiFunc.itemType.power_crystal)
                    {
                        StatsManager.instance.runStats["chargingStationChargeTotal"] += 17;
                        if (StatsManager.instance.runStats["chargingStationChargeTotal"] > 100)
                        {
                            StatsManager.instance.runStats["chargingStationChargeTotal"] = 100;
                        }
                        else
                        {
                            var chargeInt = Traverse.Create(ChargingStation.instance).Field("chargeInt");
                            chargeInt.SetValue(chargeInt.GetValue<int>() + 1);
                        }

                        UpdateChargeStation();
                    }
                    else
                    {
                        spawnItems.Add(item.Key);
                    }
                }

        SemiFunc.StatSetRunCurrency(SemiFunc.StatGetRunCurrency() - PurchasePrice);
        // TruckPopulateItems(spawnItems);
        return true;
    }

    public static void RecalculatePrice()
    {
        PurchasePrice = 0;
        foreach (var purchasedItem in PurchasedItems) PurchasePrice += Items[purchasedItem.Key] * purchasedItem.Value;
    }

    public static bool AllowBuy()
    {
        return PurchasePrice <= SemiFunc.StatGetRunCurrency();
    }

    public static Dictionary<Item, int> GetItems()
    {
        return Items;
    }

    private static void UpdateChargeStation()
    {
        var chargeFloat = StatsManager.instance.runStats["chargingStationChargeTotal"] / 100f;
        var chargeSegments = Traverse.Create(ChargingStation.instance).Field("chargeSegments")
            .GetValue<int>();
        var chargeSegmentsCurrent = Mathf.RoundToInt(chargeFloat * chargeSegments);
        Traverse.Create(ChargingStation.instance).Field("chargeTotal")
            .SetValue(StatsManager.instance.runStats["chargingStationChargeTotal"]);
        Traverse.Create(ChargingStation.instance).Field("chargeFloat")
            .SetValue(chargeFloat);
        Traverse.Create(ChargingStation.instance).Field("chargeSegmentCurrent")
            .SetValue(chargeSegmentsCurrent);
        Traverse.Create(ChargingStation.instance).Field("chargeScale")
            .SetValue(chargeFloat);
        Traverse.Create(ChargingStation.instance).Field("chargeScaleTarget")
            .SetValue(chargeSegmentsCurrent / chargeSegments);
        var chargeBar = Traverse.Create(ChargingStation.instance).Field("chargeBar")
            .GetValue<Transform>();
        if (chargeBar != null)
            chargeBar.localScale = new Vector3(chargeFloat, 1f, 1f);
        Debug.Log("Charging station charge total: " +
                  StatsManager.instance.runStats["chargingStationChargeTotal"]);
    }

    private static void TruckPopulateItems(List<Item> items)
    {
        if (SemiFunc.IsNotMasterClient())
            return;


        while (items.Count > 0)
        {
            var flag = false;
            for (var index = 0; index < items.Count; ++index)
            {
                var item = items[index];
                SpawnItem(item);
                items.RemoveAt(index);
            }
        }
    }

    private static void SpawnItem(Item item)
    {
        var rotation = ShopManager.instance.itemRotateHelper.transform.rotation;
        var truckHealerPosition = TruckHealer.instance.transform.position;
        var spawnPosition = new Vector3(truckHealerPosition.x, 1f, truckHealerPosition.z);
        if (SemiFunc.IsMasterClient())
        {
            PhotonNetwork.InstantiateRoomObject("Items/" + item.prefab.name, spawnPosition, rotation);
        }
        else
        {
            if (SemiFunc.IsMultiplayer())
                return;
            Object.Instantiate(item.prefab, spawnPosition, rotation);
        }
    }

    private static int CalculateValue(Item item)
    {
        var multiplier = Traverse.Create(ShopManager.instance).Field("itemValueMultiplier").GetValue<float>();
        var num1 = Random.Range(item.value.valueMin, item.value.valueMax) * multiplier;
        if (num1 < 1000.0)
            num1 = 1000f;
        var num2 = Mathf.Ceil(num1 / 1000f);
        if (item.itemType == SemiFunc.itemType.item_upgrade)
            num2 = ShopManager.instance.UpgradeValueGet(num2, item);
        else if (item.itemType == SemiFunc.itemType.healthPack)
            num2 = ShopManager.instance.HealthPackValueGet(num2);
        else if (item.itemType == SemiFunc.itemType.power_crystal)
            num2 = ShopManager.instance.CrystalValueGet(num2);

        return (int)num2;
    }
}