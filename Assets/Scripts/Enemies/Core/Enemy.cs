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
    private float lastContactDamageTime;
    private float contactDamageCooldown = 0.5f;

    [Header("Ajustes de Crítico")]
    [Range(0f, 100f)] public float probabilidadCritico = 10f;
    public float multiplicadorCritico = 2f;

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

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (lifeEnemy != null && lifeEnemy.IsDead()) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (Time.time < lastContactDamageTime + contactDamageCooldown) return;

        PlayerScript playerCtrl = collision.gameObject.GetComponent<PlayerScript>();
        if (playerCtrl != null)
        {
            lastContactDamageTime = Time.time;
            playerCtrl.TakeDamage(CalcularDanoConCritico((int)attackDamage), transform.position);
            Debug.Log("Enemigo dañó al jugador");
        }
    }

    public int CalcularDanoConCritico(int danoBase)
    {
        if (Random.Range(0f, 100f) <= probabilidadCritico)
        {
            int danoCritico = Mathf.RoundToInt(danoBase * multiplicadorCritico);
            Debug.Log($"<color=orange>¡CRÍTICO!</color> {gameObject.name} hizo {danoCritico} de daño (base {danoBase}).");
            return danoCritico;
        }
        return danoBase;
    }

    public virtual void TakeDamage(int amount, Vector2 sender)
    {
        lifeEnemy.TakeDamage(amount, sender);
    }

    // Congela al enemigo durante 'duracion' segundos (efecto del báculo).
    public void Inmovilizar(float duracion)
    {
        if (lifeEnemy != null && lifeEnemy.IsDead()) return;
        if (movementEnemy != null) movementEnemy.Inmovilizar(duracion);
    }

    public int GetDamage()
    {
        return attackEnemy.AttackDamage;
    }

}