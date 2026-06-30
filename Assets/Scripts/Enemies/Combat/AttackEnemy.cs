using UnityEngine;

/// <summary>Ataque cuerpo a cuerpo: golpea en un círculo alrededor del attackController.</summary>
public class AttackEnemy : AttackEnemyBase
{
    [Header("Melee")]
    [SerializeField] private Transform attackController;
    [SerializeField] private LayerMask hittableLayers;
    [SerializeField] private float circleRadius = 0.5f;

    protected override bool TargetInRange()
    {
        if (attackController == null) return false;
        return Physics2D.OverlapCircle(attackController.position, circleRadius, hittableLayers);
    }

    protected override void OnAttack()
    {
        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(attackController.position, circleRadius, hittableLayers);
        foreach (Collider2D obj in objectsHit)
            if (obj.TryGetComponent(out PlayerScript playerScript))
                playerScript.TakeDamage(ResolveDamage(), transform.position);
    }

    public override float AttackReach =>
        attackController == null
            ? circleRadius
            : Vector2.Distance(transform.position, attackController.position) + circleRadius;

    private void OnDrawGizmos()
    {
        if (attackController == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackController.position, circleRadius);
    }
}
