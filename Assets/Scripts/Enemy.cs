using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    void Start()
    {
        GetComponent<Health>().Init(data.maxHealth);
    }
}
