using BepInEx.Configuration;

namespace CartInventory;

public static class ModConfig
{
    public static ConfigEntry<bool>? EnableValuableConvert { get; private set; }
    public static ConfigEntry<bool>? ShowHud { get; private set; }
    public static ConfigEntry<float>? StatusBarX { get; private set; }
    public static ConfigEntry<float>? StatusBarY { get; private set; }
    public static ConfigEntry<bool>? EnableTruckItems { get; private set; }
    public static ConfigEntry<string>? TruckExtractionKey { get; private set; }
    public static ConfigEntry<string>? BagSplitKey { get; private set; }
    public static ConfigEntry<bool>? CarefulMode { get; private set; }
    public static ConfigEntry<bool>? EnableModuleScaling { get; private set; }
    public static ConfigEntry<bool>? EnableValuableScaling { get; private set; }
    public static ConfigEntry<bool>? EnableEnemyScaling { get; private set; }
    public static ConfigEntry<float>? D1D1Multiplier { get; private set; }
    public static ConfigEntry<float>? D1D2Multiplier { get; private set; }
    public static ConfigEntry<float>? D1D3Multiplier { get; private set; }
    public static ConfigEntry<float>? D2D1Multiplier { get; private set; }
    public static ConfigEntry<float>? D2D2Multiplier { get; private set; }
    public static ConfigEntry<float>? D2D3Multiplier { get; private set; }
    public static ConfigEntry<bool>? EnableExtractionScaling { get; private set; }

    public static ConfigEntry<bool>? HudShowLevel { get; private set; }
    public static ConfigEntry<bool>? HudShowTime { get; private set; }
    public static ConfigEntry<bool>? HudShowSaved { get; private set; }
    public static ConfigEntry<bool>? HudShowCollected { get; private set; }
    public static ConfigEntry<bool>? HudShowCollectedPercent { get; private set; }
    public static ConfigEntry<bool>? HudShowDollars { get; private set; }
    public static ConfigEntry<bool>? HudShowDollarsPercent { get; private set; }
    public static ConfigEntry<bool>? HudShowLost { get; private set; }
    public static ConfigEntry<bool>? HudShowExplored { get; private set; }
    public static ConfigEntry<bool>? HudShowCarts { get; private set; }


    public static void Configure(ConfigFile config)
    {
        Cart(config);
        Hud(config);
        Truck(config);
        CfMode(config);
        Scaling(config);
    }

    private static void Cart(ConfigFile config)
    {
        var section = "Cart";
        EnableValuableConvert = config.Bind(section, "Enable valuable convert", true);
    }

    private static void Hud(ConfigFile config)
    {
        var section = "Status hud";
        ShowHud = config.Bind(section, "Show hud", true);
        StatusBarX = config.Bind(section, "PositionX", -240f);
        StatusBarY = config.Bind(section, "PositionY", 30f);

        HudShowLevel = config.Bind(section, "Show level", true);
        HudShowTime = config.Bind(section, "Show time", true);
        HudShowSaved = config.Bind(section, "Show saved", true);
        HudShowCollected = config.Bind(section, "Show collected", true);
        HudShowCollectedPercent = config.Bind(section, "Show collected percent", true);
        HudShowDollars = config.Bind(section, "Show dollars", true);
        HudShowDollarsPercent = config.Bind(section, "Show dollars percent", true);
        HudShowLost = config.Bind(section, "Show lost", true);
        HudShowExplored = config.Bind(section, "Show explored", true);
        HudShowCarts = config.Bind(section, "Show carts", true);
    }

    private static void Truck(ConfigFile config)
    {
        var section = "Truck";
        EnableTruckItems = config.Bind(section, "Enable truck items", true);
        TruckExtractionKey = config.Bind(section, "Button extract track dollars into cart", "Slash");
        BagSplitKey = config.Bind(section, "Button to split all bags", "Period");
    }

    private static void CfMode(ConfigFile config)
    {
        var section = "Careful mode";
        CarefulMode = config.Bind(section, "Enable", true);
    }

    private static void Scaling(ConfigFile config)
    {
        var section = "Scaling";
        EnableModuleScaling = config.Bind(section, "Enable module scaling", true);
        EnableValuableScaling = config.Bind(section, "Enable valuable scaling", true);
        EnableEnemyScaling = config.Bind(section, "Enable enemy scaling", true);
        D1D3Multiplier = config.Bind(section, "Enable d1 d3 scaling", 1.2f);
        D1D2Multiplier = config.Bind(section, "Enable d1 d2 scaling", 1.4f);
        D1D1Multiplier = config.Bind(section, "Enable d1 d1 scaling", 1.8f);
        D2D3Multiplier = config.Bind(section, "Enable d2 d3 scaling", 1.1f);
        D2D2Multiplier = config.Bind(section, "Enable d2 d2 scaling", 1.3f);
        D2D1Multiplier = config.Bind(section, "Enable d2 d1 scaling", 1.6f);
        EnableExtractionScaling = config.Bind(section, "Enable extraction scaling", true);
    }
}