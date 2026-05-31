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

    [Header("Ajustes de Crítico")]
    [Tooltip("Probabilidad (0-100) de que este enemigo haga un golpe crítico")]
    [Range(0f, 100f)] public float probabilidadCritico = 10f;
    [Tooltip("Multiplicador de daño cuando el golpe es crítico")]
    public float multiplicadorCritico = 2f;

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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Un enemigo que ya está muriendo no debe seguir dañando al jugador
        // durante su animación de muerte.
        if (lifeEnemy != null && lifeEnemy.IsDead()) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript playerCtrl = collision.gameObject.GetComponent<PlayerScript>();
            
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(CalcularDanoConCritico((int)attackDamage), transform.position);
                Debug.Log("Enemigo dañó al jugador");
            }
        }
    }

    /*
    Calcula el daño final aplicando la probabilidad de crítico de este enemigo.
    Si sale crítico, multiplica el daño y lo imprime por consola.
    Usado tanto por el ataque (AttackEnemy) como por el daño de contacto.
    */
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

    public void TakeDamage(int amount, Vector2 sender)
    {
        lifeEnemy.TakeDamage(amount, sender);
    }

  public int GetDamage()
    {
        return attackEnemy.attackDamage;
    }

}