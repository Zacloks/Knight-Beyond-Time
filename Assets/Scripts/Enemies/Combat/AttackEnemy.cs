using UnityEngine;

public class AttackEnemy : MonoBehaviour, IEnemyAttack
{
    [Header("Referencias")]
    private Rigidbody2D rb;
    private Animator animator;
    private MovementEnemy movementEnemy;
    private Enemy enemy;

    [Header("Ataque")]
    [SerializeField] private Transform attackController;
    [SerializeField] private LayerMask hittableLayers;
    [SerializeField] private float circleRadius;
    [SerializeField] public int attackDamage;
    [SerializeField] private float timeBetweenAttacks;
    private float lastAttackTime;
    [SerializeField] private float attackDuration;
    [SerializeField] private float damageDelay = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (movementEnemy.currentState == EnemyState.Hurt) return;
        if (movementEnemy.currentState == EnemyState.Dead) return;
        if (movementEnemy.currentState == EnemyState.Attack) return;
        if (Time.time < lastAttackTime + timeBetweenAttacks) return;
        if (attackController == null) return;

        Collider2D collider = Physics2D.OverlapCircle(attackController.position, circleRadius, hittableLayers);

        if (collider)
        {
            lastAttackTime = Time.time;
            movementEnemy.ChangeToStateAttack(attackDuration);
            rb.linearVelocity = Vector2.zero;
            animator.SetTrigger("Attack");
            Invoke(nameof(Attack), damageDelay);
        }
    }

    public void Attack()
    {
        if (movementEnemy.currentState == EnemyState.Hurt || movementEnemy.currentState == EnemyState.Dead) return;

        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(attackController.position, circleRadius, hittableLayers);
        foreach (Collider2D obj in objectsHit)
        {
            if (obj.TryGetComponent(out PlayerScript playerScript))
            {
                int danoFinal = enemy != null ? enemy.CalcularDanoConCritico(attackDamage) : attackDamage;
                playerScript.TakeDamage(danoFinal, transform.position);
            }
        }
    }
    public float AttackReach
    {
        get
        {
            if (attackController == null) return circleRadius;
            return Vector2.Distance(transform.position, attackController.position) + circleRadius;
        }
    }

    public int AttackDamage => attackDamage;

    public void AttackFinished()
    {
        movementEnemy.ChangeToStateChase();
    }

    void OnDrawGizmos()
    {
        if (attackController == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackController.position, circleRadius);
    }
}