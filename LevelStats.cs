using System.Collections.Generic;
using System.Linq;
using CartInventory.Challenges;
using CartInventory.Extensions;
using HarmonyLib;

namespace CartInventory;

public class LevelStats
{
    public static float TotalLost;
    public static int EnemyKills = 0;
    public static List<PhysGrabCart> Carts = new();
    public static float LevelDollars;
    public static int TotalModules;
    public static int ExploredModules;
    public static float TotalValuablesDamage;
    public static bool FirstExtractionPointOpened = false;
    public static float Time = 0;
    public static float ValuableChangeTime = 0;
    public static TruckScreenText.PlayerChatBoxState TruckScreenState = TruckScreenText.PlayerChatBoxState.Idle;
    public static List<ValuableObject> ValuableObjects = new();
    public static List<ValuableObject> Orbs = new();
    public static Dictionary<PhysGrabCart, List<ValuableObject>> CartValuables = new();
    public static List<EnemyParent> ImmortalEnemies = new();
    public static int EnemyTotal = 0;

    public static int CurrentLevel => RunManager.instance.levelsCompleted + 1;

    public static bool TruckLeaving =>
        TruckScreenState is TruckScreenText.PlayerChatBoxState.LockedDestroySlackers
            or TruckScreenText.PlayerChatBoxState.LockedStartingTruck;

    public static int CurrentModCount =>
        Traverse.Create(LevelGenerator.Instance).Field("ModuleAmount").GetValue<int>();

    public static int CurrentExtractionCount =>
        Traverse.Create(LevelGenerator.Instance).Field("ExtractionAmount").GetValue<int>();

    public static void Clear()
    {
        ResetValuables();
        LevelDollars = 0;
        TotalLost = 0;
        Time = 0;
        EnemyKills = 0;
        TotalValuablesDamage = 0;
        TotalModules = 0;
        EnemyTotal = 0;
        ExploredModules = 0;
        FirstExtractionPointOpened = false;
        Orbs.Clear();
        ImmortalEnemies.Clear();
        Carts.Clear();
        CartValuables.Clear();
    }

    public static void AddModuleExplored()
    {
        ++ExploredModules;
    }

    public static (int, float) GetCollectedCount()
    {
        (int, float) collected = (0, 0);
        foreach (var valuable in ValuableObjects)
        {
            var roomValue = Traverse.Create(valuable).Field("roomVolumeCheck").GetValue<RoomVolumeCheck>();
            var inExtractionPoint = Traverse.Create(roomValue).Field("inExtractionPoint").GetValue<bool>();
            var dollars = valuable.GetDollarValue();
            if (inExtractionPoint || CartValuables.Any(cart => cart.Value.Contains(valuable)))
            {
                collected.Item1++;
                collected.Item2 += dollars;
            }
        }

        return collected;
    }

    public static void UpdateLevelDollars()
    {
        LevelDollars = 0;
        foreach (var valuable in ValuableObjects)
        {
            LevelDollars += valuable.GetDollarValue();
        }
    }

    public static void RegisterValuableObject(ValuableObject valuableObject)
    {
        if (ValuableObjects.Contains(valuableObject) || !valuableObject.name.Contains("(Clone)"))
            return;
        ValuableObjects.Add(valuableObject);
        UpdateLevelDollars();
    }

    public static void RemoveValuableObject(ValuableObject valuableObject)
    {
        if (Orbs.Contains(valuableObject))
            Orbs.Remove(valuableObject);
        if (ValuableObjects.Contains(valuableObject))
            ValuableObjects.Remove(valuableObject);
    }

    public static void ResetValuables()
    {
        ValuableObjects = new List<ValuableObject>();
    }

    public static void Update()
    {
        if (SemiFunc.RunIsLevel() || SemiFunc.RunIsShop())
            if (FirstExtractionPointOpened && !TruckLeaving)
            {
                Time += UnityEngine.Time.deltaTime;
                ValuableChangeTime += UnityEngine.Time.deltaTime;
            }

        if (ValuableChangeTime >= 120)
        {
            ValuableChangeTime -= 120;
            if (ChallengeManager.CurrentChallengeIs(Challenges.Challenges.FastExtraction))
            {
                foreach (var valuable in ValuableObjects)
                    valuable.SetDollarValue(valuable.GetDollarValue() * 0.9f);
                UpdateLevelDollars();
            }

            if (ChallengeManager.CurrentChallengeIs(Challenges.Challenges.MoneyTime))
            {
                foreach (var valuable in ValuableObjects)
                    valuable.SetDollarValue(valuable.GetDollarValue() * 1.1f);
                UpdateLevelDollars();
            }
        }
    }
}