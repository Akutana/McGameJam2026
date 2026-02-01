using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private int shopSlotCount = 3;
    public List<CardData> inventory;
    public List<CardData> currentItems;
    public List<ShopItem> shopItems;

    private void Start()
    {
        GenerateCurrentItems();
    }

    private void GenerateCurrentItems()
    {
        currentItems.Clear();
        List<CardData> temp = new List<CardData>(inventory);

        if (temp.Count == 0 ) { return; }

        for (int i = 0; i < shopSlotCount; i++)
        {
            int randomIndex = Random.Range(0, temp.Count);
            currentItems.Add(temp[randomIndex]);
            temp.RemoveAt(randomIndex);
        }

        DisplayCurrentItems();
    }

    private void DisplayCurrentItems()
    {
        for (int i = 0; i < shopSlotCount; i++)
        {
            shopItems[i].Setup(currentItems[i]);
        }
    }
}
