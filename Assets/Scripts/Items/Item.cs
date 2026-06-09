using UnityEngine;
public class Item : MonoBehaviour
{
    public ItemData datos;
    public bool enVenta = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (datos != null)
        {
            spriteRenderer.sprite = datos.sprite;
            gameObject.name = "Tienda_" + datos.itemName; 
        }
    }


    public Item Comprar(PlayerStats stats)
    {
        if (stats.TrySpendCoins(datos.precio)) return this;
        return null;
    }

    public virtual void Usar(PlayerScript jugador)
    {
        Debug.Log("Usando un item genérico.");
    }
}
