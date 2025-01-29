using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManagerScript : MonoBehaviour
{
    public GameObject omegaFleetObject;
    private LineRenderer lineRenderer;
    public int currentLevelIndex;
    private int numberOfLevels;

    // List of GameObjects to define the line positions
    public List<GameObject> levelGameObjects = new List<GameObject>();

    void Start()
    {
        LevelDatabase.LoadLevelData();
        LevelObjectDataScript[] levelObjectScripts = FindObjectsOfType<LevelObjectDataScript>();
        foreach (var script in levelObjectScripts)
        {
            //Debug.Log("found level " + script.levelIndex);
            script.UpdateLevel();
        }
        // Get the Line Renderer component
        lineRenderer = GetComponent<LineRenderer>();
        currentLevelIndex = PlayerPrefsManager.GetInt("CurrentLevel");
        if(currentLevelIndex <= 0 ) currentLevelIndex = 1;
        Debug.Log("currentlevelIndex = " + currentLevelIndex);
        GameObject startLevel = levelGameObjects[currentLevelIndex - 1];
        Vector3 startLocation = new Vector3(startLevel.transform.position.x, startLevel.transform.position.y + 0.21f, startLevel.transform.position.z);
        Debug.Log("current level index = " + currentLevelIndex);
        omegaFleetObject.transform.position = startLocation;
        if (lineRenderer == null)
        {
            Debug.LogError("Line Renderer component not found!");
            return;
        }
        ApplyGameObjectsToLevelDatabase();
        UpdateLinePositions();
    }
    void ApplyGameObjectsToLevelDatabase()
    {
        int i = 1;
        foreach (GameObject objects in levelGameObjects)
        {
            LevelDatabase.AssignGameObjectToLevel(i, objects);
            i++;
        }
        numberOfLevels = i;

    }
    //void Update()
    //{
    //    // Optionally update positions every frame if the GameObjects move
    //    UpdateLinePositions();
    //}

    // Method to update the LineRenderer positions based on the GameObjects
    public void UpdateLinePositions()
    {
        if (levelGameObjects.Count == 0) return;

        List<Vector3> unlockedPositions = new List<Vector3>();

        for (int i = 0; i < levelGameObjects.Count; i++)
        {
            if (levelGameObjects[i] != null && i + 1 < numberOfLevels)
            {
                LevelData targetLevelData = LevelDatabase.GetLevelData(i + 1);
                if (targetLevelData.isUnlocked)
                {
                    unlockedPositions.Add(levelGameObjects[i].transform.position);
                    Debug.Log("Line position " + i + " set at " + levelGameObjects[i].transform.position + ", unlock status = " + true);
                }
            }
        }

        // Update the LineRenderer positions
        lineRenderer.positionCount = unlockedPositions.Count;

        for (int i = 0; i < unlockedPositions.Count; i++)
        {
            lineRenderer.SetPosition(i, unlockedPositions[i]);
        }
    }

    public void MoveOneLevelUp()
    {
        //Start here 12/12/24 Dev Note

        // Fetch the target position from the LevelDatabase
        LevelData targetLevelData = LevelDatabase.GetLevelData(currentLevelIndex + 1);
        if (targetLevelData != null)
        {
            if(targetLevelData.isUnlocked)
            {
                currentLevelIndex++;
                Vector3 targetPosition = new Vector3(targetLevelData.Position.x, targetLevelData.Position.y + 0.21f, targetLevelData.Position.z);
                LeanTween.move(omegaFleetObject, targetPosition, 0.5f);
                PlayerPrefsManager.SetInt("CurrentLevel", currentLevelIndex);
                Debug.Log($"Current target location for fleet = {targetPosition}");
                Debug.Log($"Current target location for fleet = {currentLevelIndex}");
            }
            else
            {
                Debug.Log("Level Locked");
            }

        }
        else
        {
            Debug.LogError($"Level data for index {currentLevelIndex} not found!");
        }
    }
    public void MoveOneLevelDown()
    {
        //Start here 12/12/24 Dev Note
        // Fetch the target position from the LevelDatabase
        LevelData targetLevelData = LevelDatabase.GetLevelData(currentLevelIndex - 1);
        if (targetLevelData != null)
        {
            if (targetLevelData.isUnlocked)
            {
                currentLevelIndex--;
                Vector3 targetPosition = new Vector3(targetLevelData.Position.x, targetLevelData.Position.y + 0.21f, targetLevelData.Position.z);
                LeanTween.move(omegaFleetObject, targetPosition, 0.5f);
                PlayerPrefsManager.SetInt("CurrentLevel", currentLevelIndex);
                Debug.Log($"Current target location for fleet = {targetPosition}");
                Debug.Log($"Current target location for fleet = {currentLevelIndex}");
            }
            else
            {
                Debug.Log("Level Locked");
            }

        }
        else
        {
            Debug.LogError($"Level data for index {currentLevelIndex} not found!");
        }
    }
}
