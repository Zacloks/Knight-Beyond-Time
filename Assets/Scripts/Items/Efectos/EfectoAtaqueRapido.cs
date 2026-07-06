using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/AtaqueRapido")]
public class EfectoAtaqueRapido : EfectoBase
{
    public float multiplicador = 2f;
    public float duracion = 8f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarAtaqueRapido(multiplicador, duracion);
        Debug.Log("ATAQUE RAPIDO ACTIVADO!");
    }
}
