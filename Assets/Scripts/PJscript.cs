using System;
using System.Data;
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

    public int coins = 0;

    //Referencias:
    private Vector2 direccionMov;
    public PlayerInput playerInput;
    public Weapon equippedWeapon; 
    public Rigidbody2D entidad;

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
        direccionMov = playerInput.actions["MOVE"].ReadValue<Vector2>();
        count++;

        if (count % 1000 == 0)
        {
            currentHealth -= 1;
            healthBar.setHealth(currentHealth);
        }

        coin.setCoins(currentHealth);

    }

    void FixedUpdate()
    {
        entidad.linearVelocity = new Vector2(direccionMov.x * velocidadMov,direccionMov.y * velocidadMov);
    }
  
    public void Dash() 
    {
        currentEnergy -= 60;
        energyBar.setEnergy(currentEnergy);
        Debug.LogWarning("Dash");

    }

    public void Attack()
    {
        if (equippedWeapon == null) 
        {
            Debug.LogWarning("¡No tienes un arma equipada!");
            return;
        }

        equippedWeapon.Atacar();
        
        Debug.Log("Atacando con " + equippedWeapon.weaponName);
    }

    public void MagicAttack()
    {
        if (equippedWeapon == null) 
        {
            Debug.LogWarning("¡No tienes un arma equipada!");
            return;
        }

        equippedWeapon.Atacar();
        
        Debug.Log("Ataque mágico con " + equippedWeapon.weaponName);
    }

    public void Drop()
    {
        if (equippedWeapon == null) 
        {
            Debug.LogWarning("¡No tienes un arma equipada!");
            return;
        }
        equippedWeapon=null;
    }
}