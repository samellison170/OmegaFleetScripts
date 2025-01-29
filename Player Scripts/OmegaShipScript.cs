using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OmegaShipScript : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private ShipStats shipStats;
    [SerializeField] private Image shieldHealthBarImage;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text shieldTextDisplay;
    public float maxHealth;
    public float maxShield;
    public float baseMaxShield = 100;
    public float shieldRechargeDelay = 5f;
    public float shieldRechargeRate = 10f;

    private float health;
    private float shield;
    private bool shieldOn = true;
    private bool isRecharging = false;

    public float damage;
    public float fireRate;
    public float projectileSpeed;
    public float range;

    public bool isDead = false;
    public bool isPaused = false;

    public ParticleSystem shieldParticleSystem;
    public ParticleSystem shieldParticleSystemFlash;
    public ParticleSystem deathParticleSystem;
    private TurretScript[] turrets;

    private Coroutine rechargeCoroutine;

    private OmegaFleetSceneManager sceneManager;
    private FleetManager fleetManager;

    void Start()
    {
        maxHealth = PlayerPrefsManager.GetFloat("OmegaShipMaxHealth");
        maxShield = PlayerPrefsManager.GetFloat("OmegaShipMaxShield");
        if (PlayerPrefsManager.GetFloat("OmegaShipMaxHealth") <= 0)
        {
            Debug.Log("OmegaShipMaxHealth NOT SET!!!!");
            maxHealth = 100;
        }
        if (PlayerPrefsManager.GetFloat("OmegaShipMaxShield") <= 0)
        {
            Debug.Log("OmegaShipMaxShield NOT SET!!!!");
            maxHealth = 50;
        }
        PlayerPrefsManager.SetInt("FirstTimePlaying", 0);
        health = maxHealth;
        shield = maxShield;
        shieldTextDisplay.text = $"Shield: {shield}";
        Debug.Log("OmegaShipMaxShield PlayerPrefs = " + PlayerPrefsManager.GetFloat("OmegaShipMaxShield"));
        Debug.Log("omega ship shield health = " + shield);
        turrets = GetComponentsInChildren<TurretScript>();

        fleetManager = FindObjectOfType<FleetManager>();
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
        SaveOmegaShipStats();
    }

    public void Damage(float damage)
    {
        if (shieldOn && shield > 0)
        {
            shield -= damage;
            UpdateShieldHealthBar();
            DamagePopupScript.Create(transform.position, damage, "Shield");

            if (shield <= 0)
            {
                float excessDamage = Mathf.Abs(shield);
                shield = 0;
                shieldParticleSystem.Stop();
                UpdateShieldHealthBar();
                shieldOn = false;

                health -= excessDamage;
                UpdatedHealthBar();
                DamagePopupScript.Create(transform.position, excessDamage, "Health");

                if (health <= 0)
                {
                    health = 0;
                    UpdatedHealthBar();
                    PlayDeathSequence();
                }
            }

            if (shieldOn)
            {
                StartCoroutine(PausableCoroutine(FlashShield()));
            }

            ResetShieldRechargeCoroutine();
        }
        else
        {
            health -= damage;
            UpdatedHealthBar();
            DamagePopupScript.Create(transform.position, damage, "Health");

            if (health <= 0)
            {
                health = 0;
                UpdatedHealthBar();
                PlayDeathSequence();
            }

            ResetShieldRechargeCoroutine();
        }
    }

    private void PlayDeathSequence()
    {
        deathParticleSystem.transform.SetParent(null);
        deathParticleSystem.Play();
        Death();
        sceneManager = FindObjectOfType<OmegaFleetSceneManager>();
        sceneManager.DeathScene();
    }

    private void ResetShieldRechargeCoroutine()
    {
        if (rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
        }
        rechargeCoroutine = StartCoroutine(PausableCoroutine(WaitToRechargeShield()));
    }

    private IEnumerator FlashShield()
    {
        shieldParticleSystemFlash.Play();
        yield return new WaitForSeconds(0.1f);
        shieldParticleSystemFlash.Stop();
        yield return new WaitUntil(() => !shieldParticleSystemFlash.IsAlive());
    }

    private IEnumerator WaitToRechargeShield()
    {
        if (!isDead)
        {
            isRecharging = false;
            yield return new WaitForSeconds(shieldRechargeDelay);
            rechargeCoroutine = StartCoroutine(PausableCoroutine(RechargeShield()));
        }
    }

    private IEnumerator RechargeShield()
    {
        if (!isDead)
        {
            shieldOn = true;
            shieldParticleSystem.Play();
            isRecharging = true;
            while (shield < maxShield)
            {
                shield += shieldRechargeRate * Time.deltaTime;
                UpdateShieldHealthBar();
                if (shield > maxShield)
                {
                    shield = maxShield;
                    UpdateShieldHealthBar();
                }
                yield return null;
            }

            isRecharging = false;
        }
    }

    private IEnumerator PausableCoroutine(IEnumerator coroutine)
    {
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
            while (isPaused)
            {
                yield return null;
            }
        }
    }

    public void UpdatedHealthBar()
    {
        healthBarImage.fillAmount = health / maxHealth;
    }

    public void UpdateShieldHealthBar()
    {
        shieldTextDisplay.text = $"Shield: {shield}";
        shieldHealthBarImage.fillAmount = shield / maxShield;
    }

    public void Death()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        FleetManager fleetManagerScript = GetComponentInParent<FleetManager>();
        if (fleetManagerScript != null)
        {
            fleetManagerScript.omegaShipIsDead = true;
            fleetManagerScript.DestroyAllChildShips();
        }

        isDead = true;
    }

    public void SaveOmegaShipStats()
    {
        PlayerPrefsManager.SetFloat("OmegaShipMaxHealth", maxHealth);
        PlayerPrefsManager.SetFloat("OmegaShipMaxShield", maxShield);
        PlayerPrefsManager.SetFloat("OmegaShipEnegyCurrency", fleetManager.energyCurrencyAmount);
    }
}
