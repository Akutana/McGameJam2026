using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreepySpotlightFlicker : MonoBehaviour
{
    [Header("References")]
    public Light spotLight;
    [SerializeField] private List<EnemyData> enemies;
    [SerializeField] public EnemyData currentEnemy;
    [SerializeField] private Transform planePos;
    [SerializeField] private GameObject enemyVisualPrefab;

    [Header("Light Intensity")]
    public float baseIntensity = 400f;
    public float flickerIntensity = 50f;

    [Header("Timing")]
    public float shortFlickerDuration = 0.2f;
    public float blackoutTime = 0.2f;
    public float continuousFlickerMinTime = 0.03f;
    public float continuousFlickerMaxTime = 0.15f;
    public float moveDistance = 5f;
    public float moveDuration = 1f;

    [Header("Sprite Flicker")]
    [SerializeField] private float minSpriteAlpha = 0.2f;
    [SerializeField] private float maxSpriteAlpha = 1f;

    private Coroutine continuousFlickerCoroutine;
    private GameObject currentEnemyVisual;
    private SpriteRenderer currentSpriteRenderer;

    public static CreepySpotlightFlicker Instance { get; private set; }

    private void Start()
    {
        if (spotLight == null)
            spotLight = GetComponent<Light>();

        if (enemies.Count == 0)
            Debug.LogWarning("No Enemies ScriptableObjects assigned!");

        if (planePos == null)
            Debug.LogWarning("No planePos assigned!");

    }

    private void Update()
    {

    }

    public void IntroduceEnemy()
    {
        StartCoroutine(ToggleEnemySequence());
    }

    private IEnumerator ToggleEnemySequence()
    {
        // Pre blackout flicker
        yield return StartCoroutine(ShortFlicker(shortFlickerDuration));

        // Blackout
        spotLight.enabled = false;

        if (currentEnemyVisual == null)
        {
            SpawnEnemyVisual();
            yield return StartCoroutine(MoveEnemyUp());
            continuousFlickerCoroutine = StartCoroutine(ContinuousFlicker());
        }
        else
        {
            yield return StartCoroutine(RemoveCurrentEnemy());
        }

        yield return new WaitForSeconds(blackoutTime);

        spotLight.enabled = true;
        spotLight.intensity = baseIntensity;
    }

    public void OnEnemyDied()
    {
        if (currentEnemyVisual == null)
            return;

        StartCoroutine(EnemyDeathSequence());
    }

    private IEnumerator EnemyDeathSequence()
    {
        // Optional: death flicker / delay
        spotLight.enabled = false;

        yield return StartCoroutine(RemoveCurrentEnemy());

        yield return new WaitForSeconds(blackoutTime);

        spotLight.enabled = true;
        spotLight.intensity = baseIntensity;
    }

    private IEnumerator RemoveCurrentEnemy()
    {
        if (continuousFlickerCoroutine != null)
        {
            StopCoroutine(continuousFlickerCoroutine);
            continuousFlickerCoroutine = null;
        }

        if (currentEnemyVisual != null)
            Destroy(currentEnemyVisual);

        currentEnemyVisual = null;
        currentSpriteRenderer = null;
        currentEnemy = null;

        yield return StartCoroutine(ShortFlicker(shortFlickerDuration));
    }

    private void SpawnEnemyVisual()
    {
        currentEnemy = enemies[Random.Range(0, enemies.Count)];

        currentEnemyVisual = Instantiate(
            enemyVisualPrefab,
            planePos.position,
            planePos.rotation
        );

        currentSpriteRenderer = currentEnemyVisual.GetComponent<SpriteRenderer>();
        currentSpriteRenderer.sprite = currentEnemy.art;

        // Start invisible
        Color c = currentSpriteRenderer.color;
        c.a = 0f;
        currentSpriteRenderer.color = c;

        StartCoroutine(FadeSprite(0f, 1f, 0.3f));
    }
    private IEnumerator ShortFlicker(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            spotLight.intensity = baseIntensity + Random.Range(-flickerIntensity, flickerIntensity);

            if (currentSpriteRenderer != null)
            {
                Color c = currentSpriteRenderer.color;
                c.a = Random.Range(minSpriteAlpha, maxSpriteAlpha);
                currentSpriteRenderer.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        spotLight.intensity = baseIntensity;
    }

    private IEnumerator ContinuousFlicker()
    {
        while (true)
        {
            spotLight.intensity = baseIntensity + Random.Range(-flickerIntensity, flickerIntensity);

            if (currentSpriteRenderer != null)
            {
                Color c = currentSpriteRenderer.color;
                c.a = Random.Range(minSpriteAlpha, maxSpriteAlpha);
                currentSpriteRenderer.color = c;
            }

            yield return new WaitForSeconds(
                Random.Range(continuousFlickerMinTime, continuousFlickerMaxTime)
            );
        }
    }

    private IEnumerator MoveEnemyUp()
    {
        Vector3 startPos = planePos.position;
        Vector3 endPos = startPos + -Vector3.forward * moveDistance;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            if (currentEnemyVisual != null)
            {
                currentEnemyVisual.transform.position =
                    Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeSprite(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (currentSpriteRenderer != null)
            {
                Color c = currentSpriteRenderer.color;
                c.a = Mathf.Lerp(from, to, elapsed / duration);
                currentSpriteRenderer.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}