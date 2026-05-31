using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias del Inventario")]
    public Inventory inventario; 
    [Header("Componentes de la UI")]
    public Image[] iconosSlots; 
    public Image[] fondosSlots;
    public Color selectedColor;
    public Sprite slotVacioSprite;
    public Sprite slotUsadoSprite; // Una imagen transparente o el fondo vacío
    
    void Start()
    {
        // Al empezar, dibujamos el estado inicial del inventario
        ActualizarInterfaz();
    }
     public void ActualizarInterfaz()
 {
     int slotActivo = inventario.GetIndexSeleccionado();
    
     for (int i = 0; i < iconosSlots.Length; i++)
     {
         // 1. Sincronizar el icono del ítem (tu lógica que ya funciona bien)
         if (i < inventario.slots.Length && inventario.slots[i] != null)
         {
             iconosSlots[i].sprite = inventario.slots[i].sprite;
             iconosSlots[i].enabled = true; 
         }
         else
         {
             iconosSlots[i].sprite = slotVacioSprite;
        }

       // 2. CORRECCIÓN: Manejo de los Marcos/Fondos
       if (i == slotActivo)
        {
             fondosSlots[i].enabled = true;
         }
        else
        { 
           fondosSlots[i].enabled = false;
        }
    }
}

}