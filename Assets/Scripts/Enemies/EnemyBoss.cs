using UnityEngine;

public class EnemyBoss : Enemy
{
    public HealthBarBoss healthBarBoss;

    protected override void Start()
    {
        base.Start();
        healthBarBoss.Show(lifeEnemy.currentLife); 
    }

    public override void TakeDamage(int amount, Vector2 sender)
    {
        base.TakeDamage(amount, sender);
        healthBarBoss.SetHealth(lifeEnemy.currentLife);

        if (lifeEnemy.currentLife <= 0)
            healthBarBoss.Hide();
    }
}