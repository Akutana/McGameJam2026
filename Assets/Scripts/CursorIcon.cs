using UnityEngine;

public class CursorIcon : MonoBehaviour
{
    public Texture2D defaultCursorTexture;
    public Texture2D clickedCursorTexture;

    private bool isClicking;
    void Awake()
    {
        Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isClicking = true;
            Cursor.SetCursor(clickedCursorTexture, Vector2.zero, CursorMode.Auto);
        }

        else if (Input.GetMouseButtonUp(0))
        {
            isClicking = false;
            Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}
