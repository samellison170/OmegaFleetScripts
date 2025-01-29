using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    // List to track all saved keys
    private static List<string> savedKeys = new List<string>();
    public static event Action<string, int> OnIntChanged;
    // Set an integer value
    //public static void SetInt(string key, int value)
    //{
    //    PlayerPrefs.SetInt(key, value);
    //    AddKey(key);
    //}
    public static void SetInt(string key, int value)
    {
        int oldValue = PlayerPrefs.GetInt(key, int.MinValue); // Default to impossible value
        PlayerPrefs.SetInt(key, value);
        if (oldValue != value)
        {
            OnIntChanged?.Invoke(key, value);
        }
        AddKey(key);
    }
    //public static void SetAndCheckInt(string key, int value)
    //{
    //    int oldValue = PlayerPrefs.GetInt(key, int.MinValue); // Default to impossible value
    //    PlayerPrefs.SetInt(key, value);
    //    if (oldValue != value)
    //    {
    //        OnIntChanged?.Invoke(key, value);
    //    }
    //    AddKey(key);
    //}
    // Get an integer value
    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    // Set a string value
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        AddKey(key);
    }

    // Get a string value
    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    // Set a float value
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        AddKey(key);
    }

    // Get a float value
    public static float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    // Check if a key exists
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    // Delete a specific key
    public static void DeleteKey(string key)
    {
        if (HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            savedKeys.Remove(key);
        }
    }

    // Delete all PlayerPrefs
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        savedKeys.Clear();
    }

    // Save PlayerPrefs to disk
    public static void Save()
    {
        PlayerPrefs.Save();
    }

    // Log all PlayerPrefs
    public static void LogAllPlayerPrefs()
    {
        Debug.Log("Listing all PlayerPrefs:");
        foreach (var key in savedKeys)
        {
            if (HasKey(key))
            {
                Debug.Log($"{key}: {GetString(key, GetInt(key, 0).ToString())}");
            }
        }
    }

    // Add a key to the tracking list
    private static void AddKey(string key)
    {
        if (!savedKeys.Contains(key))
        {
            savedKeys.Add(key);
        }
    }
}
