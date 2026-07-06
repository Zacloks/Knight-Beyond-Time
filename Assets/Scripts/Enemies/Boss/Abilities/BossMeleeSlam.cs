using System.Collections;
using UnityEngine;

public class BossMeleeSlam : BossAbility
{
    [Header("Slam")]
    [Tooltip("Punto donde se centra el golpe. Si está vacío usa la posición del jefe.")]
    public Transform hitPoint;
    public float radius = 1.5f;
    public int damage = 20;
    [Tooltip("Anticipación antes de conectar (deja ver el golpe).")]
    public float windup = 0.45f;
    public float recovery = 0.4f;
    [Tooltip("Triggers posibles; se elige uno al azar en cada golpe para variar entre HeavyAtk1/2/3.")]
    public string[] animTriggers = { "Slam1", "Slam2", "Slam3" };

    public override IEnumerator Run(BossContext ctx)
    {
        ctx.movement.FaceTowards(ctx.player.position.x);

        if (ctx.animator != null && animTriggers != null && animTriggers.Length > 0)
        {
            string trigger = animTriggers[Random.Range(0, animTriggers.Length)];
            if (!string.IsNullOrEmpty(trigger)) ctx.animator.SetTrigger(trigger);
        }

        yield return new WaitForSeconds(windup);

        Vector2 center = hitPoint != null ? (Vector2)hitPoint.position : (Vector2)transform.position;
        DealDamage(ctx, center, radius, damage);

        yield return new WaitForSeconds(recovery);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 c = hitPoint != null ? hitPoint.position : transform.position;
        Gizmos.DrawWireSphere(c, radius);
    }
}
