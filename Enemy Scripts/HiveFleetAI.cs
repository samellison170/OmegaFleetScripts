using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HiveFleetAI : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed = 10f;  // Forward speed
    [SerializeField] private float strafeSpeed = 5f;  // Speed for x-axis movement

    [SerializeField] private float detectionRange = 10f;  // The range within which the enemy starts following the player
    public Image healthBar;

    private Transform player;  // Reference to the player object
    public float maxHealth = 10;
    public float damage = 1;
    private float health;
    public int value;
    private OmegaFleetSceneManager sceneManagerScript;
    private float levelBoundary;  // The maximum distance the enemy can move left or right
    private ParticleSystem deathEffect;
    private bool isFollowingPlayer = false;  // Whether the enemy is currently following the player
    private bool movingRight = true;  // Whether the enemy is currently moving right
    //private bool healthBarHidden = false;
    public GameObject targetBeacon;
    public GameObject drop;
    public bool isPaused;

    void Start()
    {
        isPaused = false;
        // Ensure player reference is set
        if (sceneManagerScript == null)
        {
            sceneManagerScript = GameObject.FindWithTag("CustomSceneManager").GetComponent<OmegaFleetSceneManager>();
        }
        deathEffect = GetComponentInChildren<ParticleSystem>();
        levelBoundary = sceneManagerScript.levelBoundary;
        health = maxHealth;
        targetBeacon.SetActive(false);
        // Ensure player reference is set
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        if (healthBar == null)
        {
            Damage(1);
        }
        if (health == maxHealth)
        {
            healthBar.gameObject.SetActive(false);
            //healthBar.transform.parent.parent.gameObject.SetActive(false);
            //healthBarHidden = true; // Set flag to prevent repeated activation/deactivation
            //Debug.Log("health bar is attached");
        }
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
        enemyStats.prefabType = "DroneShip";
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
        if (!isPaused)
        {
            // Constant forward movement
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the player is within the detection range
            if (distanceToPlayer <= detectionRange)
            {
                isFollowingPlayer = true;
            }

            if (isFollowingPlayer)
            {
                targetBeacon.SetActive(true);
                // Move toward the player's x position at a constant speed
                float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
                Vector3 strafeDirection = Vector3.right * directionToPlayer * strafeSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + strafeDirection;

                // Constrain the enemy's x position within the level boundaries
                newPosition.x = Mathf.Clamp(newPosition.x, -levelBoundary, levelBoundary);

                // Apply the constrained position
                transform.position = newPosition;
            }
            else
            {
                // Move back and forth between boundaries
                if (movingRight)
                {
                    transform.Translate(Vector3.right * strafeSpeed * Time.deltaTime);

                    // Check if the enemy has reached the right boundary
                    if (transform.position.x >= levelBoundary)
                    {
                        movingRight = false;
                    }
                }
                else
                {
                    transform.Translate(Vector3.left * strafeSpeed * Time.deltaTime);

                    // Check if the enemy has reached the left boundary
                    if (transform.position.x <= -levelBoundary)
                    {
                        movingRight = true;
                    }
                }
            }
            // Only hide the health bar once when it's full
            if (health == maxHealth)
            {
                healthBar.gameObject.SetActive(false);
                healthBar.transform.parent.parent.gameObject.SetActive(false);
                //healthBarHidden = true; // Set flag to prevent repeated activation/deactivation
            }
            else
            {
                // Show the health bar again if it's not full and was previously hidden
                healthBar.gameObject.SetActive(true);
                healthBar.transform.parent.parent.gameObject.SetActive(true);
                healthBar.fillAmount = health / maxHealth;
                //healthBarHidden = false; // Reset the flag
            }
            if (health <= 0)
            {
                Death();
            }
        }
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
        //Debug.Log("hit for " + damage + " current health at " + health + " out of " + maxHealth);
        healthBar.fillAmount = health / maxHealth;
        DamagePopupScript.Create(gameObject.transform.position, damage, "Enemy");
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
        
        //Debug.Log("DEATH power up should be working");
        Destroy(gameObject);
    }
}
