using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;

    public Sprite art;
    public int price;
    public CardType type;
    public float value;

    public void Play()
    {
        Debug.Log("Played " + cardName);
    }
}