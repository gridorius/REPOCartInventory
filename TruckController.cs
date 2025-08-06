using System.Collections.Generic;
using System.Linq;
using CartInventory.DTO;
using CartInventory.Extensions;
using HarmonyLib;
using UnityEngine;

namespace CartInventory;

public class TruckController
{
    public static List<ValuableAndPhysic> TruckItems = new();
    private Transform? _truckTransform;
    private static float objectInTruckCheckTimer = 0.5f;

    public TruckController()
    {
        _truckTransform = FindTruckInterior();
    }

    public void CheckTruckValuables()
    {
        if (_truckTransform == null)
        {
            _truckTransform = FindTruckInterior();
            return;
        }

        if (!SemiFunc.IsMasterClientOrSingleplayer())
            return;
        TruckItems.Clear();
        var coliders = Physics.OverlapBox(_truckTransform.position, _truckTransform.localScale / 2f,
            _truckTransform.rotation);
        foreach (var collider in coliders)
            if (collider.gameObject.layer == LayerMask.NameToLayer("PhysGrabObject"))
            {
                var componentInParent1 = collider.GetComponentInParent<PhysGrabObject>();
                if ((bool)(Object)componentInParent1 &&
                    TruckItems.All(x => x.Physic != componentInParent1))
                {
                    var componentInParent2 = collider.GetComponentInParent<ValuableObject>();
                    if ((bool)(Object)componentInParent2)
                        TruckItems.Add(new ValuableAndPhysic(componentInParent2, componentInParent1));
                }
            }

        if (TruckItems.Any())
        {
            foreach (var item in TruckItems)
            {
                var price = item.Valuable.GetDollarValue();
                CartInventory.Instance.SaveManager.Data.TrackDollars += (int)price;
                item.Physic.DestroyPhysGrabObject();
            }
            CartInventory.Instance.SaveManager.UpdateSaveId();
            CartInventory.Instance.SaveManager.Save();
        }
    }

    public static Transform? FindTruckInterior()
    {
        foreach (var roomVolume in Object.FindObjectsOfType<RoomVolume>())
            if (roomVolume.Truck)
                return roomVolume.transform;

        var array = Object.FindObjectsOfType<GameObject>()
            .Where(go =>
            {
                if (!go.name.Contains("Truck"))
                    return false;
                return go.name.Contains("Interior") || go.name.Contains("Inside") || go.name.Contains("Room");
            }).ToArray();
        if (array.Length != 0) return array[0].transform;

        var objectOfType = Object.FindObjectOfType<TruckDoor>();
        return objectOfType != null ? objectOfType.transform : null;
    }
}