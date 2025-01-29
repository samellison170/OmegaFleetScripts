using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonOrbit : MonoBehaviour
{
    public float orbitSpeed = 10.0f; // Speed of orbiting
    public float orbitRadius = 5.0f;
    public float spinSpeed = 5.0f; // Speed of spinning// Radius of the orbit
    public Transform target; // The object to orbit around

    void Update()
    {
        // Ensure the target is set
        if (target == null)
        {
            Debug.LogWarning("Target is not set. Please assign a target object to orbit around.");
            return;
        }
        // Calculate the new position for the object to move along the orbit
        float angle = orbitSpeed * Time.time; // Angle based on time and speed
        float x = Mathf.Cos(angle) * orbitRadius;
        float z = Mathf.Sin(angle) * orbitRadius;

        // Update the position of the object
        transform.position = new Vector3(x, transform.position.y, z) + target.position;

        // Rotate the object around its own axis
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}