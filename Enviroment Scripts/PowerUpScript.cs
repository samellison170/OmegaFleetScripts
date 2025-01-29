using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PowerUpScript : MonoBehaviour
{
    public GameObject powerUpEffect;
    private GameObject playerFleet;
    public float initialSpeed = 1.0f;   // Initial speed of the object
    public float acceleration = 2.0f;   // Rate of acceleration
    public int powerUpValue;
    private GameObject omegaShip;
    private OmegaShipScript omegaShipScript;
    public float pickupRange;

    // Reference to the player's camera
    //public Camera playerCamera;

    // Reference to the enemy object
    public Transform targetTransform;

    // Reference to the UI marker
    //public Image markerImage;

    // Offset to adjust the marker position
    public Vector3 offset = new Vector3(0, 0, 0);

    public int segments = 50;  // Number of segments to make the circle smooth
    public float lineWidth = .1f;
    //public int targetRange;

    private GameObject playerObject;
    private float currentSpeed;

    // List to keep track of running coroutines
    private int runningCoroutines;

    private TrailRenderer trailRenderer;
    private Transform target;         // Target to move towards
    private bool moveToTarget = false;
    private new ParticleSystem particleSystem; // Reference to the particle system
    private SpriteRenderer spriteRenderer;
    private bool hasTriggered = false;    // Ensure the particle system is triggered only once
    private LineRenderer lineRenderer;


    private Collider tempCollider;

    public float fadeDuration = 2.0f;
    public bool isPaused = false;

    void Start()
    {
        OmegaShipScript myScriptInstance = FindObjectOfType<OmegaShipScript>();
        if (myScriptInstance != null)
        {
            // Save the GameObject that the script is attached to
            omegaShipScript = myScriptInstance;
            omegaShip = myScriptInstance.gameObject;
        }
        FleetManager myScriptInstance2 = FindObjectOfType<FleetManager>();
        if (myScriptInstance != null)
        {
            // Save the GameObject that the script is attached to
            playerFleet = myScriptInstance2.gameObject;
            //Debug.Log("playerFleet Object saved as :" + playerFleet.name);
        }
        if (myScriptInstance != null)
        {
            // Save the GameObject that the script is attached to
            GameObject myObject = myScriptInstance.gameObject;
        }
        // Assuming the particle system is a child of this object
        particleSystem = omegaShip.GetComponentInChildren<ParticleSystem>();

        trailRenderer = gameObject.GetComponentInChildren<TrailRenderer>();
        if (particleSystem == null)
        {
            Debug.Log("particle system not found");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSpeed = initialSpeed; // Set the initial speed

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        if (SceneManager.GetActiveScene().name != "Load Scene")
        {
            AssignObjectStats();
        }
        if (SceneManager.GetActiveScene().name == "Load Scene")
        {
            RetrieveObjectStats();
        }
    }
    public void AssignObjectStats()
    {
        PowerUpStats objectStats = gameObject.GetComponent<PowerUpStats>();
        objectStats.prefabType = "EnergyPickup";
        objectStats.powerUpValue = powerUpValue;
    }
    public void RetrieveObjectStats()
    {
        PowerUpStats objectStats = gameObject.GetComponent<PowerUpStats>();
        powerUpValue = objectStats.powerUpValue;
    }
    void Update()
    {
        if (!isPaused && !omegaShipScript.isDead)
        {
            if (Vector3.Distance(omegaShip.transform.position, gameObject.transform.position) < pickupRange)
            {
                playerObject = omegaShip;
                //Decouple darkMatterGameObject for it to have the correct transforms
                powerUpEffect.transform.SetParent(null);
                // Set the target to the position of the other object's transform
                target = omegaShip.transform;
                moveToTarget = true;
                //StartCoroutine(FadeOutImage());
                //StartCoroutine(FadeOutSprite());
            }
            if (moveToTarget && target != null && powerUpEffect != null)
            {
                //Debug.Log("moving torwards player");
                //Debug.Log(darkMatterGameObject.name);

                // Increase the speed based on acceleration and time
                currentSpeed += acceleration * Time.deltaTime;

                // Move towards the target
                powerUpEffect.transform.position = Vector3.MoveTowards(powerUpEffect.transform.position, target.position, currentSpeed * Time.deltaTime);
                // Check if the object has reached the target position

                if (Vector3.Distance(powerUpEffect.transform.position, target.position) < 0.1f && hasTriggered == false)
                {
                    hasTriggered = true;
                    particleSystem.gameObject.transform.SetParent(null);
                    particleSystem.Play(); // Play the particle system
                    powerUpEffect.SetActive(false);
                    IPowerUpReceiver receiver = playerFleet.GetComponentInChildren<IPowerUpReceiver>();
                    if (receiver != null)
                    {
                        //Debug.Log("power up sent");
                        receiver.ApplyPowerUp("Energy", powerUpValue);
                        DestroyTrailObjectAfterCoroutines();
                    }
                    else
                    {
                        Debug.Log("Power Up Reciever is null" + receiver);
                    }
                }
            }
        }
        //DrawCircle();
        // If there is a target to move towards, move the object


        //// Check if the enemy is within the camera's view frustum
        //Vector3 viewportPoint = playerCamera.WorldToViewportPoint(targetTransform.position);

        //bool isVisible = (viewportPoint.z > 0 &&
        //                  viewportPoint.x > 0 && viewportPoint.x < 1 &&
        //                  viewportPoint.y > 0 && viewportPoint.y < 1);

        //if (isVisible)
        //{
        //    // Convert the enemy's world position to screen position
        //    Vector3 screenPoint = playerCamera.WorldToScreenPoint(targetTransform.position + offset);

        //    // Set the position of the marker image to the screen position
        //    markerImage.rectTransform.position = screenPoint;

        //    // Enable the marker image
        //    markerImage.enabled = true;
        //}
        //else
        //{
        //    // Disable the marker image if the enemy is not visible
        //    markerImage.enabled = false;
        //}
    }
    //void OnTriggerEnter(Collider other)
    //{
    //    tempCollider = other;

    //    // Check if the collision is with the parent's parent collider and has not triggered yet
    //    if (particleSystem != null)
    //    {
            

    //        // If the other object has a specific tag, for example, "Player"
    //        if (other.CompareTag("Player"))
    //        {
    //            if (other.gameObject.name == "OmegaShip")
    //            {
    //                playerObject = other.gameObject;
    //                //Decouple darkMatterGameObject for it to have the correct transforms
    //                powerUpEffect.transform.SetParent(null);
    //                // Set the target to the position of the other object's transform
    //                target = other.transform;
    //                moveToTarget = true;
    //                StartCoroutine(FadeOutImage());
    //                StartCoroutine(FadeOutSprite());
    //            }

    //        }

    //        //hasTriggered = true;   // Set the flag to true to prevent re-triggering
    //    }
    //}
    private IEnumerator FadeOutSprite()
    {
        runningCoroutines++;
        float startAlpha = spriteRenderer.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tempColor = spriteRenderer.color;
            tempColor.a = Mathf.Lerp(startAlpha, 0, progress);
            spriteRenderer.color = tempColor;
            progress += rate * Time.deltaTime;

            yield return null;
        }

        // Ensure the sprite is fully transparent at the end
        Color finalColor = spriteRenderer.color;
        finalColor.a = 0;
        spriteRenderer.color = finalColor;
        runningCoroutines--;
    }
    //void DrawCircle()
    //{

    //    // Reset the LineRenderer
    //    lineRenderer.positionCount = 0;

    //    // Set the number of positions for the circle
    //    lineRenderer.positionCount = segments + 1;  // +1 to close the loop
    //    lineRenderer.useWorldSpace = false;

    //    float angle = 0f;
    //    for (int i = 0; i <= segments; i++)
    //    {
    //        float x = Mathf.Cos(Mathf.Deg2Rad * angle) * targetRange;
    //        float z = Mathf.Sin(Mathf.Deg2Rad * angle) * targetRange;

    //        lineRenderer.SetPosition(i, new Vector3(x, 0, z));  // Y = 0 to stay flat
    //        angle += 360f / segments;
    //    }
    //}
    //private IEnumerator FadeOutImage()
    //{
    //    runningCoroutines++;
    //    // Get the current color of the image
    //    Color currentColor = markerImage.color;

    //    // Calculate the amount to fade per second
    //    float fadeAmount = currentColor.a / fadeDuration;

    //    // Loop over time, reducing the alpha value of the color
    //    while (currentColor.a > 0)
    //    {
    //        // Decrease the alpha value
    //        currentColor.a -= fadeAmount * Time.deltaTime;

    //        // Apply the updated color to the image
    //        markerImage.color = currentColor;

    //        // Wait for the next frame
    //        yield return null;
    //    }

    //    // Ensure the alpha is fully set to zero
    //    currentColor.a = 0;
    //    markerImage.color = currentColor;
    //    // Destroy the GameObject once the fade-out is complete
    //    runningCoroutines--;
    //}
    // Method to initiate destruction of the GameObject
    public void DestroyTrailObjectAfterCoroutines()
    {
        // Start the master coroutine to wait for all other coroutines to finish
        StartCoroutine(WaitForAllCoroutinesAndDestroy());
    }

    // Master coroutine that waits for all other coroutines to finish
    IEnumerator WaitForAllCoroutinesAndDestroy()
    {
        // Wait until all coroutines in the list are finished
        while (runningCoroutines > 0)
        {
            //Debug.Log(runningCoroutines);
            yield return null; // Wait for the next frame
        }

        // Optional: Clear the trail before destruction
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }

        // Destroy the GameObject
        Destroy(powerUpEffect);
        Destroy(gameObject);
        //Debug.Log("GameObject destroyed after all coroutines finished");
    }
    public void Death()
    {
        Destroy(gameObject);
    }
}
