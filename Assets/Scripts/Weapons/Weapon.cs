using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
    [Header("Atributos Base")]
    public string weaponName = "Arma Genérica";
    public int damage = 10;
    public float attackRate = 0.5f;

    public Sprite sprite;
    public abstract void Atacar();

    protected IEnumerator FlashRed(SpriteRenderer sprite)
    {
        Color originalColor = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sprite != null) sprite.color = originalColor;
    }

    protected void ApplyKnockback(Collider2D enemyCollider, float force)
    {
        Enemy enemy = enemyCollider.GetComponent<Enemy>();
        Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();

        if (enemy != null)
        {
            Vector2 direction = (enemyCollider.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, direction); 
        }

        if (enemyRb != null)
        {
            Vector2 direction = (enemyCollider.transform.position - transform.position).normalized;
            enemyRb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}