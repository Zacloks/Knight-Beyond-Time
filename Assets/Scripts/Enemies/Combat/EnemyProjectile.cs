using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private float speed;
    private Vector2 direction = Vector2.right;

    [Tooltip("Lista de posibles sprites para la flecha")]
    [SerializeField] private Sprite[] arrowSprites;
    private SpriteRenderer spriteRenderer;

    [Tooltip("Segundos antes de autodestruirse si no choca con nada")]
    [SerializeField] private float lifeTime = 5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Setup(int dmg, float spd, Vector2 dir)
    {
        damage = dmg;
        speed = spd;

        //Elige sprite aleatorio entre los sprites de proyectiles agregados en la lista.
        if (arrowSprites != null && arrowSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, arrowSprites.Length);
            spriteRenderer.sprite = arrowSprites[randomIndex];
        }

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
