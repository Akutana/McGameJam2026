using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.Interpolators;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager Instance;

    public static event Action OnPlayerTurnStarted;
    public static event Action OnShopTurnStarted;
    public static event Action OnEnemyTurnStarted;
    public static event System.Action<GameManager.TurnState> OnTurnChanged;

    public enum TurnState
    {
        None,
        PlayerTurn,
        EnemyTurn,
        ShoppingTurn
    }

    public enum GameResult
    {
        None,
        Win,
        Lose
    }

    public GameResult FinalResult { get; private set; } = GameResult.None;
    public TurnState CurrentTurn { get; private set; } = TurnState.None;

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
    }

    public void StartGame()
    {
        SceneManager.LoadScene("light_flicker");
    }

    public void RestartGame()
    {
        ResetGame();
        SceneManager.LoadScene("StartingMenu");
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