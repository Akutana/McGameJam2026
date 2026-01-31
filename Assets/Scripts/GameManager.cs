using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private string previousScene;
    public static event Action OnPlayerTurnStarted;

    public TurnState CurrentTurn { get; private set; } = TurnState.None;
    public enum TurnState
    {
        None,
        PlayerTurn,
        EnemyTurn,
        Shopping
    }

    private void Awake()
    {
        // Singleton pattern
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
        // reset game state here later
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        CurrentTurn = TurnState.PlayerTurn;
        Debug.Log("Player Turn");

        OnPlayerTurnStarted?.Invoke();
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
        // Safety fallback
        if (string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

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