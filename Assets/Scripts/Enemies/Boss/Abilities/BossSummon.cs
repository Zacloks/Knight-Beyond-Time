using System.Collections;
using UnityEngine;

public class BossSummon : BossAbility
{
    [Header("Canalizacion")]
    public float castTime = 1f;
    public float recovery = 0.8f;
    public string animTrigger = "Summon";

    [Header("Bonus")]
    public int healAmount = 20;
    [Range(0f, 100f)] public float damageReductionPercent = 50f;

    [Header("Activacion")]
    [Range(0f, 100f)] public float activationChance = 50f;
    public float attemptInterval = 5f;

    private float nextAttemptTime;
    private bool armed;

    public override bool CanUse(float distToPlayer)
    {
        if (!base.CanUse(distToPlayer)) return false;

        if (Time.time >= nextAttemptTime)
        {
            nextAttemptTime = Time.time + attemptInterval;
            armed = Random.Range(0f, 100f) <= activationChance;
        }

        return armed;
    }

    public override IEnumerator Run(BossContext ctx)
    {
        armed = false;

        if (ctx.animator != null && !string.IsNullOrEmpty(animTrigger))
            ctx.animator.SetTrigger(animTrigger);

        if (ctx.enemy != null)
        {
            ctx.enemy.ApplyDamageReduction(damageReductionPercent, castTime + recovery);
            ctx.enemy.Heal(healAmount);
        }

        yield return new WaitForSeconds(castTime + recovery);
    }
}
