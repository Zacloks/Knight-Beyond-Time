using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPocion", menuName = "Tienda/Pocion")]
public class ConsumibleData : ItemData
{
    [Header("Atributos de Consumible")]
    
    public string triggerConsumir = "2_Attack"; 

    public override void Usar(PlayerScript jugador)
    {
        jugador.IniciarAnimacionMitad(triggerConsumir);
        
        jugador.inventario.dropItem();
        
        Debug.Log($"Bebiste {itemName} y recuperaste salud.");
    }
}
