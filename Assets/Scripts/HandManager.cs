using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }

    [SerializeField] private int maxHandSize = 5;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int drawNumber = 3;

    private List<GameObject> handCards = new();

    // Update is called once per frame
    void Update()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
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

    public void RemoveCard(GameObject card)
    {
        if (!handCards.Contains(card)) return;

        handCards.Remove(card);
        UpdateCardPoisitions();
    }

    private void DrawCard()
    {
        if (handCards.Count >= maxHandSize) return;
        GameObject g = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);

        CardView view = g.GetComponent<CardView>();
        view.SetHandManager(this);
        handCards.Add(g);
        UpdateCardPoisitions();
    }
    private void UpdateCardPoisitions()
    {
        if(handCards.Count == 0) return;
        float cardSpacing = 1f / maxHandSize;
        float firstCardPostion = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;

        for(int i = 0; i < handCards.Count; i++)
        {
            float p = firstCardPostion + i * cardSpacing;
            Vector3 splinePosition =
    splineContainer.transform.TransformPoint(
        spline.EvaluatePosition(p)
    );
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up,forward).normalized);
            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DORotateQuaternion(rotation, 0.25f);
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

    public void ClearHand()
    {
        // Destroy all card GameObjects
        foreach (var card in handCards)
        {
            if (card != null)
                Destroy(card);
        }

        // Clear the list
        handCards.Clear();
    }

}
