using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class OmegaFleetSceneManager : MonoBehaviour
{
    public AudioClip audioPlaytimeTrack;
    public GameObject omegaFleetObject;
    public GameObject buildFleetIcon;
    public float levelBoundary = 20f;
    public GameObject leftBoundary;
    public GameObject rightBoundary;
    public float boundartVisualOffset;
    public Canvas controlCanvas;
    public Canvas menuCanvas;
    public Canvas victoryCanvas;
    public Canvas deathCanvas;
    public CanvasGroup UICanvasGroup;
    public UnityEngine.UI.Image endLevelBackground;
    public Color endLevelScreenColor; // The custom color you can set in the Inspector

    public GameObject star;
    public GameObject starBloomImage;
    public ParticleSystem starExplostionParticleSystem;
    public float fadeDuration = 2.0f;  // Duration for the fade-out
    public float blackOutFadeDuration = 1.0f;
    public float starScaleDuration = 3.0f;  // Independent scaling duration for the star
    public GameObject pauseMenu;

    private Material quadMaterial;
    private Color materialColor;
    private float fadeElapsed = 0.0f;
    private float starScaleElapsed = 0.0f;  // Separate time tracker for the star scaling
    private FleetManager fleetManager;
    public GameObject[] starAuraImages;  // The 3 sprites you want to scale down
    public ParticleSystem starAuraParticleSystem;
    private Vector3 initialScale;
    private Vector3 initialScaleParticleSystem;
    private Vector3 initialStarScale;
    private bool isScaling = false;
    private bool starIsExploding = false;

    private HiveFleetAI[] hiveFleetShips;
    private PortalCubeShipManager[] portalCubeShips;
    private TurretScript[] turretScripts;
    private PlasmaProjectileScript[] plasmaProjectileScripts;
    private PowerUpScript[] powerUpScripts;
    private AlphaShipScript[] alphaShipScripts;

    public bool isAlive;
    public bool isPaused;

    //timer function for music evaluation
    private float timer = 0f;
    private bool isRunning = false;

    private void Start()
    {
        //timer function for music evaluation
        StartTimer();
        //isPaused = true;
        leftBoundary.transform.position = new Vector3(-levelBoundary - boundartVisualOffset, 1.5f, -100);
        rightBoundary.transform.position = new Vector3(levelBoundary + boundartVisualOffset, 1.5f, -100);
        isAlive = true;
        isPaused = false;
        fleetManager = FindAnyObjectByType<FleetManager>();
        fleetManager.isAlive = isAlive;

        

        // Get the material attached to the quad for fade-out
        quadMaterial = starBloomImage.GetComponent<Renderer>().material;
        materialColor = quadMaterial.color;

        // Store the initial scale of the sprites and star
        if (starAuraImages.Length > 0)
        {
            initialScale = starAuraImages[0].transform.localScale;  // Assuming all sprites have the same initial scale
        }
        initialScaleParticleSystem = starAuraParticleSystem.transform.localScale;
        initialStarScale = star.transform.localScale;  // Store the initial scale of the star
        AudioManagerScript.Instance.PlayMusic(audioPlaytimeTrack);
        Debug.Log("music should be playing");
        //PauseGame();

    }
    //timer function for music evaluation
    public void StartTimer()
    {
        isRunning = true;
    }
    //timer function for music evaluation

    public void StopTimer()
    {
        isRunning = false;
    }
    //timer function for music evaluation

    public float GetTime()
    {
        return timer;
    }
    public void PauseGame()
    {
        isPaused = !isPaused;
        fleetManager.isPaused = isPaused;
        OmegaShipScript omegaShipScript = FindAnyObjectByType<OmegaShipScript>();
        omegaShipScript.isPaused = isPaused;
        hiveFleetShips = FindObjectsOfType<HiveFleetAI>();
        foreach(HiveFleetAI script in hiveFleetShips)
        {
            script.isPaused = isPaused;
            //Debug.Log(script.gameObject.name + " is paused");
        }
        portalCubeShips = FindObjectsOfType<PortalCubeShipManager>();
        foreach (PortalCubeShipManager script in portalCubeShips)
        {
            script.isPaused = isPaused;
        }
        turretScripts = FindObjectsOfType<TurretScript>();
        foreach (TurretScript script in turretScripts)
        {
            script.isPaused = isPaused;
        }
        plasmaProjectileScripts = FindObjectsOfType<PlasmaProjectileScript>();
        foreach (PlasmaProjectileScript script in plasmaProjectileScripts)
        {
            script.isPaused = isPaused;
        }
        powerUpScripts = FindObjectsOfType<PowerUpScript>();
        foreach (PowerUpScript script in powerUpScripts)
        {
            script.isPaused = isPaused;
        }
        alphaShipScripts = FindObjectsOfType<AlphaShipScript>();
        foreach (AlphaShipScript script in alphaShipScripts)
        {
            script.isPaused = isPaused;
        }
        if (isPaused)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }
    void Update()
    {
        //timer function for music evaluation
        if (isRunning)
        {
            timer += Time.deltaTime;
        }
        // Handle fading out the image
        if (fadeElapsed < fadeDuration)
        {
            // Fading logic (same as before)
        }

        // Handle scaling down sprites and star
        if (isScaling)
        {
            fadeElapsed += Time.deltaTime;
            starScaleElapsed += Time.deltaTime;  // Increment the star scaling time

            // Calculate the new alpha value based on the elapsed time
            float alpha = Mathf.Lerp(0.5f, 0.0f, fadeElapsed / fadeDuration);

            // Update the material's alpha value
            materialColor.a = alpha;
            quadMaterial.color = materialColor;

            for (int i = 0; i < starAuraImages.Length; i++)
            {
                // Scale each sprite down over time
                if (starAuraImages[i] != null)
                {
                    starAuraImages[i].transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, fadeElapsed / fadeDuration);
                }
            }

            // Scale down the particle system
            starAuraParticleSystem.transform.localScale = Vector3.Lerp(initialScaleParticleSystem, Vector3.zero, fadeElapsed / fadeDuration);

            // Scale down the star object using an exponential easing effect
            if (star != null)
            {
                // Calculate exponential interpolation factor
                float t = starScaleElapsed / starScaleDuration;  // Normalized time (0 to 1)
                t = Mathf.Clamp01(t);  // Ensure t is between 0 and 1

                // Exponential easing (start slow, speed up)
                float exponentialFactor = t * t;  // Quadratic easing for exponential effect
                if (exponentialFactor > 0.5f)
                {
                    if (!starIsExploding)
                    {
                        starIsExploding = true;
                        StartCoroutine(FadeOutUICanvas());
                        VictoryScreenManager victoryScreenManager = GetComponent<VictoryScreenManager>();
                        victoryScreenManager.BeginVictoryScreen();
                        starExplostionParticleSystem.Play();
                        StartCoroutine(BeginEndLevelScreen());
                        
                    }
                }
                //Debug.Log("exponential factor = "+ exponentialFactor);
                // Scale the star based on the exponential factor
                star.transform.localScale = Vector3.Lerp(initialStarScale, Vector3.zero, exponentialFactor);
            }
        }
    }


    public void DeathScene()
    {
        isAlive = false;
        fleetManager.isAlive = isAlive;
        controlCanvas.gameObject.SetActive(false);
        menuCanvas.gameObject.SetActive(true);
        //AudioManagerScript.Instance.StopMusic();
    }

    public void EndLevel()
    {
        // Start fading the image and scaling the sprites when EndLevel is called
        fadeElapsed = 0.0f;  // Reset the timer for fade-out and scaling
        starScaleElapsed = 0.0f;  // Reset the timer for star scaling
        isScaling = true;    // Start scaling down the sprites and star
        StopTimer();
        Debug.Log("end time is = " + GetTime());
    }
    private IEnumerator BeginEndLevelScreen()
    {
        victoryCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);  // Initial delay before fading in

        float elapsedTime = 0.0f;
        Color initialColor = endLevelBackground.color;  // Get the current color (including alpha)
        Color targetColor = initialColor;
        targetColor.a = 1.0f;  // Target alpha value (fully opaque)

        // Fade in the endLevelBackground to full opacity
        while (elapsedTime < blackOutFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            endLevelBackground.color = Color.Lerp(initialColor, targetColor, elapsedTime / blackOutFadeDuration);
            yield return null;
        }

        // Ensure the background is fully opaque
        endLevelBackground.color = targetColor;

        // Wait for a moment after the background reaches full opacity
        yield return new WaitForSeconds(0.5f);

        // Now fade to the custom color
        elapsedTime = 0.0f;
        Color currentColor = endLevelBackground.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            endLevelBackground.color = Color.Lerp(currentColor, endLevelScreenColor, elapsedTime / fadeDuration);
            yield return null;
        }


        // Ensure that the color stays the same (no further fades or changes)
        yield break;
    }
    // Coroutine to fade out the UICanvas
    private IEnumerator FadeOutUICanvas()
    {
        float elapsedTime = 0f;
        float startAlpha = UICanvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            UICanvasGroup.LeanAlpha(0, 0.5f);
            yield return null;
        }

        UICanvasGroup.alpha = 0f; // Ensure it is fully transparent at the end
    }
}
