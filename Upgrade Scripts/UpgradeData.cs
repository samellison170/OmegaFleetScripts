using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeData", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public int currentLevel;
    public int maxLevel;
    public float baseCost;
    public float costMultiplier;  // How much the cost increases per level
    public float statIncreasePerLevel;

    // Calculate current cost
    public float GetUpgradeCost()
    {
        return baseCost * Mathf.Pow(costMultiplier, currentLevel + 1);
    }
    public float GetDowngradeCost()
    {
        return baseCost * Mathf.Pow(costMultiplier, currentLevel);
    }
    // Calculate the value added to the stat
    public float GetStatIncrease()
    {
        if (currentLevel == maxLevel)
        {
            return statIncreasePerLevel * currentLevel;
        }
        Debug.Log("stat increase = " + statIncreasePerLevel * (currentLevel + 1));
        Debug.Log("current upgrade level = " + currentLevel);
        return statIncreasePerLevel * (currentLevel + 1);

    }
    public float GetCurrentStats()
    {
        return statIncreasePerLevel * currentLevel;
    }
    public float GetStatDecrease()
    {
        if (currentLevel == 1)
        {
            return statIncreasePerLevel * currentLevel;
        }
        Debug.Log("stat decrease = " +  statIncreasePerLevel * (currentLevel));
        return statIncreasePerLevel * (currentLevel);
    }
    // Level up the upgrade
    public bool Upgrade()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            return true;
        }
        return false;
    }
    public bool Downgrade()
    {
        if (currentLevel > 0)
        {
            currentLevel--;
            return true;
        }
        return false;
    }
}
