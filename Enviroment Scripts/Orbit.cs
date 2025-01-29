using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public float orbitSpeed = 10.0f; // Speed of orbiting
    public float orbitRadius = 5.0f; // Radius of the orbit
    public float spinSpeed = 5.0f; // Speed of spinning
    private float randomOffset;
    public float minOffset = 0f;
    public float maxOffset = 200f;// Random time offset

    void Start()
    {
        // Generate a random offset between 0 and 10 seconds (example range)
        randomOffset = Random.Range(minOffset, maxOffset);
    }

    void Update()
    {

        // Calculate the new position for the object to move along the orbit
        float angle = orbitSpeed * (Time.time + randomOffset); // Angle based on time and speed
        float x = Mathf.Cos(angle) * orbitRadius;
        float z = Mathf.Sin(angle) * orbitRadius;

        // Update the position of the object
        transform.position = new Vector3(x, transform.position.y, z);

        // Rotate the object around its own axis
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}