using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] private CardData cardData;
    private HandManager handManager;
    private Vector3 originalScale;
    private bool isHovered;
    private TextMeshPro cardNameText;
    private TextMeshPro cardDescText;

    public bool canInteract = false;

    private void Awake()
    {
        originalScale = transform.localScale;

        cardNameText = transform.GetChild(0).GetComponent<TextMeshPro>();
        cardDescText = transform.GetChild(1).GetComponent<TextMeshPro>();

        DisableInteractionTemporarily(2f);
    }

    private void OnMouseEnter()
    {
        if (!canInteract) return;

        transform.DOKill();
        transform.DOScale(originalScale * 1.15f, 0.15f);
        TooltipUI.Instance.Show(cardData.description);
        cardNameText = gameObject.transform.GetChild(0).transform.GetComponent<TextMeshPro>();
        cardDescText = gameObject.transform.GetChild(1).transform.GetComponent<TextMeshPro>();

        cardNameText.text = cardData.cardName;
        cardDescText.text = cardData.description;
        cardDescText.transform.rotation = Quaternion.identity;

        cardDescText.transform.position = new Vector3(handManager.GetMiddleCardPosX(), cardDescText.transform.position.y, cardDescText.transform.position.z);
    }

    private void OnMouseExit()
    {
        if (!canInteract) return;
        if (cardNameText == null || cardDescText == null) return;

        transform.DOKill();
        transform.DOScale(originalScale, 0.15f);
        TooltipUI.Instance.Hide();

        if (cardNameText && cardDescText)
        {
            cardNameText.text = "";
            cardDescText.text = "";
        }
    }

    private void OnMouseDown()
    {
        if (!canInteract) return;

        canInteract = false; 

        transform.DOKill();
        TooltipUI.Instance?.Hide();

        cardData?.Play();

        ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.transform.rotation = Quaternion.identity;
            ps.Play();
            StartCoroutine(FadeThenRemove(ps));
        }

        else
        {
            handManager?.OnCardPlayed();
            handManager?.RemoveCard(gameObject);
        }
    }

    private IEnumerator FadeAndStopParticles(ParticleSystem ps, float duration)
    {
        var emission = ps.emission;
        float startRate = emission.rateOverTime.constant;

        DOTween.To(() => startRate, x => {
            var rate = emission.rateOverTime;
            rate = new ParticleSystem.MinMaxCurve(x);
            emission.rateOverTime = rate;
        }, 0f, duration);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();

        if (sr != null)
            sr.DOFade(0f, duration);

        foreach (var text in texts)
            text.DOFade(0f, duration);

        yield return new WaitForSeconds(duration);

        handManager?.OnCardPlayed();
        handManager?.RemoveCard(gameObject);
    }

    private IEnumerator FadeThenRemove(ParticleSystem ps)
    {
        ps.transform.rotation = Quaternion.identity;
        ps.Play();

        StartCoroutine(FadeAndStopParticles(ps, 1)); // 1s de fade

        yield return new WaitForSeconds(ps.main.duration);
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