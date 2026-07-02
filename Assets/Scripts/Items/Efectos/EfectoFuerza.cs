using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Fuerza")]
public class EfectoFuerza : EfectoBase
{
    public float multiplicador = 2f;
    public float duracion = 8f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarFuerza(multiplicador, duracion);
        Debug.Log("FUERZA ACTIVADA!");
    }
}
