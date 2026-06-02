using UnityEngine;

public class EnemyBoss : Enemy
{
    public override bool PuedeSoltarMonedas => true;
    
    protected override void Start()
    {
        base.Start();
    }
}