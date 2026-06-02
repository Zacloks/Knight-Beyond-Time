using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Energia")]
public class EfectoEnergia : EfectoBase
{
    public float duracion = 5f;

    public override void AplicarEfecto(PlayerScript jugador)
    {
        jugador.ActivarDashInfinito(duracion);
        Debug.Log("AAAA");
    }
}