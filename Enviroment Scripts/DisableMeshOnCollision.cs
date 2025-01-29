using UnityEngine;

public class DisableMeshOnCollision : MonoBehaviour
{
    public ParticleSystem portalParticleSystem;
    public ParticleSystem vacuumParticleSystem;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with " + other.gameObject.name);
        if(other.gameObject.tag == "Fleet")
        {
            HiveFleetAI[] hiveFleetAIs = FindObjectsOfType<HiveFleetAI>();
            PortalCubeShipManager[] prefabSpawners = FindObjectsOfType<PortalCubeShipManager>();
            foreach (HiveFleetAI script in hiveFleetAIs)
            {
                script.Death();
            }
            foreach (PortalCubeShipManager script in prefabSpawners)
            {
                script.Death();
            }
            PowerUpScript[] powerUps = FindObjectsOfType<PowerUpScript>();
            foreach (PowerUpScript script in powerUps)
            {
                script.Death();
            }
        }
        // Set the triggering object and all its children to inactive
        other.gameObject.SetActive(false);
        portalParticleSystem.Stop();
        vacuumParticleSystem.Stop();  

    }
}
