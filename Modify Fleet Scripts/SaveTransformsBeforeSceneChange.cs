using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveTransformsBeforeSceneChange : MonoBehaviour
{
    public GameObject[] childObjects; // Assign the 10 child objects

    public void SaveAndLoadNextScene()
    {
        // Save the child objects' transforms
        ChildTransformsData.SaveChildTransforms(childObjects);

        // Load the new scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game Scene");
    }
}
