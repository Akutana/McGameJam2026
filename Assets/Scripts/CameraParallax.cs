using UnityEngine;
public class CameraParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float intensity = 0.5f; // how much the camera moves
    public Vector2 clampX = new Vector2(-0.3f, 0.3f);
    public Vector2 clampY = new Vector2(-0.2f, 0.2f);
    public float smoothSpeed = 5f;
    private Vector3 originalPos = new Vector3(0, 0, 0);
    private Vector3 shopPos = new Vector3(0, 0, -20);
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Quaternion originalRotation;
    public bool isShopActivated;
    private bool isTurning;
    private bool isMoving;

    void Start()
    {
        originalPos = transform.position;
        originalRotation = transform.rotation;
        targetPosition = originalPos;

        // Subscribe to both shop events
        GameManager.OnShopTurnStarted += MoveToShop;
        GameManager.OnShopTransitionFinised += ReturnFromShop;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        GameManager.OnShopTurnStarted -= MoveToShop;
        GameManager.OnShopTransitionFinised -= ReturnFromShop;
    }

    void LateUpdate()
    {
        if (isShopActivated)
        {
            HandleShopTransition();
            return;
        }

        // Get mouse position normalized (-1..1)
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Calculate offset
        float offsetX = Mathf.Clamp(mouseX * intensity, clampX.x, clampX.y);
        float offsetY = Mathf.Clamp(mouseY * intensity, clampY.x, clampY.y);

        Vector3 targetPos = targetPosition + new Vector3(offsetX, offsetY, 0);

        // Smoothly interpolate
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
    }

    void HandleShopTransition()
    {
        if (isTurning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isTurning = false;
                isMoving = true;
            }
        }

        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                isShopActivated = false;
            }
        }
    }

    void MoveToShop()
    {
        Debug.Log("Camera moving to shop");
        isShopActivated = true;
        isTurning = true;
        targetPosition = shopPos;
        targetRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
    }

    void ReturnFromShop()
    {
        Debug.Log("Camera returning from shop");
        isShopActivated = true;
        isTurning = true;
        targetPosition = originalPos;
        targetRotation = originalRotation;
    }
}