using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string message)
    {
        text.text = message;
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}