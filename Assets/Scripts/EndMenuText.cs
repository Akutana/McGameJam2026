using TMPro;
using UnityEngine;

public class EndMenuText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            resultText.text = "Game Over";
            return;
        }

        switch (GameManager.Instance.FinalResult)
        {
            case GameManager.GameResult.Win:
                resultText.text = "You've hunted all the demons and spirits";
                break;

            case GameManager.GameResult.Lose:
                resultText.text = "You've been haunted";
                break;

            default:
                resultText.text = "Game Over";
                break;
        }
    }
}