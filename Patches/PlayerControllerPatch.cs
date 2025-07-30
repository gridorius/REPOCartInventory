using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(PlayerController))]
public class PlayerControllerPatch
{
    private static float LastSplit = 0;

    private static readonly List<string> mouseBtns = new()
    {
        "leftButton",
        "rightButton",
        "middleButton",
        "forwardButton",
        "backButton"
    };

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void UpdatePatch(PlayerController __instance)
    {
        if (ChatManagerPatch.ChatState != ChatManager.ChatState.Active)
        {
            if (IsButtonPressed(ModConfig.TruckExtractionKey.Value))
                ExtractDollars();
            if (IsButtonPressed(ModConfig.BagSplitKey.Value))
                SplitBags();
        }
    }

    private static void ExtractDollars()
    {
        if (CartInventory.Instance.SaveManager.Data.TrackDollars == 0)
            return;
        CartInventory.Instance.SaveManager.UpdateSaveId();
        CartInventory.Instance.SaveManager.Load();
        var minDistance = 100000;
        PhysGrabCart? minCart = null;
        foreach (var cart in LevelStats.Carts)
            if (cart != null)
            {
                var distance = Mathf.RoundToInt(Vector3.Distance(PlayerController.instance.transform.position,
                    cart.transform.position));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minCart = cart;
                }
            }

        if (minCart != null)
        {
            SpawnHelper.SpawnTaxBagInCart(minCart, CartInventory.Instance.SaveManager.Data.TrackDollars);
            CartInventory.Instance.SaveManager.Data.TrackDollars = 0;
            CartInventory.Instance.SaveManager.Save();
        }
    }

    private static void SplitBags()
    {
        if (LevelStats.Time - LastSplit < 3)
            return;
        foreach (var bag in SpawnHelper.SpawnedBags)
        {
            var dollars = Traverse.Create(bag).Field("dollarValueCurrent").GetValue<float>();
            if (dollars < 50)
                continue;

            float newPrice = dollars / 2;
            var spawnPosition = new Vector3(bag.transform.position.x, bag.transform.position.y + 1f,
                bag.transform.position.z);
            SpawnHelper.SpawnTaxBag(spawnPosition, (int)newPrice);
            bag.DollarValueSetRPC(newPrice);
            if (SemiFunc.IsMasterClient())
                Traverse.Create(bag).Field("photonView").GetValue<PhotonView>().RPC(
                    "DollarValueSetRPC", RpcTarget.All, newPrice);
            LastSplit = LevelStats.Time;
        }
    }

    private static bool IsButtonPressed(string btn)
    {
        return (!mouseBtns.Contains(btn)
            ? (ButtonControl)Keyboard.current[btn]
            : (ButtonControl)Mouse.current[btn]).isPressed;
    }
}