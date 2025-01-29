using UnityEngine;

public static class ChildTransformsData
{
    public static Vector3[] positions;
    //public static Quaternion[] rotations;
    //public static Vector3[] scales;

    // Save child transforms in arrays
    public static void SaveChildTransforms(GameObject[] childObjects)
    {
        positions = new Vector3[childObjects.Length];
        //rotations = new Quaternion[childObjects.Length];
        //scales = new Vector3[childObjects.Length];

        for (int i = 0; i < childObjects.Length; i++)
        {
            positions[i] = childObjects[i].transform.position;
            Debug.Log(childObjects[i].transform.position);
            //rotations[i] = childObjects[i].transform.rotation;
            //scales[i] = childObjects[i].transform.localScale;
        }
    }
}
