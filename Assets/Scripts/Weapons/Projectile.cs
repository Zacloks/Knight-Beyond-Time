using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private float knockbackForce;
    private Weapon parentWeapon;
    private float immobilizeDuration = 0f; // 0 = no inmoviliza

    // Robo de vida (Libro Malvado): cura al jugador un % del daño al golpear.
    private float lifestealPct = 0f;       // 0 = sin robo de vida
    private PlayerScript lifestealTarget;

    // Explosión / destello en área (Báculo del Sol) al impactar.
    private float explosionRadius = 0f;    // 0 = sin explosión
    private int explosionDamage = 0;
    private GameObject explosionEffectPrefab;
    private Color explosionColor = Color.white;

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

    // Opcional: cura al jugador un porcentaje (0..1) del daño al golpear a un enemigo.
    public void SetRoboVida(float porcentaje, PlayerScript jugador)
    {
        lifestealPct = porcentaje;
        lifestealTarget = jugador;
    }

    // Opcional: al impactar a un enemigo, genera un destello que daña a los
    // enemigos cercanos dentro de 'radio'. 'effectPrefab' es opcional (si es null
    // se genera un destello por código).
    public void SetExplosion(float radio, int dañoArea, GameObject effectPrefab, Color color)
    {
        explosionRadius = radio;
        explosionDamage = dañoArea;
        explosionEffectPrefab = effectPrefab;
        explosionColor = color;
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

                // Robo de vida: cura al jugador solo si realmente golpeó a un enemigo.
                if (lifestealPct > 0f && lifestealTarget != null && lifestealTarget.Stats != null)
                {
                    int cura = Mathf.RoundToInt(damage * lifestealPct);
                    if (cura > 0) lifestealTarget.Stats.Heal(cura);
                }

                SpriteRenderer enemySprite = other.GetComponent<SpriteRenderer>();
                if (enemySprite != null && parentWeapon != null)
                {
                    parentWeapon.StartCoroutine("FlashRed", enemySprite);
                }

                if (explosionRadius > 0f) Explotar(transform.position, enemy);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Escenario"))
        {
            Destroy(gameObject);
        }
    }

    private void Explotar(Vector3 centro, Enemy golpeadoDirecto)
    {
        Destello.Crear(centro, explosionRadius, explosionColor);
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, centro, Quaternion.identity);

        Collider2D[] cercanos = Physics2D.OverlapCircleAll(centro, explosionRadius);
        foreach (Collider2D c in cercanos)
        {
            if (!c.CompareTag("Enemy")) continue;

            Enemy e = c.GetComponent<Enemy>();
            if (e == null || e == golpeadoDirecto) continue; 

            e.TakeDamage(explosionDamage, centro);

            SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
            if (sr != null && parentWeapon != null)
                parentWeapon.StartCoroutine("FlashRed", sr);
        }
    }
}