using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float attackDamage = 10f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public float knockbackForce = 5f;
    public float hurtDuration = 0.3f;
    
    protected float currentHealth;
    protected float attackTimer;
    protected float hurtTimer;
    protected EnemyState currentState;
    protected Transform player;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected bool facingRight = true;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").transform;
        ChangeState(EnemyState.Chase);
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Chase:  UpdateChase();  break;
            case EnemyState.Attack: UpdateAttack(); break;
            case EnemyState.Hurt:   UpdateHurt();   break;
        }
    }

    protected virtual void UpdateChase()
    {
        if (DistanceToPlayer() < attackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }
        MoveTowardsPlayer();
    }

    protected virtual void UpdateAttack()
    {
        if (DistanceToPlayer() > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    protected virtual void UpdateHurt()
    {
        hurtTimer -= Time.deltaTime;
        if (hurtTimer <= 0f)
            ChangeState(currentHealth > 0 ? EnemyState.Chase : EnemyState.Dead);
    }

    protected virtual void MoveTowardsPlayer()
{
    Vector2 dir = (player.position - transform.position).normalized;
    rb.linearVelocity = new Vector2(dir.x * moveSpeed, dir.y * moveSpeed);
    FlipSprite(dir.x);
}

    protected virtual void PerformAttack()
    {
        
    }

    public virtual void TakeDamage(float amount, Vector2 knockbackDir)
    {
        if (currentState == EnemyState.Dead) return;
        currentHealth -= amount;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        hurtTimer = hurtDuration;
        ChangeState(EnemyState.Hurt);
    }

    protected virtual void Die()
    {
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 0.5f);
    }

    protected void ChangeState(EnemyState newState)
    {
        currentState = newState;
        if (newState == EnemyState.Dead) Die();
    }

    protected float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    protected void FlipSprite(float dirX)
    {
        if (dirX > 0 && !facingRight || dirX < 0 && facingRight){
            facingRight = !facingRight;
            spriteRenderer.flipX = !facingRight;
        }
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
        PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
        if (playerScript != null)
            playerScript.TakeDamage((int)attackDamage);
        }
    }
}