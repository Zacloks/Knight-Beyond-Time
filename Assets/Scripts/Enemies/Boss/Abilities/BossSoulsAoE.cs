using System.Collections;
using UnityEngine;

public class BossSoulsAoE : BossAbility
{
    [Header("Almas atormentadas")]
    public GameObject soulPrefab;
    public Transform spawnPoint;
    public int soulCount = 5;
    public int damagePerSoul = 10;
    public float soulSpeed = 7f;
    public float spreadAngle = 60f;
    public float castTime = 1.1f;
    public float betweenSouls = 0.12f;
    public float recovery = 0.7f;
    public string animTrigger = "Souls";

    public override IEnumerator Run(BossContext ctx)
    {
        ctx.movement.FaceTowards(ctx.player != null ? ctx.player.position.x : transform.position.x);

        if (ctx.animator != null && !string.IsNullOrEmpty(animTrigger))
            ctx.animator.SetTrigger(animTrigger);

        yield return new WaitForSeconds(castTime);

        Vector2 origin = spawnPoint != null ? (Vector2)spawnPoint.position : (Vector2)transform.position;
        Vector2 toPlayer = ctx.player != null ? ((Vector2)ctx.player.position - origin).normalized : Vector2.left;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        for (int i = 0; i < soulCount; i++)
        {
            if (soulPrefab == null) break;

            float t = soulCount > 1 ? (float)i / (soulCount - 1) : 0.5f;
            float angle = baseAngle - spreadAngle * 0.5f + spreadAngle * t;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject go = Instantiate(soulPrefab, origin, Quaternion.identity);
            if (go.TryGetComponent(out EnemyProjectile proj))
                proj.Setup(ctx.enemy != null ? ctx.enemy.CalcularDanoConCritico(damagePerSoul) : damagePerSoul, soulSpeed, dir);

            if (betweenSouls > 0f && i < soulCount - 1)
                yield return new WaitForSeconds(betweenSouls);
        }

        yield return new WaitForSeconds(recovery);
    }
}
