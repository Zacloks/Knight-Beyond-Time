using System.Threading;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    private int indexInventario = 0;
    public InventoryUI inventarioUI;
    public ItemData[] slots = new ItemData[5];

    public ItemData swapRight()
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
    public ItemData swapLeft()
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

    public ItemData getEquippedItem()
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

    public bool full()
    {
        for (int i = 0; i < 5; i++)
        {
            if (slots[i] == null) return false;
        }
        return true;
    }

    public void add(ItemData item)
    {
        if (slots[indexInventario] == null)
        {
            slots[indexInventario] = item;
            inventarioUI.ActualizarInterfaz();
            return;
        }
        for (int i = 0; i < 5; i++)
        {
            if (slots[i] == null) {
                slots[i] = item;
                inventarioUI.ActualizarInterfaz();
                return;
            }
        }
    } 
}
