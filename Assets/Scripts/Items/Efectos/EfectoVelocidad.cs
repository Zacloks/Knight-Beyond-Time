using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Velocidad")]
public class EfectoVelocidad : EfectoBase
{
    public float aumento = 1.5f;
    public float duracion = 5f;

    public override void AplicarEfecto(PlayerScript jugador)
    {
        jugador.ActivarVelocidad(aumento, duracion);
        Debug.Log("AAAA");
    }
}