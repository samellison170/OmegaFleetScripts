using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelDataManager : MonoBehaviour
{
    //THIS SCRIPT STORES THE CURRENT STAGE OF THE PLAYER FROM WHAT SCENE IT IS IN
    public static LevelDataManager Instance { get; private set; }
    public string FileName { get; set; } = "Level1Data.json";
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This is called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Load Scene")
        {
            //AssignCurrentLevel();
        }
        if (SceneManager.GetActiveScene().name == "Level Select Scene")
        {
            LevelDatabase.LoadLevelData();
            LevelObjectDataScript[] levelObjectScripts = FindObjectsOfType<LevelObjectDataScript>();
            foreach (var script in levelObjectScripts)
            {
                script.UpdateLevel();
            }
            LevelSelectManagerScript levelSelectManager = FindObjectOfType<LevelSelectManagerScript>();
            levelSelectManager.UpdateLinePositions();
        }
    }
    public void AssignCurrentLevel()
    {
        Debug.Log("assign current level being called");
        int currentLevel = ParseLevelNumber(FileName);
        Debug.Log($"Current level = {currentLevel}");
        if (currentLevel > 0)
        {
            PlayerPrefsManager.SetInt("CurrentLevel", currentLevel);
            Debug.Log($"Current level assigned: {currentLevel}");
        }
        else
        {
            Debug.LogError($"Failed to parse level from file name: {FileName}");
        }
    }
    public void LevelComplete()
    {
        int currentLevel = PlayerPrefsManager.GetInt("CurrentLevel");
        PlayerPrefsManager.SetInt($"Level{currentLevel}_Complete", 1);
        PlayerPrefsManager.SetInt($"Level{currentLevel + 1}_Unlocked", 1);
        Debug.Log(" Level " + currentLevel + " is " + PlayerPrefsManager.GetInt($"Level{currentLevel}_Unlocked"));
    }
    private int ParseLevelNumber(string fileName)
    {
        // Regex to match "Level<number>Data"
        Regex regex = new Regex(@"Level(\d+)Data");
        Match match = regex.Match(fileName);

        if (match.Success && int.TryParse(match.Groups[1].Value, out int levelNumber))
        {
            return levelNumber;
        }

        return -1; // Return -1 if parsing fails
    }
}
