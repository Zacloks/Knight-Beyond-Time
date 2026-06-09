using UnityEngine;
using System.Collections;
public class PlayerStats : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth;
    public float immuneTime = 2f;
     public bool IsImmune => curImmuneTime > 0f;

    [Header("Energía")]
    public int maxEnergy = 100;
    public int currentEnergy;
    private const int energyRegen = 1;
    private const int energyRegenInterval = 100; //ticks 

    [Header("Coins")]
    public int coins = 0;

    [Header("UI")]
    public HealthBar healthBar;
    public EnergyBar energyBar;
    public Coin coinCounter;
    public Inventory inventario;

    [Header("Auxiliares")]
    private float curImmuneTime;
    private int countTicks;

    void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;

        if (healthBar != null)
        {
            healthBar.setMaxHealth(maxHealth);
            healthBar.setHealth(currentHealth);
        }

        if (energyBar != null)
            energyBar.setMaxEnergy(maxEnergy);

        if (coinCounter != null)
            coinCounter.setCoins(coins);
    }

    void Update()
    {
        TickImmunity();
        TickEnergyRegen();
    }

//------------TICK-------------
    private void TickImmunity()
    {
        if (curImmuneTime > 0f)
            curImmuneTime -= Time.deltaTime;
    }

    private void TickEnergyRegen()
    {
        countTicks++;
        if (countTicks % energyRegenInterval == 0 && currentEnergy < maxEnergy)
        {
            currentEnergy = Mathf.Min(currentEnergy + energyRegen, maxEnergy);
            if (energyBar != null) energyBar.setEnergy(currentEnergy);
        }
    }
//------------HEALTH-----------
    public void TakeDamage(int amount)
    {
        if (IsImmune) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        curImmuneTime = immuneTime;

        if (healthBar != null) healthBar.setHealth(currentHealth);
        Debug.Log("Jugador recibió daño. Vida restante: " + currentHealth);
    }
    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        TakeDamage(amount);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (healthBar != null) healthBar.setHealth(currentHealth);
    }

//------------ENERGY-----------
    public bool TryConsumeEnergy(int amount)
    {
        if (currentEnergy < amount) return false;

        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        if (energyBar != null) energyBar.setEnergy(currentEnergy);
        return true;
    }

//----------COINS-------------
    public void AddCoins(int amount)
    {
        coins += amount;
        if (coinCounter != null) coinCounter.setCoins(coins);
    }
    public bool TrySpendCoins(int price)
    {
        if (coins < price) return false;
        coins -= price;
        if (coinCounter != null) coinCounter.setCoins(coins);
        return true;
    }
//---------EFECTOS POCIONES-----

    public void ActivarDashInfinito(float duracion)
    {
        StartCoroutine(RutinaDashInfinito(duracion));
    }
        public void ActivarVelocidad(float aumento, float duracion)
    {
        StartCoroutine(RutinaVelocidad(aumento, duracion));
    }

    private IEnumerator RutinaDashInfinito(float duracion)
    {
        PlayerScript coordinator = GetComponent<PlayerScript>();
        if (coordinator != null) coordinator.SetDashInfinito(true);

        yield return new WaitForSeconds(duracion);

        if (coordinator != null) coordinator.SetDashInfinito(false);
    }

    private IEnumerator RutinaVelocidad(float aumento, float duracion)
    {
        PlayerScript coordinator = GetComponent<PlayerScript>();
        if (coordinator != null) coordinator.SetExtraVelocidad(aumento);

        yield return new WaitForSeconds(duracion);

        if (coordinator != null) coordinator.SetExtraVelocidad(1f);
    }
}