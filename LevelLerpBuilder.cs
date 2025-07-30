using System;
using System.Collections.Generic;
using UnityEngine;

namespace CartInventory;

public class LevelLerpBuilder
{
    private readonly Dictionary<float, (bool, float)> LevelValues = new();

    public LevelLerpBuilder Add(float toLevel, float toValue)
    {
        LevelValues.Add(toLevel, (true, toValue));
        return this;
    }

    public LevelLerpBuilder AddScalar(float toLevel, float toValue)
    {
        LevelValues.Add(toLevel, (false, toValue));
        return this;
    }

    public float GetValue(float outOfRange, float startRange = 1)
    {
        (float, float) previousValue = (-100, -100);
        foreach (var levelValue in LevelValues)
        {
            if (LevelStats.CurrentLevel <= levelValue.Key)
            {
                if (!levelValue.Value.Item1)
                {
                    return levelValue.Value.Item2;
                }

                if (previousValue.Item1 < 0)
                {
                    return Mathf.Lerp(
                        startRange,
                        levelValue.Value.Item2,
                        Mathf.InverseLerp(1, levelValue.Key, LevelStats.CurrentLevel)
                    );
                }

                return Mathf.Lerp(
                    previousValue.Item2,
                    levelValue.Value.Item2,
                    Mathf.InverseLerp(previousValue.Item1 + 1, levelValue.Key, LevelStats.CurrentLevel)
                );
            }

            previousValue = (levelValue.Key, levelValue.Value.Item2);
        }

        return outOfRange;
    }

    public int GetIntValue(float outOfRange, float startRange = 1)
    {
        return Mathf.RoundToInt(GetValue(outOfRange, startRange));
    }
}