using UnityEngine;
using UnityEngine.UI;

public class RadarMarker : MonoBehaviour
{
    // Reference to the player's camera
    public Camera playerCamera;

    // Reference to the enemy object
    public Transform targetTransform;

    // Reference to the UI marker
    public Image markerImage;

    // Offset to adjust the marker position
    public Vector3 offset = new Vector3(0, 0, 0);



    void Update()
    {
        // Check if the enemy is within the camera's view frustum
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(targetTransform.position);

        bool isVisible = (viewportPoint.z > 0 &&
                          viewportPoint.x > 0 && viewportPoint.x < 1 &&
                          viewportPoint.y > 0 && viewportPoint.y < 1);

        if (isVisible)
        {
            // Convert the enemy's world position to screen position
            Vector3 screenPoint = playerCamera.WorldToScreenPoint(targetTransform.position + offset);

            // Set the position of the marker image to the screen position
            markerImage.rectTransform.position = screenPoint;

            // Enable the marker image
            markerImage.enabled = true;
        }
        else
        {
            // Disable the marker image if the enemy is not visible
            markerImage.enabled = false;
        }
    }
}
