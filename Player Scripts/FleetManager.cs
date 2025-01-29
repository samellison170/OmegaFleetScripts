using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
//using static UnityEditor.ShaderUtil;

public class FleetManager : MonoBehaviour, IPowerUpReceiver
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float strafeSpeed = 5f;  // Max speed for left/right movement
    [SerializeField] private float acceleration = 10f;  // How quickly the player accelerates to full strafeSpeed
    [SerializeField] private float levelBoundary = 5f;  // The maximum distance the player can move left or right
    [SerializeField] private float fleetFireRate = 1f;
    public float energyCurrencyAmount;
    public TMP_Text energyTextDisplay;
    public GameObject AlphaShipObject;
    public Transform SpawnPoint;
    public GameObject[] shipPositions;
    public ParticleSystem spawnParticleSystem;
    public ParticleSystem energyVacuumParticleSystem;
    public ParticleSystem endLevelPortalParticleSystem;
    public ParticleSystem portalSparksParticleSystem;
    public ParticleSystem portalExplostionParticleEffect;
    public GameObject[] supplementShipStatBars;
    public ParticleSystem energyAcquiredPowerUp;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject starObject;
    // Target star death color (the color you want to lerp to)
    public Color targetStarDeathColor1 = Color.blue;
    public Color targetStarDeathColor2 = Color.blue;
    public Color targetStarDeathColor3 = Color.blue;

    //public Image[] supplementShipShieldBars;

    // Time elapsed for Star death animation
    private float starDeathElapsedTime = 0.0f;
    // Duration of the color transition
    public float starDeathDuration = 5.0f;
    // Reference to the Renderer component
    private Renderer starRenderer;
    // Starting color (will be fetched from the shader at runtime)
    private Color starStartColor1;
    private Color starStartColor2;
    private Color starStartColor3;

    private OmegaShipScript omegaShipScript;
    private AlphaShipScript[] shipScripts;
    private TurretScript[] turretScripts;
    public FleetPositionData[] positions = new FleetPositionData[10];
    public bool isAlive;
    public bool isPaused = false;
    public bool omegaShipIsDead = false;

    private bool levelEnded; 
    public bool endLevelReached = false;
    public Transform endLevelPosition;
    public float endLevelMoveSpeed = 0.1f;
    public float endPortalSpawnDelay;

    private int numberOfShipsInFleet = 0;
    private int steerValue;
    private float currentStrafeSpeed = 0f;  // Current speed for left/right movement
    private OmegaFleetSceneManager sceneManager;
    private GameObject powerUpTemp;
    private bool endAnimationActivated = false;
    void Awake()
    {
        levelEnded = false;
        omegaShipScript = FindAnyObjectByType<OmegaShipScript>();
        sceneManager = FindObjectOfType<OmegaFleetSceneManager>();
        levelBoundary = sceneManager.levelBoundary;
        //playMode = scriptA.playMode;
        // Update the health text
        if (energyTextDisplay != null)
        {
            energyTextDisplay.text = $"{energyCurrencyAmount}";
        }
    }
    public void EndLevel()
    {
        endLevelReached = true;
        omegaShipScript.EndLevel();
        shipScripts = gameObject.GetComponentsInChildren<AlphaShipScript>();
        foreach (AlphaShipScript script in shipScripts)
        {
            script.EndLevel();
        }
    }
    void Start()
    {
        starRenderer = starObject.GetComponent<Renderer>();

        // Fetch the starting color from the shader (Layer1Color)
        starStartColor1 = starRenderer.material.GetColor("_Layer1Color");
        starStartColor2 = starRenderer.material.GetColor("_Layer2Color");
        starStartColor3 = starRenderer.material.GetColor("_OutlineColor");

        // Initialize the array with default values
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = new FleetPositionData(i, false);  // Index is i, position is not used (false)
        }
    }

    // Update is called once per frame
    void Update()
    {
        //isAlive = sceneManager.isAlive;
        //correct the PointerExit Event not triggering
        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            Steer(0);
        }
        if (isAlive && !isPaused && !omegaShipIsDead && !endLevelReached)
        {
            // Constant forward movement
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Accelerate to the target strafe speed
            if (steerValue != 0)
            {
                currentStrafeSpeed = Mathf.MoveTowards(currentStrafeSpeed, strafeSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                // Decelerate to 0 when no input
                currentStrafeSpeed = Mathf.MoveTowards(currentStrafeSpeed, 0f, acceleration * Time.deltaTime);
            }

            // Left/Right movement based on steerValue
            Vector3 strafeDirection = Vector3.right * steerValue * currentStrafeSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + strafeDirection;

            // Constrain the player's x position within the level boundaries
            newPosition.x = Mathf.Clamp(newPosition.x, -levelBoundary, levelBoundary);

            // Apply the constrained position
            transform.position = newPosition;
        }
        if (endLevelReached && !levelEnded)
        {
            // Move the object towards the target position at a constant speed
            transform.position = Vector3.MoveTowards(transform.position, endLevelPosition.position, endLevelMoveSpeed * Time.deltaTime);
            //Debug.Log("trying to move us back");
            // Check if the object has reached the endLevelPosition
            if (Vector3.Distance(transform.position, endLevelPosition.position) <= 0.5f)
            {
                //Debug.Log("End Position Reached!");
                if (energyVacuumParticleSystem != null)
                {
                    //Debug.Log("Particle System exists.");

                    // Check if the particle system is not already playing
                    if (!energyVacuumParticleSystem.isPlaying)
                    {
                        energyVacuumParticleSystem.Play();
                        //Debug.Log("Playing Particle System.");
                    }
                }
                if (Vector3.Distance(transform.position, endLevelPosition.position) <= 0.5f && starDeathElapsedTime < starDeathDuration)
                {
                    // Increment the elapsed time
                    starDeathElapsedTime += Time.deltaTime;

                    // Calculate the interpolation factor (goes from 0 to 1 over time)
                    float lerpFactor = starDeathElapsedTime / starDeathDuration;

                    // Ensure the lerpFactor stays between 0 and 1
                    lerpFactor = Mathf.Clamp01(lerpFactor);
                    //Debug.Log(lerpFactor);
                    // Lerp the color between startColor and targetColor
                    Color newColor1 = Color.Lerp(starStartColor1, targetStarDeathColor1, lerpFactor);
                    Color newColor2 = Color.Lerp(starStartColor2, targetStarDeathColor2, lerpFactor);
                    Color newColor3 = Color.Lerp(starStartColor3, targetStarDeathColor3, lerpFactor);

                    // Apply the new color to the material's shader property
                    starRenderer.material.SetColor("_Layer1Color", newColor1);
                    starRenderer.material.SetColor("_Layer2Color", newColor2);
                    starRenderer.material.SetColor("_OutlineColor", newColor3);
                    //Debug.Log("colors should be changing");

                }
                if(!endAnimationActivated)
                {
                    StartCoroutine(SpawnEndLevelPortalWithDelay(endPortalSpawnDelay));
                    endAnimationActivated = true;
                }
            }
        }
        if (levelEnded)
        {
            // Move the object towards the target position at a constant speed
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, endLevelMoveSpeed * 100 * Time.deltaTime);
            //Debug.Log("ships should be moving" + levelEnded);
        }


    }
    private IEnumerator SpawnEndLevelPortalWithDelay(float delay)
    {
        sceneManager.EndLevel();
        portalSparksParticleSystem.Play();

        // Wait for delay, considering pause
        for (float t = 0; t < delay; t += Time.deltaTime)
        {
            yield return WaitWhilePaused();
            yield return null; // Proceed to the next frame
        }

        if (endLevelPortalParticleSystem != null)
        {
            portalSparksParticleSystem.Stop();
            portalExplostionParticleEffect.Play();
            endLevelPortalParticleSystem.Play();

            yield return WaitWhilePaused();

            if (virtualCamera != null)
            {
                virtualCamera.Follow = null;
                virtualCamera.LookAt = null;
                levelEnded = true;
                LevelDataManager levelDataManager = FindAnyObjectByType<LevelDataManager>();
                int currentLevel = PlayerPrefsManager.GetInt("CurrentLevel");
                PlayerPrefsManager.SetInt($"Level{currentLevel}_Complete", 1);
                PlayerPrefsManager.SetInt($"Level{currentLevel + 1}_Unlocked", 1);
                Debug.Log(" Level " + currentLevel + " is " + PlayerPrefsManager.GetInt($"Level{currentLevel}_Unlocked"));
            }
            else
            {
                Debug.LogWarning("Cinemachine Virtual Camera is not assigned!");
            }
        }
    }
    public void Steer(int value)
    {
        //Debug.Log("steer activated");
        steerValue = value;
    }
    public void ApplyPowerUp(string type, int value)
    {
        //powerUpTemp = powerUp;
        switch (type)
        {
            case "Energy":
                //Debug.Log("Energy aquired");
                energyCurrencyAmount += value;
                energyAcquiredPowerUp.Play();
                if (energyTextDisplay != null)
                {
                    energyTextDisplay.text = $"{energyCurrencyAmount}";
                }
                break;
            case "FireRate":
                ApplyFireRateToFleet(value);
                break;
            case "Ship":
                ModifyFleetShipNumber(value);
                break;
            default:
                Debug.LogWarning("Unknown power-up type: " + type);
                break;
        }

        //Debug.Log($"Applied power-up: {type} with value: {value}");
    }
    private void ModifyFleetShipNumber(int value)
    {
        StartCoroutine(SpawnShipsWithDelay(value));
    }
    private void ApplyFireRateToFleet(int value)
    {
        float upgradePercentageValue = value / 100f;
        omegaShipScript.fireRate = omegaShipScript.fireRate + upgradePercentageValue;
        turretScripts = gameObject.GetComponentsInChildren<TurretScript>();
        foreach (TurretScript script in turretScripts)
        {
            script.fireRate += upgradePercentageValue;
        }
        Debug.Log("firerate upgrade value = " + upgradePercentageValue);
        Debug.Log("omega ship firerate = " + omegaShipScript.fireRate);
    }

    private IEnumerator SpawnShipsWithDelay(int value)
    {
        for (int i = 0; i < value; i++)
        {
            // Check for pause before spawning the next ship
            yield return WaitWhilePaused();

            // Instantiate the AlphaShipObject at the position and rotation of the SpawnPoint
            GameObject newShip = Instantiate(AlphaShipObject, SpawnPoint.position, SpawnPoint.rotation);
            AlphaShipScript alphaShipScript = newShip.GetComponent<AlphaShipScript>();
            for (int j = 0; j < positions.Length; j++)
            {
                if (!positions[j].isUsed)
                {
                    supplementShipStatBars[positions[j].index].gameObject.SetActive(true);
                    alphaShipScript.fleetPosition = positions[j].index;
                    positions[j].isUsed = true;
                    break;
                }
            }
            alphaShipScript.shipPosition = shipPositions[numberOfShipsInFleet];
            newShip.transform.SetParent(this.transform);
            spawnParticleSystem.Play();
            numberOfShipsInFleet++;

            // Check for pause before the delay
            yield return WaitWhilePaused();
            yield return new WaitForSeconds(0.5f); // Delay of 0.5 seconds between spawns
        }
    }
    public void DestroyAllChildShips()
    {
        shipScripts = gameObject.GetComponentsInChildren<AlphaShipScript>();
        for (int i = 0; i < shipScripts.Length; i++)
        {
            shipScripts[i].OmegaDeath();
        }
    }
    public void UpdateSupplementalShipHealthBar(int shipNumber, float health, float maxHealth)
    {
        // Find the HealthForeground image
        Image healthImage = supplementShipStatBars[shipNumber].transform.Find("HealthForeground").GetComponent<Image>();
        healthImage.fillAmount = health / maxHealth;
    }
    public void UpdateSupplementalShipShieldBar(int shipNumber, float shield, float maxShield)
    {
        // Find the ShieldForeground image
        Image shieldImage = supplementShipStatBars[shipNumber].transform.Find("ShieldForeground").GetComponent<Image>();
        shieldImage.fillAmount = shield / maxShield;
    }
    public void SupplementalShipDeath(int shipPostion)
    {
        positions[shipPostion].isUsed = false;
    }
    private IEnumerator WaitWhilePaused()
    {
        while (isPaused)
        {
            yield return null; // Wait until the next frame while `isPaused` is true
        }
    }
}
