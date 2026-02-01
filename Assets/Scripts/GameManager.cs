using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines.Interpolators;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private string previousScene;
    public static event Action OnPlayerTurnStarted;
    public static event System.Action<GameManager.TurnState> OnTurnChanged;
    public static event Action OnShopTurnStarted;

    public enum TurnState
    {
        None,
        PlayerTurn,
        EnemyTurn,
        ShoppingTurn
    }

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
        Debug.Log("Resetting game");
        // reset game state here later
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Xavier2");

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        CurrentTurn = TurnState.PlayerTurn;
        Debug.Log("Player Turn");

        OnPlayerTurnStarted?.Invoke();

        OnTurnChanged?.Invoke(CurrentTurn);
    }

   public void OnEndTurnButtonPressed()
{
    if (CurrentTurn != TurnState.PlayerTurn) return;

    // Clear the hand using the singleton
    HandManager.Instance?.ClearHand();

    Debug.Log("Player pressed End Turn");
    StartEnemyTurn();
}

    public void StartEnemyTurn()
    {
        CurrentTurn = TurnState.EnemyTurn;
        Debug.Log("Enemy Turn");

        OnTurnChanged?.Invoke(CurrentTurn);

        // TEMP: automatically end enemy turn after 1 second
        Invoke(nameof(EndEnemyTurn), 1f);
    }

    public void EndEnemyTurn()
    {
        if (true)
        {
            MoveToShop();
        }
        StartPlayerTurn();
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

    private void MoveToShop()
    {
        OnShopTurnStarted?.Invoke();
    }
}