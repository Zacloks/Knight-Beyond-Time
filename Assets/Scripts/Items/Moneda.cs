using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Moneda : Item
{
    public int value = 10;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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