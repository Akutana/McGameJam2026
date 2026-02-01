using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private GameObject test;

    private CardData cardData;
    private int price;
    private bool hasBeenBought;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Setup(CardData card)
    {
        cardData = card;
        price = card.price;

        image.sprite = card.art;
        image.enabled = true;

        hasBeenBought = false;   
        textMesh.enabled = false;
    }

    public void OnMouseOver()
    {
        if (hasBeenBought) { return;  }
        textMesh.text = price.ToString();
        textMesh.enabled = true;

        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instance.Currency >= price)
            {
                hasBeenBought = true;
                Debug.Log("adding card");
                image.enabled = false;
                HandManager.Instance.AddCardToDeck(cardData);
                GameManager.Instance.Currency -= price;
                GameManager.Instance.UpdateCurrencyDisplay();
            }
        }
    }

    private void OnMouseExit()
    {
        textMesh.enabled = false;
    }
}
