using UnityEngine;
using System.Collections;

public abstract class Weapon : Item
{
    [Header("Atributos Base")]
    public string weaponName = "Arma Genérica";
    public int damage = 10;
    public float attackRate = 0.5f;

    public Sprite sprite;
    public bool usaAnimacionJugador = true;

    [Header("Críticos")]
    [Range(0f, 100f)] public float probabilidadCritico = 5f;
    public float multiplicadorCritico = 2f;

    [Header("Durabilidad")]
    public int maxDurabilidad = 20;
    public int durabilidadActual;

    protected virtual void Awake()
    {
        if (durabilidadActual <= 0)
            durabilidadActual = maxDurabilidad;
    }

    public abstract bool Atacar();

    public virtual bool AtaqueEspecial()
    {
        return false;
    }
    protected virtual void durabilidadCheck()
    {
        durabilidadActual = maxDurabilidad;
    }
    public int CalcularDañoCritico(int dañoBase)
    {
        float random = Random.Range(0f, 100f);
        if (random <= probabilidadCritico)
        {
            Debug.Log($"¡CRÍTICO con {weaponName}!");
            return Mathf.RoundToInt(dañoBase * multiplicadorCritico);
        }
        return dañoBase; 
    }

    public void GastarDurabilidad(int cantidad = 1)
    {
        durabilidadActual -= cantidad;
        if (durabilidadActual <= 0)
        {
            durabilidadActual = 0;
            Debug.Log($"¡El arma {weaponName} se ha roto!");
        }
    }

    protected IEnumerator FlashRed(SpriteRenderer spr)
    {
        Color originalColor = spr.color;
        spr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (spr != null) spr.color = originalColor;
    }

    protected void ApplyKnockback(Collider2D enemyCollider, float force)
    {
        Enemy enemy = enemyCollider.GetComponent<Enemy>();
        Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();

        if (enemy != null)
        {
            Vector2 direction = (enemyCollider.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, transform.position); 
        }

        if (enemyRb != null)
        {
            Vector2 direction = (enemyCollider.transform.position - transform.position).normalized;
            enemyRb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}