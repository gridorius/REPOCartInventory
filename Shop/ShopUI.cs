using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MenuLib;
using MenuLib.MonoBehaviors;
using UnityEngine;

namespace CartInventory.Shop;

public class ShopUI
{
    private static readonly List<(REPOSlider, Item, int)> Sliders = new();
    private static REPOPopupPage? _popup;
    private static REPOLabel _label;
    private static REPOButton _buyButton;
    private static string _selectedType = "All";
    private static Dictionary<string, REPOButton> _buttons = new();
    private static string _buyText = "<color=#CCCCCC><b>Buy</b></color>";
    private static string _buyAllowText = "<color=#63ff95><b>Buy</b></color>";
    private static string _buyErrorText = "<color=#c41b27><b>Error</b></color>";

    public static void Initialize()
    {
        MenuAPI.AddElementToEscapeMenu(parent =>
        {
            MenuAPI.CreateREPOButton("Internet shop", () => { ShowPopup(); }, parent, new Vector2(350f, 20f));
        });
    }

    public static void ShowPopup()
    {
        if (!SemiFunc.IsMasterClientOrSingleplayer() || !ModConfig.EnableInternetShop.Value ||
            _popup != null && _popup.menuPage.enabled)
            return;
        _popup = MenuAPI.CreateREPOPopupPage("Shop", REPOPopupPage.PresetSide.Left,
            false, true, 1.5f);
        _popup.AddElement(p =>
        {
            _label = MenuAPI.CreateREPOLabel($"TOTAL: 0/{SemiFunc.StatGetRunCurrency()}", p,
                new Vector2(30f, 20f));
            _buyButton = MenuAPI.CreateREPOButton(_buyText, () =>
            {
                var state = ShopSystem.Buy();
                if (!state)
                {
                    _buyButton.labelTMP.text = _buyErrorText;
                    _buyButton.StartCoroutine(ButtonError(0.5f));
                }
                else
                {
                    _popup.ClosePage(false);
                }
            }, p, new Vector2(370f, 20f));
        });
        AddItems();
        CreateOutfitButtons();

        _popup.OpenPage(false);
    }

    private static IEnumerator ButtonError(float time)
    {
        yield return new WaitForSeconds(time);
        _buyButton.labelTMP.text = _buyText;
    }

    private static void AddItems()
    {
        Sliders.Clear();
        ShopSystem.UpdateShopItems();
        foreach (var item in ShopSystem.GetItems())
            _popup.AddElementToScrollView(scrollView =>
            {
                var slider = MenuAPI
                    .CreateREPOSlider(
                        Regex.Replace(item.Key.name, @"^Item\s(Upgrade)?(\sPlayer)?", "") + $"({item.Value}$)", "",
                        i =>
                        {
                            ShopSystem.PurchasedItems[item.Key] = i;
                            OnChangeItems();
                        }, scrollView,
                        Vector2.zero,
                        0,
                        item.Key.maxAmountInShop);
                Sliders.Add((slider, item.Key, item.Value));
                return slider.rectTransform;
            });
    }

    private static void OnChangeItems()
    {
        ShopSystem.RecalculatePrice();
        _label.labelTMP.text = $"TOTAL: {ShopSystem.PurchasePrice}/{SemiFunc.StatGetRunCurrency()}";
        if (ShopSystem.AllowBuy())
            _buyButton.labelTMP.text = _buyAllowText;
        else
            _buyButton.labelTMP.text = _buyText;
    }

    private static void CreateOutfitButtons()
    {
        _buttons.Clear();
        var types = new List<string>
        {
            "All",
            "Drone",
            "Cart",
            "Upgrade",
            "Tools",
            "Crystal",
            "Grenade",
            "Melee",
            "HealthPack",
            "Gun",
            "Mine"
        };
        var index = 0;
        foreach (var type in types)
        {
            var buttonText = $"<size=14><color=#CCCCCC>{type}</color></size>";
            var yPosition = 300 - (index - 1) * 14;
            _popup?.AddElement((MenuAPI.BuilderDelegate)(parent =>
            {
                _buttons[type] = MenuAPI.CreateREPOButton(buttonText,
                    () => OnChangeType(type), parent,
                    new Vector2(370f, yPosition));
            }));
            index++;
        }

        OnChangeType(_selectedType);
    }

    private static void OnChangeType(string type)
    {
        _selectedType = type;
        foreach (var keyValuePair in _buttons)
            keyValuePair.Value.labelTMP.text = $"<size=14><color=#CCCCCC>{keyValuePair.Key}</color></size>";
        _buttons[type].labelTMP.text = $"<size=14><color=#63ff95><b>{type}</b></color></size>";
        switch (type)
        {
            case "All":
                ShowTypeSliders();
                break;
            case "Drone":
                ShowTypeSliders(SemiFunc.itemType.drone);
                break;
            case "Cart":
                ShowTypeSliders(SemiFunc.itemType.cart, SemiFunc.itemType.pocket_cart);
                break;
            case "Upgrade":
                ShowTypeSliders(SemiFunc.itemType.item_upgrade, SemiFunc.itemType.player_upgrade);
                break;
            case "Tools":
                ShowTypeSliders(SemiFunc.itemType.orb, SemiFunc.itemType.tool, SemiFunc.itemType.tracker);
                break;
            case "Crystal":
                ShowTypeSliders(SemiFunc.itemType.power_crystal);
                break;
            case "Grenade":
                ShowTypeSliders(SemiFunc.itemType.grenade);
                break;
            case "Melee":
                ShowTypeSliders(SemiFunc.itemType.melee);
                break;
            case "HealthPack":
                ShowTypeSliders(SemiFunc.itemType.healthPack);
                break;
            case "Gun":
                ShowTypeSliders(SemiFunc.itemType.gun);
                break;
            case "Mine":
                ShowTypeSliders(SemiFunc.itemType.mine);
                break;
        }
    }

    private static void ShowTypeSliders(params SemiFunc.itemType[] types)
    {
        var all = types.Length == 0;
        foreach (var valueTuple in Sliders)
            if (all || types.Contains(valueTuple.Item2.itemType))
                valueTuple.Item1.repoScrollViewElement.visibility = true;
            else
                valueTuple.Item1.repoScrollViewElement.visibility = false;
    }
}