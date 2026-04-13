using UnityEngine;

public class LifeEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Rigidbody2D rb;
    private Animator animator;
    private MovementEnemy movementEnemy;

    [Header("Estadisticas Vida")]
    [SerializeField] private int maxLife;
    private int currentLife;

    [Header("Estadisticas Retroceso")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float minKnockbackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        movementEnemy.ChangeToStateDead();
        Destroy(gameObject, 1f);
        return;
    }

        Knockback(sender);
    }

    public void Knockback(Vector2 sender)
    {
        Vector2 direction = ((Vector2)transform.position - sender).normalized;
        Vector2 force = new Vector2(Mathf.Sign(direction.x) * knockbackForce.x, 0f); // ← Y siempre 0
        
        movementEnemy.ChangeToStateHurt(minKnockbackTime, force);
        animator.SetTrigger("Hit");
    }
}