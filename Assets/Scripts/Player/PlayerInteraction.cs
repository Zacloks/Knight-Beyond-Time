using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInteraction : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference pick;   


    private Item alcanzable;
    private Cofre cofreCercano;

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
        if (pick == null || !pick.action.triggered) return;

        if (alcanzable != null) TryPickUp();
        else if (cofreCercano != null) cofreCercano.Abrir();
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
                alcanzable.MostrarInfo();
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

        if (collision.gameObject.CompareTag("Cofre"))
        {
            Cofre cofre = collision.gameObject.GetComponent<Cofre>();
            if (cofre != null && !cofre.YaAbierto)
                cofreCercano = cofre;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Consumible"))
        {
            if (alcanzable != null) alcanzable.OcultarInfo();
            alcanzable = null;
        }

        if (collision.gameObject.CompareTag("Cofre"))
        {
            cofreCercano = null;
        }

        Debug.Log("Fuera de alcance");
    }
}