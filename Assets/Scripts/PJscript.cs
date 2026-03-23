using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    public float velocidadMov = 10;
    private Vector2 direccionMov;

    public InputActionReference mover;
    public Rigidbody2D entidad;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entidad = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        direccionMov = mover.action.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        entidad.linearVelocity = new Vector2(direccionMov.x * velocidadMov,direccionMov.y * velocidadMov);
    }
}
