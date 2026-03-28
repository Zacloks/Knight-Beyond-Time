using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Atributos del Arma")]
    public string weaponName = "Espada Medieval";
    public int damage = 10;
    public float attackRate = 0.5f; 
    public bool isMelee = true; 

    [Header("Referencias")]
    public GameObject projectilePrefab; 

    private bool isAttacking = false;
    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    public void Swing()
    {
        if (!isAttacking)
        {
            StartCoroutine(SwingRoutine());
        }
    }

    IEnumerator SwingRoutine()
    {
        isAttacking = true;

        transform.localRotation = Quaternion.Euler(0, 0, -45f);
        
        yield return new WaitForSeconds(0.15f);

        transform.localRotation = originalRotation;
        
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking && other.CompareTag("Enemy"))
        {
            Debug.Log("<color=red>¡IMPACTO!</color> Golpeaste a " + other.name + " infligiendo " + damage + " de daño.");

            SpriteRenderer enemySprite = other.GetComponent<SpriteRenderer>();
            if (enemySprite != null)
            {
                StartCoroutine(FlashRed(enemySprite));
            }

            Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                enemyRb.AddForce(direction * 5f, ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator FlashRed(SpriteRenderer sprite)
    {
        Color originalColor = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
    }
}