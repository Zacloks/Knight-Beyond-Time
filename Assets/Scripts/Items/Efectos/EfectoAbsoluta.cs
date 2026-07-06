using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Absoluta")]
public class EfectoAbsoluta : EfectoBase
{
    public float multiplicadorFuerza = 2f;
    public float multiplicadorVelocidad = 1.8f;
    [Range(0f, 1f)] public float reduccionResistencia = 0.5f;
    public float multiplicadorAtaqueRapido = 2f;
    public float duracion = 6f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarFuerza(multiplicadorFuerza, duracion);
        stats.ActivarVelocidad(multiplicadorVelocidad, duracion);
        stats.ActivarResistencia(reduccionResistencia, duracion);
        stats.ActivarAtaqueRapido(multiplicadorAtaqueRapido, duracion);
        Debug.Log("POCION ABSOLUTA ACTIVADA!");
    }
}
