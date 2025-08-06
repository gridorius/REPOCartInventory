#nullable disable
using System.Collections.Generic;
using System.Linq;
using CartInventory.DTO;
using CartInventory.Extensions;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace CartInventory.Patches;

[HarmonyPatch(typeof(PhysGrabCart))]
internal class PhysGrabCartPatch
{
    private static float objectInCartCheckTimer = 0.5f;

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void Update(PhysGrabCart __instance, Transform ___inCart)
    {
        if (objectInCartCheckTimer > 0.0)
        {
            objectInCartCheckTimer -= Time.deltaTime;
        }
        else
        {
            List<PhysGrabObject> cartItems = new();
            var colliderArray = Physics.OverlapBox(___inCart.position,
                ___inCart.localScale * ModConfig.CartVacuumCleanerScale.Value, ___inCart.rotation);
            foreach (var collider in colliderArray)
                if (collider.gameObject.layer == LayerMask.NameToLayer("PhysGrabObject"))
                {
                    var physObject = collider.GetComponentInParent<PhysGrabObject>();
                    if (physObject != null && !cartItems.Contains(physObject))
                        cartItems.Add(physObject);
                }

            ObjectsInCart(__instance, cartItems);
        }
    }

    private static void ObjectsInCart(PhysGrabCart cart, List<PhysGrabObject> cartItems)
    {
        if (!CartInventory.Instance.IsLoaded)
            return;

        var valuables = cartItems.Where(x =>
            x != null && x.GetComponent<ValuableObject>() != null
        ).Select(v => new ValuableAndPhysic(v.GetComponent<ValuableObject>(), v)).ToList();
        if (valuables.Count == 0)
            return;

        var cartBoxes = valuables
            .Where(v => SpawnHelper.SpawnedBags.Contains(v.Valuable))
            .ToList();
        LevelStats.CartValuables[cart] = valuables.Select(vp => vp.Valuable).ToList();
        float cartPrice = 0;

        if (!ModConfig.EnableValuableConvert.Value || !SemiFunc.IsMasterClientOrSingleplayer())
            return;

        foreach (var value in valuables)
        {
            var componentValue = value.Valuable.GetDollarValue();
            if (SpawnHelper.SpawnedBags.Contains(value.Valuable))
                continue;
            cartPrice += componentValue;
            cartItems.Remove(value.Physic);
            value.Physic.DestroyPhysGrabObject();
        }

        if (!cartBoxes.Any())
        {
            SpawnHelper.SpawnTaxBagInCart(cart, (int)cartPrice);
        }
        else if (cartBoxes.Count() > 1)
        {
            foreach (var box in cartBoxes.Skip(1))
            {
                var componentValue = box.Valuable.GetDollarValue();
                cartPrice += componentValue;
                cartItems.Remove(box.Physic);
                SpawnHelper.SpawnedBags.Remove(box.Valuable);
                box.Physic.DestroyPhysGrabObject();
            }
        }

        if (cartPrice > 0 && cartBoxes.Any())
        {
            var first = cartBoxes.First();
            var componentValue = first.Valuable.GetDollarValue();
            var newPrice = cartPrice + componentValue;
            first.Valuable.SetDollarValue(newPrice);
        }
    }
}