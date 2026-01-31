using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager: MonoBehaviour
{
    public static GameManager Instance;

    private string previousScene;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetGame()
    {
        Debug.Log("Resetting game");
    }

    public void StartGame()
    {
        Debug.Log("hujfdkshjkhs");  
        SceneManager.LoadScene("GameScene");
    }

    public void RestartGame()
    {
        ResetGame();
        SceneManager.LoadScene("GameScene");
    }

    public void DisplaySettings()
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Settings");
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(previousScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
