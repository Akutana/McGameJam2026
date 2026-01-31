using UnityEngine;

public class UIButtonActions : MonoBehaviour
{
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
    
    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void DisplaySettings()
    {
        GameManager.Instance.DisplaySettings();
    }

    public void ExitSettings()
    {
        GameManager.Instance.LoadPreviousScene();
    }
}
