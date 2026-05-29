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
        int tempLife = currentLife - damageAmount;
        tempLife = Mathf.Clamp(tempLife, 0, maxLife);
        currentLife = tempLife;

        if (currentLife <= 0)
        {
            movementEnemy.ChangeToStateDead();
            waveManager?.NotifyEnemyDied();
            soltarMonedas();
            Destroy(gameObject, 1f);
            return;
        }
            Knockback(sender);
        }

    private void soltarMonedas()
    {
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
        Vector2 force = new Vector2(Mathf.Sign(direction.x) * knockbackForce.x, 0f); // ← Y siempre 0
        
        movementEnemy.ChangeToStateHurt(minKnockbackTime, force);
        animator.SetTrigger("Hit");
    }

    public void RegisterWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }
}