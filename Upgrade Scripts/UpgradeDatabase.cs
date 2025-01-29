using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Upgrades/UpgradeDatabase")]
public class UpgradeDatabase : ScriptableObject
{
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    public UpgradeData GetUpgradeByName(string name)
    {
        //foreach (UpgradeData upgrade in upgrades)
        //{
        //    Debug.Log(upgrade.name);
        //}
        return upgrades.Find(upgrade => upgrade.upgradeName == name);
    }
    public void CheckUpgradeByName()
    {
        foreach (UpgradeData upgrade in upgrades)
        {
            Debug.Log(upgrade.name);
        }
        //return upgrades.Find(upgrade => upgrade.upgradeName == name);
    }
}
