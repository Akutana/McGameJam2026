using UnityEngine;

public class CameraParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float intensity = 0.5f; // how much the camera moves
    public Vector2 clampX = new Vector2(-0.3f, 0.3f);
    public Vector2 clampY = new Vector2(-0.2f, 0.2f);
    public float smoothSpeed = 5f;

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;
    }

    void LateUpdate()
    {
        // Get mouse position normalized (-1..1)
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Calculate offset
        float offsetX = Mathf.Clamp(mouseX * intensity, clampX.x, clampX.y);
        float offsetY = Mathf.Clamp(mouseY * intensity, clampY.x, clampY.y);

        Vector3 targetPos = originalPos + new Vector3(offsetX, offsetY, 0);

        // Smoothly interpolate
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
    }
}
