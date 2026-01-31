using UnityEngine;
using UnityEngine.UI;

public class UIButtonActions : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(
                PlayerPrefs.GetFloat("MusicVolume", 1f)
            );
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(
                PlayerPrefs.GetFloat("SFXVolume", 1f)
            );
        }
    }

    public void OnMusicChanged(float value)
    {
        Debug.Log("hgfjsdagkfhjakgdshfjkgshdafkghsajgfdks");
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    public void QuitGame() => GameManager.Instance.QuitGame();
    public void RestartGame() => GameManager.Instance.RestartGame();
    public void StartGame() => GameManager.Instance.StartGame();
    public void DisplaySettings() => GameManager.Instance.DisplaySettings();
    public void ExitSettings() => GameManager.Instance.LoadPreviousScene();
}
