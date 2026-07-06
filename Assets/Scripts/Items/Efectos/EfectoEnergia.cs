using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Energia")]
public class EfectoEnergia : EfectoBase
{
    public float duracion = 5f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarDashInfinito(duracion);
        Debug.Log("DASH INFINITO !!!");
    }
}