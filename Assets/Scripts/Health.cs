using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private float health;

    private void Start()
    {
        health = maxHealth;
    }

    public void DealDamage(float damage)
    {
        if (health == 0) { return; }

        health = Mathf.Max(0, health - damage);

        Debug.Log(health);
    }
}