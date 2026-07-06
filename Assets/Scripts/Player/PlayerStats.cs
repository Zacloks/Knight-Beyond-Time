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

    [Header("Feedback de daño")]
    public float damageFlashDuration = 0.1f;
    public Color damageFlashColor = Color.red;

    [Header("Energía")]
    private const int energyRegen = 1;
    private const int energyRegenInterval = 10;

    [Header("Auxiliares")]
    private float curImmuneTime;
    private int countTicks;

    private GameManager GM => GameManager.Instance;

    public int maxHealth     { get => GM.maxHealth;     set => GM.maxHealth = value; }
    public int currentHealth { get => GM.currentHealth; set => GM.currentHealth = value; }
    public int maxEnergy     { get => GM.maxEnergy;     set => GM.maxEnergy = value; }
    public int currentEnergy { get => GM.currentEnergy; set => GM.currentEnergy = value; }
    public int coins         { get => GM.coins;         set => GM.coins = value; }

    [Header("Buffs Temporales")]
    public float multiplicadorDaño = 1f;
    [Range(0f, 1f)] public float reduccionDaño = 0f;
    public float multiplicadorVelocidadAtaque = 1f;
    public float bonusCritico = 0f;
    public float multiplicadorAlcance = 1f;

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

        int amountFinal = Mathf.Max(0, Mathf.RoundToInt(amount * (1f - reduccionDaño)));
        currentHealth = Mathf.Clamp(currentHealth - amountFinal, 0, maxHealth);
        curImmuneTime = immuneTime;

        GM.NotifyChanged();
        Debug.Log("Jugador recibió daño. Vida restante: " + currentHealth);

        StartCoroutine(FlashRojo());

        if (currentHealth <= 0)
            PlayerMuerto?.Invoke();
    }

    private IEnumerator FlashRojo()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        if (sprites.Length == 0) yield break;

        Color[] originales = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            originales[i] = sprites[i].color;
            Color c = damageFlashColor;
            c.a = originales[i].a;
            sprites[i].color = c;
        }

        yield return new WaitForSeconds(damageFlashDuration);

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
                sprites[i].color = originales[i];
        }
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

    public void ActivarRegeneracion(int cantidadPorTick, float intervalo, float duracion)
    {
        StartCoroutine(RutinaRegeneracion(cantidadPorTick, intervalo, duracion));
    }

    private IEnumerator RutinaRegeneracion(int cantidadPorTick, float intervalo, float duracion)
    {
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            Heal(cantidadPorTick);
            yield return new WaitForSeconds(intervalo);
            tiempo += intervalo;
        }
    }

    public void ActivarFuerza(float multiplicador, float duracion)
    {
        StartCoroutine(RutinaFuerza(multiplicador, duracion));
    }

    private IEnumerator RutinaFuerza(float multiplicador, float duracion)
    {
        multiplicadorDaño = multiplicador;
        yield return new WaitForSeconds(duracion);
        multiplicadorDaño = 1f;
    }

    public void ActivarResistencia(float reduccion, float duracion)
    {
        StartCoroutine(RutinaResistencia(reduccion, duracion));
    }

    private IEnumerator RutinaResistencia(float reduccion, float duracion)
    {
        reduccionDaño = reduccion;
        yield return new WaitForSeconds(duracion);
        reduccionDaño = 0f;
    }

    public void ActivarAtaqueRapido(float multiplicador, float duracion)
    {
        StartCoroutine(RutinaAtaqueRapido(multiplicador, duracion));
    }

    private IEnumerator RutinaAtaqueRapido(float multiplicador, float duracion)
    {
        multiplicadorVelocidadAtaque = multiplicador;
        yield return new WaitForSeconds(duracion);
        multiplicadorVelocidadAtaque = 1f;
    }

    public void ActivarSuerte(float bonus, float duracion)
    {
        StartCoroutine(RutinaSuerte(bonus, duracion));
    }

    private IEnumerator RutinaSuerte(float bonus, float duracion)
    {
        bonusCritico = bonus;
        yield return new WaitForSeconds(duracion);
        bonusCritico = 0f;
    }

    public void ActivarTamaño(float escala, float multiplicadorDañoTamaño, float multiplicadorAlcanceTamaño, float duracion)
    {
        StartCoroutine(RutinaTamaño(escala, multiplicadorDañoTamaño, multiplicadorAlcanceTamaño, duracion));
    }

    private IEnumerator RutinaTamaño(float escala, float multiplicadorDañoTamaño, float multiplicadorAlcanceTamaño, float duracion)
    {
        transform.localScale *= escala;
        multiplicadorDaño = multiplicadorDañoTamaño;
        multiplicadorAlcance = multiplicadorAlcanceTamaño;
        yield return new WaitForSeconds(duracion);
        transform.localScale /= escala;
        multiplicadorDaño = 1f;
        multiplicadorAlcance = 1f;
    }
}
