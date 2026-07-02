using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryUI : MonoBehaviour
{
    [Header("Componentes de la UI")]
    public Image[] iconosSlots;
    public Image[] fondosSlots;
    public Image hotbar;
    public Color selectedColor;
    public Sprite slotVacioSprite;
    public Sprite slotUsadoSprite; // Una imagen transparente o el fondo vacío
    [Header("Info del arma seleccionada")]
    public TextMeshProUGUI textoNombreArma;
    public TextMeshProUGUI textoTipoArma;
    void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += ActualizarInterfaz;
        ActualizarInterfaz();
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= ActualizarInterfaz;
    }

    public void ActualizarInterfaz()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        int slotActivo = gm.selectedSlot;

        for (int i = 0; i < iconosSlots.Length; i++)
        {
            // 1. Icono del ítem
            iconosSlots[i].preserveAspect = true;

            if (i < gm.inventorySlots.Length && gm.inventorySlots[i] != null)
            {
                iconosSlots[i].sprite = gm.inventorySlots[i].sprite;
                iconosSlots[i].enabled = true;
            }
            else
            {
                iconosSlots[i].sprite = slotVacioSprite;
                iconosSlots[i].enabled = true;
            }

            if (i < fondosSlots.Length && fondosSlots[i] != null)
                fondosSlots[i].enabled = (i == slotActivo);
        }
        ActualizarInfoArma(gm, slotActivo);
    }
    private void ActualizarInfoArma(GameManager gm, int slotActivo)
    {
        // Limpiar si no hay item
        if (slotActivo >= gm.inventorySlots.Length || gm.inventorySlots[slotActivo] == null)
        {
            if (textoNombreArma != null) textoNombreArma.text = "";
            if (textoTipoArma != null)   textoTipoArma.text = "";
            return;
        }

        ItemData item = gm.inventorySlots[slotActivo];

        string nombre = item.itemName;
        string tipo   = "Ítem";

        if (item.prefab != null)
        {
            Weapon arma = item.prefab.GetComponent<Weapon>();
            if (arma != null)
            {
                if (!string.IsNullOrEmpty(arma.weaponName))
                    nombre = arma.weaponName;

                if      (arma is WeaponMelee)    tipo = "Melee";
                else if (arma is WeaponMagic)     tipo = "Mágica";
                else if (arma is WeaponDistance)  tipo = "Largo alcance";
            }
        }
        if (textoNombreArma != null) textoNombreArma.text = nombre;
        if (textoTipoArma != null)   textoTipoArma.text   = tipo;
    }
}
