using UnityEngine;

public class MovementEnemy : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    private Transform player;
    private Vector2 moveDirection;
    private Animator animator;
    [SerializeField] public EnemyState currentState;

    [Header("Movimiento")]
    public float movementSpeedBase;

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
}