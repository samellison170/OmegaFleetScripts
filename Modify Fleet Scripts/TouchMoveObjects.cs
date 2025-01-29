using UnityEngine;

public class TouchMoveObjects : MonoBehaviour
{
    public GameObject[] movableObjects;  // Array for your 10 child objects
    public GameObject parentObject;      // The parent object that should not be overlapped
    public float minDistance = 1.0f;     // Minimum distance between child objects to avoid overlap
    public float minDistanceFromParent = 3.0f; // Minimum distance to avoid overlap with the parent
    public float maxRadius = 5.0f;       // Maximum radius the child objects can move from the parent

    private GameObject selectedObject = null;  // The object being dragged
    private Vector3 initialTouchPosition;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Raycast to detect if a touch started on any of the movable objects
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        foreach (GameObject obj in movableObjects)
                        {
                            if (hit.transform == obj.transform)
                            {
                                selectedObject = obj;
                                initialTouchPosition = touchPosition;
                                break;
                            }
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (selectedObject != null)
                    {
                        Vector3 newPosition = selectedObject.transform.position + (touchPosition - initialTouchPosition);

                        // Check for overlap and distance from parent object before moving
                        if (CanMoveWithoutOverlap(selectedObject, newPosition) && IsWithinParentRadius(newPosition))
                        {
                            selectedObject.transform.position = newPosition;
                            initialTouchPosition = touchPosition;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    // Deselect object when touch ends
                    selectedObject = null;
                    break;
            }
        }
    }

    // Function to check if the selected object can move to the new position without overlapping other objects or the parent
    bool CanMoveWithoutOverlap(GameObject selectedObj, Vector3 newPosition)
    {
        // Check if the new position overlaps with other child objects
        foreach (GameObject obj in movableObjects)
        {
            if (obj != selectedObj)
            {
                float distance = Vector3.Distance(newPosition, obj.transform.position);
                if (distance < minDistance)
                {
                    return false;  // Can't move, it would overlap with another child object
                }
            }
        }

        // Check if the new position is too close to the parent object
        float distanceFromParent = Vector3.Distance(newPosition, parentObject.transform.position);
        if (distanceFromParent < minDistanceFromParent)
        {
            return false;  // Can't move, it would overlap with the larger parent object
        }

        return true;  // Safe to move
    }

    // Function to check if the new position is within a custom radius from the parent object
    bool IsWithinParentRadius(Vector3 newPosition)
    {
        float distanceFromParent = Vector3.Distance(newPosition, parentObject.transform.position);
        return distanceFromParent <= maxRadius;
    }
}
