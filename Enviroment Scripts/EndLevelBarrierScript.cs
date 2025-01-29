
using UnityEngine;

public class EndLevelBarrierScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with " + other.gameObject.name);
        if (other.CompareTag("Fleet"))
        {
            
            FleetManager playerFleet = other.GetComponent<FleetManager>();

            // Check if the FleetManager component exists
            if (playerFleet != null)
            {
                playerFleet.EndLevel();
            }
            else
            {
                Debug.LogError("FleetManager component not found on the object with the 'Fleet' tag: " + other.name);
            }
        }
    }
}
