using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Resistencia")]
public class EfectoResistencia : EfectoBase
{
    [Range(0f, 1f)] public float reduccion = 0.5f;
    public float duracion = 8f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarResistencia(reduccion, duracion);
        Debug.Log("RESISTENCIA ACTIVADA!");
    }
}
