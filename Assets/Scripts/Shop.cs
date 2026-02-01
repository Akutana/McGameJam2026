using UnityEngine;

public class Shop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GameManager.OnShopTurnStarted += TurnAround;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TurnAround()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f); //Quaternion.Slerp(transform.rotation, transform.rotation * Quaternion.Euler(0f, 180f, 0f), Time.deltaTime * 10f);
    }
}
