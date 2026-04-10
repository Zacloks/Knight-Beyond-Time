using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private float knockbackForce;
    private Weapon parentWeapon; 
    public float lifeTime;

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
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage, knockbackDir * knockbackForce);
                SpriteRenderer enemySprite = other.GetComponent<SpriteRenderer>();
                if (enemySprite != null && parentWeapon != null)
                {
                    parentWeapon.StartCoroutine("FlashRed", enemySprite);
                }
            }else if (other.CompareTag("Escenario")) {
                Destroy(gameObject);
            }   

            Destroy(gameObject);
        }
    }
}