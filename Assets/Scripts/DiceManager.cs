using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance { get; private set; }

    private readonly List<DiceRoller> diceRollers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterDice(DiceRoller dice)
    {
        if (!diceRollers.Contains(dice))
            diceRollers.Add(dice);
    }

    public void UnregisterDice(DiceRoller dice)
    {
        diceRollers.Remove(dice);
    }


    public bool AreAnyDiceRolling()
    {
        foreach (var dice in diceRollers)
        {
            if (dice == null)
                continue;

            if (dice.isRolling)
                return true;
        }

        return false;
    }

    public bool AreAllDiceStopped()
    {
        return !AreAnyDiceRolling();
    }

    public int GetTotalDiceValue()
    {
        int total = 0;

        foreach (var dice in diceRollers)
        {
            if (dice == null)
                continue;

            if (dice.isDiceActive)
                total += dice.diceFaceNumber;
        }

        return total;
    }
}