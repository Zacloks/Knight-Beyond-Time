using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class LifeEnemy : MonoBehaviour
{
    [System.Serializable]
    public class PosibleDrop
    {
        [FormerlySerializedAs("arma")]
        [Tooltip("Item (ItemData) que puede caer: un arma, una pocion, lo que sea.")]
        public ItemData item;
        [Range(0f, 100f)]
        [Tooltip("Probabilidad (0-100%) de que este item caiga al morir.")]
        public float probabilidad = 25f;
    }

    [System.Serializable]
    public class MonedaPosible
    {
        [Tooltip("Valor de la moneda (1, 5, 10, ...).")]
        public int valor = 1;
        [Tooltip("Peso: mayor = sale más seguido. NO hace falta que sumen 100.")]
        public float peso = 1f;
    }

    [Header("Referencias")]
    private Animator animator;
    private MovementEnemy movementEnemy;
    private Enemy enemy;

    [Header("Monedas al morir")]
    public GameObject monedaPrefab;
    public int minMonedas = 2;
    public int maxMonedas = 4;
    [Tooltip("Valores de moneda y su peso. Peso alto = sale más seguido (ej. 1=70, 5=25, 10=5).")]
    public List<MonedaPosible> valoresMoneda = new List<MonedaPosible>()
    {
        new MonedaPosible { valor = 1,  peso = 70f },
        new MonedaPosible { valor = 5,  peso = 25f },
        new MonedaPosible { valor = 10, peso = 5f },
    };

    [Header("Items que puede soltar al morir")]
    [FormerlySerializedAs("dropsDeArmas")]
    [Tooltip("Items que este enemigo puede dropear (armas, pociones, etc.). Cada uno con su probabilidad. Vacia = no suelta nada.")]
    public List<PosibleDrop> dropsDeItems = new List<PosibleDrop>();
    [FormerlySerializedAs("maxArmasPorMuerte")]
    [Tooltip("Maximo de items que puede soltar de una vez. 0 = sin limite (cada item se tira por separado).")]
    public int maxItemsPorMuerte = 1;
    [FormerlySerializedAs("radioDropArmas")]
    [Tooltip("Radio (en unidades) alrededor del enemigo donde aparecen los items soltados.")]
    public float radioDrop = 0.6f;

    [Header("Estadisticas Vida")]
    [SerializeField] private int maxLife;
    [Tooltip("Segundos antes de destruir el cuerpo al morir. Igualar a la duración del clip de muerte.")]
    [SerializeField] private float tiempoHastaDestruir = 1f;
    [Tooltip("Si está activo, los golpes no letales NO provocan knockback ni estado Hurt (poise de jefe). Sigue recibiendo daño.")]
    [SerializeField] private bool superArmor = false;
    public int currentLife { get; private set; }
    public int MaxLife => maxLife;
    private bool isDead = false;
    private float damageReductionMult = 1f;
    private float damageReductionUntil = 0f;

    [Header("Estadisticas Retroceso")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float minKnockbackTime;
    private WaveManager waveManager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
        enemy = GetComponent<Enemy>();

        if (maxLife <= 0)
            Debug.LogWarning($"{name}: 'maxLife' es {maxLife} en LifeEnemy. El enemigo morirá de un solo golpe. Asígnalo en el Inspector del prefab.", this);

        currentLife = maxLife;
    }

    public void TakeDamage(int damageAmount, Vector2 sender)
    {
        if (isDead) return;

        if (Time.time < damageReductionUntil)
            damageAmount = Mathf.Max(0, Mathf.RoundToInt(damageAmount * damageReductionMult));

        int tempLife = currentLife - damageAmount;
        tempLife = Mathf.Clamp(tempLife, 0, maxLife);
        currentLife = tempLife;

        if (currentLife <= 0)
        {
            isDead = true;
            movementEnemy.ChangeToStateDead();
            waveManager?.NotifyEnemyDied(gameObject);
            soltarMonedas();
            soltarItems();
            Destroy(gameObject, tiempoHastaDestruir);
            return;
        }

        // Con super armor (jefes) no se interrumpe el ataque en curso: nada de knockback/Hurt.
        if (superArmor) return;

        Knockback(sender);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;
        currentLife = Mathf.Clamp(currentLife + amount, 0, maxLife);
    }

    public void ApplyDamageReduction(float percent, float duration)
    {
        damageReductionMult = 1f - Mathf.Clamp01(percent / 100f);
        damageReductionUntil = Time.time + Mathf.Max(0f, duration);
    }

    private void soltarMonedas()
    {
        if (enemy == null || !enemy.PuedeSoltarMonedas) return;

        if (monedaPrefab == null)
        {
            Debug.LogWarning($"{name}: deberia soltar monedas pero el campo 'monedaPrefab' del LifeEnemy esta vacio.", this);
            return;
        }
        if (valoresMoneda == null || valoresMoneda.Count == 0) return;

        int cantidad = Random.Range(minMonedas, maxMonedas + 1);

        for (int i = 0; i < cantidad; i++)
        {
            // Dispersión centrada alrededor del enemigo (antes salían siempre hacia
            // arriba y quedaban fuera del camino cerca de los bordes).
            Vector2 v = new Vector2(Random.Range(-0.6f, 0.6f), Random.Range(-0.35f, 0.35f));
            Vector2 posicionSpawn = (Vector2)transform.position + v;

            GameObject nuevaMoneda = Instantiate(monedaPrefab, posicionSpawn, Quaternion.identity);

            Moneda scriptMoneda = nuevaMoneda.GetComponent<Moneda>();

            if (scriptMoneda != null)
                scriptMoneda.crearMoneda(ElegirValorMoneda());
        }
    }

    private int ElegirValorMoneda()
    {
        float total = 0f;
        foreach (MonedaPosible m in valoresMoneda)
            if (m != null && m.peso > 0f) total += m.peso;

        if (total <= 0f) return valoresMoneda[0].valor;

        float r = Random.Range(0f, total);
        foreach (MonedaPosible m in valoresMoneda)
        {
            if (m == null || m.peso <= 0f) continue;
            r -= m.peso;
            if (r <= 0f) return m.valor;
        }
        return valoresMoneda[valoresMoneda.Count - 1].valor;
    }
    // Recorre la lista de items posibles, tira la probabilidad de cada uno de forma
    // independiente y suelta los que salgan.
    // Si el nivel define una TablaDropsNivel (via DropsDelNivel en la escena), esa
    // tabla MANDA: el mismo enemigo suelta loot distinto según el nivel. Si no hay
    // tabla, usa la lista propia del prefab.
    private void soltarItems()
    {
        List<PosibleDrop> lista = dropsDeItems;
        int maxItems = maxItemsPorMuerte;
        float radio = radioDrop;

        TablaDropsNivel tabla = DropsDelNivel.Instancia != null ? DropsDelNivel.Instancia.tabla : null;
        if (tabla != null)
        {
            lista = tabla.drops;
            maxItems = tabla.maxItemsPorMuerte;
            radio = tabla.radioDrop;
        }

        if (lista == null || lista.Count == 0) return;

        List<ItemData> ganadores = new List<ItemData>();
        foreach (PosibleDrop drop in lista)
        {
            if (drop == null || drop.item == null) continue;
            if (Random.Range(0f, 100f) <= drop.probabilidad)
                ganadores.Add(drop.item);
        }

        if (maxItems > 0)
        {
            while (ganadores.Count > maxItems)
                ganadores.RemoveAt(Random.Range(0, ganadores.Count));
        }

        foreach (ItemData item in ganadores)
            SpawnItemEnSuelo(item, radio);
    }

    private void SpawnItemEnSuelo(ItemData data, float radio)
    {
        if (data.prefab == null)
        {
            Debug.LogWarning($"{name}: el item '{data.itemName}' no tiene 'prefab' asignado en su ItemData, no se puede soltar.", this);
            return;
        }

        Vector2 offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.3f, 1f)).normalized
                         * Random.Range(0f, radio);
        Vector3 pos = (Vector3)((Vector2)transform.position + offset);

        Item soltado = Instantiate(data.prefab, pos, Quaternion.identity);
        soltado.datos = data;
        soltado.enVenta = false;                       
        soltado.gameObject.tag = "Consumible";       

        SpriteRenderer sr = soltado.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        Collider2D col = soltado.GetComponent<Collider2D>();
        if (col != null) { col.enabled = true; col.isTrigger = true; }

        soltado.gameObject.AddComponent<DroppedItem>();
    }

    public void Knockback(Vector2 sender)
    {
        Vector2 origin = sender;
        if (movementEnemy != null && movementEnemy.PlayerTransform != null)
            origin = movementEnemy.PlayerTransform.position;

        float dirX = transform.position.x - origin.x;

        if (Mathf.Abs(dirX) < 0.01f) dirX = (transform.localScale.x < 0) ? -1f : 1f;
        dirX = Mathf.Sign(dirX);

        Vector2 force = new Vector2(dirX * knockbackForce.x, knockbackForce.y);

        movementEnemy.ChangeToStateHurt(minKnockbackTime, force);
        animator.SetTrigger("Hit");
    }

    public void RegisterWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }
}