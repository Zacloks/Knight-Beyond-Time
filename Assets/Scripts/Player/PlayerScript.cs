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
using UnityEngine.AI;
public class PlayerScript : MonoBehaviour
{
    [Header("Movimiento y Límites")]
    public float velocidadMov = 7;
   /* public float minX = -8.4f, maxX = 8.39f;
    public float minY = -4.94f, maxY = 4.2f;
*/
    public float minX = -100000f, maxX = 10000f;
    public float minY = -100000f, maxY = 46.49283f;

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
    public bool isDrinking = false;

    [Header("Componentes")]
    public Rigidbody2D entidad;
    public Animator anim;
    public HealthBar healthBar;
    public EnergyBar energyBar; 
    public Coin coinCounter;
    private Vector2 direccionMov;
    private Vector2 lastDirection = Vector2.right; 
    private int count;

    [Header("Estados de pociones")]
    public bool dashInfinito = false;
    public float extraVelocidad = 1;
    
    [Header("Sistema inventario")]
    public Inventory inventario;
    private Item alcanzable;
    [Header("SPUM Integration")]
    public bool useSPUM = true; // Ponerlo como true si se usará un SPUM.
    public SPUMEquipmentManager spumEquipment;
    public SPUMPlayerBridge spumBridge;
    private Item itemEnMano;
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

        updateItem(inventario.getEquippedItem());
    }

    void Update()
    {
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
            if (isAtacking || isDrinking)
            {
                if (anim != null) anim.SetFloat("Speed", 0);   
                return;
            }

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
            dropItem();
        }

        if (cambiarItemLeft != null && cambiarItemLeft.action.triggered)
        {
            swapLeftItem();
        }
        if (cambiarItemRight != null && cambiarItemRight.action.triggered)
        {
            swapRightItem();
        }
    }

    void EjecutarAtaque()
    {
        ItemData item = inventario.getEquippedItem();

        if (item != null)
        {
            itemEnMano.Usar(this);

        }

        else if (anim != null)
        {
            anim.SetTrigger("2_Attack");
            Debug.Log("¡Ataque ejecutado con J!");

            StartCoroutine(AtackCoroutine());        
        }
    }

    public void IniciarAnimacion(string nombreAnimacion)
    {
        if (anim != null)
        {
            anim.SetTrigger(nombreAnimacion);
            StartCoroutine(AtackCoroutine());
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
        if (anim != null)
        {
            anim.SetTrigger("attackMagic");
            Debug.Log("¡Ataque ejecutado con K!");

            StartCoroutine(AtackCoroutine());       
        }
    }

    void EjecutarDash()
    {
        if (dashInfinito)
        {
            StartCoroutine(DashCoroutine());
        }
        else if (currentEnergy >= 60) 
        {
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
        if (isDrinking)
        {
            entidad.linearVelocity = Vector2.zero;
            return;
        }
        // Movimiento físico
        if (direccionMov.sqrMagnitude < 0.01f) {
            entidad.linearVelocity = Vector2.zero;
            // O entidad.linearVelocity = lastDirection * velocidadMov * 0.3f;

            if (isDashing) entidad.linearVelocity = lastDirection * velocidadMov * extraVelocidad;

        } else {
            entidad.linearVelocity = direccionMov * velocidadMov * extraVelocidad;

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
    private void swapLeftItem()
    {
        updateItem(inventario.swapLeft());
    }
    private void swapRightItem()
    {
        updateItem(inventario.swapRight());
    }
    
    private void updateItem(ItemData i)
    {
        if (itemEnMano != null)
        {
            Destroy(itemEnMano.gameObject);
            itemEnMano = null;
        }

        if (i == null) 
        {
            if (useSPUM && spumEquipment != null) spumEquipment.UnequipItem();
        } 
        else if (useSPUM && spumEquipment != null) 
        {
            spumEquipment.EquipItem(i.sprite);
        } 

        if (i != null && i.prefab != null)
        {
            itemEnMano = Instantiate(i.prefab, transform);
            itemEnMano.datos = i;
            
            SpriteRenderer sr = itemEnMano.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
            
            Collider2D col = itemEnMano.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }
    public void dropItem()
    {
        inventario.dropItem();
        if (useSPUM && spumEquipment != null) {
            spumEquipment.UnequipItem();
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

    private void Pick()
    {
        Item item;
        if (inventario.full()) {
            Debug.Log("Inventario Lleno");
            return;
        }
        if (alcanzable.enVenta) item = alcanzable.Comprar(this);

        else item = alcanzable;

        if (item != null)
        {
            inventario.add(item.datos);
            updateItem(inventario.getEquippedItem());
            Destroy(item.gameObject);
        }
        Debug.Log("Destruido??");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("En alcance");
        if (collision.gameObject.CompareTag("Consumible"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
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

    public void buy(int price)
    {
        coins -= price;

        coinCounter.setCoins(coins);
    }

    public void IniciarAnimacionMitad(string nombreAnimacion)
{
    if (anim != null)
    {
        StartCoroutine(AnimacionMitadCoroutine(nombreAnimacion));
    }
}

private IEnumerator AnimacionMitadCoroutine(string nombreAnimacion)
{
    anim.SetTrigger(nombreAnimacion);

    yield return new WaitForEndOfFrame();

    AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
    
    float tiempoMitad = stateInfo.length / 2f;

    yield return new WaitForSeconds(tiempoMitad);

    anim.CrossFade("0_Idle", 0.05f); 
}

public void ActivarDashInfinito(float duracion)
{
    StartCoroutine(RutinaDashInfinito(duracion));
}

private IEnumerator RutinaDashInfinito(float duracion)
{
    dashInfinito = true;

    yield return new WaitForSeconds(duracion);

    dashInfinito = false;
}

    public void ActivarVelocidad(float aumento, float duracion)
    {
        StartCoroutine(RutinaVelocidad(aumento, duracion));
    }
    private IEnumerator RutinaVelocidad(float aumento, float duracion)
{
    extraVelocidad = aumento;

        yield return new WaitForSeconds(duracion);

        extraVelocidad = 1;
}

}