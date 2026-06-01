using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Moneda : MonoBehaviour
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
                spriteRenderer.color = new Color(0.75f, 0.45f, 0.2f); // bronce
                break;

            case 5:
                spriteRenderer.color = new Color(1f, 0.85f, 0f); // oro
                break;

            case 10:
                spriteRenderer.color = new Color(0.2f, 0.6f, 1f); // azul
                break;
        }
    }
}