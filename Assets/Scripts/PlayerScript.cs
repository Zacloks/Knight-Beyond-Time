using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [Header("Movimiento y Límites")]
    public float velocidadMov = 10;
    public float minX = -8.4f, maxX = 8.39f;
    public float minY = -4.94f, maxY = 4.2f;

    [Header("Referencias de Input")]
    public InputActionReference mover;
    public InputActionReference atacar; 
    public InputActionReference attackMagic; 

    [Header("Atributos RPG")]
    public int maxHealth = 100;
    public int currentHealth;
    public int coins = 0;

    [Header("Componentes")]
    public Rigidbody2D entidad;
    public Animator anim;
    public HealthBar healthBar;
    public Coin coin;

    private Vector2 direccionMov;
    private int count;

    void Start()
    {
        entidad = GetComponent<Rigidbody2D>();

        // Localizar el Animator en el hijo para compatibilidad con SPUM
        if (anim == null) {
            anim = GetComponentInChildren<Animator>();
        }

        currentHealth = maxHealth;
        if(healthBar != null) healthBar.setMaxHealth(maxHealth);
        if(coin != null) coin.setCoins(coins);
    }

    void Update()
    {
        // 1. Leer Inputs
        direccionMov = mover.action.ReadValue<Vector2>();

        // 2. Lógica de Ataque (Tecla J)
        if (atacar != null && atacar.action.triggered)
        {
            EjecutarAtaque();
        }
        
        // Lógica de Ataque Mágico (Tecla K)
        if(attackMagic != null && attackMagic.action.triggered)
        {
            EjecutarAtaqueMagic();
        }

        // 3. Animación de Movimiento y Giro
        if (anim != null)
        {
            float fuerza = direccionMov.magnitude;
            anim.SetFloat("Speed", fuerza);

            // Flip: Girar el personaje según dirección
            if (direccionMov.x > 0.1f) transform.localScale = new Vector3(-1, 1, 1);
        else if (direccionMov.x < -0.1f) transform.localScale = new Vector3(1, 1, 1);
        }

        // 4. Lógica de salud pasiva
        count++;
        if (count % 1000 == 0) {
            currentHealth -= 1;
            if(healthBar != null) healthBar.setHealth(currentHealth);
        }
    }

    void EjecutarAtaque()
    {
        if (anim != null)
        {
            // "2_Attack" es el nombre estándar del Trigger en SPUM
            anim.SetTrigger("2_Attack");
            Debug.Log("¡Ataque ejecutado con J!");
        }
    }

    void EjecutarAtaqueMagic()
    {
        if (anim != null)
        {
            anim.SetTrigger("attackMagic");
            Debug.Log("¡Ataque ejecutado con K!");
        }
    }

    void FixedUpdate()
    {
        // Movimiento físico
        if (direccionMov.sqrMagnitude < 0.01f) {
            entidad.linearVelocity = Vector2.zero;
        } else {
            entidad.linearVelocity = direccionMov * velocidadMov;
        }

        // Clamping (Límites)
        float clampedX = Mathf.Clamp(entidad.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(entidad.position.y, minY, maxY);
        entidad.position = new Vector2(clampedX, clampedY);
    }

    [Header("Inmunidad")]
    public float immuneTime = 2f;
    private float curInmuneTime;

    public void TakeDamage(int amount)
    {
        if (curInmuneTime > 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if(healthBar != null) healthBar.setHealth(currentHealth);

        curInmuneTime = immuneTime; 
        Debug.Log("Jugador recibió daño. Vida restante: " + currentHealth);
        anim.SetTrigger("5_Debuff");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                TakeDamage(enemy.GetDamage());
            }
        }
    }

}