using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemData[] slots => GameManager.Instance.inventorySlots;
    private int indexInventario
    {
        get => GameManager.Instance.selectedSlot;
        set => GameManager.Instance.selectedSlot = value;
    }

    private int LastIndex => slots.Length - 1;

    public ItemData swapRight()
    {
        indexInventario = (indexInventario < LastIndex) ? indexInventario + 1 : 0;
        GameManager.Instance.NotifyChanged();
        return slots[indexInventario];
    }
    public ItemData swapLeft()
    {
        indexInventario = (indexInventario > 0) ? indexInventario - 1 : LastIndex;
        GameManager.Instance.NotifyChanged();
        return slots[indexInventario];
    }

    public ItemData getEquippedItem()
    {
        return slots[indexInventario];
    }
    public void dropItem()
    {
        slots[indexInventario] = null;

        int[] dur = GameManager.Instance.durabilidadSlots;
        if (dur != null && indexInventario < dur.Length) dur[indexInventario] = -1;

        Debug.Log("Botaste el item!");
        GameManager.Instance.NotifyChanged();
    }

    public int GetIndexSeleccionado()
    {
        return indexInventario;
    }

    public bool full()
    {
        for (int i = 0; i < slots.Length; i++)
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
            GameManager.Instance.NotifyChanged();
            return;
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                GameManager.Instance.NotifyChanged();
                return;
            }
        }
    }
}
