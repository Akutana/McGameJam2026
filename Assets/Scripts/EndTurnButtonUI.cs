using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonUI : MonoBehaviour
{
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();

        // Hook the click in code
        btn.onClick.AddListener(OnEndTurnClicked);
    }

    private void OnEndTurnClicked()
    {
        // Safe to end turn
        GameManager.Instance?.OnEndTurnButtonPressed();
    }

    private void OnEnable()
    {
        // Always show button, no need to subscribe/unsubscribe
        btn.gameObject.SetActive(true);
    }
}