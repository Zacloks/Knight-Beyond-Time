using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("Barras y contadores")]
    public HealthBar healthBar;
    public EnergyBar energyBar;
    public Coin coinCounter;

    void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= Refresh;
    }

    public void Refresh()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        if (healthBar != null && healthBar.slider != null)
        {
            healthBar.slider.maxValue = gm.maxHealth;
            healthBar.slider.value = gm.currentHealth;
        }

        if (energyBar != null && energyBar.slider != null)
        {
            energyBar.slider.maxValue = gm.maxEnergy;
            energyBar.slider.value = gm.currentEnergy;
        }

        if (coinCounter != null)
            coinCounter.setCoins(gm.coins);
    }
}
