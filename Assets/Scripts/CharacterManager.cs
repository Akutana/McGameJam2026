using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    public List<PlayableCharacter> characters = new List<PlayableCharacter>();

    int currentIndex = 0;
    bool topCameraActive = false;
    public Camera topCamera;
    void Start()
    {
        for (int i = 0; i < characters.Count; i++)
            characters[i].Deactivate();

        characters[currentIndex].Activate();
        EnableCharacterCamera(currentIndex);

        topCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!topCameraActive && characters[currentIndex].IsOutOfStamina())
        {
            SwitchToNextCharacter();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            topCameraActive = !topCameraActive;

            if (topCameraActive)
            {
                characters[currentIndex].Deactivate();
                EnableTopCamera();
            }
            else
            {
                characters[currentIndex].Activate();
                EnableCharacterCamera(currentIndex);
                topCamera.gameObject.SetActive(false);
            }
        }
    }
    void EnableCharacterCamera(int index)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].characterCamera.gameObject.SetActive(i == index);
        }
    }

    void EnableTopCamera()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].characterCamera.gameObject.SetActive(false);
        }

        topCamera.gameObject.SetActive(true);
    }

    void SwitchToNextCharacter()
    {
        characters[currentIndex].Deactivate();

        currentIndex++;
        if (currentIndex >= characters.Count)
            currentIndex = 0;

        characters[currentIndex].Activate();
        EnableCharacterCamera(currentIndex);
    }
}
