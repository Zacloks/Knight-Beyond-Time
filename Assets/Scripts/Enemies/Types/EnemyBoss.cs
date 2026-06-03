using UnityEngine;

public class EnemyBoss : Enemy
{
    public override bool PuedeSoltarMonedas => true;
    public HealthBarBoss healthBarBoss;

    protected override void Start()
    {
        base.Start();
        if (healthBarBoss != null) healthBarBoss.Show(lifeEnemy.currentLife);
    }

    public override void TakeDamage(int amount, Vector2 sender)
    {
        base.TakeDamage(amount, sender);
        if (healthBarBoss == null) return;

        healthBarBoss.SetHealth(lifeEnemy.currentLife);

        if (lifeEnemy.currentLife <= 0)
            healthBarBoss.Hide();
    }
}