using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MovementEnemy))]
[RequireComponent(typeof(AttackEnemy))]
[RequireComponent(typeof(LifeEnemy))]

public abstract class Enemy : MonoBehaviour
{
    // Referencias a los componentes especializados
    protected MovementEnemy movementEnemy;
    protected LifeEnemy lifeEnemy;
    protected AttackEnemy attackEnemy;

    [Header("Ajustes de Ataque")]
    public float attackDamage = 10f;

    protected virtual void Start()
    {
        // Inicializamos las referencias
        movementEnemy = GetComponent<MovementEnemy>();
        lifeEnemy = GetComponent<LifeEnemy>();
        attackEnemy = GetComponent<AttackEnemy>();

        // Estado inicial
        if (movementEnemy != null)
        {
            movementEnemy.currentState = EnemyState.Chase;
        }
    }

    // Lógica de daño al chocar con el jugador
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Aquí usamos PlayerScript que es el que unificamos antes
            PlayerScript playerCtrl = collision.gameObject.GetComponent<PlayerScript>();
            
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage((int)attackDamage);
                Debug.Log("Enemigo dañó al jugador");
            }
        }
    }
}