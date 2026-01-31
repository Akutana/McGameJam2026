using UnityEngine;
using System.Collections;

public class CreepySpotlightFlicker : MonoBehaviour
{
    public Light spotLight;
    public GameObject[] planePrefabs; // Array of custom plane prefabs
    public GameObject planePos;       // The position where planes should appear

    [Header("Intensity")]
    public float baseIntensity = 400.0f;
    public float flickerIntensity = 50.0f;

    [Header("Timing")]
    public float shortFlickerDuration = 0.2f;       // Flicker before blackout
    public float blackoutTime = 0.2f;               // Blackout duration
    public float continuousFlickerMinTime = 0.03f;  // Flicker while plane is visible
    public float continuousFlickerMaxTime = 0.15f;
    public float moveDistance = 5f;                 // How far the plane moves up
    public float moveDuration = 1f;                 // Time to move up

    private Coroutine continuousFlickerCoroutine;
    private GameObject currentPlane = null;

    void Start()
    {
        if (spotLight == null)
            spotLight = GetComponent<Light>();

        if (planePrefabs.Length == 0)
            Debug.LogWarning("No plane prefabs assigned!");

        if (planePos == null)
            Debug.LogWarning("No planePos assigned!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(TogglePlaneSequence());
        }
    }

    IEnumerator TogglePlaneSequence()
    {
        // Short flicker before blackout
        yield return StartCoroutine(ShortFlicker(shortFlickerDuration));

        // Blackout
        spotLight.enabled = false;

        if (currentPlane == null)
        {
            // Pick a random plane from the array
            GameObject prefab = planePrefabs[Random.Range(0, planePrefabs.Length)];

            // Instantiate at planePos position and rotation
            currentPlane = Instantiate(prefab, planePos.transform.position, planePos.transform.rotation);

            // Move plane up along local Y-axis
            yield return StartCoroutine(MovePlaneUp(currentPlane, moveDistance, moveDuration));

            // Start continuous flicker
            continuousFlickerCoroutine = StartCoroutine(ContinuousFlicker());
        }
        else
        {
            // Stop continuous flicker
            if (continuousFlickerCoroutine != null)
                StopCoroutine(continuousFlickerCoroutine);

            // Destroy current plane
            Destroy(currentPlane);
            currentPlane = null;

            // One last short flicker after disappearance
            yield return StartCoroutine(ShortFlicker(shortFlickerDuration));
        }

        yield return new WaitForSeconds(blackoutTime);
        spotLight.enabled = true;
        spotLight.intensity = baseIntensity;
    }

    IEnumerator ShortFlicker(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            spotLight.intensity = baseIntensity + Random.Range(-flickerIntensity, flickerIntensity);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spotLight.intensity = baseIntensity;
    }

    IEnumerator ContinuousFlicker()
    {
        while (true)
        {
            spotLight.intensity = baseIntensity + Random.Range(-flickerIntensity, flickerIntensity);
            yield return new WaitForSeconds(Random.Range(continuousFlickerMinTime, continuousFlickerMaxTime));
        }
    }

    IEnumerator MovePlaneUp(GameObject plane, float distance, float duration)
    {
        // Start at planePos position
        Vector3 startPos = planePos.transform.position;
        Vector3 endPos = startPos + plane.transform.up * distance; // Move along local Y-axis
        float elapsed = 0f;

        while (elapsed < duration)
        {
            plane.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        plane.transform.position = endPos;
    }
}







