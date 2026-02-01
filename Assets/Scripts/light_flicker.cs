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

    [Header("Attack Animation")]
    [SerializeField] private float attackLungeDistance = 0.3f;
    [SerializeField] private float attackLungeDuration = 0.08f;

    [Header("Hit Flash")]
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private int hitFlashCount = 2;

    [Header("Death Animation")]
    [SerializeField] private float deathFadeDuration = 1.2f;
    [SerializeField] private float deathDropDistance = 1f; // How far down the sprite moves

    private Coroutine continuousFlickerCoroutine;
    private GameObject currentEnemyVisual;
    private SpriteRenderer currentSpriteRenderer;

    public static CreepySpotlightFlicker Instance { get; private set; }

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (spotLight == null)
            spotLight = GetComponent<Light>();

        if (enemies.Count == 0)
            Debug.LogWarning("No Enemies ScriptableObjects assigned!");

        if (planePos == null)
            Debug.LogWarning("No planePos assigned!");
    }

    private void Update()
    {
        //Debug.Log("enemy health" + Instance.currentEnemy.maxHealth);
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
        // Stop continuous flicker
        if (continuousFlickerCoroutine != null)
        {
            StopCoroutine(continuousFlickerCoroutine);
            continuousFlickerCoroutine = null;
        }

        // Fade out and drop the enemy sprite
        yield return StartCoroutine(FadeAndDropEnemy());

        // Optional: short flicker after death
        spotLight.enabled = false;
        yield return new WaitForSeconds(blackoutTime);
        spotLight.enabled = true;
        spotLight.intensity = baseIntensity;

        // Clean up
        if (currentEnemyVisual != null)
            Destroy(currentEnemyVisual);

        currentEnemyVisual = null;
        currentSpriteRenderer = null;
        currentEnemy = null;
    }

    private IEnumerator FadeAndDropEnemy()
    {
        if (currentEnemyVisual == null || currentSpriteRenderer == null)
            yield break;

        Vector3 startPosition = currentEnemyVisual.transform.position;
        Vector3 endPosition = startPosition + Vector3.down * deathDropDistance;
        Color startColor = currentSpriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < deathFadeDuration)
        {
            if (currentEnemyVisual != null && currentSpriteRenderer != null)
            {
                // Fade out alpha
                float alpha = Mathf.Lerp(1f, 0f, elapsed / deathFadeDuration);
                Color newColor = startColor;
                newColor.a = alpha;
                currentSpriteRenderer.color = newColor;

                // Move down
                currentEnemyVisual.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / deathFadeDuration);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully transparent and at final position
        if (currentSpriteRenderer != null)
        {
            Color finalColor = startColor;
            finalColor.a = 0f;
            currentSpriteRenderer.color = finalColor;
        }

        if (currentEnemyVisual != null)
        {
            currentEnemyVisual.transform.position = endPosition;
        }
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
        EnemyData enemyTemplate = enemies[Random.Range(0, enemies.Count)];

        // Create a runtime copy using Instantiate
        currentEnemy = Instantiate(enemyTemplate);

        // Apply difficulty scaling
        if (GameManager.Instance != null)
        {
            float healthMultiplier = GameManager.Instance.GetHealthMultiplier();
            float damageMultiplier = GameManager.Instance.GetDamageMultiplier();

            currentEnemy.maxHealth = Mathf.RoundToInt(currentEnemy.maxHealth * healthMultiplier);
            currentEnemy.damage = Mathf.RoundToInt(currentEnemy.damage * damageMultiplier);

            //currentEnemy.

            Debug.Log($"Spawned enemy with {currentEnemy.maxHealth} health ({healthMultiplier:F2}x) and {currentEnemy.damage} damage ({damageMultiplier:F2}x)");
        }

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

    public void EnemyAction()
    {
        if (currentEnemyVisual == null)
            return;

        SoundPlayer.Instance?.PlayEnemyAttackSound();
        StartCoroutine(AttackLunge());
    }

    public void PlayHitFlash()
    {
        if (currentSpriteRenderer != null)
            StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        Color originalColor = currentSpriteRenderer.color;

        for (int i = 0; i < hitFlashCount; i++)
        {
            currentSpriteRenderer.color = hitFlashColor;
            yield return new WaitForSeconds(hitFlashDuration * 0.5f);

            currentSpriteRenderer.color = originalColor;
            yield return new WaitForSeconds(hitFlashDuration * 0.5f);
        }
    }

    private IEnumerator AttackLunge()
    {
        Vector3 startPos = currentEnemyVisual.transform.position;
        Vector3 forwardPos = startPos + -currentEnemyVisual.transform.forward * attackLungeDistance;

        float elapsed = 0f;

        // Lunge forward
        while (elapsed < attackLungeDuration)
        {
            currentEnemyVisual.transform.position =
                Vector3.Lerp(startPos, forwardPos, elapsed / attackLungeDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // Snap back
        while (elapsed < attackLungeDuration)
        {
            currentEnemyVisual.transform.position =
                Vector3.Lerp(forwardPos, startPos, elapsed / attackLungeDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        currentEnemyVisual.transform.position = startPos;
    }

    private IEnumerator JiggleSprite()
    {
        if (currentEnemyVisual == null)
            yield break;

        Vector3 originalPosition = currentEnemyVisual.transform.position;
        float jiggleDuration = 0.5f;
        float jiggleIntensity = 0.2f;
        float jiggleSpeed = 30f;
        float elapsed = 0f;


        while (elapsed < jiggleDuration)
        {
            if (currentEnemyVisual != null)
            {
                // Random offset in X and Y directions
                float offsetX = Mathf.Sin(Time.time * jiggleSpeed) * jiggleIntensity;
                float offsetY = Mathf.Cos(Time.time * jiggleSpeed * 1.3f) * jiggleIntensity;

                currentEnemyVisual.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to original position
        if (currentEnemyVisual != null)
        {
            currentEnemyVisual.transform.position = originalPosition;
        }
    }
}