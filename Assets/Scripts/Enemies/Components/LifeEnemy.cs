using UnityEngine;

public class LifeEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Animator animator;
    private MovementEnemy movementEnemy;
    private Enemy enemy;

    [Header("Monedas al morir")]
    public GameObject monedaPrefab;
    public int minMonedas = 2;
    public int maxMonedas = 4;
    public int[] posiblesValores = {1, 5, 10};

    [Header("Estadisticas Vida")]
    [SerializeField] private int maxLife;
    [Tooltip("Segundos antes de destruir el cuerpo al morir. Igualar a la duración del clip de muerte.")]
    [SerializeField] private float tiempoHastaDestruir = 1f;
    [Tooltip("Si está activo, los golpes no letales NO provocan knockback ni estado Hurt (poise de jefe). Sigue recibiendo daño.")]
    [SerializeField] private bool superArmor = false;
    public int currentLife { get; private set; }
    public int MaxLife => maxLife;
    private bool isDead = false;

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

        int tempLife = currentLife - damageAmount;
        tempLife = Mathf.Clamp(tempLife, 0, maxLife);
        currentLife = tempLife;

        if (currentLife <= 0)
        {
            isDead = true;
            movementEnemy.ChangeToStateDead();
            waveManager?.NotifyEnemyDied(gameObject);
            soltarMonedas();
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

    private void soltarMonedas()
    {
        if (enemy == null || !enemy.PuedeSoltarMonedas) return;

        if (monedaPrefab == null)
        {
            Debug.LogWarning($"{name}: deberia soltar monedas pero el campo 'monedaPrefab' del LifeEnemy esta vacio.", this);
            return;
        }
        if (posiblesValores.Length == 0) return;

        int cantidad = Random.Range(minMonedas, maxMonedas + 1);

        for (int i = 0; i < cantidad; i++)
        {
            Vector2 v = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f));
            Vector2 posicionSpawn = (Vector2)transform.position + v;

            GameObject nuevaMoneda = Instantiate(monedaPrefab, posicionSpawn, Quaternion.identity);

            Moneda scriptMoneda = nuevaMoneda.GetComponent<Moneda>();

            if (scriptMoneda != null)
            {
                int idx = Random.Range(0, posiblesValores.Length);
                int valorElegido = posiblesValores[idx];
                scriptMoneda.crearMoneda(valorElegido);
            }
        }
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