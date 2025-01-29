using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Object to follow
    public Vector3 offset;    // Offset from the target
    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            //Debug.Log(target.position);
        }
    }
}