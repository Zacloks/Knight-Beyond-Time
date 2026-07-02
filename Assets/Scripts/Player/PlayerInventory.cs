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

    [Header("Drop")]
    public float dropDistance = 0.3f;

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
        ItemData data = inventario.getEquippedItem();
        if (data == null) return;

        SpawnItemEnSuelo(data);

        inventario.dropItem();
        if (useSPUM && spumEquipment != null)
            spumEquipment.UnequipItem();

        // destruye la instancia equipada y equipa lo que quede
        UpdateEquippedItem(inventario.getEquippedItem());
    }

    public void ConsumeEquippedItem()
    {
        if (inventario.getEquippedItem() == null) return;

        inventario.dropItem();
        if (useSPUM && spumEquipment != null)
            spumEquipment.UnequipItem();

        UpdateEquippedItem(inventario.getEquippedItem());
    }

    // Suelta el item 
    private void SpawnItemEnSuelo(ItemData data)
    {
        if (data.prefab == null) return;

        Vector2 dir = Vector2.right;
        PlayerMovement mov = GetComponent<PlayerMovement>();
        if (mov != null) dir = mov.LastDirection;

        Vector3 pos = transform.position + (Vector3)(dir.normalized * dropDistance);

        Item soltado = Instantiate(data.prefab, pos, Quaternion.identity);
        soltado.datos = data;
        soltado.enVenta = false;
        soltado.gameObject.tag = "Consumible";

        SpriteRenderer sr = soltado.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
           
            sr.sortingOrder = OrdenDebajoDelJugador();
        }

        Collider2D col = soltado.GetComponent<Collider2D>();
        if (col != null) { col.enabled = true; col.isTrigger = true; }

        // Sombra redonda + flotar para integrarlo con el mapa
        soltado.gameObject.AddComponent<DroppedItem>();
    }

    private int OrdenDebajoDelJugador()
    {
        int min = int.MaxValue;
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            if (r.sortingOrder < min) min = r.sortingOrder;

        if (min == int.MaxValue) min = 0;
        return min - 1;
    }

    public void RomperItemEquipado(Item item)
    {
        if (item != itemEnMano) return;

        Debug.Log($"El arma {(item.datos != null ? item.datos.itemName : "?")} se rompió y desaparece.");
        inventario.dropItem();                          
        UpdateEquippedItem(inventario.getEquippedItem()); 
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
            {
                spumEquipment.ResetRightItemRotation();
                spumEquipment.UnequipItem();
            }
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

            if (useSPUM && spumEquipment != null)
            {
                if (itemEnMano is WeaponDistance armaDist)
                {
                    SpriteRenderer spumRenderer = spumEquipment.GetRightItemRenderer();
                    if (spumRenderer != null) armaDist.weaponRenderer = spumRenderer;
                    spumEquipment.SetRightItemRotation(armaDist.anguloVisual);
                }
                else
                {
                    spumEquipment.ResetRightItemRotation();
                }
            }
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