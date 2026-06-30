using System.Collections;
using UnityEngine;

/// <summary>
/// Salto con impacto: el jefe carga, salta hacia la posición donde estaba el
/// jugador y al aterrizar golpea en un AoE. Ideal a media/larga distancia.
/// El arco del salto es visual (sobre 'visual'); el collider sigue en el suelo.
/// Animaciones del asset: ChargeJump -> Falling -> JumpAtk.
/// </summary>
public class BossJumpSlam : BossAbility
{
    [Header("Salto")]
    [Tooltip("Carga antes de despegar.")]
    public float chargeTime = 0.4f;
    [Tooltip("Tiempo en el aire hasta aterrizar.")]
    public float jumpDuration = 0.5f;
    public float recovery = 0.5f;
    [Tooltip("Velocidad máxima de desplazamiento durante el salto.")]
    public float maxJumpSpeed = 18f;

    [Header("Impacto al aterrizar")]
    [Tooltip("Punto donde se centra el impacto (como el AttackController del Slam). Si está vacío usa la posición del jefe.")]
    public Transform hitPoint;
    public float impactRadius = 2.5f;
    public int damage = 30;

    [Header("Arco visual (opcional)")]
    [Tooltip("Transform del sprite que sube/baja para simular el salto. Si está vacío, no hay arco.")]
    public Transform visual;
    public float arcHeight = 2f;

    [Header("VFX / Animación")]
    public string animTrigger = "Jump";
    [Tooltip("Efecto opcional al aterrizar.")]
    public GameObject impactVfx;

    public override IEnumerator Run(BossContext ctx)
    {
        ctx.movement.FaceTowards(ctx.player.position.x);

        if (ctx.animator != null && !string.IsNullOrEmpty(animTrigger))
            ctx.animator.SetTrigger(animTrigger);

        // Carga en el sitio.
        yield return new WaitForSeconds(chargeTime);

        // Punto de aterrizaje = donde está el jugador al despegar.
        Vector2 target = ctx.player != null ? (Vector2)ctx.player.position : (Vector2)transform.position;

        Vector3 visualBase = visual != null ? visual.localPosition : Vector3.zero;
        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / jumpDuration);

            // Desplazamiento horizontal hacia el objetivo (se frena al llegar).
            if (ctx.rb != null)
            {
                Vector2 toTarget = target - (Vector2)transform.position;
                float remaining = Mathf.Max(jumpDuration - elapsed, Time.fixedDeltaTime);
                Vector2 vel = toTarget / remaining;
                ctx.rb.linearVelocity = Vector2.ClampMagnitude(vel, maxJumpSpeed);
            }

            // Arco visual (sin afectar al collider).
            if (visual != null)
                visual.localPosition = visualBase + Vector3.up * (Mathf.Sin(t * Mathf.PI) * arcHeight);

            yield return new WaitForFixedUpdate();
        }

        if (ctx.rb != null) ctx.rb.linearVelocity = Vector2.zero;
        if (visual != null) visual.localPosition = visualBase;

        // Impacto.
        Vector3 center = HitCenter();
        if (impactVfx != null) Destroy(Instantiate(impactVfx, center, Quaternion.identity), 1.5f);
        DealDamage(ctx, center, impactRadius, damage);

        yield return new WaitForSeconds(recovery);
    }

    private Vector3 HitCenter() => hitPoint != null ? hitPoint.position : transform.position;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f);
        Gizmos.DrawWireSphere(HitCenter(), impactRadius);
    }
}
