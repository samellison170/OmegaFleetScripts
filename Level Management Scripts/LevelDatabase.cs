using System.Collections.Generic;
using UnityEngine;

public static class LevelDatabase
{
    // Dictionary to store all level data, with the level index as the key
    public static Dictionary<int, LevelData> Levels = new Dictionary<int, LevelData>()
    {
        { 1, new LevelData("Earth", 1, 0, true, false) },
        { 2, new LevelData("Alpha Centauri", 2, 0, false, false) },
        { 3, new LevelData("Tau Ceti", 3, 0, false, false) }
    };

    // Function to update a level's score
    public static void UpdateScore(int levelIndex, int newScore)
    {
        if (Levels.ContainsKey(levelIndex))
        {
            // Only update if the new score is higher
            if (newScore > Levels[levelIndex].HighScore)
            {
                Levels[levelIndex].HighScore = newScore;
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"Level {levelIndex} does not exist in the database!");
        }
    }
    // Function to associate a GameObject with a level and update its position
    public static void AssignGameObjectToLevel(int levelIndex, GameObject levelObject)
    {
        if (Levels.ContainsKey(levelIndex))
        {
            Levels[levelIndex].UpdatePosition(levelObject);
            Debug.Log($"Position for {Levels[levelIndex].LevelName} updated to {Levels[levelIndex].Position}");
        }
        else
        {
            Debug.LogError($"Level {levelIndex} does not exist in the database!");
        }
    }

    // Function to get a level's data
    public static LevelData GetLevelData(int levelIndex)
    {
        if (Levels.ContainsKey(levelIndex))
        {
            return Levels[levelIndex];
        }
        else
        {
            UnityEngine.Debug.LogError($"Level {levelIndex} does not exist in the database!");
            return null;
        }
    }
    public static void SaveLevelData()
    {
        foreach (var level in Levels)
        {
            PlayerPrefsManager.SetInt($"Level{level.Key}_HighScore", level.Value.HighScore);
            PlayerPrefsManager.SetInt($"Level{level.Key}_Unlocked", level.Value.isUnlocked ? 1 : 0);
        }
        PlayerPrefsManager.Save();
    }

    public static void LoadLevelData()
    {
        foreach (var level in Levels)
        {
            if (PlayerPrefsManager.HasKey($"Level{level.Key}_HighScore"))
            {
                level.Value.HighScore = PlayerPrefsManager.GetInt($"Level{level.Key}_HighScore");
            }
            if (PlayerPrefsManager.HasKey($"Level{level.Key}_Unlocked"))
            {
                level.Value.isUnlocked = PlayerPrefsManager.GetInt($"Level{level.Key}_Unlocked") == 1;
            }
            if (PlayerPrefsManager.HasKey($"Level{level.Key}_Complete"))
            {
                level.Value.isComplete = PlayerPrefsManager.GetInt($"Level{level.Key}_Complete") == 1;
            }
            //Debug.Log("level " + level.Value.LevelIndex + " completion status = " + level.Value.isComplete + ": Unlock status = " + level.Value.isUnlocked);
        }
    }
}
