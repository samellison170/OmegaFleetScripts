using UnityEngine;

[CreateAssetMenu(fileName = "NewShipStats", menuName = "Stats/ShipStats")]
public class ShipStats : ScriptableObject
{
    public float maxHealth = 100f;
    public float maxShield = 50f;
    public float shieldRechargeDelay = 5f;
    public float shieldRechargeRate = 10f;
}
