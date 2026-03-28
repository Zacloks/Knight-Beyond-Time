using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
    [Header("Atributos Base")]
    public string weaponName = "Arma Genérica";
    public int damage = 10;
    public float attackRate = 0.5f;

    // Este método lo definirá cada hijo (Melee, Rango, Magia)
    public abstract void Atacar();

    // Esto es común para todas las armas, así que se queda en el padre
    protected IEnumerator FlashRed(SpriteRenderer sprite)
    {
        Color originalColor = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sprite != null) sprite.color = originalColor;
    }

    // Lógica de empuje común
    protected void ApplyKnockback(Collider2D enemy, float force)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyRb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}