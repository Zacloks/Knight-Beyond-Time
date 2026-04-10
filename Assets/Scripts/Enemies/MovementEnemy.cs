using UnityEngine;

public class MovementEnemy : MonoBehaviour
{
    [Header("Referencias")]
    private Rigidbody2D rb;
    private Transform player;
    private Vector2 moveDirection;
    private Animator animator;
    public EnemyState currentState;

    [Header("Estadisticas Movimiento")]
    public float movementSpeedBase;

    [Header("Estadisticas Retroceso")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float minKnockbackTime;

    [Header("Animator")]
    public bool chasing = true;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        player = playerObj.transform;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        LookDirectionMovement();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Chase:
                Chase();
                animator.SetBool("Chasing",chasing);
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Hurt:
                moveDirection = Vector2.zero;
                break;
            case EnemyState.Dead:
                break;
            case EnemyState.Idle:
                break;

        }
        rb.linearVelocity = moveDirection * movementSpeedBase * 1.6f;
    }

    private void Chase()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        moveDirection = dir;
    }

    private void LookDirectionMovement()
    {
        if ((moveDirection.x < 0 && !LookingRight()) ||
            (moveDirection.x > 0 &&  LookingRight()))
            Spin();
    }

    private void Spin()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool LookingRight(){
        return transform.localScale.x == -1;
    }

    public void Knockback(Vector2 sender)
    {
        Vector2 direction = ((Vector2)transform.position - sender).normalized;
        Vector2 force = new(Mathf.Sign(direction.x) * knockbackForce.x, knockbackForce.y);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        animator.SetTrigger("Hit");
    }
}