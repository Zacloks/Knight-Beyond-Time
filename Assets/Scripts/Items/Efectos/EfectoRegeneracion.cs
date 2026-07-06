using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Regeneracion")]
public class EfectoRegeneracion : EfectoBase
{
    public int cantidadPorTick = 10;
    public float intervalo = 1f;
    public float duracion = 5f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarRegeneracion(cantidadPorTick, intervalo, duracion);
        Debug.Log("REGENERACION ACTIVADA!");
    }
}
