using UnityEngine;

public class BossEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(float amount, Vector2 knockbackDir)
    {
        base.TakeDamage(amount, knockbackDir * 0.3f);
    }

    protected override void Die()
    {
        base.Die();
    }
}