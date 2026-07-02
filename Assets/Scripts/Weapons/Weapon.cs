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

    protected PlayerMovement ownerMovement;
    protected PlayerStats ownerStats;

    protected virtual void Awake()
    {
        if (durabilidadActual <= 0)
            durabilidadActual = maxDurabilidad;

        ownerMovement = GetComponentInParent<PlayerMovement>();
        ownerStats = GetComponentInParent<PlayerStats>();
    }

    protected float AttackRateActual()
    {
        float mult = ownerStats != null ? ownerStats.multiplicadorVelocidadAtaque : 1f;
        if (mult <= 0f) mult = 1f;
        return attackRate / mult;
    }

    protected float MultiplicadorAlcanceJugador()
    {
        return ownerStats != null ? ownerStats.multiplicadorAlcance : 1f;
    }


    public Vector2 FacingDir()
    {
        if (ownerMovement != null)
            return ownerMovement.LastDirection.x >= 0f ? Vector2.right : Vector2.left;

        return transform.lossyScale.x < 0f ? Vector2.right : Vector2.left;
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
        float mult = ownerStats != null ? ownerStats.multiplicadorDaño : 1f;
        float critExtra = ownerStats != null ? ownerStats.bonusCritico : 0f;
        int dañoConBuff = Mathf.RoundToInt(dañoBase * mult);

        float random = Random.Range(0f, 100f);
        if (random <= probabilidadCritico + critExtra)
        {
            Debug.Log($"¡CRÍTICO con {weaponName}!");
            return Mathf.RoundToInt(dañoConBuff * multiplicadorCritico);
        }
        return dañoConBuff;
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

    // Si el arma se quedó sin durabilidad, pide al inventario que la rompa
    // (la quita del slot y de la mano). Llamar DESPUÉS de aplicar el efecto del
    // ataque, para que el golpe que la rompe sí cuente.
    protected void ComprobarRotura()
    {
        if (durabilidadActual > 0) return;

        PlayerInventory inv = GetComponentInParent<PlayerInventory>();
        if (inv != null) inv.RomperItemEquipado(this);
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