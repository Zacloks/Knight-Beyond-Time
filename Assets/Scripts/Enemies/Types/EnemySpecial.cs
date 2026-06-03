using UnityEngine;

public class EnemySpecial : Enemy
{
    public override bool PuedeSoltarMonedas => true;

    protected override void Start()
    {
        base.Start();
    }
}