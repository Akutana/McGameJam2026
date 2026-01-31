using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    //public AudioMixer mixer;

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
    }

    public void LoadVolumes()
    {
        //mixer.SetFloat("MusicVolume", Mathf.Log10(music) * 20);
        //mixer.SetFloat("SFXVolume", Mathf.Log10(sfx) * 20);

    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        //mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        //mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
}
