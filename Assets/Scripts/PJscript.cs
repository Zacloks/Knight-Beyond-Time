using System;
using System.Data;
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

    //Referencias:
    private Vector2 direccionMov;
    public InputActionReference mover;
    public Rigidbody2D entidad;

    public HealthBar healthBar;
    public EnergyBar energyBar;

    int count = 0;
  
    void Start()
    {
        entidad = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        currentEnergy = maxEnergy;
        energyBar.setMaxEnergy(maxEnergy);
    }

    // Update is called once per frame
    void Update()
    {
        direccionMov = mover.action.ReadValue<Vector2>();

        count++;

        if (count % 1000 == 0)
        {
            currentHealth -= 1;
            healthBar.setHealth(currentHealth);
        }
    }

    void FixedUpdate()
    {
        entidad.linearVelocity = new Vector2(direccionMov.x * velocidadMov,direccionMov.y * velocidadMov);

    }
}
