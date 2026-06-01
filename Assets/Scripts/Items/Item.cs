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

    public Item Comprar(PlayerScript player)
    {
        if (player.coins - datos.precio >= 0) {
            player.buy(datos.precio);
            return this;
        }

        return null;
    }
}
