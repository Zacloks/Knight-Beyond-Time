using UnityEngine;

public class MovementEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Rigidbody2D rb;
    private Transform player;
    private Vector2 moveDirection;
    public EnemyState currentState;

    public Transform PlayerTransform => player;

    [Header("Estadisticas Movimiento")]
    public float movementSpeedBase;

    [Header("Animator")]
    private Animator animator;
    private IEnemyAttack attackEnemy;

    [Header("Hurt")]
    private float hurtEndTime;
    [Range(0f, 1f)] public float knockbackDamping = 0.85f;

    [Header("Ataque")]
    [Tooltip("Distancia de parada si el enemigo NO tiene componente de ataque (fallback).")]
    public float attackRange = 1.5f;
    private float attackEndTime;
    private bool attackHeld;

    [Header("Separación (anti-amontonamiento)")]
    public float separationRadius = 1.3f;
    public float separationStrength = 1.8f;

    [Header("Inmovilización (efecto báculo)")]
    private float immobilizeEndTime;
    public bool IsImmobilized => Time.time < immobilizeEndTime;

    public bool IsBusy => currentState == EnemyState.Hurt
                       || currentState == EnemyState.Dead
                       || currentState == EnemyState.Attack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackEnemy = GetComponent<IEnemyAttack>();
        TryFindPlayer();
    }

    private bool TryFindPlayer()
    {
        if (player != null) return true;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        return player != null;
    }

    private void Update()
    {
        LookDirectionMovement();
    }

    private void FixedUpdate()
    {
        if (IsImmobilized)
        {
            moveDirection = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            return;
        }

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

        if (IsBusy) return;

        Vector2 desired = moveDirection * movementSpeedBase * 1.6f;

        Vector2 separation = Vector2.ClampMagnitude(GetSeparation(), 1f) * separationStrength * movementSpeedBase;
        rb.linearVelocity = desired + separation;
    }

    // CHASE
    private void ChaseBehavior()
    {
        if (!TryFindPlayer())
        {
            moveDirection = Vector2.zero;
            animator.SetBool("Chasing", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        float stopDistance = attackEnemy != null ? attackEnemy.AttackReach : attackRange;

        if (distanceToPlayer <= stopDistance)
        {
            moveDirection = Vector2.zero;
            animator.SetBool("Chasing", false);
        }
        else
        {
            animator.SetBool("Chasing", true);
            Chase();
        }
    }

    private void Chase()
    {
        Vector2 enemyPos = transform.position;
        Vector2 playerPos = player.position;

        Vector2 fromPlayer = enemyPos - playerPos;
        if (fromPlayer.sqrMagnitude < 0.0001f) fromPlayer = Random.insideUnitCircle;

        float ringRadius = attackEnemy != null ? attackEnemy.AttackReach : attackRange;
        Vector2 ringPoint = playerPos + fromPlayer.normalized * ringRadius;

        moveDirection = (ringPoint - enemyPos).normalized;
    }

    private Vector2 GetSeparation()
    {
        Vector2 separation = Vector2.zero;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (Collider2D col in neighbors)
        {
            if (col.gameObject == gameObject || !col.CompareTag("Enemy")) continue;

            Vector2 away = (Vector2)transform.position - (Vector2)col.transform.position;
            float dist = away.magnitude;
            if (dist > 0.0001f)
                separation += away.normalized * (1f - Mathf.Clamp01(dist / separationRadius));
            else
            {
                separation += Random.insideUnitCircle.normalized;
            }
        }

        return separation;
    }

    // ATTACK
    private void AttackBehavior()
    {
         moveDirection = Vector2.zero;
        if (attackHeld) return;
        if (Time.time > attackEndTime)
            ChangeToStateChase();
    }

    public void ChangeToStateAttack(float duration)
    {
        animator.SetBool("Chasing", false);
        rb.linearVelocity = Vector2.zero;
        attackEndTime = Time.time + duration;
        currentState = EnemyState.Attack;
    }

    // Mantiene el estado Attack indefinidamente (para corutinas de ataque del jefe).
    public void BeginSustainedAttack()
    {
        animator.SetBool("Chasing", false);
        rb.linearVelocity = Vector2.zero;
        attackHeld = true;
        currentState = EnemyState.Attack;
    }

    public void EndSustainedAttack()
    {
        attackHeld = false;
        // No revivir si el enemigo murió mientras atacaba.
        if (currentState == EnemyState.Attack)
            ChangeToStateChase();
    }

    // Voltea el sprite para mirar hacia un punto en X (mismo criterio que el movimiento).
    public void FaceTowards(float targetX)
    {
        float dx = targetX - transform.position.x;
        if (Mathf.Abs(dx) < 0.01f) return;

        bool targetRight = dx > 0;
        bool currentlyRight = transform.localScale.x > 0;
        if (targetRight != currentlyRight) Spin();
    }

    public void ChangeToStateChase()
    {
        currentState = EnemyState.Chase;
    }

    // HURT
    private void HurtBehavior()
    {
        moveDirection = Vector2.zero;

        rb.linearVelocity *= knockbackDamping;

        if (Time.time > hurtEndTime)
        {
            rb.linearVelocity = Vector2.zero;
            ChangeToStateChase();
        }
    }

    public void ChangeToStateHurt(float timeToWait, Vector2 knockbackForce)
    {
        animator.SetBool("Chasing", false);
        currentState = EnemyState.Hurt;
        hurtEndTime = Time.time + Mathf.Max(timeToWait, 0.2f);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    // INMOVILIZACIÓN (efecto báculo)
    public void Inmovilizar(float duracion)
    {
        immobilizeEndTime = Time.time + duracion;
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("Chasing", false);
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
        return transform.localScale.x < 0;
    }
}