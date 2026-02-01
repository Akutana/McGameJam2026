using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Scriptable Objects/Enemies")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int damage;
    public int maxHealth;
    public Sprite art;
}
