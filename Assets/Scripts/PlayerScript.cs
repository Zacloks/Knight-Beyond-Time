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
using JetBrains.Annotations;

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
    public InputActionReference dropearItem;
    public InputActionReference cambiarItemLeft;
    public InputActionReference cambiarItemRight;
    public InputActionReference pick;


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
    
    private Vector2 direccionMov;
    private Vector2 lastDirection = Vector2.right; 
    private int count;
    
    [Header("Sistema inventario")]
    private int indexInventario = 0;
    public Weapon[] inventario = new Weapon[5];
    //public Weapon equippedWeapon = inventario[indexInventario];
    private Consumible alcanzable;

    [Header("SPUM Integration")]
    public bool useSPUM = false; // Ponerlo como true si se usará un SPUM.
    public SPUMPlayerBridge spumBridge;
    public SPUMEquipmentManager spumEquipment;


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

        //SetSpriteArma(equippedWeapon.sprite);
        updateEquippedWeapon();

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

        if (pick != null && pick.action.triggered) Debug.Log("Picked");

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

        if (dropearItem != null && dropearItem.action.triggered)
        {
            dropSpriteArma();
        }

        if (cambiarItemLeft != null && cambiarItemLeft.action.triggered)
        {
            swapLeft();
        }
        if (cambiarItemRight != null && cambiarItemRight.action.triggered)
        {
            swapRight();
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

    private void swapLeft()
    {
        if (indexInventario>0)
        {
            indexInventario--;
        } else
        {
            indexInventario = 4;
        }
        Debug.Log(indexInventario);
        updateEquippedWeapon();
    }

    private void swapRight()
    {
        if (indexInventario < 4)
        {
            indexInventario++;
        } else
        {
            indexInventario = 0;
        }
        Debug.Log(indexInventario);
        updateEquippedWeapon();
    }

    private void updateEquippedWeapon()
    {
        Weapon w = inventario[indexInventario];
        if (w == null) spumEquipment.UnequipWeapon();
        else if (useSPUM && spumEquipment != null) {
            spumEquipment.EquipWeapon(w.sprite);
        } 
    }

    private void dropSpriteArma()
    {
        if (useSPUM && spumEquipment != null && inventario[indexInventario] != null) {
            spumEquipment.UnequipWeapon();
            inventario[indexInventario] = null;
            Debug.Log("Botaste el item!");
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("En alcance");
        if (collision.gameObject.CompareTag("Consumible"))
        {
            Consumible item = collision.gameObject.GetComponent<Consumible>();
            if (item != null)
            {
                alcanzable = item;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        alcanzable = null;
        Debug.Log("Fuera de alcance");
    }

}