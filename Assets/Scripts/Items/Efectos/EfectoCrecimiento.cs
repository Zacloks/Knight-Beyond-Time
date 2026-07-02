using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Crecimiento")]
public class EfectoCrecimiento : EfectoBase
{
    public float escala = 1.5f;
    public float multiplicadorDaño = 1.5f;
    public float multiplicadorAlcance = 1.5f;
    public float duracion = 8f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarTamaño(escala, multiplicadorDaño, multiplicadorAlcance, duracion);
        Debug.Log("CRECIMIENTO ACTIVADO!");
    }
}
