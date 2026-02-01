using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public float maxHealth;
    public Health health;

    [Header("Death Settings")]
    [SerializeField] private Image fadeImage; // Use Image instead of CanvasGroup
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private string endMenuSceneName = "EndMenu";

    private bool isDead = false;

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

        // Get health component
        health = GetComponent<Health>();

        // Make sure fade image starts invisible
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);

            // Make sure the canvas persists too
            Canvas canvas = fadeImage.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                DontDestroyOnLoad(canvas.gameObject);
            }
        }
    }

    void Start()
    {
        health.Init(maxHealth);
    }

    private void Update()
    {
        // Check if player died
        if (health.GetCurrentHealth() <= 0 && !isDead)
        {
            isDead = true;
            Debug.Log("Player died! Starting fade...");
            StartCoroutine(HandleDeath());
        }

        Debug.Log(health.GetCurrentHealth());
    }

    private IEnumerator HandleDeath()
    {
        Debug.Log("HandleDeath started");

        // Activate fade image
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Debug.Log("Fade image activated");
        }
        else
        {
            Debug.LogError("Fade image is null!");
            yield break;
        }

        // Fade to black
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully black
        color.a = 1f;
        fadeImage.color = color;

        Debug.Log("Fade complete, loading end menu");

        GameManager.Instance.SetGameResult(GameManager.GameResult.Lose);

        // Load end menu
        SceneManager.LoadScene(endMenuSceneName);

        //color.a = 0f;
        //fadeImage.color = color;
    }
}