using UnityEngine;
public class Inventory : MonoBehaviour
{
    private int indexInventario = 0;
    public InventoryUI inventarioUI;
    public Item[] slots = new Item[5];

    public Item swapRight()
    {
        if (indexInventario < 4)
        {
            indexInventario++;
        } else
        {
            indexInventario = 0;
        }
        Debug.Log(indexInventario);
        inventarioUI.ActualizarInterfaz();
        return slots[indexInventario];
    }
    public Item swapLeft()
    {
        if (indexInventario>0)
        {
            indexInventario--;
        } else
        {
            indexInventario = 4;
        }
        Debug.Log(indexInventario);
        inventarioUI.ActualizarInterfaz();
        return slots[indexInventario];
    }

    public Item getEquippedItem()
    {
        return slots[indexInventario];
    }
    public void dropItem()
    {
        slots[indexInventario] = null;
        Debug.Log("Botaste el item!");
        inventarioUI.ActualizarInterfaz();
    }

    public int GetIndexSeleccionado()
    {
        return indexInventario;
    }
}
