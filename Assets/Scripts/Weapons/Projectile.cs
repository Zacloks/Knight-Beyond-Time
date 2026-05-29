using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private float knockbackForce;
    private Weapon parentWeapon; 

    public void Setup(int dmg, float spd, float kb, Weapon weapon)
    {
        damage = dmg;
        speed = spd;
        knockbackForce = kb;
        parentWeapon = weapon;
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