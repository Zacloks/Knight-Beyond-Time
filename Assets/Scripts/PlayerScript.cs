using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerScript : MonoBehaviour
{
    [Header("Movimiento y Límites")]
    public float velocidadMov = 7;
    public float minX = -8.4f, maxX = 8.39f;
    public float minY = -4.94f, maxY = 4.2f;

    [Header("Referencias de Input")]
    public InputActionReference mover;
    public InputActionReference atacar; 
    public InputActionReference attackMagic; 
    public InputActionReference dash;

    [Header("Atributos RPG")]
    public int maxHealth = 100;
    public int currentHealth;
    public int maxEnergy = 100;
    public int currentEnergy;
    public int coins = 0;
    public float dashSpeed = 12;
    public bool isDashing = false;

    [Header("Componentes")]
    public Rigidbody2D entidad;
    public Animator anim;
    public HealthBar healthBar;
    public EnergyBar energyBar; 
    public Coin coin;
    public Weapon equippedWeapon;
    private Vector2 direccionMov;
    private Vector2 lastDirection = Vector2.right; 
    private int count;

    void Start()
    {
        entidad = GetComponent<Rigidbody2D>();

        // Localizar el Animator en el hijo para compatibilidad con SPUM
        if (anim == null) anim = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;
        if (healthBar != null) healthBar.setMaxHealth(maxHealth);

        currentEnergy = maxEnergy;
        if(energyBar != null) energyBar.setMaxEnergy(maxEnergy);

        if(coin != null) coin.setCoins(coins);
    }

    void Update()
    {
        // 1. Leer Inputs
        if (!isDashing) direccionMov = mover.action.ReadValue<Vector2>();

        // 2. Lógica de Ataque (Tecla J)
        if (atacar != null && atacar.action.triggered) EjecutarAtaque();
        
        
        // Lógica de Ataque Mágico (Tecla K)
        if(attackMagic != null && attackMagic.action.triggered) EjecutarAtaqueMagic();
        

        if (dash != null && dash.action.triggered && !isDashing) EjecutarDash();

        // 3. Animación de Movimiento y Giro
        if (anim != null)
        {
            float fuerza = direccionMov.magnitude;
            anim.SetFloat("Speed", fuerza);

            // Flip: Girar el personaje según dirección
            if (direccionMov.x > 0.1f) transform.localScale = new Vector3(-1, 1, 1);
            else if (direccionMov.x < -0.1f) transform.localScale = new Vector3(1, 1, 1);
        }

        // 4. Lógica de energía pasiva
        count++;
        if (count % 100 == 0 && currentEnergy < 100) {
            currentEnergy += 1;
            if (energyBar != null) energyBar.setEnergy(currentEnergy);
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

    void EjecutarDash()
    {
        if (currentEnergy >= 60) {
                currentEnergy -= 60;
                energyBar.setEnergy(currentEnergy);

                Debug.Log("Dash activado, energía: " + currentEnergy);
                StartCoroutine(DashCoroutine());         
            }
    }

    private IEnumerator DashCoroutine()
    {
        float originalVelocidad = velocidadMov;
        velocidadMov = dashSpeed;
        isDashing = true;

        anim.SetTrigger("7_Dash");

        yield return new WaitForSeconds((float)0.5);  

        velocidadMov = originalVelocidad;  
        isDashing = false;

        Debug.LogWarning("Dash terminado");
    }

    void FixedUpdate()
    {
        // Movimiento físico
        if (direccionMov.sqrMagnitude < 0.01f) {
            entidad.linearVelocity = Vector2.zero;

            if (isDashing) entidad.linearVelocity = lastDirection * velocidadMov;

        } else {
            entidad.linearVelocity = direccionMov * velocidadMov;

            if (!isDashing)
            {
                if (direccionMov.x < 0) lastDirection = Vector2.left;
                else lastDirection = Vector2.right;
            }
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
    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Salió la colisión");
    }

}