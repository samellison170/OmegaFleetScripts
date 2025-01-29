using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuStartButtonScript : MonoBehaviour
{
    public GameObject mainMenuStartButton;
    [SerializeField] public GameObject VideoPlayerObject;
    private VideoPlayer cinematic;
    [SerializeField] public Image fadeInImage;
    private RenderTexture renderTexture;
    private void Start()
    {
        Debug.Log("------------first time playing " + PlayerPrefsManager.GetInt("FirstTimePlaying"));
        if (PlayerPrefsManager.GetInt("FirstTimePlaying") == 1)
        {
            PlayerPrefsManager.SetInt("CurrentLevel", 1);
            StartGameSwitch();
        }
        else
        {
            LevelSelectSwitch();
        }
    }
    public void LevelSelectSwitch()
    {
        TMP_Text buttonText = gameObject.GetComponentInChildren<TMP_Text>();
        buttonText.text = "LEVEL SELECT";
    }
    public void StartGameSwitch()
    {
        TMP_Text buttonText = gameObject.GetComponentInChildren<TMP_Text>();
        buttonText.text = "START";
    }
    public void StartGame()
    {
        Debug.Log("------------first time playing " + PlayerPrefsManager.GetInt("FirstTimePlaying"));
        if (PlayerPrefsManager.GetInt("FirstTimePlaying") == 1)
        {
            VideoPlayerObject.SetActive(false); // Hide the video initially
            SetRenderTextureResolution(1080, 1920);
            VideoPlayerObject.SetActive(true);
            cinematic = VideoPlayerObject.GetComponentInChildren<VideoPlayer>();
            cinematic.playOnAwake = false;
            cinematic.time = 0;
            cinematic.Prepare();
            cinematic.prepareCompleted += OnVideoPrepared;
            //PlayerPrefsManager.SetInt("FirstTimePlaying", 0);
        }
        if (PlayerPrefsManager.GetInt("FirstTimePlaying") == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level Select Scene");
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
    IEnumerator LoadSceneAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay (91 seconds)
        SceneManager.LoadScene("Load Scene"); // Load the specified scene
    }
    void UpdateAlpha(float alpha)
    {
        // Update the alpha of the image color
        Color color = fadeInImage.color;
        color.a = alpha;
        fadeInImage.color = color;
    }
    public void SkipCinematic()
    {
        SceneManager.LoadScene("Load Scene");
    }
}
