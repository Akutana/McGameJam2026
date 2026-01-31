using UnityEditor.Rendering;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] private float maxRandomForceValue;
    [SerializeField] private float rollingForce;
    [SerializeField] private DiceSide[] diceSides;
    private Rigidbody rb;

    private float forceX, forceY, forceZ;
    private bool isRolling;

    public int diceFaceNumber;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    private void Update()
    {
        if (rb == null) { return; }

        if (Input.GetMouseButtonDown(0) && !isRolling)
        {
            isRolling = true;
            RollDice();
        }

        if (rb.linearVelocity.magnitude < 0.1f)
        {
            CheckTopSide();
        }

        if (rb.linearVelocity.y == 0)
        {
            isRolling = false;
        }
    }

    private void RollDice()
    {
        rb.isKinematic = false;

        forceX = Random.Range(0, maxRandomForceValue);
        forceY = Random.Range(0, maxRandomForceValue);
        forceZ = Random.Range(0, maxRandomForceValue);

        rb.AddForce(Vector3.up * rollingForce);
        rb.AddTorque(forceX, forceY, forceZ);
    }

    private void CheckTopSide()
    {
        foreach (var side in diceSides)
        {
            float dotProduct = Vector3.Dot(side.transform.up, Vector3.up);
            if (dotProduct > 0.9f)
            {
                diceFaceNumber = int.Parse(side.name);
                return;
            }
        }
    }

}
