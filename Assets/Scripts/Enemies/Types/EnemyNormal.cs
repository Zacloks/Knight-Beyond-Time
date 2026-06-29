using UnityEngine;

public class EnemyNormal : Enemy
{
    public override bool PuedeSoltarMonedas => true;

    protected override void Start()
    {
        base.Start();
    }
}