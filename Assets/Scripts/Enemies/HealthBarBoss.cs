using UnityEngine;
using UnityEngine.UI;

public class HealthBarBoss : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(int maxHealth)
    {
        gameObject.SetActive(true);
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}