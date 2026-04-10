using Unity.VisualScripting;
using UnityEngine;

public class LifeEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Rigidbody2D rb2D;
    private Animator animator;
    private MovementEnemy movementEnemy;

    [Header("Estadisticas Vida")]
    [SerializeField] private int maxLife;
    private int currentLife;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
        currentLife = maxLife;
    }

    public void TakeDamage(int damageAmount, Vector2 sender)
    {
        int tempLife = currentLife - damageAmount;
        tempLife = Mathf.Clamp(tempLife, 0, maxLife);
        currentLife = tempLife;

        if (currentLife == 0)
        {
            Destroy(gameObject);
            return;
        }
        movementEnemy.currentState = EnemyState.Hurt;
        movementEnemy.Knockback(sender);
    }
}