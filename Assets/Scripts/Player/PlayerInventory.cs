using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInventory : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference dropearItem;
    public InputActionReference cambiarItemLeft;
    public InputActionReference cambiarItemRight;

    [Header("Inventario")]
    public Inventory inventario;

    [Header("SPUM Integration")]
    public bool useSPUM = true;
    public SPUMEquipmentManager spumEquipment;

    private Item itemEnMano;

    void Update()
    {
        if (dropearItem != null && dropearItem.action.triggered) DropItem();
        if (cambiarItemLeft != null && cambiarItemLeft.action.triggered) SwapLeft();
        if (cambiarItemRight != null && cambiarItemRight.action.triggered) SwapRight();
    }

    void Start()
    {
        UpdateEquippedItem(inventario.getEquippedItem());
    }

    public ItemData GetEquippedItemData() {return inventario.getEquippedItem();}     
    public Item GetEquippedItemInstance() {return itemEnMano;}

//-------ITEM SWAP Y EQUIP--------

    private void SwapLeft() {UpdateEquippedItem(inventario.swapLeft());}
    private void SwapRight() {UpdateEquippedItem(inventario.swapRight());}

    public void DropItem()
    {
        inventario.dropItem();
        if (useSPUM && spumEquipment != null)
            spumEquipment.UnequipItem();
    }

    private void UpdateEquippedItem(ItemData itemData)
    {
        if (itemEnMano != null)
        {
            Destroy(itemEnMano.gameObject);//destruye el objeto anteriormente puesto
            itemEnMano = null;
        }

        if (useSPUM && spumEquipment != null)
        {
            if (itemData == null)
                spumEquipment.UnequipItem();
            else
                spumEquipment.EquipItem(itemData.sprite);
        }

        if (itemData != null && itemData.prefab != null)
        {
            itemEnMano = Instantiate(itemData.prefab, transform);
            itemEnMano.datos = itemData;

            SpriteRenderer sr = itemEnMano.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            Collider2D col = itemEnMano.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }

//--------PICK ITEMS--------
    public bool TryPickUp(Item item)
    {
        if (inventario.full())
        {
            Debug.Log("Inventario lleno.");
            return false;
        }

        inventario.add(item.datos);
        UpdateEquippedItem(inventario.getEquippedItem());
        return true;
    }

    public bool Full()
    {
        return inventario.full();
    }
}