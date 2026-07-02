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

    [Header("Ajustes de Ataque")]
    public float attackDamage = 10f;

    [Header("Ajustes de Crítico")]
    [Range(0f, 100f)] public float probabilidadCritico = 10f;
    public float multiplicadorCritico = 2f;

    [Header("Multiplicador global de daño")]
    public float multiplicadorDaño = 1.5f;

    public virtual bool PuedeSoltarMonedas => false;

    protected virtual void Start()
    {
        movementEnemy = GetComponent<MovementEnemy>();
        lifeEnemy = GetComponent<LifeEnemy>();
        attackEnemy = GetComponent<IEnemyAttack>();

        if (attackEnemy == null)
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

    public int GetDamage()
    {
        return attackEnemy != null ? attackEnemy.AttackDamage : Mathf.RoundToInt(attackDamage);
    }

}