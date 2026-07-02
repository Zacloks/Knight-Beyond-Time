using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerInteraction : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference pick;

    // Todos los items dentro del alcance a la vez (puede haber varios superpuestos).
    private readonly List<Item> itemsEnRango = new List<Item>();
    private Item alcanzable;               // item objetivo actual (el que se recoge)
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
            itemsEnRango.Remove(itemARecoger);
            Destroy(itemARecoger.gameObject);
            ActualizarCartel();           
        }
    }

//-------------TRIGGERS-----------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Consumible"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item != null && !itemsEnRango.Contains(item))
            {
                itemsEnRango.Add(item);
                ActualizarCartel();
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
            Item item = collision.gameObject.GetComponent<Item>();
            if (item != null)
            {
                item.OcultarInfo();        
                itemsEnRango.Remove(item);
                ActualizarCartel();
            }
        }

        if (collision.gameObject.CompareTag("Cofre"))
        {
            cofreCercano = null;
        }
    }

    private void ActualizarCartel()
    {
        itemsEnRango.RemoveAll(i => i == null);

        Item actual = itemsEnRango.Count > 0 ? itemsEnRango[itemsEnRango.Count - 1] : null;

        foreach (Item i in itemsEnRango)
            if (i != actual) i.OcultarInfo();

        if (actual != null) actual.MostrarInfo();

        alcanzable = actual;
    }
}
