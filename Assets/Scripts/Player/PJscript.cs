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

public class PJscript : MonoBehaviour
{
    //Atributos: 
    public float velocidadMov = 10; //check
    public int maxHealth = 100; //check
    public int currentHealth; //check
    public int maxEnergy = 100; //check 
    public int currentEnergy; //check 
    public int immuneTime = 2; //check 
    public int curInmuneTime; //checl
    public float dashSpeed = 15; //check

    public int coins = 0; //check

    //Referencias:
    private Vector2 direccionMov; //check
    private Vector2 lastDirection = Vector2.right; //check
    public PlayerInput playerInput; //mhhhhhhhhhhhhhhhh
    public Weapon equippedWeapon; //check 
    public Rigidbody2D entidad; //check
    public bool isDashing = false; //check
    //Referencias
    public InputActionReference mover; //check
    public Animator anim; //check

    public HealthBar healthBar; //check
    public EnergyBar energyBar; //check
    public Coin coin; //check

    int count; //check
  
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        entidad = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
        if (anim == null) {
        anim = GetComponentInChildren<Animator>();
        }
        currentHealth = maxHealth;
        if(healthBar != null) healthBar.setMaxHealth(maxHealth);
        currentEnergy = maxEnergy;
        if(energyBar != null) energyBar.setMaxEnergy(maxEnergy);
        if(coin != null) coin.setCoins(coins);
    }

    // Update is called once per frame
   void Update() //check
{
    // 1. LEER EL TECLADO
    if (!isDashing) direccionMov = mover.action.ReadValue<Vector2>();

    // 2. ENVIAR AL ANIMATOR (Solo si existe)
    if (anim != null)
    {
        // Calculamos la fuerza del movimiento (va de 0 a 1)
        float fuerza = direccionMov.magnitude;
        
        // Enviamos el dato al parámetro "Speed" del Animator
        anim.SetFloat("Speed", fuerza);
        
        // Debug para confirmar en consola que el número NO es cero
        if(fuerza > 0.01f) {
            Debug.Log("¡Movimiento detectado! Enviando Speed: " + fuerza);
        }
    }

    // --- Lógica de salud (Count) ---
    count++;
    if (count % 100 == 0 && currentEnergy < 100) {
        currentEnergy += 1;
        if (energyBar != null) energyBar.setEnergy(currentEnergy);
    }
}
    void FixedUpdate(){
    
        if (direccionMov.sqrMagnitude < 0.01f) 

        {
            entidad.linearVelocity = Vector2.zero;
            entidad.angularVelocity = 0f; // Evita que rote solo
            
            if (isDashing) entidad.linearVelocity = lastDirection * velocidadMov;
        }

        else 
        {
            // 2. APLICAR MOVIMIENTO
            entidad.linearVelocity = direccionMov * velocidadMov;

            if (!isDashing)
            {
                if (direccionMov.x < 0) lastDirection = Vector2.left;
                else if (direccionMov.x > 0) lastDirection = Vector2.right;
            }
        }

        // 3. LÍMITES DE PANTALLA (Clamping)
        float clampedX = Mathf.Clamp(entidad.position.x, -8.4f, 8.39f);
        float clampedY = Mathf.Clamp(entidad.position.y, -4.94f, 3.49f);
        entidad.position = new Vector2(clampedX, clampedY);

    }
  
    public void Dash(InputAction.CallbackContext context) 
    {   
        if (context.performed && !isDashing) {
            
            if (currentEnergy >= 60) {
                currentEnergy -= 60;
                energyBar.setEnergy(currentEnergy);

                Debug.Log("Dash activado, energía: " + currentEnergy);
                StartCoroutine(DashCoroutine());                
                
                Debug.LogWarning("Dash");
                
            }
        }
    }

    private IEnumerator DashCoroutine()
    {
        float originalVelocidad = velocidadMov;
        velocidadMov = dashSpeed;
        isDashing = true;

        yield return new WaitForSeconds((float)0.5);  

        velocidadMov = originalVelocidad;  
        isDashing = false;

        Debug.LogWarning("Dash terminado");
    }


    public void Attack(InputAction.CallbackContext context)
    {   
        if (context.performed) {
            if (equippedWeapon == null) 
            {
                Debug.LogWarning("¡No tienes un arma equipada!");
                return;
            }

            equippedWeapon.Atacar();
            
            Debug.Log("Atacando con " + equippedWeapon.weaponName);
        }
    }

    public void MagicAttack(InputAction.CallbackContext context)
    {
        if (context.performed) {
            if (equippedWeapon == null) 
            {
                Debug.LogWarning("¡No tienes un arma equipada!");
                return;
            }

            equippedWeapon.Atacar();
            
            Debug.Log("Ataque mágico con " + equippedWeapon.weaponName);
        }
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed) {
            if (equippedWeapon == null) 
            {
                Debug.LogWarning("¡No tienes un arma equipada!");
                return;
            }
            equippedWeapon=null;
        }
    }
    public void TakeDamage(int amount)
    {
        if (curInmuneTime > 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.setHealth(currentHealth);

        curInmuneTime = immuneTime;

    }
}