using UnityEngine;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField] private CardData cardData;
     private HandManager handManager;
    private Vector3 originalScale;
    private bool isHovered;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        isHovered = true;
        transform.DOScale(originalScale * 1.15f, 0.15f);
        TooltipUI.Instance.Show(cardData.description);
    }

    private void OnMouseExit()
    {
        isHovered = false;
        transform.DOScale(originalScale, 0.15f);
        TooltipUI.Instance.Hide();
    }

    private void OnMouseDown()
    {
        cardData.Play();
        TooltipUI.Instance.Hide();
        handManager.RemoveCard(gameObject);
        Destroy(gameObject);
    }

    public void SetData(CardData data)
    {
        cardData = data;
    }

    public void SetHandManager(HandManager manager)
    {
        handManager = manager;
    }
}