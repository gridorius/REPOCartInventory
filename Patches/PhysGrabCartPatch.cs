#nullable disable
using System.Collections.Generic;
using System.Linq;
using CartInventory.DTO;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(PhysGrabCart))]
internal partial class PhysGrabCartPatch
{
    [HarmonyPatch("ObjectsInCart")]
    [HarmonyPostfix]
    private static void ObjectsInCart(PhysGrabCart __instance, List<PhysGrabObject> ___itemsInCart)
    {
        if (!CartInventory.Instance.IsLoaded)
            return;

        var valuables = ___itemsInCart.Where(x =>
            x != null && x.GetComponent<ValuableObject>() != null
        ).Select(v => new ValuableAndPhysic(v.GetComponent<ValuableObject>(), v)).ToList();
        if (valuables.Count == 0)
            return;

        var cartBoxes = valuables
            .Where(v => SpawnHelper.SpawnedBags.Contains(v.Valuable))
            .ToList();
        LevelStats.CartValuables[__instance] = valuables.Select(vp => vp.Valuable).ToList();
        float cartPrice = 0;

        if (!ModConfig.EnableValuableConvert.Value || !SemiFunc.IsMasterClientOrSingleplayer())
            return;

        foreach (var value in valuables)
        {
            var componentValue = Traverse.Create(value.Valuable).Field("dollarValueCurrent").GetValue<float>();
            if (SpawnHelper.SpawnedBags.Contains(value.Valuable))
                continue;
            cartPrice += componentValue;
            CartInventory.Logger.LogInfo($"additional item value: {componentValue}");
            ___itemsInCart.Remove(value.Physic);
            value.Physic.DestroyPhysGrabObject();
        }

        if (!cartBoxes.Any())
        {
            CartInventory.Logger.LogWarning($"Spawn bag {cartPrice}");
            SpawnHelper.SpawnTaxBagInCart(__instance, (int)cartPrice);
        }
        else if (cartBoxes.Count() > 1)
            foreach (var box in cartBoxes.Skip(1))
            {
                var componentValue = Traverse.Create(box.Valuable).Field("dollarValueCurrent").GetValue<float>();
                cartPrice += componentValue;
                ___itemsInCart.Remove(box.Physic);
                SpawnHelper.SpawnedBags.Remove(box.Valuable);
                box.Physic.DestroyPhysGrabObject();
            }

        if (cartPrice > 0 && cartBoxes.Any())
        {
            var first = cartBoxes.First();
            var componentValue = Traverse.Create(first.Valuable).Field("dollarValueCurrent").GetValue<float>();
            var newPrice = cartPrice + componentValue;
            first.Valuable.DollarValueSetRPC(newPrice);
            if (SemiFunc.IsMasterClient())
            {
                Traverse.Create(first.Valuable).Field("photonView").GetValue<PhotonView>().RPC(
                    "DollarValueSetRPC", RpcTarget.All, newPrice);
            }
        }
    }
}