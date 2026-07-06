using System.Collections;
using UnityEngine;

public class BossRangedCast : BossAbility
{
    [Header("Cast a distancia")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public int damage = 15;
    public float projectileSpeed = 10f;
    public int projectilesPerCast = 1;
    public float spreadAngle = 0f;
    public float betweenShots = 0.1f;
    public float chargeTime = 0.8f;
    public float recovery = 0.5f;
    public string animTrigger = "LightCast";

    public override IEnumerator Run(BossContext ctx)
    {
        ctx.movement.FaceTowards(ctx.player != null ? ctx.player.position.x : transform.position.x);

        if (ctx.animator != null && !string.IsNullOrEmpty(animTrigger))
            ctx.animator.SetTrigger(animTrigger);

        yield return new WaitForSeconds(chargeTime);

        Vector2 origin = shootPoint != null ? (Vector2)shootPoint.position : (Vector2)transform.position;
        Vector2 toPlayer = ctx.player != null ? ((Vector2)ctx.player.position - origin).normalized : Vector2.left;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        int count = Mathf.Max(1, projectilesPerCast);
        for (int i = 0; i < count; i++)
        {
            if (projectilePrefab == null) break;

            float t = count > 1 ? (float)i / (count - 1) : 0.5f;
            float angle = baseAngle - spreadAngle * 0.5f + spreadAngle * t;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject go = Instantiate(projectilePrefab, origin, Quaternion.identity);
            if (go.TryGetComponent(out EnemyProjectile proj))
                proj.Setup(ctx.enemy != null ? ctx.enemy.CalcularDanoConCritico(damage) : damage, projectileSpeed, dir);

            if (betweenShots > 0f && i < count - 1)
                yield return new WaitForSeconds(betweenShots);
        }

        yield return new WaitForSeconds(recovery);
    }
}
