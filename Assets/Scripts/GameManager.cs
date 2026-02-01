using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.Interpolators;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI currencyDisplay;
    public Image currencyIcon;
    private string previousScene;
    public static event Action OnPlayerTurnStarted;
    public static event System.Action<GameManager.TurnState> OnTurnChanged;
    public static event Action OnShopTurnStarted;
    public static event Action OnShopTransitionFinised;
    public int TotalEnemiesDefeated { get; set; } = 0;
    public int NumberofRerolls { get; set; } = 3;
    public int Currency { get; set; } = 0;

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

    public void SetGameResult(GameResult result)
    {
        FinalResult = result;
    }

    [Header("Difficulty Scaling")]
    [SerializeField] private float healthIncreasePerKill = 2f; // +2 health per enemy killed
    [SerializeField] private float damageIncreasePerKill = 1f;

    [Header("Turn Timing")]
    [SerializeField] private float delayBeforePlayerTurn = 1.5f; // Delay after enemy attacks
    [SerializeField] private float deathAnimationDelay = 1.5f; // Delay before going to shop after enemy death

    public float GetHealthMultiplier()
    {
        return 1f + (TotalEnemiesDefeated * healthIncreasePerKill / 10f); // Scales gradually
    }

    public float GetDamageMultiplier()
    {
        return 1f + (TotalEnemiesDefeated * damageIncreasePerKill / 10f); // Scales gradually
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
        currencyDisplay.text = Currency.ToString();
        DontDestroyOnLoad(gameObject);
    }

    public void ResetGame()
    {
        TotalEnemiesDefeated = 0;
        Currency = 0;
        currencyDisplay.text = Currency.ToString();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("light_flicker");
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        StartPlayerTurn();
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "light_flicker")
        {
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
            ShowCurrency();
            StartPlayerTurn();
        }
    }

    public void ShowCurrency()
    {
        Color c = currencyDisplay.color;
        c.a = 1f;
        currencyDisplay.color = c;


        c = currencyIcon.color;
        c.a = 1f;
        currencyIcon.color = c;
    }

    public void HideCurrency()
    {
        Color c = currencyDisplay.color;
        c.a = 0f;
        currencyDisplay.color = c;

        c = currencyIcon.color;
        c.a = 0f;
        currencyIcon.color = c;
    }

    public void UpdateCurrencyDisplay()
    {
        currencyDisplay.text = Currency.ToString();
    }

    public void StartPlayerTurn()
    {
        NumberofRerolls = 3;
        CurrentTurn = TurnState.PlayerTurn;

        OnPlayerTurnStarted?.Invoke();
        OnTurnChanged?.Invoke(CurrentTurn);
    }

    public void OnEndTurnButtonPressed()
    {

        // Handle shopping turn FIRST, before any other checks
        if (CurrentTurn == TurnState.ShoppingTurn)
        {
            Debug.Log("In shopping turn, ending shop now");
            EndShoppingTurn();
            return;
        }

        // Check dice rolling only for combat turns
        if (DiceManager.Instance == null)
        {
            Debug.LogError("DiceManager is null!");
            return;
        }

        if (DiceManager.Instance.AreAnyDiceRolling())
        {
            Debug.Log("Dice still rolling, cannot end turn");
            return;
        }

        if (CurrentTurn != TurnState.PlayerTurn)
        {
            Debug.LogWarning($"Not player turn (current: {CurrentTurn}), ignoring");
            return;
        }


        // Clear the hand using the singleton
        HandManager.Instance?.ClearHand();

        // Only deal damage if enemy exists
        if (CreepySpotlightFlicker.Instance != null && CreepySpotlightFlicker.Instance.currentEnemy != null)
        {
            int damage = DiceManager.Instance.GetTotalDiceValue();
            CreepySpotlightFlicker.Instance.currentEnemy.maxHealth -= damage;
            Debug.Log("Dealt " + damage + " damage to enemy.");

            CreepySpotlightFlicker.Instance?.PlayHitFlash();
            SoundPlayer.Instance?.PlayAttackenemySound();

            // Check if enemy died
            if (CreepySpotlightFlicker.Instance.currentEnemy.maxHealth <= 0)
            {
                TotalEnemiesDefeated++;

                if (TotalEnemiesDefeated >= 5)
                {
                    FinalResult = GameResult.Win;
                    SceneManager.LoadScene("EndMenu");
                    return;
                }

                // Trigger death animation
                CreepySpotlightFlicker.Instance.OnEnemyDied();
                Currency += 5;
                currencyDisplay.text = Currency.ToString();

                // Wait for death animation before going to shop
                Invoke(nameof(StartShoppingTurn), deathAnimationDelay);
                return;
            }
        }

        StartEnemyTurn();
    }

    public void EndShoppingTurn()
    {
        Debug.Log("=== EndShoppingTurn called ===");
        OnShopTransitionFinised?.Invoke();

        // Spawn a new enemy when leaving the shop
        if (CreepySpotlightFlicker.Instance != null)
        {
            Debug.Log("Spawning new enemy after shop");
            CreepySpotlightFlicker.Instance.IntroduceEnemy();
        }
        else
        {
            Debug.LogError("CreepySpotlightFlicker.Instance is null!");
        }

        StartPlayerTurn();
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

        // Add delay before returning to player turn
        Invoke(nameof(StartPlayerTurn), delayBeforePlayerTurn);
    }

    public void StartEnemyTurn()
    {
        CurrentTurn = TurnState.EnemyTurn;
        Debug.Log("Enemy Turn");

        OnTurnChanged?.Invoke(CurrentTurn);

        PlayerStats.Instance.health.DealDamage(CreepySpotlightFlicker.Instance.currentEnemy.damage);

        MusicPlayer.Instance.PlayerisLowOnHealth = PlayerStats.Instance.IsLowOnHealth();
  
        Invoke(nameof(EndEnemyTurn), 1f);
    }

    public void RestartGame()
    {
        ResetGame();
        SceneManager.LoadScene("StartingMenu");
    }

    public void DisplaySettings()
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Settings");
        HideCurrency();
    }

    public void LoadPreviousScene()
    {
        // Safety fallback
        if (string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene("StartingMenu");
            HideCurrency();
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