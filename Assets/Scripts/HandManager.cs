using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Reflection;

public enum CardType
{
    HEAL,
    TAKE_DAMAGE,
    REROLL
}

[System.Serializable]
public class Card
{
    public GameObject cardObject;
    public CardType cardType;
    public float cardValue;

    public Card(GameObject obj, CardType type, float value)
    {
        cardObject = obj;
        cardType = type;
        cardValue = value;
    }
}

public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }

    [Header("Hand Settings")]
    [SerializeField] private int maxHandSize = 5;
    [SerializeField] private int drawNumber = 3;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SplineContainer splineContainer;

    [Header("Card Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<CardData> availableCards;

    [SerializeField] private CreepySpotlightFlicker lightFlicker;

    private List<Card> handCards = new();

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject currentEnemy;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Draw initial hand only if it's already player turn
        TryDrawInitialHand();
    }

    private void TryDrawInitialHand()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentTurn != GameManager.TurnState.PlayerTurn) return;

        Debug.Log("HandManager: Drawing initial hand");

        StartCoroutine(IntroduceEnemyDelay());

        StartCoroutine(DrawCardsWithDelay(drawNumber, 0.15f));
    }

    private IEnumerator IntroduceEnemyDelay()
    {
        yield return new WaitForSeconds(2f);
        lightFlicker.IntroduceEnemy();
    }

    private IEnumerator DrawCardsWithDelay(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(delay);
        }
    }

    private void OnEnable()
    {
        GameManager.OnPlayerTurnStarted += HandlePlayerTurnStarted;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerTurnStarted -= HandlePlayerTurnStarted;
    }

    private void HandlePlayerTurnStarted()
    {
        Debug.Log("HandManager: Player Turn Started - Drawing Cards");

        for (int i = 0; i < drawNumber; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (handCards.Count >= maxHandSize) return;
        if (availableCards.Count == 0) return;

        CardData data = availableCards[UnityEngine.Random.Range(0, availableCards.Count)];
        GameObject g = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);

        CardView view = g.GetComponent<CardView>();
        if (view != null)
        {
            view.SetData(data);
            view.SetHandManager(this);
        }

        // Création de la Card
        Card newCard = new Card(
            g,
            data.type,
            data.value
        );

        handCards.Add(newCard);
        UpdateCardPositions();

    }

    public void RemoveCard(GameObject cardObject)
    {
        int index = handCards.FindIndex(c => c.cardObject == cardObject);
        if (index == -1) return;

        Destroy(handCards[index].cardObject);
        handCards.RemoveAt(index);

        UpdateCardPositions();
    }


    public void AddCardToDeck(CardData newCard)
    {
        availableCards.Add(newCard);
    }

    public void ClearHand()
    {
        foreach (var card in handCards)
        {
            if (card.cardObject != null)
                Destroy(card.cardObject);
        }
        handCards.Clear();
    }

    private IEnumerator DisableAllCardsTemporarily(float duration)
    {
        foreach (var card in handCards)
        {
            if (card.cardObject == null) continue;

            CardView view = card.cardObject.GetComponent<CardView>();
            if (view != null)
            {
                view.DisableInteractionTemporarily(duration);
            }
        }

        yield return null;
    }

    private void UpdateCardPositions()
    {
        if (handCards.Count == 0) return;

        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < handCards.Count; i++)
        {
            float t = firstCardPosition + i * cardSpacing;

            Vector3 splinePos = splineContainer.transform.TransformPoint(spline.EvaluatePosition(t));
            Vector3 forward = spline.EvaluateTangent(t);
            Vector3 up = spline.EvaluateUpVector(t);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            handCards[i].cardObject.transform.DOMove(splinePos, 0.25f);
            handCards[i].cardObject.transform.DORotateQuaternion(rotation, 0.25f);
        }
    }

    public float GetMiddleCardPosX()
    {
        if (handCards.Count == 3)
            return handCards[1].cardObject.transform.position.x;
        else if (handCards.Count == 2)
        {
            float biggerX;
            float smallerX;

            biggerX = Mathf.Max(handCards[1].cardObject.transform.position.x, handCards[0].cardObject.transform.position.x);
            smallerX = Mathf.Min(handCards[1].cardObject.transform.position.x, handCards[0].cardObject.transform.position.x);

            return smallerX + (biggerX - smallerX) / 2;
        }

        else if (handCards.Count == 1)
            return handCards[0].cardObject.transform.position.x;
        else
            return 0.0f;
    }

    public void PlayCard(Card card)
    {

        StartCoroutine(DisableAllCardsTemporarily(1f));

        if (card.cardType == CardType.REROLL)
        {
            GameManager.Instance.NumberofRerolls += (int)card.cardValue;
        }

        else if (card.cardType == CardType.HEAL)
        {
            player.GetComponent<Health>().Heal(card.cardValue);
        }

        else
        {
            //currentEnemy.GetComponent<Health>().DealDamage(card.cardValue);
            Debug.Log("Damage card played by player");
        }
    }
}