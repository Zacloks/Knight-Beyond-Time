using UnityEngine;

public class BossContext
{
    public Transform self;
    public Transform player;
    public Rigidbody2D rb;
    public Animator animator;
    public Enemy enemy;
    public MovementEnemy movement;
    public LayerMask hittableLayers;

    public float DistanceToPlayer =>
        player == null ? Mathf.Infinity : Vector2.Distance(self.position, player.position);
}
