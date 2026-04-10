using UnityEngine;
using System.Collections;

public class WeaponMelee : Weapon 
{
    [Header("Configuración Melee")]
    public float attackRadius = 1.5f;
    public LayerMask enemyLayer;
    private bool isAttacking = false;
    private bool canDamage = false;
    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    public override void Atacar()
    {
        if (!isAttacking)
        {
            StartCoroutine(SwingRoutine());
        }
    }

    IEnumerator SwingRoutine()
    {
        isAttacking = true;
        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);
        
        foreach (Collider2D col in enemigosGolpeados)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Vector2 dir = (col.transform.position - transform.position).normalized;
                    
                    enemy.TakeDamage(damage, dir);
                    SpriteRenderer enemySprite = col.GetComponent<SpriteRenderer>();
                    if (enemySprite != null) 
                    {
                        StartCoroutine(FlashRed(enemySprite)); 
                    }

                    Debug.Log($"<color=red>¡Impacto visual en {col.name}!</color>");
                }
            }
        }

        transform.localRotation = Quaternion.Euler(0, 0, -45f);
        yield return new WaitForSeconds(0.15f);
        transform.localRotation = originalRotation;

        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }  

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canDamage && other.CompareTag("Enemy"))
        {
            Debug.Log($"<color=orange>{weaponName}:</color> ¡Corte melee! - {damage} daño.");
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage, knockbackDir);
            }
            SpriteRenderer enemySprite = other.GetComponent<SpriteRenderer>();
            if (enemySprite != null) StartCoroutine(FlashRed(enemySprite));

            ApplyKnockback(other, 5f);
            canDamage = false; 
        }
    }
}