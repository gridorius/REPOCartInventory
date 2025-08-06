using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Logging;
using CartInventory.Shop;
using HarmonyLib;
using MenuLib;
using MenuLib.MonoBehaviors;
using MenuLib.Structs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CartInventory;

[BepInPlugin("Gridorius.CartInventory", "CartInventory", "1.0")]
public class CartInventory : BaseUnityPlugin
{
    private TruckController _truckController;
    internal static CartInventory Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    public bool IsLoaded { get; private set; }
    public SaveManager SaveManager { get; set; }

    private void Awake()
    {
        Instance = this;
        SaveManager = new SaveManager();
        ModConfig.Configure(Config);
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        Patch();

        IsLoaded = true;
        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        _truckController = new TruckController();
        ShopUI.Initialize();
    }

    private void Update()
    {
        if (ModConfig.EnableTruckItems.Value)
            _truckController.CheckTruckValuables();
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }
}