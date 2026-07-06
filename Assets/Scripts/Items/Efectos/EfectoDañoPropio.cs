using UnityEngine;

[CreateAssetMenu(menuName = "Efectos/DañoPropio")]
public class EfectoDañoPropio : EfectoBase
{
    public int cantidad = 15;

    public override void AplicarEfecto(PlayerStats stats)
    {
        stats.TakeDamage(cantidad);
        Debug.Log("DAÑO PROPIO!");
    }
}
