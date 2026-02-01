using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;

    public Sprite art;
    public int price;

    public void Play()
    {
        Debug.Log("Played " + cardName);
    }
}