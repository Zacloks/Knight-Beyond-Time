using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Vida")]
public class EfectoVida : EfectoBase
{
    public int cantidadCuracion = 200;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.Heal(cantidadCuracion);
        //stats.currentHealth += cantidadCuracion;
        //if (stats.currentHealth > stats.maxHealth) stats.currentHealth = stats.maxHealth;
        //if (stats.healthBar != null) stats.healthBar.setHealth(stats.currentHealth);
        Debug.Log($"Estrategia ejecutada: Curados {cantidadCuracion} puntos.");
    }
}