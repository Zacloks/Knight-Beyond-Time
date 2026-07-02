using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/Misteriosa")]
public class EfectoMisteriosa : EfectoBase
{
    public EfectoBase[] posiblesEfectos;

    public override void AplicarEfecto(PlayerStats stats)
    {
        if (posiblesEfectos == null || posiblesEfectos.Length == 0) return;

        EfectoBase elegido = posiblesEfectos[Random.Range(0, posiblesEfectos.Length)];
        if (elegido != null) elegido.AplicarEfecto(stats);

        Debug.Log("POCION MISTERIOSA ACTIVADA!");
    }
}
