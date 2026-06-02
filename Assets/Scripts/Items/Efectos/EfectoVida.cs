using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Vida")]
public class EfectoVida : EfectoBase
{
    public int cantidadCuracion = 20;

    public override void AplicarEfecto(PlayerScript jugador)
    {
        jugador.currentHealth += cantidadCuracion;
        if (jugador.currentHealth > jugador.maxHealth) jugador.currentHealth = jugador.maxHealth;
        if (jugador.healthBar != null) jugador.healthBar.setHealth(jugador.currentHealth);
        
        Debug.Log($"Estrategia ejecutada: Curados {cantidadCuracion} puntos.");
    }
}