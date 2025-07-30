using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CartInventory.Extensions;

public static class EnemyDirectorExtensions
{
    public static List<EnemySetup> GetEnemies(this EnemyDirector enemyDirector)
    {
        List<EnemySetup> enemiesDifficulty1 = enemyDirector.enemiesDifficulty1;
        List<EnemySetup> enemiesDifficulty2 = enemyDirector.enemiesDifficulty2;
        List<EnemySetup> enemiesDifficulty3 = enemyDirector.enemiesDifficulty3;
        var enemies =
            new List<EnemySetup>(enemiesDifficulty1.Count + enemiesDifficulty2.Count + enemiesDifficulty3.Count);
        enemies.AddRange(enemiesDifficulty1);
        enemies.AddRange(enemiesDifficulty2);
        enemies.AddRange(enemiesDifficulty3);
        return enemies;
    }

    public static bool TryGetEnemyByName(
        this EnemyDirector enemyDirector,
        string name,
        [NotNullWhen(true)] out EnemySetup? enemySetup)
    {
        enemySetup = enemyDirector.GetEnemyByName(name);
        return enemySetup != null;
    }

    public static EnemySetup? GetEnemyByName(this EnemyDirector enemyDirector, string name)
    {
        return enemyDirector.GetEnemies().FirstOrDefault(x => x.name.Equals(name));
    }

    public static bool TryGetEnemyThatContainsName(
        this EnemyDirector enemyDirector,
        string name,
        out EnemySetup? enemySetup)
    {
        enemySetup = enemyDirector.GetEnemyThatContainsName(name);
        return enemySetup != null;
    }

    public static EnemySetup? GetEnemyThatContainsName(this EnemyDirector enemyDirector, string name)
    {
        return enemyDirector.GetEnemies()
            .FirstOrDefault(x => x.name.Contains(name));
    }
}