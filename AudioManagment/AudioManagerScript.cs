using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManagerScript : MonoBehaviour
{
    public static AudioManagerScript Instance { get; private set; }
    private List<AudioSource> audioSources = new List<AudioSource>();
    public AudioSource audioSource;
    public float generalVolumeLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the manager across scenes
            FindAllAudioSources();
        }
        else
        {
            Destroy(gameObject); // Ensures there's only one instance
        }
    }
    private void Start()
    {
        SetVolume(PlayerPrefsManager.GetFloat("Volume Level"));
        Debug.Log("AudioManager start function working");
    }
    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (SceneManager.GetActiveScene().name == "Load Scene")
    //    {
    //        Debug.Log("AudioManager Load Scene working");
    //    }
    //}
    // Finds all AudioSources in the scene
    void FindAllAudioSources()
    {
        audioSources.AddRange(FindObjectsOfType<AudioSource>());
    }

    // Controls volume for all AudioSources
    public void SetVolume(float volume)
    {
        FindAllAudioSources();
        foreach (var audioSource in audioSources)
        {
            if(audioSource != null)
            {
                if (volume == 0)
                {
                    audioSource.mute = true;
                    generalVolumeLevel = volume;
                }
                audioSource.mute = false;
                audioSource.volume = volume;
                generalVolumeLevel = volume;
                //Debug.Log(audioSource.name);
            }
            else
            {
                Debug.Log("Audio Source is NULL");
            }

        }
        PlayerPrefsManager.SetFloat("Volume Level", volume);
        //Debug.Log(PlayerPrefsManager.GetFloat("Volume Level"));
    }
    //public void PlaySampleSound()
    //{
    //    sampleAudioSource.Play();
    //}
    // Mute or unmute all AudioSources
    public void SetMute(bool isMuted)
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.mute = isMuted;
        }
    }

    // Play all audio sources
    public void PlayAll()
    {
        foreach (var audioSource in audioSources)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    // Pause all audio sources
    public void PauseAll()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Pause();
        }
    }
    public void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log("playing " + clip);
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

}
