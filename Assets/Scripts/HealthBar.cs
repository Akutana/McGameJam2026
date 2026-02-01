using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider slider;
    public Health health;

    void Update()
    {
        slider.value = health.GetHealthNormalized();
    }


}
