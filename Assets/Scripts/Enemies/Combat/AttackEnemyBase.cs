using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MovementEnemy))]
public abstract class AttackEnemyBase : MonoBehaviour, IEnemyAttack
{
    [Header("Referencias")]
    protected Rigidbody2D rb;
    protected Animator animator;
    protected MovementEnemy movementEnemy;
    protected Enemy enemy;

    [Header("Ataque (común)")]
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected float timeBetweenAttacks = 1.5f;
    [SerializeField] protected float attackDuration = 0.6f;
    [FormerlySerializedAs("damageDelay")]
    [FormerlySerializedAs("shootDelay")]
    [SerializeField] protected float attackDelay = 0.2f;
    [SerializeField] protected float staggerAtaque = 0.25f;

    [Header("Preparación primer ataque")]
    [SerializeField] protected float tiempoPreparacionPrimerAtaque = 0.3f;

    private float lastAttackTime;
    private float currentAttackCooldown;
    private bool primerAtaqueRealizado;
    private float tiempoEntradaRango = -1f;

    protected Transform Player => movementEnemy != null ? movementEnemy.PlayerTransform : null;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
        enemy = GetComponent<Enemy>();
        currentAttackCooldown = NextCooldown();
    }

    protected virtual void Update()
    {
        if (movementEnemy.IsBusy) return;
        if (Time.time < lastAttackTime + currentAttackCooldown) return;

        if (!TargetInRange())
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
        currentAttackCooldown = NextCooldown();
        movementEnemy.ChangeToStateAttack(attackDuration);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");
        Invoke(nameof(ExecutePayload), attackDelay);
    }

    private float NextCooldown() =>
        timeBetweenAttacks + Random.Range(-staggerAtaque, staggerAtaque);

    private void ExecutePayload()
    {
        if (movementEnemy.currentState == EnemyState.Hurt || movementEnemy.currentState == EnemyState.Dead) return;
        OnAttack();
    }

    protected int ResolveDamage() =>
        enemy != null ? enemy.CalcularDanoConCritico(attackDamage) : attackDamage;

    public void AttackFinished() => movementEnemy.ChangeToStateChase();

    protected abstract bool TargetInRange();

    protected abstract void OnAttack();

    public abstract float AttackReach { get; }
    public int AttackDamage => attackDamage;
}
