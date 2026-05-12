using UnityEngine;
using System.Collections;

public class WeaponMelee : Weapon 
{
    [Header("Configuración Melee")]
    public float attackRadius = 1.5f;
    [Range(0, 360)] public float attackAngle = 90f;
    public float coneOffset = 0f; 
    public float damageDelay = 0.15f; 
    public LayerMask enemyLayer;
    
    private bool isAttacking = false;
    private Quaternion originalRotation;

    void Start() {
        originalRotation = transform.localRotation;
    }

    public override bool Atacar() {
        if (!isAttacking) {
            StartCoroutine(SwingRoutine());
            return true; 
        }
        return false; 
    }

    IEnumerator SwingRoutine()
    {
        isAttacking = true;
        Vector2 facingDir = transform.lossyScale.x < 0 ? Vector2.right : Vector2.left;
        facingDir = Quaternion.AngleAxis(coneOffset, Vector3.forward) * facingDir;

        yield return new WaitForSeconds(damageDelay);

        Collider2D[] enemigosEnRadio = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);
        
        foreach (Collider2D col in enemigosEnRadio)
        {
            Vector2 dirToEnemy = (col.transform.position - transform.position).normalized;
            
            float angleToEnemy = Vector2.Angle(facingDir, dirToEnemy);

            if (angleToEnemy <= attackAngle / 2f) 
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, transform.position);
                    SpriteRenderer enemySprite = col.GetComponent<SpriteRenderer>();
                    if (enemySprite != null) StartCoroutine(FlashRed(enemySprite));
                }
            }
        }

        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }  

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Vector3 facingDir = transform.lossyScale.x < 0 ? Vector3.right : Vector3.left;
        Vector3 directionWithOffset = Quaternion.AngleAxis(coneOffset, Vector3.forward) * facingDir;

        Vector3 leftDir = Quaternion.AngleAxis(-attackAngle / 2, Vector3.forward) * directionWithOffset;
        Vector3 rightDir = Quaternion.AngleAxis(attackAngle / 2, Vector3.forward) * directionWithOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftDir * attackRadius);
        Gizmos.DrawRay(transform.position, rightDir * attackRadius);
    }
}