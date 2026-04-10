using UnityEngine;

public class AttackEnemy : MonoBehaviour
{
    private Animator animator;

    private MovementEnemy movementEnemy;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            animator.SetTrigger("Attack");
            
    }
}