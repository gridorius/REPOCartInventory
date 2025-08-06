using BepInEx.Configuration;

namespace CartInventory;

public static class ModConfig
{
    public static ConfigEntry<bool>? EnableValuableConvert { get; private set; }
    public static ConfigEntry<float>? CartVacuumCleanerScale { get; private set; }
    public static ConfigEntry<bool>? EnableInternetShop { get; private set; }
    public static ConfigEntry<bool>? SkipShopLevel { get; private set; }
    public static ConfigEntry<string>? InternetShopKey { get; private set; }
    public static ConfigEntry<bool>? ShowHud { get; private set; }
    public static ConfigEntry<float>? StatusBarX { get; private set; }
    public static ConfigEntry<float>? StatusBarY { get; private set; }
    public static ConfigEntry<bool>? EnableTruckItems { get; private set; }
    public static ConfigEntry<string>? TruckExtractionKey { get; private set; }
    public static ConfigEntry<string>? BagSplitKey { get; private set; }
    public static ConfigEntry<bool>? CarefulMode { get; private set; }
    public static ConfigEntry<bool>? CarefulOrbDamage { get; private set; }
    public static ConfigEntry<int>? CarefulSkipChance { get; private set; }
    public static ConfigEntry<int>? CarefulSpawnT3Price { get; private set; }
    public static ConfigEntry<int>? CarefulSpawnT2Price { get; private set; }
    public static ConfigEntry<int>? CarefulSpawnT1Price { get; private set; }
    public static ConfigEntry<bool>? EnableModuleScaling { get; private set; }
    public static ConfigEntry<bool>? EnableValuableScaling { get; private set; }
    public static ConfigEntry<bool>? EnableEnemyScaling { get; private set; }
    public static ConfigEntry<bool>? EnableImmortalEnemy { get; private set; }
    public static ConfigEntry<int>? ImmortalEnemyChance { get; private set; }
    public static ConfigEntry<int>? EnemyScalingSkipLevels { get; private set; }
    public static ConfigEntry<float>? EnemyTier1Multiplier { get; private set; }
    public static ConfigEntry<float>? EnemyTier2Multiplier { get; private set; }
    public static ConfigEntry<float>? EnemyTier3Multiplier { get; private set; }
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
    public static ConfigEntry<bool>? HudShowImmortal { get; private set; }
    public static ConfigEntry<bool>? HudShowKills { get; private set; }
    public static ConfigEntry<bool>? HudShowCarts { get; private set; }

    private static ConfigDescription ChanceRange = new ConfigDescription("", new AcceptableValueRange<int>(0, 100));

    public static void Configure(ConfigFile config)
    {
        Cart(config);
        Shop(config);
        Hud(config);
        Truck(config);
        CfMode(config);
        Scaling(config);
    }

    private static void Cart(ConfigFile config)
    {
        var section = "Cart";
        var floatDescriptionRange = new ConfigDescription("", new AcceptableValueRange<float>(0.5f, 10f));
        EnableValuableConvert = config.Bind(section, "Enable valuable convert", true);
        CartVacuumCleanerScale = config.Bind(section, "Cart vacuum cleaner scale", 0.6f, floatDescriptionRange);
    }

    private static void Shop(ConfigFile config)
    {
        var section = "Internet shop";
        EnableInternetShop = config.Bind(section, "Enable new shop interface", true);
        SkipShopLevel = config.Bind(section, "Skip shop level", true);
        InternetShopKey = config.Bind(section, "Shop open button", "l");
    }

    private static void Hud(ConfigFile config)
    {
        var section = "Status hud";
        ShowHud = config.Bind(section, "Show hud", true);
        StatusBarX = config.Bind(section, "Hud position x", -240f);
        StatusBarY = config.Bind(section, "Hud position y", 30f);
        HudShowLevel = config.Bind(section, "Show level", true);
        HudShowTime = config.Bind(section, "Show time", true);
        HudShowSaved = config.Bind(section, "Show saved dollars", true);
        HudShowCollected = config.Bind(section, "Show collected valuables", true);
        HudShowCollectedPercent = config.Bind(section, "Show collected valuables percent", true);
        HudShowDollars = config.Bind(section, "Show level dollars", true);
        HudShowDollarsPercent = config.Bind(section, "Show level dollars collected percent", true);
        HudShowLost = config.Bind(section, "Show lost dollars", true);
        HudShowExplored = config.Bind(section, "Show amount of explored modules", true);
        HudShowImmortal = config.Bind(section, "Show immortal enemies", true);
        HudShowKills = config.Bind(section, "Show kills", true);
        HudShowCarts = config.Bind(section, "Show carts info", true);
    }

    private static void Truck(ConfigFile config)
    {
        var section = "Truck";
        EnableTruckItems = config.Bind(section, "Enable save items in truck", true);
        TruckExtractionKey = config.Bind(section, "Button to extract truck dollars into cart", "Slash");
        BagSplitKey = config.Bind(section, "Button to split grabbed bag", "Period");
    }

    private static void CfMode(ConfigFile config)
    {
        var section = "Careful mode";
        var intDescriptionRange = new ConfigDescription("", new AcceptableValueRange<int>(2, 20000));
        CarefulMode = config.Bind(section, "Enable", true);
        CarefulOrbDamage = config.Bind(section, "Register orb damage", false);
        CarefulSkipChance = config.Bind(section, "Chance skip spawn, damage saved to next iteration", 40, ChanceRange);
        CarefulSpawnT3Price = config.Bind(section, "Damage to spawn enemy tier 3", 5000, intDescriptionRange);
        CarefulSpawnT2Price = config.Bind(section, "Damage to spawn enemy tier 2", 3000, intDescriptionRange);
        CarefulSpawnT1Price = config.Bind(section, "Damage to spawn enemy tier 1", 2000, intDescriptionRange);
    }

    private static void Scaling(ConfigFile config)
    {
        var section = "Scaling";
        var floatDescriptionRange = new ConfigDescription("", new AcceptableValueRange<float>(1f, 10f));
        EnableModuleScaling = config.Bind(section, "Enable module amount scaling", true);
        EnableValuableScaling = config.Bind(section, "Enable valuable amount and price scaling", true);
        EnableEnemyScaling = config.Bind(section, "Enable enemy amount scaling", true);
        EnableImmortalEnemy = config.Bind(section, "Enable immortal enemies", true);
        ImmortalEnemyChance = config.Bind(section, "Immortal spawn chance", 30, ChanceRange);
        EnemyScalingSkipLevels = config.Bind(section, "Enemy scaling skip levels", 1);
        EnemyTier3Multiplier = config.Bind(section, "Enemy tier 3 amount multiplier", 1.2f, floatDescriptionRange);
        EnemyTier2Multiplier = config.Bind(section, "Enemy tier 2 amount multiplier", 1.4f, floatDescriptionRange);
        EnemyTier1Multiplier = config.Bind(section, "Enemy tier 1 amount multiplier", 1.8f, floatDescriptionRange);
        EnableExtractionScaling = config.Bind(section, "Enable extraction amount scaling", true);
    }
}