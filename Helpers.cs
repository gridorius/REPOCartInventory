using UnityEngine;

namespace CartInventory;

public class Helpers
{
    public static bool Chance(int chance)
    {
        var roll = Random.Range(1, 99);
        return roll >= 100 - (chance > 100 ? 100 : chance < 0 ? 0 : chance);
    }

    public static bool Chance(float chance) => Chance((int)chance);
}