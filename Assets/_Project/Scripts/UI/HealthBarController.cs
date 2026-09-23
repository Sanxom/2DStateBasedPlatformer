using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Gradient colorGradient;
    
    [SerializeField] private Slider healthSlider;

    private Image fillImage;

    private void Awake()
    {
        fillImage = healthSlider.fillRect.GetComponent<Image>();
    }

    public void SetSlider(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        fillImage.color = colorGradient.Evaluate(healthSlider.normalizedValue);
    }
}