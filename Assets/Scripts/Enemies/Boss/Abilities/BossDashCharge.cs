using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Embestida: el jefe encara al jugador, telegrafía y se lanza en línea recta a
/// gran velocidad, dañando por contacto a quien atraviese. Sirve para cerrar
/// distancia. Funciona porque MovementEnemy no toca la velocidad en estado Attack.
/// </summary>
public class BossDashCharge : BossAbility
{
    [Header("Embestida")]
    [Tooltip("Anticipación antes de lanzarse.")]
    public float windup = 0.55f;
    public float dashSpeed = 14f;
    public float dashDuration = 0.4f;
    public float recovery = 0.5f;
    [Tooltip("Radio de daño por contacto durante la embestida.")]
    public float contactRadius = 1.0f;
    public int damage = 18;
    public string animTrigger = "Dash";

    public override IEnumerator Run(BossContext ctx)
    {
        ctx.movement.FaceTowards(ctx.player.position.x);

        Vector2 dir = ((Vector2)ctx.player.position - (Vector2)transform.position).normalized;
        if (dir == Vector2.zero) dir = ctx.self.localScale.x > 0 ? Vector2.right : Vector2.left;

        if (ctx.animator != null && !string.IsNullOrEmpty(animTrigger))
            ctx.animator.SetTrigger(animTrigger);

        yield return new WaitForSeconds(windup);

        // Cada jugador recibe daño una sola vez por embestida.
        HashSet<PlayerScript> alreadyHit = new();
        float end = Time.time + dashDuration;

        while (Time.time < end)
        {
            if (ctx.rb != null) ctx.rb.linearVelocity = dir * dashSpeed;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, contactRadius, ctx.hittableLayers);
            foreach (Collider2D h in hits)
            {
                if (h.TryGetComponent(out PlayerScript player) && alreadyHit.Add(player))
                {
                    int dmg = ctx.enemy != null ? ctx.enemy.CalcularDanoConCritico(damage) : damage;
                    player.TakeDamage(dmg, transform.position);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        if (ctx.rb != null) ctx.rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(recovery);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, contactRadius);
    }
}
