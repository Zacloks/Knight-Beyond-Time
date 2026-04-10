using UnityEngine;

public class AttackEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Animator animator;

    [Header("Estadisticas Ataque")]
    public int cantDamage;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Attack");
        }
    }
}