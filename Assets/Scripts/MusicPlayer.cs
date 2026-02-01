using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    [SerializeField] private AudioClip mainTheme;
    [SerializeField] private AudioClip shopTheme;
    [SerializeField] private AudioClip lowHpTheme;
    [SerializeField] private AudioClip gameOverTheme;

    [SerializeField] private float fadeDuration = 1.5f;

    private AudioSource audioSource;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 1f;
    }

    private void OnEnable()
    {
        GameManager.OnTurnChanged += HandleTurnChanged;
    }

    private void OnDisable()
    {
        GameManager.OnTurnChanged -= HandleTurnChanged;
    }

    private void HandleTurnChanged(GameManager.TurnState state)
    {
        if (GameManager.Instance.FinalResult != GameManager.GameResult.None)
            return;

        if (state == GameManager.TurnState.ShoppingTurn)
            Play(shopTheme);
        else
            Play(mainTheme);
    }

    public void PlayGameOver()
    {
        Play(gameOverTheme);
    }

    private void Play(AudioClip clip)
    {
        if (clip == null || audioSource.clip == clip)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeTo(clip));
    }

    private IEnumerator FadeTo(AudioClip newClip)
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play(); 

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
    }
}