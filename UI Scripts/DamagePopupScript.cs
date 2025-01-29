using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using CodeMonkey.Utils;

public class DamagePopupScript : MonoBehaviour
{
    public static DamagePopupScript Create(Vector3 position, float damageAmount, string type)
    {
        // Add 5 to the Z axis of the position
        position.y += 5f;

        Transform damagePopupTransform = Instantiate(GameAssets.i.DamagePopupTransform, position, Quaternion.identity);

        DamagePopupScript damagePopup = damagePopupTransform.GetComponent<DamagePopupScript>();
        damagePopup.Setup(damageAmount, type);

        return damagePopup;
    }
    private static int sortingOrder;
    private const float DISAPPEAR_TIMER_MAX = 1f;

    private TMP_Text textMesh;
    private float disapearTimer;
    private Color enemyTextColor;
    private Color shieldTextColor;
    private Color healthTextColor;
    private Color currentColor;
    private void Awake()
    {
        textMesh = transform.GetComponent<TMP_Text>();
        enemyTextColor = UtilsClass.GetColorFromString("ffa600");
        shieldTextColor = UtilsClass.GetColorFromString("0055ff");
        healthTextColor = UtilsClass.GetColorFromString("ff002b");
    }
    public void Setup(float damageAmount, string type)
    {
        textMesh.text = damageAmount.ToString();

        switch (type)
        {
            case "Enemy":
                textMesh.color = enemyTextColor;
                currentColor = textMesh.color;
                break;
            case "Shield":
                textMesh.color = shieldTextColor;
                currentColor = textMesh.color;
                break;
            case "Health":
                textMesh.color = healthTextColor;
                currentColor = textMesh.color;
                break;
            default:
                Debug.LogWarning("Unknown Color type for damage popup: " + type);
                break;
        }
        disapearTimer = DISAPPEAR_TIMER_MAX;
        sortingOrder++;
        textMesh.geometrySortingOrder = (VertexSortingOrder)sortingOrder;
    }
    private void Update()
    {
        float moveYSpeed = 5f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        if (disapearTimer > DISAPPEAR_TIMER_MAX * .5)
        {
            //first half of popup
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            //second half
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }
        disapearTimer -= Time.deltaTime;
        if(disapearTimer < 0)
        {
            // Start Dissapearing
            float disapeppearSpeed = 3f;
            currentColor.a -= disapeppearSpeed * Time.deltaTime;
            textMesh.color = currentColor;
            if(currentColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
