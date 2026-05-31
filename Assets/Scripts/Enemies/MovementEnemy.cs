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
    private AttackEnemy attackEnemy;

    [Header("Hurt")]
    private float hurtEndTime;
    public float hurtDuration = 0.5f;
    [Tooltip("Cuánto conserva la velocidad del empujón cada FixedUpdate (0-1). Menor = frena más rápido.")]
    [Range(0f, 1f)] public float knockbackDamping = 0.85f;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Separación (anti-amontonamiento)")]
    [Tooltip("Radio en el que el enemigo detecta a otros enemigos para no encimarse")]
    public float separationRadius = 1.3f;
    [Tooltip("Qué tanto pesa la separación frente a perseguir al jugador")]
    public float separationStrength = 1.8f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        player = playerObj.transform;
        animator = GetComponent<Animator>();
        attackEnemy = GetComponent<AttackEnemy>();
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

        // Hurt maneja su propia velocidad (el frenado del empujón) y Dead queda quieto.
        if (currentState == EnemyState.Hurt || currentState == EnemyState.Dead) return;

        Vector2 desired = moveDirection * movementSpeedBase * 1.6f;
        // Separación CONTINUA: se aplica también mientras atacan o están quietos, no
        // solo al perseguir. Así los enemigos no se amontonan en una bola cuando uno
        // se detiene a atacar. Se limita para que no domine sobre el movimiento.
        Vector2 separation = Vector2.ClampMagnitude(GetSeparation(), 1f) * separationStrength * movementSpeedBase;
        rb.linearVelocity = desired + separation;
    }

    // CHASE
    private void ChaseBehavior()
    {
        animator.SetBool("Chasing", true);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se detiene al alcance real de su ataque (círculo del attackController),
        // así mantiene la distancia justa en vez de pegarse al jugador.
        float stopDistance = attackEnemy != null ? attackEnemy.AttackReach : attackRange;

        if (distanceToPlayer <= stopDistance)
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
        Vector2 enemyPos = transform.position;
        Vector2 playerPos = player.position;

        // En vez de ir al centro exacto del jugador (lo que hace que todos se
        // apilen en una bola), apuntamos a un punto del "anillo" alrededor del
        // jugador en el ángulo donde ya estamos. Así cada enemigo se aproxima
        // desde su propio lado y entre todos lo rodean.
        Vector2 fromPlayer = enemyPos - playerPos;
        if (fromPlayer.sqrMagnitude < 0.0001f) fromPlayer = Random.insideUnitCircle;

        float ringRadius = attackEnemy != null ? attackEnemy.AttackReach : attackRange;
        Vector2 ringPoint = playerPos + fromPlayer.normalized * ringRadius;

        moveDirection = (ringPoint - enemyPos).normalized;
        // La separación se aplica de forma continua en FixedUpdate, no aquí.
    }

    /*
    Devuelve un vector que apunta lejos de los enemigos cercanos.
    Los vecinos más pegados empujan más fuerte (peso 1 al tocarse, 0 al borde del radio).
    */
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
                // Dos enemigos exactamente encimados (ej. mismo punto de spawn):
                // los empujamos en una dirección aleatoria para que se despeguen.
                separation += Random.insideUnitCircle.normalized;
        }

        return separation;
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

        // El Rigidbody no tiene linearDamping, así que frenamos el empujón a mano
        // para que el enemigo no patine de forma rara durante el aturdimiento.
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
        // Comparamos por signo (no por igualdad exacta) para que el volteo
        // funcione aunque el enemigo tenga una escala distinta de ±1.
        return transform.localScale.x < 0;
    }
}