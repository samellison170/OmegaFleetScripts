using System.Collections;
using UnityEngine;


public class AlphaShipScript : MonoBehaviour, IDamageable
{
    public GameObject shipPosition = null;  // Assigned by another script after instantiation
    public int shipNumber;
    private GameObject waypoint;
    public float speed = 5f;
    private bool reachedWaypoint = false;
    private float damage;
    public float maxHealth = 100;
    public float maxShield = 50;
    public int fleetPosition;
    public bool isPaused = false;
    private float health;
    private float shield;
    private TurretScript[] turrets;
    private Coroutine rechargeCoroutine;
    private float shieldRechargeDelay = 5f;  // Delay before shield starts recharging after taking damage
    private float shieldRechargeRate = 10f;  // Amount of shield recharged per second

    private bool shieldOn = true;
    private bool isRecharging = false;

    private float fireRate;
    private float projectileSpeed;
    private float projectileLifetime;

    public ParticleSystem shieldParticleSystem;
    public ParticleSystem shieldParticleSystemFlash;
    public ParticleSystem deathParticleSystem;
    private FleetManager fleetManager;


    private void Start()
    {
        fleetManager = GameObject.FindGameObjectWithTag("Fleet").GetComponent<FleetManager>();
        health = maxHealth;
        shield = maxShield;
        waypoint = GameObject.FindGameObjectWithTag("Waypoint");
        //Debug.Log("ship waypoint = " + waypoint.name);

        turrets = GetComponentsInChildren<TurretScript>();

        // Access and set a variable in each TurretScript
        foreach (TurretScript script in turrets)
        {
            script.damage = damage;
        }
    }
    public void EndLevel()
    {
        foreach (TurretScript script in turrets)
        {
            script.isShooting = false;
        }
    }
    void Update()
    {
        if (!isPaused)
        {
            Move();
        }
    }

    public void Move()
    {

            if (!reachedWaypoint && waypoint != null)
            {
                // Move towards the waypoint
                transform.position = Vector3.MoveTowards(transform.position, waypoint.transform.position, speed * Time.deltaTime);

                // Check if the object has reached the waypoint
                if (Vector3.Distance(transform.position, waypoint.transform.position) < 0.1f)
                {
                    reachedWaypoint = true;
                }
            }
            else if (reachedWaypoint && shipPosition != null)
            {
                // Move towards the shipPosition if it has been assigned
                transform.position = Vector3.MoveTowards(transform.position, shipPosition.transform.position, speed * Time.deltaTime);

                // Optionally, you can check if it has reached the shipPosition and stop the movement
                if (Vector3.Distance(transform.position, shipPosition.transform.position) < 0.1f)
                {
                    // You can trigger some action when the object reaches the ship position
                    //Debug.Log("Reached ship position!");
                }
            }
    }
    public void OmegaDeath()
    {
        fleetManager.SupplementalShipDeath(fleetPosition);
        Destroy(gameObject);
    }
    public void Damage(float damage)
    {
        if (shieldOn && shield > 0)
        {
            shield -= damage;


            fleetManager.UpdateSupplementalShipShieldBar(fleetPosition, shield, maxShield);
            if (shield <= 0)
            {
                shield = 0;
                shieldParticleSystem.Stop();
                fleetManager.UpdateSupplementalShipShieldBar(fleetPosition, shield, maxShield);
                shieldOn = false;
            }
            if (shieldOn)
            {
                Debug.Log(shieldOn);
                StartCoroutine(FlashShield());
            }
            //Debug.Log(rechargeCoroutine);
            // Reset the shield recharge delay timer
            if (rechargeCoroutine != null)
            {
                //Debug.Log("stopping co-routine");
                StopCoroutine(rechargeCoroutine);
            }
            rechargeCoroutine = StartCoroutine(WaitToRechargeShield());
        }
        else
        {
            health -= damage;
            fleetManager.UpdateSupplementalShipHealthBar(fleetPosition, health, maxHealth);
            // Reset the shield recharge delay timer
            if (rechargeCoroutine != null)
            {
                //Debug.Log("stopping co-routine");
                StopCoroutine(rechargeCoroutine);
            }
            rechargeCoroutine = StartCoroutine(WaitToRechargeShield());

            if (health <= 0)
            {
                health = 0;
                fleetManager.UpdateSupplementalShipHealthBar(fleetPosition, health, maxHealth);
                // Play death particle system
                deathParticleSystem.transform.SetParent(null);
                deathParticleSystem.Play();
                OmegaDeath();
            }
        }
    }
    private IEnumerator FlashShield()
    {
        // Activate the flash particle system
        shieldParticleSystemFlash.Play();

        // Wait for a few frames (e.g., 0.1 seconds)
        yield return new WaitForSeconds(0.1f); // Adjust the duration as needed

        // Deactivate the flash particle system
        shieldParticleSystemFlash.Stop();

        // Optionally, wait until the particles are fully stopped
        yield return new WaitUntil(() => !shieldParticleSystemFlash.IsAlive());
    }

    private IEnumerator WaitToRechargeShield()
    {
        isRecharging = false;
        yield return new WaitForSeconds(shieldRechargeDelay);
        rechargeCoroutine = StartCoroutine(RechargeShield());
    }

    private IEnumerator RechargeShield()
    {
        shieldOn = true;
        shieldParticleSystem.Play();
        isRecharging = true;
        while (shield < maxShield)
        {
            shield += shieldRechargeRate * Time.deltaTime;
            fleetManager.UpdateSupplementalShipShieldBar(fleetPosition, shield, maxShield);
            if (shield > maxShield)
            {
                shield = maxShield;
                fleetManager.UpdateSupplementalShipShieldBar(fleetPosition, shield, maxShield);
            }
            yield return null;  // Wait for the next frame
        }


        isRecharging = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //StartCoroutine(FlashShield());
        }
    }
}
