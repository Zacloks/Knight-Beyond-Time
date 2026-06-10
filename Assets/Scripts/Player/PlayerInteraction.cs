using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInteraction : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference pick;   


    private Item alcanzable;

    //Referencias
    private PlayerStats stats;
    private PlayerInventory inventory;

    void Awake()
    {
        stats     = GetComponent<PlayerStats>();
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (pick != null && pick.action.triggered && alcanzable != null)
            TryPickUp();
    }

//-------------PICK UP-----------
    private void TryPickUp()
    {
        if (inventory.Full()) {
            Debug.Log("Inventario Lleno");
            return;
        }

        Item itemARecoger;
        if (alcanzable.enVenta) itemARecoger = alcanzable.Comprar(stats);
        else itemARecoger = alcanzable;

        if (itemARecoger == null)
        {
            Debug.Log("No se pudo obtener el ítem (compra fallida o nulo).");
            return;
        }

        bool recogido = inventory.TryPickUp(itemARecoger);
        if (recogido)
        {
            Destroy(itemARecoger.gameObject);
            Debug.Log("Item recogido: {itemARecoger.datos.name}");
        }
    }

//-------------TRIGGERS-----------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("En alcance");
        if (collision.gameObject.CompareTag("Consumible"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item != null)
            {
                alcanzable = item;
            }
        }

        if (collision.gameObject.CompareTag("Moneda"))
        {
            Moneda coin = collision.gameObject.GetComponent<Moneda>();
            if (coin != null)
            {
                stats.AddCoins(coin.value);
                Destroy(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        alcanzable = null;
        Debug.Log("Fuera de alcance");
    }
}