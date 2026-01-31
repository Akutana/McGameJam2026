using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

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

    private List<GameObject> handCards = new();

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

        for (int i = 0; i < drawNumber; i++)
        {
            DrawCard();
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

        // Pick a random card from availableCards
        CardData data = availableCards[Random.Range(0, availableCards.Count)];

        // Instantiate card prefab
        GameObject g = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);

        // Set card data on CardView
        CardView view = g.GetComponent<CardView>();
        if (view != null)
            view.SetData(data);

        // Add to hand and update positions
        handCards.Add(g);
        UpdateCardPositions();
    }

    public void RemoveCard(GameObject card)
    {
        if (!handCards.Contains(card)) return;

        handCards.Remove(card);
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
            if (card != null)
                Destroy(card);
        }
        handCards.Clear();
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

            handCards[i].transform.DOMove(splinePos, 0.25f);
            handCards[i].transform.DORotateQuaternion(rotation, 0.25f);
        }
    }
}