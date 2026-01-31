using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetStamina(float current, float max)
    {
        slider.value = current / max;
    }
}