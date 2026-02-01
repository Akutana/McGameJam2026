using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth;
    public Health health;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        health = GetComponent<Health>();
        
        health.Init(maxHealth);
    }

    private void Update()
    {
        Debug.Log("player health" + health.GetCurrentHealth());
    }
}
