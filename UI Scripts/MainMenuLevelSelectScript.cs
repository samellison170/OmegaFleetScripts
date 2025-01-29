using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLevelSelectScript : MonoBehaviour
{
    public GameObject levelSelectStartButton;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefsManager.GetInt("FirstTimePlaying") != 1)
        {
            levelSelectStartButton.SetActive(false);
        }
    }

    public void LoadLevelSelectScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level Select Scene");
    }
}
