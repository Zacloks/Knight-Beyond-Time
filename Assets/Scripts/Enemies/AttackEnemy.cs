using UnityEngine;

public class AttackEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Rigidbody2D rb;
    private Animator animator;
    private MovementEnemy movementEnemy;

    [Header("Ataque")]
    [SerializeField] private Transform attackController;
    [SerializeField] private LayerMask hittableLayers;
    [SerializeField] private float circleRadius;
    [SerializeField] public int attackDamage;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float lastAttackTime;
    [SerializeField] private float attackDuration;
    [SerializeField] private float timeToWaitAfterAttack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
    }

    private void Update()
    {
        if (movementEnemy.currentState == EnemyState.Hurt) return;
        if (movementEnemy.currentState == EnemyState.Dead) return;
        if (movementEnemy.currentState == EnemyState.Attack) return;
        if (Time.time < lastAttackTime + timeBetweenAttacks) return;

        Collider2D collider = Physics2D.OverlapCircle(attackController.position, circleRadius, hittableLayers);
          
        if (collider)
        {
        lastAttackTime = Time.time;
        movementEnemy.ChangeToStateAttack(attackDuration);
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");
        }
    }

    public void Attack()
    {
        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(attackController.position, circleRadius, hittableLayers);
        foreach (Collider2D obj in objectsHit)
        {
        if (obj.TryGetComponent(out PlayerScript playerScript))
            {
                playerScript.TakeDamage(attackDamage);
            }
        }
    }

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