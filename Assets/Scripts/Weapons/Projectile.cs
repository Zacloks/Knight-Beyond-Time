using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private float knockbackForce;
    private Weapon parentWeapon;
    private float immobilizeDuration = 0f; // 0 = no inmoviliza

    public void Setup(int dmg, float spd, float kb, Weapon weapon)
    {
        damage = dmg;
        speed = spd;
        knockbackForce = kb;
        parentWeapon = weapon;
    }

    // Opcional: hace que el proyectil inmovilice al enemigo al impactar (báculo).
    public void SetInmovilizacion(float duracion)
    {
        immobilizeDuration = duracion;
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.position);

                if (immobilizeDuration > 0f) enemy.Inmovilizar(immobilizeDuration);

                SpriteRenderer enemySprite = other.GetComponent<SpriteRenderer>();
                if (enemySprite != null && parentWeapon != null)
                {
                    parentWeapon.StartCoroutine("FlashRed", enemySprite);
                }
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Escenario"))
        {
            Destroy(gameObject);
        }
    }
}