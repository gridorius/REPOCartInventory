using UnityEngine;

namespace CartInventory.Extensions;

public static class StringExtension
{
    public static string FormatDollars(this int nuber)
    {
        return nuber > 0 ? nuber.ToString("#,#$").Replace(",", " ") : "0$";
    }

    public static string FormatDollars(this float nuber)
    {
        return nuber > 0 ? nuber.ToString("#,#$").Replace(",", " ") : "0$";
    }

    public static string GetTimeString(this float seconds)
    {
        seconds = Mathf.Clamp(seconds, 0.0f, 360000f);
        var num1 = (int)(seconds * 1000.0) % 1000;
        var num2 = (int)seconds % 60;
        var num3 = (int)(seconds / 60.0) % 60;
        return $"{(int)(seconds / 3600.0):00}:{num3:00}:{num2:00}<size=75%>.{num1:000}</size>";
    }
}