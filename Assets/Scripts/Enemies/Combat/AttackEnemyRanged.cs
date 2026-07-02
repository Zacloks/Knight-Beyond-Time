using UnityEngine;

[RequireComponent(typeof(MovementEnemy))]
public class AttackEnemyRanged : MonoBehaviour, IEnemyAttack
{
    [Header("Referencias")]
    private Animator animator;
    private MovementEnemy movementEnemy;
    private Enemy enemy;
    private Rigidbody2D rb;
    private Transform player;

    [Header("Ataque a distancia")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private int attackDamage = 8;
    [SerializeField] private float shootRange = 6f;
    [SerializeField] private float projectileSpeed = 9f;
    [SerializeField] private float timeBetweenAttacks = 1.5f;
    [SerializeField] private float staggerAtaque = 0.35f;
    [SerializeField] private float attackDuration = 0.6f;
    [SerializeField] private float shootDelay = 0.25f;

    [Header("Preparación primer ataque")]
    [SerializeField] private float tiempoPreparacionPrimerAtaque = 0.8f;
    private bool primerAtaqueRealizado = false;
    private float tiempoEntradaRango = -1f;

    private float lastAttackTime;
    private float currentAttackCooldown;

    void Start()
    {
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        currentAttackCooldown = timeBetweenAttacks + Random.Range(-staggerAtaque, staggerAtaque);
    }

    private void Update()
    {
        if (movementEnemy.currentState == EnemyState.Hurt) return;
        if (movementEnemy.currentState == EnemyState.Dead) return;
        if (movementEnemy.currentState == EnemyState.Attack) return;
        if (Time.time < lastAttackTime + currentAttackCooldown) return;

        if (player == null) player = movementEnemy.PlayerTransform;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > shootRange)
        {
            if (!primerAtaqueRealizado) tiempoEntradaRango = -1f;
            return;
        }

        if (!primerAtaqueRealizado)
        {
            if (tiempoEntradaRango < 0f) tiempoEntradaRango = Time.time;
            if (Time.time < tiempoEntradaRango + tiempoPreparacionPrimerAtaque) return;
            primerAtaqueRealizado = true;
        }

        lastAttackTime = Time.time;
        currentAttackCooldown = timeBetweenAttacks + Random.Range(-staggerAtaque, staggerAtaque);
        movementEnemy.ChangeToStateAttack(attackDuration);
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");
        Invoke(nameof(Shoot), shootDelay);
    }

    private void Shoot()
    {
        if (movementEnemy.currentState == EnemyState.Hurt || movementEnemy.currentState == EnemyState.Dead) return;
        if (projectilePrefab == null || player == null) return;

        Vector2 origin = shootPoint != null ? (Vector2)shootPoint.position : (Vector2)transform.position;
        Vector2 dir = ((Vector2)player.position - origin).normalized;

        GameObject go = Instantiate(projectilePrefab, origin, Quaternion.identity);

        if (go.TryGetComponent(out EnemyProjectile proj))
        {
            int danoFinal = enemy != null ? enemy.CalcularDanoConCritico(attackDamage) : attackDamage;
            proj.Setup(danoFinal, projectileSpeed, dir);
        }
        else
        {
            Debug.LogWarning($"{name}: el projectilePrefab no tiene el script EnemyProjectile.", this);
        }
    }

    public float AttackReach => shootRange;
    public int AttackDamage => attackDamage;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
