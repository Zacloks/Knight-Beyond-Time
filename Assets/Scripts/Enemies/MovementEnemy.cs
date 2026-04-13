using UnityEngine;

public class MovementEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Rigidbody2D rb;
    private Transform player;
    private Vector2 moveDirection;
    public EnemyState currentState;

    [Header("Estadisticas Movimiento")]
    public float movementSpeedBase;

    [Header("Animator")]
    private Animator animator;
    public bool chasing = false;

    [Header("Hurt")]
    private float hurtEndTime;
    public float hurtDuration = 0.5f;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        player = playerObj.transform;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        LookDirectionMovement();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Chase:
                ChaseBehavior();
                break;
            case EnemyState.Attack:
                AttackBehavior();
                break;
            case EnemyState.Hurt:
                HurtBehavior();
                break;
            case EnemyState.Dead:
                DeadBehavior();
                break;
            case EnemyState.Idle:
                moveDirection = Vector2.zero;
                break;
        }

        if (currentState != EnemyState.Hurt)
        {
            rb.linearVelocity = moveDirection * movementSpeedBase * 1.6f;
        }
    }

    // CHASE
    private void ChaseBehavior()
    {
        animator.SetBool("Chasing", true);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            moveDirection = Vector2.zero;
            ChangeToStateAttack();
        }
        else
        {
            Chase();
        }
    }

    private void Chase()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        moveDirection = dir;
    }

    // ATTACK
    private void AttackBehavior()
    {
        moveDirection = Vector2.zero;

        if (Time.time > hurtEndTime)
        {
            ChangeToStateChase();
        }
    }

    public void ChangeToStateAttack(float duration)
    {
        animator.SetBool("Chasing", false);
        rb.linearVelocity = Vector2.zero;
        hurtEndTime = Time.time + duration;
        currentState = EnemyState.Attack;
    }

    private void ChangeToStateAttack()
    {
        ChangeToStateAttack(0f);
    }

    public void ChangeToStateChase()
    {
        currentState = EnemyState.Chase;
        chasing = true;
    }

    // HURT
    private void HurtBehavior()
    {
        moveDirection = Vector2.zero;

        if (Time.time > hurtEndTime)
        {
            ChangeToStateChase();
        }
    }

    public void ChangeToStateHurt(float timeToWait, Vector2 knockbackForce)
    {
        animator.SetBool("Chasing", false);
        currentState = EnemyState.Hurt;
        hurtEndTime = Time.time + timeToWait;
        
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    // DEAD
    private void DeadBehavior()
    {
        moveDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Chasing", false);
        animator.SetBool("Dead", true);
    }

    public void ChangeToStateDead()
    {
        currentState = EnemyState.Dead;
    }

    // MOVEMENT
    private void LookDirectionMovement()
    {
        if ((moveDirection.x < 0 && !LookingRight()) ||
            (moveDirection.x > 0 && LookingRight()))
            Spin();
    }

    private void Spin()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool LookingRight()
    {
        return transform.localScale.x == -1;
    }
}