using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Moneda : Item
{
    public int value = 10;
    private SpriteRenderer spriteRenderer;

    [Header("Imán (atracción al jugador)")]
    [Tooltip("Distancia a la que la moneda empieza a volar hacia el jugador.")]
    public float radioIman = 2.5f;
    [Tooltip("Velocidad con la que la moneda se acerca al jugador.")]
    public float velocidadIman = 9f;
    private Transform jugador;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (jugador == null)
        {
            PlayerInventory inv = FindObjectOfType<PlayerInventory>();
            if (inv == null) return;
            jugador = inv.transform;
        }

        if (Vector2.Distance(transform.position, jugador.position) <= radioIman)
            transform.position = Vector2.MoveTowards(transform.position, jugador.position,
                                                     velocidadIman * Time.deltaTime);
    }

    public void crearMoneda(int v)
    {
        value = v;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        switch (value)
        {
            case 1:
                spriteRenderer.color = Color.white; // amarilla (deja el sprite base tal cual)
                break;

            case 5:
                spriteRenderer.color = new Color(0.3f, 0.55f, 1f); // azul (2º valor)
                break;

            case 10:
                spriteRenderer.color = new Color(1f, 0.25f, 0.25f); // rojo (mayor valor)
                break;
        }
    }
}