using System;
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
    public static GameObject? GrabbedObject = null;
    public static bool IsGrab = false;
    private static float LastSplit;

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

    [HarmonyPatch("FixedUpdate")]
    [HarmonyPostfix]
    private static void PlayerGrabItemPatch(PlayerController __instance)
    {
        if (SemiFunc.RunIsShop() || __instance == null)
            return;
        if (__instance.physGrabActive && __instance.physGrabObject != null)
        {
            GrabbedObject = __instance.physGrabObject;
            IsGrab = true;
        }
        else
        {
            IsGrab = false;
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
        if (LevelStats.Time - LastSplit < 3 || !IsGrab)
            return;
        var component = GrabbedObject.GetComponent<ValuableObject>();
        if (component != null && SpawnHelper.SpawnedBags.Contains(component))
        {
            var dollars = Traverse.Create(component).Field("dollarValueCurrent").GetValue<float>();
            if (dollars < 50)
                return;

            var newPrice = dollars / 2;
            var spawnPosition = new Vector3(component.transform.position.x, component.transform.position.y + 1f,
                component.transform.position.z);
            SpawnHelper.SpawnTaxBag(spawnPosition, (int)newPrice);
            component.DollarValueSetRPC(newPrice);
            if (SemiFunc.IsMasterClient())
                Traverse.Create(component).Field("photonView").GetValue<PhotonView>().RPC(
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