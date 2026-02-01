using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private GameObject test;
    private int price;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Setup(CardData card)
    {
        image.sprite = card.art;
        price = card.price;
    }

    public void OnMouseOver()
    {
        textMesh.text = price.ToString();
        textMesh.enabled = true;
    }

    private void OnMouseExit()
    {
        textMesh.enabled = false;
    }
}
