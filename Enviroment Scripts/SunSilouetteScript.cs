using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSilouetteScript : MonoBehaviour
{

    public Transform cameraTransform;
    public float zRotationSpeed = 30f; // Speed of Z-axis rotation in degrees per second

    void Update()
    {
        if (cameraTransform != null)
        {
            // Calculate the direction from the object to the camera
            Vector3 directionToCamera = cameraTransform.position - transform.position;
            directionToCamera.Normalize(); // Normalize the direction

            // Create a rotation that looks in the direction of the camera, with Z-axis forward
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

            // Create a rotation around the local Z-axis
            Quaternion zRotation = Quaternion.Euler(0, 0, zRotationSpeed * Time.time);

            // Combine the rotations: first look at the camera, then apply the Z rotation
            transform.rotation = lookRotation * zRotation;
        }
    }
}
