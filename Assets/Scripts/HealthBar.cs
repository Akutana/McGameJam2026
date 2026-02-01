using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Health health;

    void Update()
    {
        slider.value = health.GetHealthNormalized();
    }
}
