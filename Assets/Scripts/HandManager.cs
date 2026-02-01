using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Reflection;

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

    private List<GameObject> handCards = new();

    private Vector3 cameraOriginalPos;
    private CameraParallax cameraParallax;
    private float drawDelay;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cameraParallax = Camera.main.GetComponent<CameraParallax>();
    }

    private void Start()
    {
        // Draw initial hand only if it's already player turn
        TryDrawInitialHand();

        GameManager.OnShopTurnStarted += AssignDrawDelayValue;
        cameraOriginalPos = Camera.main.transform.position;
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

        if (IsCameraAtOrigin())
        {
            DrawHand();
        }
        else
        {
            StartCoroutine(WaitForCameraThenDraw());
        }
    }

    private void DrawHand()
    {
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

        handCards.Add(g);
        UpdateCardPositions();
    }

    public void RemoveCard(GameObject card)
    {
        if (!handCards.Contains(card)) return;

        handCards.Remove(card);

        Destroy(card);

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

    private IEnumerator DisableAllCardsTemporarily(float duration)
    {
        foreach (var card in handCards)
        {
            if (card == null) continue;

            CardView view = card.GetComponent<CardView>();
            if (view != null)
            {
                view.DisableInteractionTemporarily(duration);
            }
        }

        yield return null;
    }

    public void OnCardPlayed()
    {
        StartCoroutine(DisableAllCardsTemporarily(1f));
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

    public float GetMiddleCardPosX()
    {
        if (handCards.Count == 3)
            return handCards[1].transform.position.x;
        else if (handCards.Count == 2)
        {
            float biggerX;
            float smallerX;

            biggerX = Mathf.Max(handCards[1].transform.position.x, handCards[0].transform.position.x);
            smallerX = Mathf.Min(handCards[1].transform.position.x, handCards[0].transform.position.x);

            return smallerX + (biggerX - smallerX) / 2;
        }

        else if (handCards.Count == 1)
            return handCards[0].transform.position.x;
        else
            return 0.0f;
    }

    private void AssignDrawDelayValue()
    {
        drawDelay = drawDelay == 10f ? 0.15f : 10f;
    }

    private bool IsCameraAtOrigin()
    {
        return Vector3.Distance(Camera.main.transform.position, cameraOriginalPos) < 0.1f;
    }

    private IEnumerator WaitForCameraThenDraw()
    {
        while (!IsCameraAtOrigin())
        {
            yield return null;
        }

        cameraParallax.isShopActivated = false;
        DrawHand();
    }
}