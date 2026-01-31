using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{

    public float moveSpeed = 6f;
    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 10f;

    [HideInInspector] public float currentStamina;

    public Camera characterCamera;

    private Rigidbody rb;
    private Vector3 moveInput;
    bool isActive = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!isActive)
        {
            return;
        }

    
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(x, 0f, z).normalized;

        if(moveInput != Vector3.zero)
        {
            currentStamina -= staminaDrainPerSecond * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }

    }

    private void FixedUpdate()
    {
        Vector3 moveDir = transform.forward * moveInput.z + transform.right * moveInput.x;
        moveDir.Normalize();

        Vector3 velocity = moveDir * moveSpeed;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    public void Activate()
    {
        isActive = true;
        characterCamera.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        isActive = false;

        // STOP movement completely
        moveInput = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        characterCamera.gameObject.SetActive(false);
        currentStamina = maxStamina;
    }

    public bool IsOutOfStamina()
    {
        return currentStamina <= 0f;
    }
}
