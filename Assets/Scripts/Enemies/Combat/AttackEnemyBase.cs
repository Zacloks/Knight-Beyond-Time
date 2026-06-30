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
    [Tooltip("Segundos desde que inicia la animación hasta que se aplica el daño/disparo.")]
    [FormerlySerializedAs("damageDelay")]
    [FormerlySerializedAs("shootDelay")]
    [SerializeField] protected float attackDelay = 0.2f;

    private float lastAttackTime;

    protected Transform Player => movementEnemy != null ? movementEnemy.PlayerTransform : null;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
        enemy = GetComponent<Enemy>();
    }

    protected virtual void Update()
    {
        if (movementEnemy.IsBusy) return;
        if (Time.time < lastAttackTime + timeBetweenAttacks) return;
        if (!TargetInRange()) return;

        lastAttackTime = Time.time;
        movementEnemy.ChangeToStateAttack(attackDuration);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");
        Invoke(nameof(ExecutePayload), attackDelay);
    }

    private void ExecutePayload()
    {
        if (movementEnemy.currentState == EnemyState.Hurt || movementEnemy.currentState == EnemyState.Dead) return;
        OnAttack();
    }

    /// <summary>Daño final ya con crítico aplicado (centraliza el patrón repetido).</summary>
    protected int ResolveDamage() =>
        enemy != null ? enemy.CalcularDanoConCritico(attackDamage) : attackDamage;

    /// <summary>Llamado por animation event al terminar el clip de ataque (opcional).</summary>
    public void AttackFinished() => movementEnemy.ChangeToStateChase();

    /// <summary>¿Hay un objetivo válido al alcance para iniciar el ataque?</summary>
    protected abstract bool TargetInRange();

    /// <summary>Ejecuta el golpe/disparo (se llama tras attackDelay).</summary>
    protected abstract void OnAttack();

    public abstract float AttackReach { get; }
    public int AttackDamage => attackDamage;
}
