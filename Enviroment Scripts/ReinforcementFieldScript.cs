using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Make sure to import the UI namespace if fieldIcon is a UI element

public class ReinforcementFieldScript : MonoBehaviour, IDamageable
{
    public ParticleSystem zoneParticleSystem;
    public ParticleSystem activationParticleSystem;
    public GameObject fieldIcon; // Assuming this has an Image component
    public TMP_Text shipValueText;
    public float fadeDuration = 2f; // Duration for fade out
    public int shipValue;
    public float upgradeThreshold;
    private float currentCharge;
    [SerializeField] private Image chargeProgressCircularImage;
    [SerializeField] private ParticleSystem chargeUpAnimation;

    private Image fieldIconImage;

    // Start is called before the first frame update
    void Start()
    {
        shipValueText.text = "+" + shipValue;
        // Get the Image component if fieldIcon is a UI image
        if (fieldIcon != null)
        {
            fieldIconImage = fieldIcon.GetComponent<Image>();
        }
        if (SceneManager.GetActiveScene().name != "Load Scene")
        {
            AssignObjectStats();
        }
        if (SceneManager.GetActiveScene().name == "Load Scene")
        {
            RetrieveObjectStats();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "OmegaShip")
        {
            activationParticleSystem.Play();
            zoneParticleSystem.Stop();
            IPowerUpReceiver receiver = other.GetComponentInParent<IPowerUpReceiver>();
            receiver.ApplyPowerUp("Ship", shipValue);
            // Start fading out the fieldIcon after the trigger is activated
            if (fieldIconImage != null)
            {
                FleetManager fleetScript = other.gameObject.GetComponent<FleetManager>();
                StartCoroutine(FadeOutAndDestroy());
            }
            else
            {
                Debug.Log("Icon is Null" + fieldIconImage);
            }
        }
    }
    public void AssignObjectStats()
    {
        ReinforcementFieldStats objectStats = gameObject.GetComponent<ReinforcementFieldStats>();
        objectStats.prefabType = "RienforceField";
        objectStats.shipValue = shipValue;
        objectStats.upgradeThreshold = upgradeThreshold;
    }
    public void RetrieveObjectStats()
    {
        ReinforcementFieldStats objectStats = gameObject.GetComponent<ReinforcementFieldStats>();
        shipValue = objectStats.shipValue;
        upgradeThreshold = objectStats.upgradeThreshold;
    }
    private IEnumerator FadeOutAndDestroy()
    {
        //Debug.Log("function activated");
        float elapsedTime = 0f;
        Color originalColor = fieldIconImage.color;

        // Gradually decrease the alpha value over time
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); // Interpolates alpha from 1 to 0
            fieldIconImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null; // Wait for the next frame
        }

        // Ensure the alpha is set to 0 at the end
        fieldIconImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        //Debug.Log("icon should be gone now");
        // Destroy the GameObject after the fade-out is complete
        Destroy(gameObject);
    }
    public void Damage(float damage)
    {
        currentCharge += damage;
        chargeProgressCircularImage.fillAmount = currentCharge / upgradeThreshold;
        if (currentCharge >= upgradeThreshold)
        {
            if(shipValue <= 9)
            {
                chargeUpAnimation.Play();
                shipValue++;
                shipValueText.text = "+" + shipValue;
                currentCharge = 0;
                chargeProgressCircularImage.fillAmount = 0;
            }
        }
        //Debug.Log("charge level = " + currentCharge);
    }
}
