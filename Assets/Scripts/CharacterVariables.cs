using UnityEngine;

[CreateAssetMenu(fileName = "CharacterVariables", menuName = "Scriptable Objects/CharacterVariables")]
public class CharacterVariables : ScriptableObject
{
    [SerializeField]public  float moveSpeed = 6f;
    [SerializeField]public  float maxStamina = 100f;
    [SerializeField] public float staminaDrainPerSecond = 10f;
}
