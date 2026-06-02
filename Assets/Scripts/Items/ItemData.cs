
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoItemData", menuName = "Tienda/Item Data")]
public class ItemData: ScriptableObject
{
    [Header("Atributos")]
        public string itemName;
        public int precio;
        public Sprite sprite;
        public Item prefab;
        public EfectoBase estrategiaEfecto;

    public virtual void Usar(PlayerScript player)
    {
        player.IniciarAnimacion("2_Attack");
    }
}