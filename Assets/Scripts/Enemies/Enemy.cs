using UnityEngine;

/*
Con solo añadir el script de EnemyNormal, EnemySpecial
o Enemy Boss gracias a los RequireComponent automaticamente
se le añaden los demas scripts
*/

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MovementEnemy))]
[RequireComponent(typeof(AttackEnemy))]
[RequireComponent(typeof(LifeEnemy))]

public abstract class Enemy : MonoBehaviour
{
    [Header("Referencias")]
    protected MovementEnemy movementEnemy;
    protected LifeEnemy lifeEnemy;
    protected AttackEnemy attackEnemy;

    [Header("Ajustes de Ataque")]
    public float attackDamage = 10f;
    private float lastContactDamageTime;
    private float contactDamageCooldown = 0.5f;

    protected virtual void Start()
    {
        movementEnemy = GetComponent<MovementEnemy>();
        lifeEnemy = GetComponent<LifeEnemy>();
        attackEnemy = GetComponent<AttackEnemy>();

        if (movementEnemy != null)
        {
            movementEnemy.currentState = EnemyState.Chase;
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (Time.time < lastContactDamageTime + contactDamageCooldown) return;

        PlayerScript playerCtrl = collision.gameObject.GetComponent<PlayerScript>();
        if (playerCtrl != null)
        {
            lastContactDamageTime = Time.time;
            playerCtrl.TakeDamage((int)attackDamage);
        }
    }

    public virtual void TakeDamage(int amount, Vector2 sender)
    {
        lifeEnemy.TakeDamage(amount, sender);
    }

    public int GetDamage()
    {
        return attackEnemy.attackDamage;
    }

}