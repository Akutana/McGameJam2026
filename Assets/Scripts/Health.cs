using UnityEngine;

public class Health : MonoBehaviour
{
    private float maxHealth;
    private float health;

    public float GetCurrentHealth()
    {
        return health;
    }

    public void Init(float max)
    {
        maxHealth = max;
        health = maxHealth;
    }

    public void DealDamage(float damage)
    {
        if (health == 0) { return; }

        health = Mathf.Max(0, health - damage);
    }

    public void Heal(float healAmount)
    {
        if (health == maxHealth) { return; }

        health = Mathf.Min(maxHealth, health + healAmount);
    }
}