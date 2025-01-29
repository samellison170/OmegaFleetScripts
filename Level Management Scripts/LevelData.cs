using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string LevelName; // The name of the level
    public int LevelIndex;  // The index/order of the level
    public int HighScore;   // The player's best score for this level
    public bool isUnlocked;
    public bool isComplete;
    public Vector3 Position;   // The position of the level's GameObject

    public LevelData(string levelName, int levelIndex, int highScore, bool isUnlocked, bool isComplete, Vector3 position)
    {
        LevelName = levelName;
        LevelIndex = levelIndex;
        HighScore = highScore;
        this.isUnlocked = isUnlocked;
        this.isComplete = isComplete;
        Position = position;
    }
    // Constructor overload for adding data without knowing position initially
    public LevelData(string levelName, int levelIndex, int highScore, bool isUnlocked, bool isComplete)
    {
        LevelName = levelName;
        LevelIndex = levelIndex;
        HighScore = highScore;
        this.isUnlocked = isUnlocked;
        this.isComplete = isComplete;
        Position = Vector3.zero; // Default to (0,0,0)
    }
    // Update position data using a GameObject
    public void UpdatePosition(GameObject levelObject)
    {
        if (levelObject != null)
        {
            Position = levelObject.transform.position;
        }
    }
}