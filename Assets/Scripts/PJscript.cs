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

public class PlayerCtrl : MonoBehaviour
{
    //Atributos: 
    public float velocidadMov = 10;
    public int maxHealth = 100;
    public int currentHealth;
    public int maxEnergy = 100;
    public int currentEnergy;

    public float dashSpeed = 15;

    public int coins = 0;

    //Referencias:
    private Vector2 direccionMov;
    private Vector2 lastDirection = Vector2.right;
    public PlayerInput playerInput;
    public Weapon equippedWeapon; 
    public Rigidbody2D entidad;
    public bool isDashing = false;

    public HealthBar healthBar;
    public EnergyBar energyBar;
    public Coin coin;

    int count;
  
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        entidad = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        currentEnergy = maxEnergy;
        energyBar.setMaxEnergy(maxEnergy);
        coin.setCoins(0);
    }

    // Update is called once per frame
    void Update()
    {   
        if (!isDashing) direccionMov = playerInput.actions["MOVE"].ReadValue<Vector2>();
        count++;

        if (count % 100 == 0 && currentEnergy < 100)
        {
            currentEnergy += 1;
            energyBar.setEnergy(currentEnergy);
        }

    }

    void FixedUpdate()
    {

        coin.setCoins(currentHealth);
        
        entidad.linearVelocity = new Vector2(direccionMov.x * velocidadMov,direccionMov.y * velocidadMov);
       
        if (isDashing && direccionMov.x == 0 && direccionMov.y == 0) {

            entidad.linearVelocity = new Vector2(lastDirection.x * velocidadMov, lastDirection.y * velocidadMov);

        } else if (!isDashing && (direccionMov.x != 0 || direccionMov.y != 0)) {

            if (direccionMov.x < 0) lastDirection = Vector2.left;
            else if (direccionMov.x > 0) lastDirection = Vector2.right;
             
        }

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
}