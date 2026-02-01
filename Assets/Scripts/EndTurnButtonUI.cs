using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonUI : MonoBehaviour
{
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();

        // Hook the click in code
        btn.onClick.AddListener(() =>
        {
            GameManager.Instance?.OnEndTurnButtonPressed();
        });
    }

    private void OnEnable()
    {

        if (DiceManager.Instance.AreAllDiceStopped())
        {
            GameManager.OnTurnChanged += HandleTurnChanged;
        }
      
    }
    private void HandleTurnChanged(GameManager.TurnState newTurn)
    {
        Debug.Log($"EndTurnButtonUI: Turn changed to {newTurn}");
        // Show button only on player's turn
        btn.gameObject.SetActive(newTurn == GameManager.TurnState.PlayerTurn);
    }
}
