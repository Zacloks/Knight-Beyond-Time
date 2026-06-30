using System.Collections;
using UnityEngine;

/// <summary>
/// Esquiva defensiva: el jefe salta hacia atrás (lejos del jugador) para crear
/// distancia, sin hacer daño. Pensada para usarse cuando el jugador está pegado;
/// configúrala con maxRange bajo. Animación del asset: BackDash.
/// </summary>
public class BossBackDash : BossAbility
{
    [Header("Retroceso")]
    public float windup = 0.1f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.3f;
    public float recovery = 0.35f;
    public string animTrigger = "BackDash";

    public override IEnumerator Run(BossContext ctx)
    {
        // Sigue mirando al jugador mientras retrocede.
        ctx.movement.FaceTowards(ctx.player.position.x);

        Vector2 away = ((Vector2)transform.position - (Vector2)ctx.player.position).normalized;
        if (away == Vector2.zero) away = ctx.self.localScale.x > 0 ? Vector2.left : Vector2.right;

        if (ctx.animator != null && !string.IsNullOrEmpty(animTrigger))
            ctx.animator.SetTrigger(animTrigger);

        yield return new WaitForSeconds(windup);

        float end = Time.time + dashDuration;
        while (Time.time < end)
        {
            if (ctx.rb != null) ctx.rb.linearVelocity = away * dashSpeed;
            yield return new WaitForFixedUpdate();
        }

        if (ctx.rb != null) ctx.rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(recovery);
    }
}
