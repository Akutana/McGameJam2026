using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] private float maxRandomForceValue;
    [SerializeField] private float rollingForce;
    [SerializeField] private DiceSide[] diceSides;
    private Rigidbody rb;
    private TextMeshPro textMesh;
    public bool isDiceActive = true;

    private float forceX, forceY, forceZ;
    public bool isRolling = false;

    public int diceFaceNumber;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        textMesh = GetComponentInChildren<TextMeshPro>();
        rb.isKinematic = true;
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    private void Start()
    {
        DiceManager.Instance?.RegisterDice(this);
    }

    private void Update()
    {
        if (rb == null) { return; }

        if (rb.IsSleeping())
        {
            CheckTopSide();
            isRolling = false;
        }


         TextLogic();
       
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.NumberofRerolls > 0)
        {
            textMesh.enabled = true;

            if (Input.GetMouseButtonDown(0) && !isRolling)
            {
                isRolling = true;
                RollDice();
            }
        }
    }

    private void OnMouseExit()
    {
        textMesh.enabled = false;
    }

    private void RollDice()
    {
        rb.isKinematic = false;

        forceX = Random.Range(0, maxRandomForceValue);
        forceY = Random.Range(0, maxRandomForceValue);
        forceZ = Random.Range(0, maxRandomForceValue);

        rb.AddForce(Vector3.up * rollingForce);
        rb.AddTorque(forceX, forceY, forceZ);

        GameManager.Instance.NumberofRerolls--;
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

    private void TextLogic()
    {
        if (isRolling)
        {
            textMesh.enabled = false;
        }

        textMesh.transform.LookAt(Camera.main.transform);
        textMesh.transform.position = transform.position + Vector3.up * -0.1f;
        textMesh.transform.Rotate(0, 180, 0);
    }
}
