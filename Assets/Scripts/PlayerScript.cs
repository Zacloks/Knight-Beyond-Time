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
using System.Net.Http.Headers;

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
    public bool isAtacking = false;

    [Header("Componentes")]
    public Rigidbody2D entidad;
    public Animator anim;
    public HealthBar healthBar;
    public EnergyBar energyBar; 
    public Coin coinCounter;
    
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

        currentHealth = 50;
        if (healthBar != null) healthBar.setMaxHealth(maxHealth);
        healthBar.setHealth(currentHealth);

        currentEnergy = maxEnergy;
        if(energyBar != null) energyBar.setMaxEnergy(maxEnergy);

        if(coinCounter != null) coinCounter.setCoins(coins);

        //SetSpriteArma(equippedWeapon.sprite);
        updateEquippedWeapon();

    }

    void Update()
    {
        // 0. Contador de inmunidad: baja con el tiempo para que el jugador
        //    vuelva a ser vulnerable tras immuneTime segundos.
        if (curInmuneTime > 0) curInmuneTime -= Time.deltaTime;

        // 1. Leer Inputs
        if (!isDashing) direccionMov = mover.action.ReadValue<Vector2>();

        // 2. Lógica de Ataque (Tecla J)
        if (!isDashing && atacar != null && atacar.action.triggered) EjecutarAtaque();
        
        // Lógica de Ataque Mágico (Tecla K)
        if(!isDashing && attackMagic != null && attackMagic.action.triggered) EjecutarAtaqueMagic();

        if (!isAtacking && dash != null && dash.action.triggered && !isDashing) EjecutarDash();

        if (pick != null && pick.action.triggered && alcanzable != null) Pick();

        // 3. Animación de Movimiento y Giro
        if (anim != null)
        {
            float fuerza = direccionMov.magnitude;
            anim.SetFloat("Speed", fuerza);

            // Flip: Girar el personaje según dirección
            if (!isAtacking && direccionMov.x > 0.1f) transform.localScale = new Vector3(-1, 1, 1);
            else if (!isAtacking && direccionMov.x < -0.1f) transform.localScale = new Vector3(1, 1, 1);
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
        Weapon armaActual = inventario[indexInventario];

        if (armaActual != null)
        {
            bool ataqueExitoso = armaActual.Atacar(); 

            if (ataqueExitoso) 
            {
                string tipo = armaActual.GetType().Name;
                Debug.Log("Ataque confirmado. Arma: " + tipo);

                if (tipo == "WeaponMelee" && anim != null)
                {
                    anim.SetTrigger("2_Attack");
                }
                StartCoroutine(AtackCoroutine());
            }
            else 
            {
                Debug.Log("Ataque ignorado por Cooldown.");
            }
        }
        else
        {
            Debug.LogWarning("No hay arma equipada.");
        }
    }

    private IEnumerator AtackCoroutine()
    {
        isAtacking = true;

        yield return new WaitForSeconds(1);

        isAtacking = false;
    }

    void EjecutarAtaqueMagic()
    {   
        Weapon armaActual = inventario[indexInventario];
        if (armaActual != null)
        {
            bool exitoMagia = armaActual.AtaqueEspecial();

            if (exitoMagia && anim != null)
            {
                anim.SetTrigger("attackMagic");
                Debug.Log("¡Ataque Especial / Mágico ejecutado con K!");
                StartCoroutine(AtackCoroutine());
            }
        }
        else
        {
            Debug.LogWarning("No hay arma equipada para usar magia.");
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
        // Mientras dura el retroceso por daño no controlamos el movimiento:
        // dejamos que la velocidad del empujón lleve al jugador hacia atrás.
        if (!isHurt)
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
        }

        // Clamping (Límites) — siempre, también durante el retroceso.
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
        // Guardas null: sin SPUM o sin equipo asignado no hacemos nada (antes
        // lanzaba NullReferenceException al no tener arma ni spumEquipment).
        if (!useSPUM || spumEquipment == null) return;

        if (w == null) spumEquipment.UnequipWeapon();
        else spumEquipment.EquipWeapon(w.sprite);
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

    [Header("Retroceso al recibir daño")]
    public float hurtKnockbackForce = 8f;
    public float hurtDuration = 0.3f;
    private bool isHurt = false;

    // Sobrecarga sin fuente: retrocede hacia atrás según la última dirección de avance.
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, (Vector2)transform.position + lastDirection);
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (curInmuneTime > 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(healthBar != null) healthBar.setHealth(currentHealth);

        curInmuneTime = immuneTime;
        Debug.Log("Jugador recibió daño. Vida restante: " + currentHealth);
        anim.SetTrigger("5_Debuff");

        // Retroceso: empujar en dirección contraria a la fuente del daño y
        // bloquear el control un instante mientras corre la animación de hurt.
        Vector2 dir = ((Vector2)transform.position - sourcePosition).normalized;
        if (dir == Vector2.zero) dir = -lastDirection;
        StartCoroutine(HurtRoutine(dir));
    }

    private IEnumerator HurtRoutine(Vector2 direction)
    {
        isHurt = true;
        isDashing = false;
        entidad.linearVelocity = direction * hurtKnockbackForce;
        yield return new WaitForSeconds(hurtDuration);
        isHurt = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // El daño por contacto con enemigos lo aplica el propio enemigo en
        // Enemy.OnCollisionEnter2D (con crítico y sin dañar si está muriendo).
        // No lo duplicamos aquí para que el daño por contacto sea consistente.
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Salió la colisión");
    }

    private void Pick()
    {
        alcanzable.Pick(this);
        Debug.Log("Destruido??");
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

        if (collision.gameObject.CompareTag("Moneda"))
        {
            Moneda coin = collision.gameObject.GetComponent<Moneda>();
            if (coin != null)
            {
                coins += coin.value;
                coinCounter.setCoins(coins);
                Destroy(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        alcanzable = null;
        Debug.Log("Fuera de alcance");
    }

    public void testVida(int vida)
    {
        currentHealth += vida;

        if (currentHealth > 100) currentHealth = 100;
        healthBar.setHealth(currentHealth);
    }

}