using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Suerte")]
public class EfectoSuerte : EfectoBase
{
    public float bonusCritico = 40f;
    public float duracion = 8f;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.ActivarSuerte(bonusCritico, duracion);
        Debug.Log("SUERTE ACTIVADA!");
    }
}
