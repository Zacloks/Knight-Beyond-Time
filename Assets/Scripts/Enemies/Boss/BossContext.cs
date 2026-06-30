using UnityEngine;

/// <summary>
/// Conjunto de referencias que el cerebro del jefe entrega a cada habilidad
/// para que pueda ejecutarse (moverse, animar, hacer daño con crítico, etc.).
/// </summary>
public class BossContext
{
    public Transform self;
    public Transform player;
    public Rigidbody2D rb;
    public Animator animator;
    public Enemy enemy;            // para CalcularDanoConCritico
    public MovementEnemy movement; // para girar / sostener el estado de ataque
    public LayerMask hittableLayers;

    public float DistanceToPlayer =>
        player == null ? Mathf.Infinity : Vector2.Distance(self.position, player.position);
}
