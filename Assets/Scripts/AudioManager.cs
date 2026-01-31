using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer mixer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitDefaultsIfNeeded();
        LoadVolumes();
    }

    void InitDefaultsIfNeeded()
    {
        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", 1f);

        if (!PlayerPrefs.HasKey("SFXVolume"))
            PlayerPrefs.SetFloat("SFXVolume", 1f);
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
