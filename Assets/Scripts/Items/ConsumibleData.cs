using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPocion", menuName = "Tienda/Pocion")]
public class ConsumibleData : ItemData
{
    [Header("Atributos de Consumible")]
    
    public string triggerConsumir = "2_Attack"; 

    public void Usar(PlayerScript jugador, PlayerInventory inventory)
    {
        jugador.IniciarAnimacionMitad(triggerConsumir);
        
        inventory.DropItem();
        
        Debug.Log($"Bebiste {itemName} y recuperaste salud.");
    }
}
