using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelObjectDataScript : MonoBehaviour
{
    public int levelIndex;
    private string LevelName; // The name of the level
    private int HighScore;   // The player's best score for this level
    private bool isUnlocked;
    private bool isComplete;
    public SpriteRenderer goldStar;
    public SpriteRenderer completionXSprite;
    public SpriteRenderer greenUnlockCircle;
    public SpriteRenderer shineCompleteSprite;
    public SpriteRenderer shineNextLevelSprite;
    public TMP_Text levelName;


    private void Awake()
    {
        LevelData targetLevelData = LevelDatabase.GetLevelData(levelIndex);
        LevelName = targetLevelData.LevelName;
        HighScore = targetLevelData.HighScore;
        isUnlocked = targetLevelData.isUnlocked;
        isComplete = targetLevelData.isComplete;
        levelName.text = LevelName;
        if (isUnlocked)
        {
            //Debug.Log(isUnlocked);
            goldStar.enabled = true;
            greenUnlockCircle.enabled = true;
            if (!isComplete)
            {
                shineNextLevelSprite.enabled = true;
            }
        }
        if (isComplete)
        {
            shineCompleteSprite.enabled = true; 
            completionXSprite.enabled = true;
            shineNextLevelSprite.enabled = false;
        }
    }
    public void UpdateLevel()
    {
        //start here 12/11/24 DEV NOTE
        LevelData targetLevelData = LevelDatabase.GetLevelData(levelIndex);
        LevelName = targetLevelData.LevelName;
        //levelIndex = targetLevelData.LevelIndex;
        HighScore = targetLevelData.HighScore;
        isUnlocked = targetLevelData.isUnlocked;
        isComplete = targetLevelData.isComplete;
        Debug.Log("Level " + levelIndex + " isUnlock state = " + isUnlocked + ": isComplete status = " + isComplete);
        if (isUnlocked)
        {
            //Debug.Log(isUnlocked);
            goldStar.enabled = true;
            greenUnlockCircle.enabled = true;
            if (!isComplete)
            {
                shineNextLevelSprite.enabled = true;
            }
        }
        if (isComplete)
        {
            shineCompleteSprite.enabled = true;
            completionXSprite.enabled = true;
            shineNextLevelSprite.enabled = false;
        }
    }
}
