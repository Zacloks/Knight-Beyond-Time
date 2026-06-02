using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Consumible : Item
{
    [Header("Configuración de Consumible")]
    public string triggerAnimacion = "2_attack";
    
    [Header("Tiempos de la Pausa (Segundos)")]
    public float tiempoHastaElPico = 0.23f; 
    
    public float tiempoPausaBebida = 0.4f;

    void Start()
    {
        SpriteRenderer miDibujo = GetComponent<SpriteRenderer>();

        if (miDibujo != null && datos != null)
        {
            miDibujo.sprite = datos.sprite;
        }
    }
    public override void Usar(PlayerScript jugador)
    {
        StartCoroutine(RutinaConsumirConPausa(jugador));
    }

    private IEnumerator RutinaConsumirConPausa(PlayerScript jugador)
    {
        Animator anim = jugador.anim; 
        jugador.isDrinking = true;

        jugador.IniciarAnimacion(triggerAnimacion);
        
        SpriteRenderer spriteEnMano = null;
        int ordenOriginal = 0;
        
        SpriteRenderer[] todasLasPartes = jugador.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer parte in todasLasPartes)
        {
            if(parte.sprite == datos.sprite)
            {
                spriteEnMano = parte;
                ordenOriginal = parte.sortingOrder; 
                parte.sortingOrder = 100; 
                break; 
            }
        }

        yield return new WaitForSeconds(tiempoHastaElPico);

        if (anim != null) anim.speed = 0f;


        if (datos.estrategiaEfecto != null)
        {
            datos.estrategiaEfecto.AplicarEfecto(jugador);
        }

        yield return new WaitForSeconds(tiempoPausaBebida);
    
        if (anim != null) 
        {
            anim.speed = 1f; 
            anim.ResetTrigger(triggerAnimacion); 
            anim.CrossFade("Base Layer.0_Idle", 0.15f, 0); 
        }

        yield return new WaitForSeconds(0.1f);

        if (spriteEnMano != null) 
        {
            spriteEnMano.sortingOrder = ordenOriginal;
        }

        jugador.inventario.dropItem(); 
        jugador.dropItem();
        Destroy(gameObject);
        
        jugador.isDrinking = false;
    }
}