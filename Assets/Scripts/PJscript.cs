using System;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    //Atributos: 
    public float velocidadMov = 10;

    public int maxHealth = 100;
    public int currentHealth;
    public int maxEnergy = 100;
    public int currentEnergy;

    public int coins = 0;

    //Referencias
    public Vector2 direccionMov;
    public InputActionReference mover;
    public Rigidbody2D entidad;
    public Animator anim;

    public HealthBar healthBar;
    public EnergyBar energyBar;
    public Coin coin;

    int count;
  
    void Start()
    {
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
   void Update()
{
    // 1. LEER EL TECLADO
    direccionMov = mover.action.ReadValue<Vector2>();

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
    if (count % 1000 == 0) {
        currentHealth -= 1;
        if(healthBar != null) healthBar.setHealth(currentHealth);
    }
}
    void FixedUpdate(){
        if (direccionMov.sqrMagnitude < 0.01f) 
        {
        entidad.linearVelocity = Vector2.zero;
        entidad.angularVelocity = 0f; // Evita que rote solo
        }
    else 
    {
        // 2. APLICAR MOVIMIENTO
        entidad.linearVelocity = direccionMov * velocidadMov;
    }

    // 3. LÍMITES DE PANTALLA (Clamping)
    float clampedX = Mathf.Clamp(entidad.position.x, -8.4f, 8.39f);
    float clampedY = Mathf.Clamp(entidad.position.y, -4.94f, 3.49f);
    entidad.position = new Vector2(clampedX, clampedY);
    }
}
