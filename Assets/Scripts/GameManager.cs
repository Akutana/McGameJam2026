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
 	public int TotalEnemiesDefeated { get; set; } = 0;
    public int NumberofRerolls { get; set; } = 3;
    
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
        SceneManager.LoadScene("light_flicker");

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        NumberofRerolls = 3;
        CurrentTurn = TurnState.PlayerTurn;
        Debug.Log("Player Turn");

        OnPlayerTurnStarted?.Invoke();

        OnTurnChanged?.Invoke(CurrentTurn);
    }

    public void OnEndTurnButtonPressed()
    {
        if (DiceManager.Instance.AreAnyDiceRolling())
        {
            return;
        }
        Debug.Log("End Turn Button Pressed");
        if (CurrentTurn != TurnState.PlayerTurn) return;

        // Clear the hand using the singleton
        HandManager.Instance?.ClearHand();

        // Only deal damage if we're NOT in shopping turn and enemy exists
        if (CurrentTurn != TurnState.ShoppingTurn && CreepySpotlightFlicker.Instance != null && CreepySpotlightFlicker.Instance.currentEnemy != null)
        {
            int damage = DiceManager.Instance.GetTotalDiceValue();
            CreepySpotlightFlicker.Instance.currentEnemy.health -= damage;
            Debug.Log("Dealt " + damage + " damage to enemy.");

            // Check if enemy died
            if (CreepySpotlightFlicker.Instance.currentEnemy.health <= 0)
            {
                TotalEnemiesDefeated++;
                Debug.Log("Enemy defeated! Total enemies defeated: " + TotalEnemiesDefeated);
                CreepySpotlightFlicker.Instance.OnEnemyDied();
                StartShoppingTurn();
                return; // Don't continue to normal enemy turn
            }
        }

        StartEnemyTurn();
    }

    public void StartShoppingTurn()
    {
        CurrentTurn = TurnState.ShoppingTurn;
        Debug.Log("Shopping Turn");
        OnShopTurnStarted?.Invoke();
        OnTurnChanged?.Invoke(CurrentTurn);
    }

    public void EndEnemyTurn()
    {
        CreepySpotlightFlicker.Instance?.EnemyAction();

        StartPlayerTurn();
    }

    public void StartEnemyTurn()
    {
        CurrentTurn = TurnState.EnemyTurn;
        Debug.Log("Enemy Turn");

        OnTurnChanged?.Invoke(CurrentTurn);

   

        Invoke(nameof(EndEnemyTurn), 1f);
    }

    public void RestartGame()
    {
        ResetGame();
        SceneManager.LoadScene("Xavier2");
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