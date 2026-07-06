
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoItemData", menuName = "Tienda/Item Data")]
public class ItemData: ScriptableObject
{
    [Header("Atributos")]
        public string itemName;
        [TextArea] public string descripcion;
        public int precio;
        public Sprite sprite;
        public Item prefab;
        public EfectoBase estrategiaEfecto;

    public bool EsValido()
    {
        return this != null && !string.IsNullOrWhiteSpace(itemName) && sprite != null;
    }

    public virtual void Usar(PlayerScript player)
    {
        player.IniciarAnimacion("2_Attack");
    }
}