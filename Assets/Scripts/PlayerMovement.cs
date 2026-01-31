using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 6f;

    private Rigidbody rb;
    private Vector3 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(x, 0f, z).normalized;
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = transform.forward * moveInput.z + transform.right * moveInput.x;
        moveDir.Normalize();

        Vector3 velocity = moveDir * moveSpeed;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
