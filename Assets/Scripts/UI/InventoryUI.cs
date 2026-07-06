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

    [Header("Barra de durabilidad")]
    public Color colorAlto  = new Color(0.30f, 0.85f, 0.30f); // verde
    public Color colorMedio = new Color(0.95f, 0.85f, 0.20f); // amarillo
    public Color colorBajo  = new Color(0.90f, 0.25f, 0.25f); // rojo
    [Range(0f, 1f)] public float umbralMedio = 0.5f;          // <= 50% amarillo
    [Range(0f, 1f)] public float umbralBajo  = 0.25f;         // < 25% rojo

    private Image[] barrasBg;
    private Image[] barrasFill;
    private PlayerInventory playerInv;

    void Awake()
    {
        CrearBarras();
    }

    void Update()
    {
        ActualizarDurabilidad();
    }

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

//----------BARRA DE DURABILIDAD----------
    // Crea una barrita (fondo + relleno) debajo de cada icono, una sola vez.
    private void CrearBarras()
    {
        if (iconosSlots == null) return;

        barrasBg   = new Image[iconosSlots.Length];
        barrasFill = new Image[iconosSlots.Length];

        for (int i = 0; i < iconosSlots.Length; i++)
        {
            if (iconosSlots[i] == null) continue;

            // Fondo oscuro, fino y en la parte baja del slot (no tapa el arma).
            GameObject bg = new GameObject("BarraDurabilidad", typeof(RectTransform), typeof(Image));
            RectTransform bgRt = bg.GetComponent<RectTransform>();
            bgRt.SetParent(iconosSlots[i].rectTransform, false);
            bgRt.anchorMin = new Vector2(0.12f, 0.04f);
            bgRt.anchorMax = new Vector2(0.88f, 0.16f);
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;
            Image bgImg = bg.GetComponent<Image>();
            bgImg.sprite = SpriteBlanco();          // sin sprite, un Image de UI no dibuja
            bgImg.color = new Color(0f, 0f, 0f, 0.6f);
            bgImg.raycastTarget = false;

            // Relleno (ancho = fracción de durabilidad, controlado por el anchor).
            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            RectTransform fillRt = fill.GetComponent<RectTransform>();
            fillRt.SetParent(bgRt, false);
            fillRt.anchorMin = new Vector2(0f, 0f);
            fillRt.anchorMax = new Vector2(1f, 1f);
            fillRt.offsetMin = Vector2.zero;   // sin margen: el HUD trabaja en unidades chicas
            fillRt.offsetMax = Vector2.zero;
            Image fillImg = fill.GetComponent<Image>();
            fillImg.sprite = SpriteBlanco();
            fillImg.color = colorAlto;
            fillImg.raycastTarget = false;

            barrasBg[i]   = bgImg;
            barrasFill[i] = fillImg;
        }
    }

    // Muestra/oculta y colorea la barra de cada slot según la durabilidad.
    private void ActualizarDurabilidad()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || barrasFill == null) return;
        if (playerInv == null) playerInv = FindObjectOfType<PlayerInventory>();

        int activo = gm.selectedSlot;

        for (int i = 0; i < barrasFill.Length; i++)
        {
            if (barrasFill[i] == null || barrasBg[i] == null) continue;

            Weapon armaPrefab = ObtenerArmaPrefab(gm, i);
            if (armaPrefab == null)
            {
                // Slot vacío o no es arma: sin barra.
                barrasBg[i].enabled = false;
                barrasFill[i].enabled = false;
                continue;
            }

            barrasBg[i].enabled = true;
            barrasFill[i].enabled = true;

            float max = Mathf.Max(1, armaPrefab.maxDurabilidad);
            float actual;

            if (i == activo && playerInv != null &&
                playerInv.GetEquippedItemInstance() is Weapon armaViva)
            {
                actual = armaViva.durabilidadActual;   // arma equipada: durabilidad viva
            }
            else
            {
                // Otros slots: durabilidad persistida del slot (-1 = fresca => máxima).
                int guardada = (gm.durabilidadSlots != null && i < gm.durabilidadSlots.Length)
                               ? gm.durabilidadSlots[i] : -1;
                actual = guardada >= 0 ? guardada : max;
            }

            float frac = Mathf.Clamp01(actual / max);

            // El ancho del relleno = fracción de durabilidad.
            RectTransform fr = barrasFill[i].rectTransform;
            fr.anchorMax = new Vector2(frac, 1f);

            barrasFill[i].color = frac < umbralBajo  ? colorBajo
                                : frac <= umbralMedio ? colorMedio
                                : colorAlto;
        }
    }

    // Devuelve el componente Weapon del prefab del slot, o null si no es un arma.
    private Weapon ObtenerArmaPrefab(GameManager gm, int i)
    {
        if (i >= gm.inventorySlots.Length) return null;
        ItemData data = gm.inventorySlots[i];
        if (data == null || data.prefab == null) return null;
        return data.prefab.GetComponent<Weapon>();
    }

    // Un Image de UI sin sprite no dibuja nada; usamos este sprite blanco para las barras.
    private static Sprite _blanco;
    private static Sprite SpriteBlanco()
    {
        if (_blanco != null) return _blanco;

        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        Color[] px = new Color[16];
        for (int i = 0; i < px.Length; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();

        _blanco = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));
        _blanco.name = "BlancoUI";
        return _blanco;
    }
}
