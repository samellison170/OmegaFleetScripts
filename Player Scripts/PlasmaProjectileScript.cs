using System.Collections;
using UnityEngine;

public class PlasmaProjectileScript : MonoBehaviour
{
    public float speed = 20f;
    public float range = 50f; // Maximum distance before the projectile is destroyed
    public float damage = 1;
    public GameObject hitEffectPrefab;
    public bool isPaused;

    private Vector3 spawnPosition;

    void Start()
    {
        // Store the spawn position
        spawnPosition = transform.position;
    }

    void Update()
    {
        // Move the projectile forward in a straight line
        if (!isPaused)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Check the distance traveled from the spawn position
            if (Vector3.Distance(spawnPosition, transform.position) >= range)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                AudioSource audio = hitEffect.GetComponent<AudioSource>();
                audio.volume = PlayerPrefsManager.GetFloat("Volume Level");
                Destroy(hitEffect, 2f);
                damageable.Damage(damage);
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Power Up Field"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(damage);
            }
        }
    }
}
