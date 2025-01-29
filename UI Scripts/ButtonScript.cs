
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ButtonScript : MonoBehaviour
{
    private OmegaShipScript OmegaShip;
    [SerializeField] public GameObject VideoPlayerObject;
    private VideoPlayer cinematic;
    [SerializeField] public Image fadeInImage;
    private RenderTexture renderTexture;
    [SerializeField] public GameObject SettingsCanvas;
    private LevelSelectManagerScript levelSelectManagerScript;
    public void StartGame()
    {
        levelSelectManagerScript = FindAnyObjectByType<LevelSelectManagerScript>();
        VideoPlayerObject.SetActive(false); // Hide the video initially
        SetRenderTextureResolution(1080, 1920);

        if (PlayerPrefsManager.GetInt("FirstTimePlaying") != 1)
        {
            VideoPlayerObject.SetActive(true);
            cinematic = VideoPlayerObject.GetComponentInChildren<VideoPlayer>();
            cinematic.playOnAwake = false;
            cinematic.time = 0;
            cinematic.Prepare();
            cinematic.prepareCompleted += OnVideoPrepared;
        }
        else
        {
            SceneManager.LoadScene("Load Scene");
        }
    }
    private void OnVideoPrepared(VideoPlayer vp)
    {
        StartCoroutine(LoadSceneAfterTime(91f));
        // Fades the alpha from 1 to 0 over 2 seconds
        LeanTween.value(gameObject, UpdateAlpha, 1f, 0f, 2f);
        VideoPlayerObject.SetActive(true); // Show the video once it's prepared
        cinematic.Play();
    }
    void UpdateAlpha(float alpha)
    {
        // Update the alpha of the image color
        Color color = fadeInImage.color;
        color.a = alpha;
        fadeInImage.color = color;
    }
    IEnumerator LoadSceneAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay (91 seconds)
        SceneManager.LoadScene("Game Scene"); // Load the specified scene
    }
    void SetRenderTextureResolution(int width, int height)
    {
        // Check if the RenderTexture exists, then release it
        if (renderTexture != null)
        {
            renderTexture.Release(); // Release the current texture
        }

        // Create a new RenderTexture with the desired size
        renderTexture = new RenderTexture(width, height, 24); // 24 is the depth buffer size (optional)
        renderTexture.Create(); // Create the new texture
    }

    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game Scene");
    }
    public void ModifyFleet()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Modify Fleet Scene");
    }
    public void MainMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Scene");
    }
    public void UpgradeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Upgrade Scene");
    }
    public void LevelSelectScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level Select Scene");
    }
    public void KillOmegaShip()
    {
        OmegaShip = FindAnyObjectByType<OmegaShipScript>();
        IDamageable damageable = OmegaShip.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Apply damage to the enemy
            damageable.Damage(99999);
        }
    }
    public void DeletePlayerData()
    {
        PlayerPrefsManager.SetInt("FirstTimePlaying", 1);
        foreach (var level in LevelDatabase.Levels)
        {
            if (PlayerPrefsManager.HasKey($"Level{level.Key}_HighScore"))
            {
                PlayerPrefsManager.DeleteKey($"Level{level.Key}_HighScore");
                level.Value.HighScore = PlayerPrefsManager.GetInt($"Level{level.Key}_HighScore");
            }
            if (PlayerPrefsManager.HasKey($"Level{level.Key}_Unlocked"))
            {
                PlayerPrefsManager.DeleteKey($"Level{level.Key}_Unlocked");
                level.Value.HighScore = PlayerPrefsManager.GetInt($"Level{level.Key}_Unlocked");
            }
            if (PlayerPrefsManager.HasKey($"Level{level.Key}_Complete"))
            {
                PlayerPrefsManager.DeleteKey($"Level{level.Key}_Complete");
                level.Value.HighScore = PlayerPrefsManager.GetInt($"Level{level.Key}_Complete");
            }
        }
        PlayerPrefsManager.SetInt("CurrentLevel", 1);
        MainMenuStartButtonScript mainMenuStartButtonScript = FindAnyObjectByType<MainMenuStartButtonScript>();
        mainMenuStartButtonScript.StartGameSwitch();
        Debug.Log("------------first time playing ------ after delete = " + PlayerPrefsManager.GetInt("FirstTimePlaying"));
    }
    public void Settings()
    {
        SettingsCanvas.SetActive(true);
    }
    public void SettingsToMainMenu()
    {
        SettingsCanvas.SetActive(false);
    }
    public void LoadLevelSelected()
    {
        //Debug.Log($"levelSelectManagerScript.currentLevelIndex set to = {levelSelectManagerScript.currentLevelIndex}");
        //PlayerPrefsManager.GetInt("CurrentLevel", levelSelectManagerScript.currentLevelIndex);
        Debug.Log($"current level set to = {PlayerPrefsManager.GetInt("CurrentLevel")}");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Load Scene");
    }
}
