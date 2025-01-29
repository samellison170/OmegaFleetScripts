using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLevelData : MonoBehaviour
{
    public string fileName = "LevelData.json";
    public List<GameObject> enemyPrefabs;        // Assign enemy prefabs in the inspector
    public List<GameObject> powerUpPrefabs;      // Assign power-up prefabs in the inspector
    public List<GameObject> powerUpFieldPrefabs; // Assign power-up field prefabs in the inspector
    public bool willSaveOnStart = false;
    public bool willLoadOnStart = false;

    private void Start()
    {
        if (willSaveOnStart)
        {
            SaveLevelDataToFile();
        }
        if (willLoadOnStart)
        {
            fileName = $"Level{PlayerPrefsManager.GetInt("CurrentLevel")}Data";
            Debug.Log(fileName);
            LoadLevelDataFromFile();
        }
    }

    // Save all objects with specified tags to a file
    public void SaveLevelDataToFile()
    {
        ObjectDataList dataList = new ObjectDataList();

        // Save enemies
        SaveObjectsWithTag("Enemy", enemyPrefabs, dataList.enemies);

        // Save power-ups
        SaveObjectsWithTag("PowerUp", powerUpPrefabs, dataList.powerUps);

        // Save power-up fields
        SaveObjectsWithTag("Power Up Field", powerUpFieldPrefabs, dataList.powerUpFields);

        // Convert to JSON and save to file
        string json = JsonUtility.ToJson(dataList, true);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(filePath, json);

        Debug.Log($"Level data saved to: {filePath}");
    }

    // Helper function to save objects by tag
    private void SaveObjectsWithTag(string tag, List<GameObject> prefabList, List<ObjectData> dataList)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            // Save EnemyStats
            EnemyStats enemyStats = obj.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                Vector3 position = obj.transform.position;
                Quaternion rotation = obj.transform.rotation;
                string prefabType = enemyStats.prefabType;
                //Debug.Log("prefab type = " + prefabType);
                //Debug.Log("enemy stats = " + enemyStats.prefabType);
                //Dev Note Start Here 12/21/2024
                float health = enemyStats.maxHealth;
                float damage = enemyStats.damage;
                float spawnedEnemyDamage = enemyStats.spawnedEnemyDamage;
                float spawnedEnemyMaxHealth = enemyStats.spawnedEnemyMaxHealth;

                dataList.Add(new ObjectData(prefabType, position, rotation, health, damage, 0, 0,0,0, spawnedEnemyDamage, spawnedEnemyMaxHealth));
            }

            // Save ReinforcementFieldStats
            ReinforcementFieldStats reinforcementStats = obj.GetComponent<ReinforcementFieldStats>();
            if (reinforcementStats != null)
            {
                Vector3 position = obj.transform.position;
                Quaternion rotation = obj.transform.rotation;
                string prefabType = reinforcementStats.prefabType;
                int shipValue = reinforcementStats.shipValue;
                float upgradeThreshold = reinforcementStats.upgradeThreshold;

                dataList.Add(new ObjectData(prefabType, position, rotation, 0, 0, shipValue, upgradeThreshold, 0, 0, 0, 0));
            }

            // Save UpgradeFieldStats
            UpgradeFieldStats upgradeStats = obj.GetComponent<UpgradeFieldStats>();
            if (upgradeStats != null)
            {
                Vector3 position = obj.transform.position;
                Quaternion rotation = obj.transform.rotation;
                string prefabType = upgradeStats.prefabType;
                int percentageUpgradeValue = upgradeStats.percentageUpgradeValue;
                float upgradeThreshold = upgradeStats.upgradeThreshold;

                dataList.Add(new ObjectData(prefabType, position, rotation, 0, 0, 0, upgradeThreshold, percentageUpgradeValue, 0, 0, 0));
            }
            // Save PowerUpStats
            PowerUpStats powerUpStats = obj.GetComponent<PowerUpStats>();
            if (powerUpStats != null)
            {
                Vector3 position = obj.transform.position;
                Quaternion rotation = obj.transform.rotation;
                string prefabType = powerUpStats.prefabType;
                int powerUpValue = powerUpStats.powerUpValue;

                dataList.Add(new ObjectData(prefabType, position, rotation, 0, 0, 0, 0, 0, powerUpValue, 0, 0));
            }
        }
    }

    // Load all objects with specified tags from a file
    public void LoadLevelDataFromFile()
    {
        // Load the TextAsset from Resources
        TextAsset jsonFile = Resources.Load<TextAsset>("LevelData/" + fileName);
        Debug.Log(fileName);
        if (jsonFile != null)
        {
            // Convert TextAsset to string
            string json = jsonFile.text;

            // Deserialize the JSON into ObjectDataList
            ObjectDataList dataList = JsonUtility.FromJson<ObjectDataList>(json);

            // Load enemies
            LoadObjectsFromData(dataList.enemies, enemyPrefabs, "Enemy");

            // Load power-ups
            LoadObjectsFromData(dataList.powerUps, powerUpPrefabs, "PowerUp");

            // Load power-up fields
            LoadObjectsFromData(dataList.powerUpFields, powerUpFieldPrefabs, "Power Up Field");

            //Debug.Log("Level data loaded successfully.");
        }
        else
        {
            Debug.LogWarning($"JSON file not found in Resources/LevelData/{fileName}");
        }
    }
    //test function
    // Load all objects with specified tags from a file
    ////public void LoadLevelDataFromFile()
    ////{
    ////    TextAsset jsonFile = Resources.Load<TextAsset>("LevelData/" + fileName);

    ////    if (jsonFile != null)
    ////    {
    ////        // Convert TextAsset to string
    ////        string json = jsonFile.text;

    ////        ObjectDataList dataList = JsonUtility.FromJson<ObjectDataList>(json);

    ////        // Load enemies
    ////        LoadObjectsFromData(dataList.enemies, enemyPrefabs, "Enemy");

    ////        // Load power-ups
    ////        LoadObjectsFromData(dataList.powerUps, powerUpPrefabs, "PowerUp");

    ////        // Load power-up fields
    ////        LoadObjectsFromData(dataList.powerUpFields, powerUpFieldPrefabs, "Power Up Field");

    ////        //Debug.Log("Level data loaded successfully.");
    ////    }
    ////    else
    ////    {
    ////        Debug.LogWarning($"File not found: {jsonFile}");
    ////    }
    ////}
    // Helper function to load objects from data
    private void LoadObjectsFromData(List<ObjectData> dataList, List<GameObject> prefabList, string tag)
    {
        foreach (ObjectData data in dataList)
        {
            Vector3 position = data.ToVector3();
            Quaternion rotation = data.ToQuaternion();
            GameObject prefab = prefabList.Find(p => p.name == data.prefabType);

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, position, rotation);
                obj.tag = tag;

                // Apply to EnemyStats
                EnemyStats enemyStats = obj.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.maxHealth = data.maxHealth;
                    enemyStats.damage = data.damage;
                    enemyStats.spawnedEnemyDamage = data.spawnedEnemyDamage;
                    enemyStats.spawnedEnemyMaxHealth = data.spawnedEnemyMaxHealth;
                }

                // Apply to ReinforcementFieldStats
                ReinforcementFieldStats reinforcementStats = obj.GetComponent<ReinforcementFieldStats>();
                if (reinforcementStats != null)
                {
                    reinforcementStats.shipValue = data.shipValue;
                    reinforcementStats.upgradeThreshold = data.upgradeThreshold;
                }

                // Apply to UpgradeFieldStats
                UpgradeFieldStats upgradeStats = obj.GetComponent<UpgradeFieldStats>();
                if (upgradeStats != null)
                {
                    upgradeStats.percentageUpgradeValue = data.percentageUpgradeValue;
                    upgradeStats.upgradeThreshold = data.upgradeThreshold;
                }
                PowerUpStats powerUpStats = obj.GetComponent<PowerUpStats>();
                if (powerUpStats != null)
                {
                    powerUpStats.powerUpValue = data.powerUpValue;
                }
            }
            else
            {
                Debug.LogWarning($"Prefab not found for type: {data.prefabType}");
            }
        }
    }

    // Data structure for saving object information
    [System.Serializable]
    public class ObjectData
    {
        public string prefabType;
        public float x, y, z;
        public float rotX, rotY, rotZ, rotW;
        public float maxHealth, damage;
        public int shipValue;
        public float upgradeThreshold;
        public int percentageUpgradeValue;
        public int powerUpValue;
        public float spawnedEnemyDamage;
        public float spawnedEnemyMaxHealth;

        public ObjectData(
            string prefabType, Vector3 position, Quaternion rotation,
            float maxHealth = 0, float damage = 0,
            int shipValue = 0, float upgradeThreshold = 0,
            int percentageUpgradeValue = 0, int powerUpValue = 0, float spawnedEnemyDamage = 0, float spawnedEnemyMaxHealth = 0)
        {
            this.prefabType = prefabType;
            x = position.x;
            y = position.y;
            z = position.z;
            rotX = rotation.x;
            rotY = rotation.y;
            rotZ = rotation.z;
            rotW = rotation.w;
            this.maxHealth = maxHealth;
            this.damage = damage;
            this.shipValue = shipValue;
            this.upgradeThreshold = upgradeThreshold;
            this.percentageUpgradeValue = percentageUpgradeValue;
            this.powerUpValue = powerUpValue;
            this.spawnedEnemyDamage = spawnedEnemyDamage;
            this.spawnedEnemyMaxHealth = spawnedEnemyMaxHealth;
        }

        public Vector3 ToVector3() => new Vector3(x, y, z);
        public Quaternion ToQuaternion() => new Quaternion(rotX, rotY, rotZ, rotW);
    }

    // Data structure for saving lists of objects
    [System.Serializable]
    public class ObjectDataList
    {
        public List<ObjectData> enemies = new List<ObjectData>();
        public List<ObjectData> powerUps = new List<ObjectData>();
        public List<ObjectData> powerUpFields = new List<ObjectData>();
    }
}
