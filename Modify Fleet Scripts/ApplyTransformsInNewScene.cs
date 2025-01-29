using UnityEngine;

public class ApplyTransformsInNewScene : MonoBehaviour
{
    public GameObject[] childObjects; // Assign the corresponding child objects in the new scene

    void Start()
    {
        // Ensure there is saved data
        if (ChildTransformsData.positions != null && ChildTransformsData.positions.Length == childObjects.Length)
        {
            for (int i = 0; i < childObjects.Length; i++)
            {
                // Apply the saved local transforms to the child objects
                childObjects[i].transform.localPosition = ChildTransformsData.positions[i];
                Debug.Log(childObjects[i].transform.localPosition);
                //childObjects[i].transform.localRotation = ChildTransformsData.rotations[i];
                //childObjects[i].transform.localScale = ChildTransformsData.scales[i];
            }
        }
    }
}
