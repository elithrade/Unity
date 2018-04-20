using UnityEngine;

public static class Difficulty 
{
    private static float _secondsToMaximumDifficulty = 60;

    public static float GetDifficultyPercentage()
    {
        // Time.time is the seconds since start of the game
        // Difficulty is set between 0 and 1
        return Mathf.Clamp01(Time.time / _secondsToMaximumDifficulty);
    }
}
