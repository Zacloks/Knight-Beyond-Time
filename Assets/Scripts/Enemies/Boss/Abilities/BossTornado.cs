using System.Collections;
using UnityEngine;

/// <summary>
/// Giro tipo tornado: el jefe gira dañando alrededor durante unos segundos,
/// opcionalmente avanzando hacia el jugador. El daño se aplica por "ticks" para
/// no fulminar de golpe. Animaciones del asset: TornadoAtk Full (intro) + Loop.
/// </summary>
public class BossTornado : BossAbility
{
    [Header("Tornado")]
    [Tooltip("Intro antes de empezar a girar (TornadoAtk Full).")]
    public float windup = 0.4f;
    [Tooltip("Duración del giro con daño (TornadoAtk Loop).")]
    public float spinDuration = 2.5f;
    public float recovery = 0.6f;

    [Header("Daño")]
    [Tooltip("Punto donde se centra el área (como el AttackController del Slam). Si está vacío usa la posición del jefe.")]
    public Transform hitPoint;
    public float radius = 2.2f;
    public int damagePerTick = 8;
    [Tooltip("Cada cuánto vuelve a aplicar daño mientras gira.")]
    public float tickInterval = 0.5f;

    [Header("Avance (opcional)")]
    [Tooltip("Velocidad con la que persigue al jugador mientras gira (0 = quieto).")]
    public float driftSpeed = 1.5f;

    [Header("Animación")]
    public string animTrigger = "Tornado";

    public override IEnumerator Run(BossContext ctx)
    {
        if (ctx.animator != null && !string.IsNullOrEmpty(animTrigger))
            ctx.animator.SetTrigger(animTrigger);

        yield return new WaitForSeconds(windup);

        float elapsed = 0f;
        float nextTick = 0f;

        while (elapsed < spinDuration)
        {
            elapsed += Time.fixedDeltaTime;

            // Avance lento hacia el jugador mientras gira.
            if (ctx.rb != null && driftSpeed > 0f && ctx.player != null)
            {
                Vector2 dir = ((Vector2)ctx.player.position - (Vector2)transform.position).normalized;
                ctx.rb.linearVelocity = dir * driftSpeed;
                ctx.movement.FaceTowards(ctx.player.position.x);
            }

            // Daño por ticks.
            if (elapsed >= nextTick)
            {
                nextTick += tickInterval;
                DealDamage(ctx, HitCenter(), radius, damagePerTick);
            }

            yield return new WaitForFixedUpdate();
        }

        if (ctx.rb != null) ctx.rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(recovery);
    }

    private Vector3 HitCenter() => hitPoint != null ? hitPoint.position : transform.position;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(HitCenter(), radius);
    }
}
