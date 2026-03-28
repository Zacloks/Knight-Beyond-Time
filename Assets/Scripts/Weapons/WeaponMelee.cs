using UnityEngine;
using System.Collections;

public class WeaponMelee : Weapon 
{
    private bool isAttacking = false;
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
        transform.localRotation = Quaternion.Euler(0, 0, -45f);
        
        yield return new WaitForSeconds(0.15f);

        transform.localRotation = originalRotation;
        
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking && other.CompareTag("Enemy"))
        {
            Debug.Log($"<color=orange>{weaponName}:</color> ¡Corte melee! - {damage} daño.");

            SpriteRenderer enemySprite = other.GetComponent<SpriteRenderer>();
            if (enemySprite != null) StartCoroutine(FlashRed(enemySprite));

            ApplyKnockback(other, 5f);
        }
    }
}