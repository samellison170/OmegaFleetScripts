using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PortalCubeShipManager : MonoBehaviour, IDamageable
{
    [SerializeField] private Image healthBar;
    public GameObject prefabToSpawn;   // The prefab you want to spawn
    public int numberOfPrefabs = 5;    // Number of prefabs to spawn in each wave
    public float spawnInterval = 0.2f; // Time between each spawn in a wave
    public float delayBeforeStart = 3f; // Delay before the first wave starts
    public float delayBetweenWaves = 5f; // Delay between each wave
    public ParticleSystem chargeParticleEffect;
    public ParticleSystem portalParticleEffect;
    public ParticleSystem deathEffect;
    public GameObject drop;
    public int value;
    public bool isPaused;

    //Enemy Stats Section
    public float damage = 100;
    public float maxHealth = 10;
    private float health;
    public float spawnedEnemyDamage;
    public float spawnedEnemyMaxHealth;

    // Internal state variables
    private float spawnTimer = 0f;
    private float pauseSpawnTimer;
    private float waveDelayTimer = 0f;
    private float pauseWaveDelayTimer;
    private int currentPrefabIndex = 0;
    private bool isSpawning = false;

    void Start()
    {
        health = maxHealth;
        // Initialize timers
        spawnTimer = delayBeforeStart; // Start the delay before the first wave
        chargeParticleEffect.Stop();
        portalParticleEffect.Stop();
        if (SceneManager.GetActiveScene().name != "Load Scene")
        {
            AssignEnemyStats();
        }
        if (SceneManager.GetActiveScene().name == "Load Scene")
        {
            RetrieveEnemyStats();
        }
    }
    //this function assigns the stats to the EnemyStats script for saving and loading the levels, needs to be in every enemies script
    public void AssignEnemyStats()
    {
        EnemyStats enemyStats = gameObject.GetComponent<EnemyStats>();
        enemyStats.prefabType = "PortalCubeShip";
        enemyStats.damage = damage;
        enemyStats.maxHealth = maxHealth;
    }
    public void RetrieveEnemyStats()
    {
        EnemyStats enemyStats = gameObject.GetComponent<EnemyStats>();
        damage = enemyStats.damage;
        maxHealth = enemyStats.maxHealth;
    }
    void Update()
    {
        //Debug.Log("Spawn timer = " + spawnTimer + ": Wave Timer = " + waveDelayTimer);
        if (health <= 0)
        {
            Death();
        }

        // Handle health bar visibility
        if (health == maxHealth)
        {
            healthBar.gameObject.SetActive(false);
            healthBar.transform.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
            healthBar.transform.parent.parent.gameObject.SetActive(true);
            healthBar.fillAmount = health / maxHealth;
        }

        // Handle the spawning logic
        if (!isPaused)
        {
            if (!isSpawning)
            {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0)
                {
                    StartWave();
                }
            }
            else
            {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0 && currentPrefabIndex < numberOfPrefabs)
                {
                    SpawnPrefab();
                }
                else if (currentPrefabIndex >= numberOfPrefabs)
                {
                    waveDelayTimer -= Time.deltaTime;

                    if (waveDelayTimer <= 0)
                    {
                        StartNewWave();
                    }
                }
            }
        }
        else
        {
            if (chargeParticleEffect.isPlaying) chargeParticleEffect.Pause();
            if (portalParticleEffect.isPlaying) portalParticleEffect.Pause();
        }
    }

    void StartWave()
    {
        // Start the first wave
        chargeParticleEffect.Play();
        portalParticleEffect.Play();

        spawnTimer = spawnInterval; // Set the timer for the interval between spawns
        isSpawning = true; // Start the spawning process
    }

    void SpawnPrefab()
    {
        // Spawn the prefab
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward) * Quaternion.Euler(0f, 180f, 180f);
        GameObject spawnedPrefab = Instantiate(prefabToSpawn, transform.position, rotation);

        // Ensure the spawned prefab has a HiveFleetAI component
        HiveFleetAI hiveFleetAI = spawnedPrefab.GetComponent<HiveFleetAI>();
        EnemyStats enemyStats = gameObject.GetComponent<EnemyStats>();
        EnemyStats spawnedEnemyStats = spawnedPrefab.GetComponent<EnemyStats>();
        spawnedEnemyStats.damage = enemyStats.spawnedEnemyDamage;
        spawnedEnemyStats.maxHealth = enemyStats.spawnedEnemyMaxHealth;
        Slider tmpSlider = spawnedPrefab.GetComponentInChildren<Slider>();
        //Debug.Log(tmpSlider.name);
        hiveFleetAI.damage = enemyStats.spawnedEnemyDamage;
        hiveFleetAI.maxHealth = enemyStats.spawnedEnemyMaxHealth;
        //Debug.Log(hiveFleetAI.damage + " " + hiveFleetAI.maxHealth);
        tmpSlider.enabled = false;
        //Image[] tempHealthBars = spawnedPrefab.gameObject.GetComponentsInChildren<Image>();
        //foreach(Image tempHealthBar in tempHealthBars)
        //{
        //    if (tempHealthBar.name == "Fill")
        //    {
        //        Debug.Log(tempHealthBar.name);
        //        tempHealthBar.enabled = false;
        //    }
        //}
        //hiveFleetAI.healthBar.fillAmount = .5f;

        // Reset the timer for the next spawn
        spawnTimer = spawnInterval;
        currentPrefabIndex++;
    }


    void StartNewWave()
    {
        // Reset for the next wave
        currentPrefabIndex = 0;
        waveDelayTimer = delayBetweenWaves; // Set the timer for the next wave delay
        isSpawning = false; // Stop the spawning until the next wave starts
    }

    public void Death()
    {
        deathEffect.Play();
        deathEffect.transform.SetParent(null);
        GameObject tempGameObject = Instantiate(drop, transform.position, Quaternion.identity);
        PowerUpStats tempPowerUpStats = tempGameObject.GetComponent<PowerUpStats>();
        PowerUpScript tempPowerUpScript = tempGameObject.GetComponent<PowerUpScript>();
        if (tempPowerUpStats != null)
        {
            tempPowerUpStats.powerUpValue = value;
            //Debug.Log("tempPowerUpStats = " + tempPowerUpStats.powerUpValue);
        }
        else
        {
            Debug.Log("tempPowerUpStats is Null");
        }
        if (tempPowerUpScript != null)
        {
            tempPowerUpScript.powerUpValue = value;
            //Debug.Log("tempPowerUpScript = " + tempPowerUpScript.powerUpValue);
        }
        else
        {
            Debug.Log("tempPowerUpScript is Null");
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with " + other);
        // Check if the collided object has the "Enemy" tag
        if (other.CompareTag("Player"))
        {
            //Debug.Log("collided with " + other);
            // Try to get the IDamageable component from the object
            IDamageable damageable = other.GetComponent<IDamageable>();

            // If the object implements IDamageable, apply damage
            if (damageable != null)
            {
                //Debug.Log("boom got em");
                // Apply damage to the enemy
                damageable.Damage(damage);

                Death();
            }
        }
    }
    public void Damage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / maxHealth;
        DamagePopupScript.Create(gameObject.transform.position, damage, "Enemy");
    }
}
