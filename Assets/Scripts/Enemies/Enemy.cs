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

    protected virtual void Start()
    {
        movementEnemy = GetComponent<MovementEnemy>();
        lifeEnemy = GetComponent<LifeEnemy>();
        attackEnemy = GetComponent<AttackEnemy>();
        movementEnemy.currentState = EnemyState.Chase;
    }

    public void TakeDamage(int amount, Vector2 sender)
    {
        lifeEnemy.TakeDamage(amount, sender);
    }

    public int GetDamage()
    {
        return attackEnemy.cantDamage;
    }

}