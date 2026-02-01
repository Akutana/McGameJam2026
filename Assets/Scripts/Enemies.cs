using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Scriptable Objects/Enemies")]
public class Enemies : ScriptableObject
{
    public string cardName;
    public int damage;
    public int health;
    public Sprite art;
}
