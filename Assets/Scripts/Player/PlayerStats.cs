using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Valores iniciales (solo se usan en la PRIMERA escena)")]
    public int maxHealthInicial = 100;
    public int maxEnergyInicial = 100;

    [Header("Vida")]
    public float immuneTime = 2f;
    public bool IsImmune => curImmuneTime > 0f;

    [Header("Energía")]
    private const int energyRegen = 1;
    private const int energyRegenInterval = 100;

    [Header("Auxiliares")]
    private float curImmuneTime;
    private int countTicks;

    private GameManager GM => GameManager.Instance;

    public int maxHealth     { get => GM.maxHealth;     set => GM.maxHealth = value; }
    public int currentHealth { get => GM.currentHealth; set => GM.currentHealth = value; }
    public int maxEnergy     { get => GM.maxEnergy;     set => GM.maxEnergy = value; }
    public int currentEnergy { get => GM.currentEnergy; set => GM.currentEnergy = value; }
    public int coins         { get => GM.coins;         set => GM.coins = value; }

    //EVENTOS: NOTIFICAN ALGO A OTRO ARCHIVO/MODULO
    public event System.Action PlayerMuerto;
    void Start()
    {
        if (GM == null)
        {
            Debug.LogError("[PlayerStats] No hay GameManager en la escena. " +
                           "Asegúrate de que SceneSetup lo instancie en la primera escena.");
            return;
        }

        if (!GM.initialized)
        {
            GM.maxHealth = maxHealthInicial;
            GM.maxEnergy = maxEnergyInicial;
            GM.currentHealth = maxHealthInicial;
            GM.currentEnergy = maxEnergyInicial;
            GM.initialized = true;
        }

        GM.NotifyChanged();
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
            GM.NotifyChanged();
        }
    }
//------------HEALTH-----------
    public void TakeDamage(int amount)
    {
        if (IsImmune) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        curImmuneTime = immuneTime;

        GM.NotifyChanged();
        Debug.Log("Jugador recibió daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
            PlayerMuerto.Invoke();
    }
    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        TakeDamage(amount);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        GM.NotifyChanged();
    }

//------------ENERGY-----------
    public bool TryConsumeEnergy(int amount)
    {
        if (currentEnergy < amount) return false;

        currentEnergy = Mathf.Clamp(currentEnergy - amount, 0, maxEnergy);
        GM.NotifyChanged();
        return true;
    }

//----------COINS-------------
    public void AddCoins(int amount)
    {
        coins += amount;
        GM.NotifyChanged();
    }
    public bool TrySpendCoins(int price)
    {
        if (coins < price) return false;
        coins -= price;
        GM.NotifyChanged();
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
