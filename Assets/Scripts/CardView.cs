using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CardView : MonoBehaviour
{
    [SerializeField] private CardData cardData;
    private HandManager handManager;
    private Vector3 originalScale;

    public bool canInteract = false;

    private void Awake()
    {
        originalScale = transform.localScale;
        DisableInteractionTemporarily(2f); 
    }

    private void OnMouseEnter()
    {
        if (!canInteract) return;

        transform.DOKill();
        transform.DOScale(originalScale * 1.15f, 0.15f);

        TooltipUI.Instance?.Show(cardData.description);
    }

    private void OnMouseExit()
    {
        if (!canInteract) return;

        transform.DOKill();
        transform.DOScale(originalScale, 0.15f);

        TooltipUI.Instance?.Hide();
    }

    private void OnMouseDown()
    {
        if (!canInteract) return;

        canInteract = false; 

        transform.DOKill();
        TooltipUI.Instance?.Hide();

        cardData?.Play();

        handManager?.OnCardPlayed();
        handManager?.RemoveCard(gameObject);
    }

    public void SetData(CardData data)
    {
        cardData = data;
    }

    public void SetHandManager(HandManager manager)
    {
        handManager = manager;
    }

    public void DisableInteractionTemporarily(float duration)
    {
        StartCoroutine(InteractionCooldown(duration));
    }

    private IEnumerator InteractionCooldown(float duration)
    {
        canInteract = false;
        yield return new WaitForSeconds(duration);
        canInteract = true;
    }
}