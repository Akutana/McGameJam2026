using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer mixer;
    public AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumes();
        musicSource.Play();
    }

    private void Update()
    {
        mixer.GetFloat("MusicVolume", out float v);
        Debug.Log("Mixer music dB = " + v);

    }

    public void LoadVolumes()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        mixer.SetFloat("MusicVolume", Mathf.Log10(music) * 20f);
        mixer.SetFloat("SFXVolume", Mathf.Log10(sfx) * 20f);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
}
