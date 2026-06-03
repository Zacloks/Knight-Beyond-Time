using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private float speed;
    private Vector2 direction = Vector2.right;

    [Tooltip("Segundos antes de autodestruirse si no choca con nada")]
    [SerializeField] private float lifeTime = 5f;

    public void Setup(int dmg, float spd, Vector2 dir)
    {
        damage = dmg;
        speed = spd;
        if (dir != Vector2.zero) direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerScript player))
                player.TakeDamage(damage, transform.position);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Escenario"))
        {
            Destroy(gameObject);
        }
    }
}
