using System.Collections;
using System.Globalization;
using CartInventory.Extensions;
using CartInventory.Patches;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace CartInventory;

internal class ItemsStatsUI(CartInventory cartInventory) : SemiUI
{
    public static ItemsStatsUI instance;
    private static readonly Color DangerColor = new(0.89f, 0.15f, 0.24f);
    private static readonly Color DollarColor = new(0.15f, 0.53f, 0.39f);
    private static readonly Color GrayColor = new(0.79f, 0.79f, 0.79f);
    private static readonly Color KeyColor = new(0.95f, 0.47f, 0.21f);
    private bool dontScoot = true;
    public TextMeshProUGUI statsText;

    protected override void Start()
    {
        base.Start();
        if (instance != null)
            DestroyImmediate(instance.gameObject);
        instance = this;
        statsText = gameObject.GetComponent<TextMeshProUGUI>();
        statsText.alignment = TextAlignmentOptions.TopLeft;
        statsText.color = Color.white;
        statsText.fontStyle = FontStyles.Normal;
    }

    protected override void Update()
    {
        base.Update();
        statsText.text = "";
        if (!MapToolControllerPatch.MapIsOpen)
            SetText();
        if (gameObject.transform.localPosition.x != ModConfig.StatusBarX.Value
            || gameObject.transform.localPosition.y != ModConfig.StatusBarY.Value)
            gameObject.transform.localPosition = new Vector3(
                ModConfig.StatusBarX.Value,
                ModConfig.StatusBarY.Value, 0.0f);
    }

    public static void Create(StatsUI orig)
    {
        if (!(SemiFunc.RunIsShop() || SemiFunc.RunIsLevel()) || !ModConfig.ShowHud.Value)
            return;
        var gameObject1 = GameObject.Find("Game Hud");
        var gameObject2 = GameObject.Find("Tax Haul");
        var gameObject = new GameObject(nameof(ItemsStatsUI));
        gameObject.transform.localPosition = new Vector3(ModConfig.StatusBarX.Value,
            ModConfig.StatusBarY.Value, 0.0f);
        gameObject.AddComponent(typeof(TextMeshProUGUI));
        var statsText = gameObject.GetComponent<TextMeshProUGUI>();
        statsText.font = gameObject2.GetComponent<TMP_Text>().font;
        statsText.fontSize = 19f;
        statsText.enableWordWrapping = false;
        statsText.alignment = TextAlignmentOptions.TopLeft;
        gameObject.transform.SetParent(gameObject1.transform, false);
        gameObject.AddComponent<ItemsStatsUI>();
    }

    private IEnumerator EnableOvercharge()
    {
        yield return new WaitForSeconds(2f);
        var objectOfType = FindObjectOfType<OverchargeUI>(true);
        if ((bool)(Object)objectOfType)
            objectOfType.gameObject.SetActive(true);
    }

    private void ScootLogic()
    {
        SemiUIScoot(new Vector2(0f, 0.0f));
    }

    public static string ColoredText(string text, Color color)
    {
        var htmlStringRgb = ColorUtility.ToHtmlStringRGB(color);
        return ColoredText(text, "#" + htmlStringRgb);
    }

    public static string ColoredText(string text, string hexColor)
    {
        return $"<color={hexColor}>{text}</color>";
    }

    private void SetText()
    {
        if (!(SemiFunc.RunIsShop() || SemiFunc.RunIsLevel()))
            return;
        if (ModConfig.HudShowLevel.Value)
            DrawLine("<size=150%>LEVEL", LevelStats.CurrentLevel + "<size=100%>", GrayColor);
        if (ModConfig.HudShowTime.Value)
            DrawLine("TIME", LevelStats.Time.GetTimeString(), GrayColor);
        if (ModConfig.HudShowSaved.Value && CartInventory.Instance.SaveManager.Data.TrackDollars > 0)
            DrawLine("SAVED", CartInventory.Instance.SaveManager.Data.TrackDollars.FormatDollars(), DollarColor);
        if (SemiFunc.RunIsShop())
            return;
        var collected = LevelStats.GetCollectedCount();
        if (ModConfig.HudShowCollected.Value)
        {
            DrawLine("COLLECTED",
                $"{collected.Item1} / {LevelStats.ValuableObjects.Count}" + (ModConfig.HudShowCollectedPercent.Value
                    ? $"({(LevelStats.ValuableObjects.Count > 0
                        ? ((float)collected.Item1 / (float)LevelStats.ValuableObjects.Count) : 1m):p})"
                    : ""),
                GrayColor);
        }

        if (ModConfig.HudShowDollars.Value)
        {
            DrawLine("DOLLARS",
                $"{collected.Item2.FormatDollars()} / {LevelStats.LevelDollars.FormatDollars()}" + (
                    ModConfig.HudShowDollarsPercent.Value
                        ? $"({collected.Item2 / LevelStats.LevelDollars:p})"
                        : ""),
                DollarColor);
        }

        if (ModConfig.HudShowLost.Value && LevelStats.TotalLost > 0)
            DrawLine("LOST", LevelStats.TotalLost.FormatDollars(), DangerColor);
        if (ModConfig.HudShowExplored.Value)
            DrawLine("EXPLORED", $"{LevelStats.ExploredModules}/{LevelStats.TotalModules}", GrayColor);
        if (ModConfig.HudShowCarts.Value)
        {
            var index = 0;
            DrawLine("CARTS", "", Color.green);
            foreach (var cart in LevelStats.Carts)
            {
                if (!cart)
                    continue;
                var haul = Traverse.Create(cart).Field("haulCurrent").GetValue<int>();
                var distance = Mathf.RoundToInt(Vector3.Distance(PlayerController.instance.transform.position,
                    cart.transform.position));

                statsText.text +=
                    $"<line-height=60%>  cart-{index++} / {ColoredText(distance.ToString(), GrayColor)}m";
                if (haul > 0)
                    statsText.text += $" / {ColoredText(haul.FormatDollars(), DollarColor)}";
                statsText.text += "\n";
            }
        }
    }

    private void DrawLine(string key, string value, Color valueColor)
    {
        statsText.text +=
            $"<line-height=60%><font-weight=900>{ColoredText(key + ": ", KeyColor)}</font-weight> {ColoredText(value, valueColor)}\n";
    }
}