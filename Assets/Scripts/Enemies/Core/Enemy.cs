using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MovementEnemy))]
[RequireComponent(typeof(LifeEnemy))]

public abstract class Enemy : MonoBehaviour
{
    [Header("Referencias")]
    protected MovementEnemy movementEnemy;
    protected LifeEnemy lifeEnemy;
    protected IEnemyAttack attackEnemy;

    [Header("Ajustes de Crítico")]
    [Range(0f, 100f)] public float probabilidadCritico = 10f;
    public float multiplicadorCritico = 2f;

    [Header("Multiplicador global de daño")]
    public float multiplicadorDaño = 1.5f;

    public virtual bool PuedeSoltarMonedas => false;

    protected virtual bool RequiresAttackComponent => true;

    protected virtual void Start()
    {
        movementEnemy = GetComponent<MovementEnemy>();
        lifeEnemy = GetComponent<LifeEnemy>();
        attackEnemy = GetComponent<IEnemyAttack>();

        if (attackEnemy == null && RequiresAttackComponent)
            Debug.LogWarning($"{name}: falta un componente de ataque (AttackEnemy o AttackEnemyRanged).", this);

        if (movementEnemy != null)
        {
            movementEnemy.currentState = EnemyState.Chase;
        }
    }

    public int CalcularDanoConCritico(int danoBase)
    {
        int danoConMultiplicador = Mathf.RoundToInt(danoBase * multiplicadorDaño);

        if (Random.Range(0f, 100f) <= probabilidadCritico)
        {
            int danoCritico = Mathf.RoundToInt(danoConMultiplicador * multiplicadorCritico);
            Debug.Log($"<color=orange>¡CRÍTICO!</color> {gameObject.name} hizo {danoCritico} de daño (base {danoBase}).");
            return danoCritico;
        }
        return danoConMultiplicador;
    }

    public virtual void TakeDamage(int amount, Vector2 sender)
    {
        lifeEnemy.TakeDamage(amount, sender);
    }

    public void Inmovilizar(float duracion)
    {
        if (lifeEnemy != null && lifeEnemy.IsDead()) return;
        if (movementEnemy != null) movementEnemy.Inmovilizar(duracion);
    }

    public virtual void Heal(int amount)
    {
        if (lifeEnemy != null) lifeEnemy.Heal(amount);
    }

    public void ApplyDamageReduction(float percent, float duration)
    {
        if (lifeEnemy != null) lifeEnemy.ApplyDamageReduction(percent, duration);
    }
}