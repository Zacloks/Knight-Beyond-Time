using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    [HideInInspector] public int damage; 
    public float lifeTime = 2f; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(int weaponDamage)
    {
        damage = weaponDamage;
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
           
            Debug.Log("Impacto en: " + other.name + " con daño: " + damage);
            Destroy(gameObject); 
        }
    }
}