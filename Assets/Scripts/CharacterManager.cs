using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    public List<PlayableCharacter> characters = new List<PlayableCharacter>();

    int currentIndex = 0;

    void Start()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].Deactivate();
        }

        characters[currentIndex].Activate();
    }

    void Update()
    {
        if (characters[currentIndex].IsOutOfStamina())
        {
            SwitchToNextCharacter();
        }
    }

    void SwitchToNextCharacter()
    {
        characters[currentIndex].Deactivate();

        currentIndex++;
        if (currentIndex >= characters.Count)
            currentIndex = 0;

        characters[currentIndex].Activate();
    }
}
