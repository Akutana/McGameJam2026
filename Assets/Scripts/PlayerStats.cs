using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public float maxHealth;
    public Health health;

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
    }

    void Start()
    {
        health.Init(maxHealth);
    }

    private void Update()
    {
        Debug.Log("player health: " + health.GetCurrentHealth());
    }
}