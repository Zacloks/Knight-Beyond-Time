using UnityEngine;

/// <summary>Ataque a distancia: instancia un EnemyProjectile hacia el jugador.</summary>
public class AttackEnemyRanged : AttackEnemyBase
{
    [Header("A distancia")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootRange = 6f;
    [SerializeField] private float projectileSpeed = 9f;

    protected override bool TargetInRange()
    {
        if (Player == null) return false;
        return Vector2.Distance(transform.position, Player.position) <= shootRange;
    }

    protected override void OnAttack()
    {
        if (projectilePrefab == null || Player == null) return;

        Vector2 origin = shootPoint != null ? (Vector2)shootPoint.position : (Vector2)transform.position;
        Vector2 dir = ((Vector2)Player.position - origin).normalized;

        GameObject go = Instantiate(projectilePrefab, origin, Quaternion.identity);

        if (go.TryGetComponent(out EnemyProjectile proj))
            proj.Setup(ResolveDamage(), projectileSpeed, dir);
        else
            Debug.LogWarning($"{name}: el projectilePrefab no tiene el script EnemyProjectile.", this);
    }

    public override float AttackReach => shootRange;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
