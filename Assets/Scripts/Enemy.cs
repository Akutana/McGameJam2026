using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    void Start()
    {
        Debug.Log("rgfhjshfjkshjkdhjks");
        GetComponent<Health>().Init(data.maxHealth);
    }

    private void Update()
    {
        Debug.Log("enemy health" + GetComponent<Health>().GetCurrentHealth());
    }
}
