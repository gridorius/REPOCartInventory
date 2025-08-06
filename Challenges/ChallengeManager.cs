using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace CartInventory.Challenges;

public class ChallengeManager
{
    public static Challenges CurrentChallenge = Challenges.None;

    public static bool CurrentChallengeIs(Challenges challenge) => CurrentChallenge == challenge;

    public static string CurrentChallengeName() => CurrentChallenge.ToString();

    public static void RollChallenge()
    {
        var challengeList = Enum.GetValues(typeof(Challenges)).Cast<Challenges>().ToArray();
        CurrentChallenge = challengeList[Random.Range(0, challengeList.Length)];
    }
}