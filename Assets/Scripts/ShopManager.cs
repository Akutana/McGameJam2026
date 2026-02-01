using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private int shopSlotCount = 3;
    public List<CardData> inventory;
    public List<CardData> currentItems;
    public List<ShopItem> shopItems;

    private void Awake()
    {
        if (currentItems == null)
            currentItems = new List<CardData>();
    }

    private void OnEnable()
    {
        GameManager.OnShopTurnStarted += RestockShop;
    }

    private void OnDisable()
    {
        GameManager.OnShopTurnStarted -= RestockShop;
    }

    private void RestockShop()
    {
        GenerateCurrentItems();
    }

    private void GenerateCurrentItems()
    {
        currentItems.Clear();

        if (inventory == null || inventory.Count == 0)
        {
            Debug.LogWarning("Shop inventory is empty");
            return;
        }

        List<CardData> temp = new List<CardData>(inventory);
        int amount = Mathf.Min(shopSlotCount, temp.Count);

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, temp.Count);
            currentItems.Add(temp[randomIndex]);
            temp.RemoveAt(randomIndex);
        }

        DisplayCurrentItems();
    }

    private void DisplayCurrentItems()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (i < currentItems.Count)
            {
                shopItems[i].gameObject.SetActive(true);
                shopItems[i].Setup(currentItems[i]);
            }
            else
            {
                shopItems[i].gameObject.SetActive(false);
            }
        }
    }
}