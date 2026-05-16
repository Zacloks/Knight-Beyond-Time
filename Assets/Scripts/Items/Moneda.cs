using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Moneda : MonoBehaviour
{
    public int value = 10;
    private SpriteRenderer spriteRenderer;
    
    
    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = new Color(0.2f, 0.6f, 1f); //Para probar
    }

    public void crearMoneda(int v)
    {
        value = v;

        switch (value)
        {
            case 5:
                spriteRenderer.color = new Color(1f, 0.85f, 0f);
                break;

            case 10:
                spriteRenderer.color = new Color(0.2f, 0.6f, 1f); 
                break;
        }
    }
}