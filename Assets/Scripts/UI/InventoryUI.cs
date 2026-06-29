using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Componentes de la UI")]
    public Image[] iconosSlots;
    public Image[] fondosSlots;
    public Image hotbar;
    public Color selectedColor;
    public Sprite slotVacioSprite;
    public Sprite slotUsadoSprite; // Una imagen transparente o el fondo vacío

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
    }
}
