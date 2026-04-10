using Unity.VisualScripting;
using UnityEngine;

public class LifeEnemy : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb2D;
    private Animator animator;
    private MovementEnemy movementEnemy;

    [Header("Life")]
    [SerializeField] private int maxLife;
    private int currentLife;

    [Header("Knockback")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float minKnockbackTime;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementEnemy = GetComponent<MovementEnemy>();
        currentLife = maxLife;
    }

    public void TakeDamage(int damageAmount, Transform sender)
    {
        int tempLife = currentLife - damageAmount;
        tempLife = Mathf.Clamp(tempLife, 0, maxLife);
        currentLife = tempLife;

        if (currentLife == 0)
        {
            Destroy(gameObject);
            return;
        }

        Knockback(sender);
    }

    private void Knockback(Transform sender)
    {
        Vector2 direction = (transform.position - sender.position).normalized;
        Vector2 force = new(Mathf.Sign(direction.x) * knockbackForce.x, knockbackForce.y);
        rb2D.linearVelocity = Vector2.zero;
        rb2D.AddForce(force, ForceMode2D.Impulse);
        animator.SetTrigger("Hit");
    }
}