using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Reduccion")]
public class EfectoReduccion : EfectoBase
{
    public float escala = 0.6f;
    public float multiplicadorDaño = 0.6f;
    public float multiplicadorAlcance = 0.6f;
    public float duracion = 8f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarTamaño(escala, multiplicadorDaño, multiplicadorAlcance, duracion);
        Debug.Log("REDUCCION ACTIVADA!");
    }
}
