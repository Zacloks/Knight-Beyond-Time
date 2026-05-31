using UnityEngine;

public class LifeEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Rigidbody2D rb;
    private Animator animator;
    private MovementEnemy movementEnemy;
    public GameObject monedaPrefab; 
    public int cantidadMonedas = 2; 
    private int[] posiblesValores = {1, 5, 10};

    [Header("Estadisticas Vida")]
    [SerializeField] private int maxLife;
    private int currentLife;
    private bool isDead = false;

    [Header("Estadisticas Retroceso")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float minKnockbackTime;
    private WaveManager waveManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
        currentLife = maxLife;
    }

    public void TakeDamage(int damageAmount, Vector2 sender)
    {
        // Si ya está muriendo, ignoramos golpes extra. Evita que la lógica de
        // muerte (y NotifyEnemyDied) se ejecute varias veces y descuadre la oleada.
        if (isDead) return;

        int tempLife = currentLife - damageAmount;
        tempLife = Mathf.Clamp(tempLife, 0, maxLife);
        currentLife = tempLife;

        if (currentLife <= 0)
        {
            isDead = true;
            movementEnemy.ChangeToStateDead();
            waveManager?.NotifyEnemyDied();
            soltarMonedas();
            Destroy(gameObject, 1f);
            return;
        }
            Knockback(sender);
        }

    public bool IsDead()
    {
        return isDead;
    }

    private void soltarMonedas()
    {
        // Sin prefab de moneda asignado no soltamos nada (evita Instantiate(null),
        // que lanzaría excepción en cada muerte e impediría destruir al enemigo).
        if (monedaPrefab == null) return;

        for (int i = 0; i < cantidadMonedas; i++)
        {
            Vector2 v = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f));
            Vector2 posicionSpawn = (Vector2)transform.position + v;

            GameObject nuevaMoneda = Instantiate(monedaPrefab, posicionSpawn, Quaternion.identity);

            Moneda scriptMoneda = nuevaMoneda.GetComponent<Moneda>();
            
            if (scriptMoneda != null)
            {
                int idx = Random.Range(0, 3);
                int valorElegido = posiblesValores[idx];
                scriptMoneda.crearMoneda(valorElegido); 
            }
        }
    }
    public void Knockback(Vector2 sender)
    {
        Vector2 direction = ((Vector2)transform.position - sender).normalized;
        // Si el atacante está justo encima del enemigo (misma posición), empujamos
        // a la derecha por defecto para evitar un empuje nulo o errático.
        if (direction == Vector2.zero) direction = Vector2.right;

        // Empuje contrario al golpe, escalado por knockbackForce (x = horizontal, y = vertical).
        Vector2 force = new Vector2(direction.x * knockbackForce.x, direction.y * knockbackForce.y);

        movementEnemy.ChangeToStateHurt(minKnockbackTime, force);
        animator.SetTrigger("Hit");
    }

    public void RegisterWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }
}