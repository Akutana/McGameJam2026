using UnityEngine;

public class FaceDetector : MonoBehaviour
{
    private DiceRoller dice;

    private void Awake()
    {
        dice = FindAnyObjectByType<DiceRoller>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (dice == null) { return; }

        if (dice.GetComponent<Rigidbody>().linearVelocity == Vector3.zero)
        {
            dice.diceFaceNumber = int.Parse(other.name);
        }
    }
}
