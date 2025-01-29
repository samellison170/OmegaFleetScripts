using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public float startDelay = 2f;  // Custom delay before the turret starts firing

    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float fireRate = 1f;
    public float damage;
    public float projectileSpeed;
    public float range;
    public bool shipIsDead = false;
    public bool isShooting = true;
    public bool isPaused = false;

    private OmegaShipScript omegaShip;
    private ParticleSystem plasmaMuzzleFlashEffect;
    private AudioSource plasmaSoundEffect;
    private float timeSinceStart;
    private float lastFireTime;
    private bool firingStarted = false;

    private void Start()
    {
        omegaShip = GameObject.Find("OmegaShip").GetComponent<OmegaShipScript>();
        plasmaMuzzleFlashEffect = GetComponentInChildren<ParticleSystem>();
        plasmaSoundEffect = GetComponentInChildren<AudioSource>();
        fireRate = omegaShip.fireRate;
        damage = omegaShip.damage;
        projectileSpeed = omegaShip.projectileSpeed;
        range = omegaShip.range;

        timeSinceStart = 0f;
        lastFireTime = -1f / fireRate;  // Ensures firing can start immediately after delay if needed
        plasmaSoundEffect.volume = PlayerPrefsManager.GetFloat("Volume Level");
    }

    private void Update()
    {
        if (!isPaused)
        {
            if (!firingStarted)
            {
                timeSinceStart += Time.deltaTime;
                if (timeSinceStart >= startDelay)
                {
                    firingStarted = true;
                    lastFireTime = Time.time;
                }
            }
            else if (isShooting && !isPaused && !shipIsDead)
            {
                if (Time.time - lastFireTime >= 1f / fireRate)
                {
                    Shoot();
                    lastFireTime = Time.time;
                }
            }
            shipIsDead = omegaShip.isDead;  // Update status if ship is dead
        }


    }

    void Shoot()
    {
        // Instantiate the projectile at the shoot point
        plasmaSoundEffect.Play();
        plasmaMuzzleFlashEffect.Play();
        GameObject instance = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        PlasmaProjectileScript plasmaProjectile = instance.GetComponent<PlasmaProjectileScript>();
        plasmaProjectile.speed = projectileSpeed;
        plasmaProjectile.damage = damage;
        plasmaProjectile.range = range;
    }
}
