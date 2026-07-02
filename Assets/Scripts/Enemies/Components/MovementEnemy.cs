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
    public float chaseSpeedMultiplier = 1.65f;

    [Header("Animator")]
    private Animator animator;
    private IEnemyAttack attackEnemy;

    [Header("Hurt")]
    private float hurtEndTime;
    [Range(0f, 1f)] public float knockbackDamping = 0.7f;
    public float velocidadMaxKnockback = 4f;
    [Range(0f, 1f)] public float knockbackMultiplicador = 0.55f;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    private float attackEndTime;

    [Header("Separación (anti-amontonamiento)")]
    public float separationRadius = 1.3f;
    public float separationStrength = 1.8f;
    [Range(0f, 1f)] public float separacionAlAtacar = 0.55f;

    [Header("Inteligencia del enemigo")]
    public float lookAhead = 0.7f;
    public float lookAheadRadius = 0.25f;
    [Range(0f, 1f)] public float avoidanceStrength = 0.65f;
    [Range(0f, 60f)] public float anguloVariacion = 8f;

    [Header("IA / Surround táctico")]
    public float intervaloElegirAngulo = 0.4f;
    [Range(4, 16)] public int candidatosAngulo = 8;
    [Range(0f, 2f)] public float pesoContinuidad = 0.6f;
    public float radioEscaneoSurround = 5f;

    private float anguloPersonal;
    private float anguloPreferido;  
    private float ultimoRecalculo;

    [Header("Inmovilización (efecto báculo)")]
    private float immobilizeEndTime;
    public bool IsImmobilized => Time.time < immobilizeEndTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackEnemy = GetComponent<IEnemyAttack>();
        TryFindPlayer();

        anguloPersonal = Random.Range(-anguloVariacion, anguloVariacion) * Mathf.Deg2Rad;

        anguloPreferido = Random.Range(0f, Mathf.PI * 2f);
        ultimoRecalculo = -999f;
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

        if (currentState == EnemyState.Hurt || currentState == EnemyState.Dead || currentState == EnemyState.Attack) return;

        Vector2 desired = moveDirection * movementSpeedBase * chaseSpeedMultiplier;

        Vector2 separation = Vector2.ClampMagnitude(GetSeparation(), 1f) * separationStrength * movementSpeedBase;
        rb.linearVelocity = desired + separation;
    }
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

        if (Time.time >= ultimoRecalculo + intervaloElegirAngulo)
        {
            anguloPreferido = ElegirAnguloAtaque(playerPos);
            ultimoRecalculo = Time.time;
        }

        float anguloFinal = anguloPreferido + anguloPersonal;
        Vector2 dirAtaque = new Vector2(Mathf.Cos(anguloFinal), Mathf.Sin(anguloFinal));

        float ringRadius = attackEnemy != null ? attackEnemy.AttackReach : attackRange;
        Vector2 ringPoint = playerPos + dirAtaque * ringRadius;

        Vector2 desiredDir = ((ringPoint - enemyPos)).normalized;

        moveDirection = AplicarEvasion(desiredDir);
    }

    private float ElegirAnguloAtaque(Vector2 playerPos)
    {
        Vector2 fromPlayerActual = (Vector2)transform.position - playerPos;
        if (fromPlayerActual.sqrMagnitude < 0.0001f) fromPlayerActual = Vector2.right;
        float miAngulo = Mathf.Atan2(fromPlayerActual.y, fromPlayerActual.x);

        Collider2D[] cercanos = Physics2D.OverlapCircleAll(playerPos, radioEscaneoSurround);

        float mejorAngulo = miAngulo;
        float mejorScore = float.NegativeInfinity;
        int total = Mathf.Max(4, candidatosAngulo);

        for (int i = 0; i < total; i++)
        {
            float anguloCand = (i / (float)total) * Mathf.PI * 2f;
            Vector2 dirCand = new Vector2(Mathf.Cos(anguloCand), Mathf.Sin(anguloCand));

            float minDist = Mathf.PI;
            foreach (Collider2D col in cercanos)
            {
                if (col == null) continue;
                if (col.gameObject == gameObject || !col.CompareTag("Enemy")) continue;

                Vector2 toOther = ((Vector2)col.transform.position - playerPos);
                if (toOther.sqrMagnitude < 0.0001f) continue;

                float anguloOtro = Mathf.Atan2(toOther.y, toOther.x);
                float diff = Mathf.Abs(Mathf.DeltaAngle(anguloCand * Mathf.Rad2Deg, anguloOtro * Mathf.Rad2Deg)) * Mathf.Deg2Rad;
                if (diff < minDist) minDist = diff;
            }

            float diffActual = Mathf.Abs(Mathf.DeltaAngle(anguloCand * Mathf.Rad2Deg, miAngulo * Mathf.Rad2Deg)) * Mathf.Deg2Rad;
            float score = minDist - diffActual * pesoContinuidad;

            if (score > mejorScore)
            {
                mejorScore = score;
                mejorAngulo = anguloCand;
            }
        }

        return mejorAngulo;
    }

    private Vector2 AplicarEvasion(Vector2 desiredDir)
    {
        if (avoidanceStrength <= 0f || lookAhead <= 0f) return desiredDir;

        if (CaminoLibre(desiredDir)) return desiredDir;

        float[] desvios = { 25f, -25f, 45f, -45f, 70f, -70f, 100f, -100f, 135f, -135f };
        foreach (float grados in desvios)
        {
            Vector2 candidata = Rotar(desiredDir, grados);
            if (CaminoLibre(candidata))
                return Vector2.Lerp(desiredDir, candidata, avoidanceStrength).normalized;
        }

        return (-desiredDir).normalized;
    }

    private bool CaminoLibre(Vector2 dir)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, lookAheadRadius, dir, lookAhead);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.gameObject == gameObject) continue;
            if (!hit.collider.CompareTag("Enemy")) continue;
            return false;
        }
        return true;
    }

    private Vector2 Rotar(Vector2 v, float grados)
    {
        float r = grados * Mathf.Deg2Rad;
        float cos = Mathf.Cos(r);
        float sin = Mathf.Sin(r);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
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


        Vector2 separation = Vector2.ClampMagnitude(GetSeparation(), 1f) * separationStrength * movementSpeedBase * separacionAlAtacar;
        rb.linearVelocity = separation;

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

    public void ChangeToStateChase()
    {
        currentState = EnemyState.Chase;
    }

    // HURT
    private void HurtBehavior()
    {
        moveDirection = Vector2.zero;

        rb.linearVelocity *= knockbackDamping;

        float maxVel = velocidadMaxKnockback;
        if (rb.linearVelocity.sqrMagnitude > maxVel * maxVel)
            rb.linearVelocity = rb.linearVelocity.normalized * maxVel;

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
        rb.AddForce(knockbackForce * knockbackMultiplicador, ForceMode2D.Impulse);
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
        moveDirection = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
            col.enabled = false;
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