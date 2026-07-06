using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public ItemData datos;
    public bool enVenta = false;
    private SpriteRenderer spriteRenderer;

    [Header("Globo de info")]
    public GameObject globoInfo;
    public TMP_Text textoInfo;
    public SpriteRenderer fondoGlobo;

    [Header("Tamaño del globo")]
    [Tooltip("Factor de escala del cartel para items soltados (no de tienda). <1 = más chico.")]
    public float escalaGlobo = 0.65f;
    [Tooltip("Factor de escala del cartel de TIENDA. >1 = más grande para leerse mejor.")]
    public float escalaGloboTienda = 1.3f;
    public float anchoGlobo = 2.2f;
    public float altoMinimo = 0.4f;
    public float altoMaximo = 2.5f;
    public Vector2 fondoPadding = new Vector2(0.1f, 0.15f);
    public float margenInferior = 0.1f;

    private Vector3 escalaBaseGlobo = Vector3.one;
    private static Sprite _panelSprite;

    void Awake()
    {
        if (globoInfo == null) CrearGloboRuntime();

        if (globoInfo != null)
        {
            escalaBaseGlobo = globoInfo.transform.localScale; // escala original
            globoInfo.SetActive(false);
        }
        if (textoInfo != null) textoInfo.text = "";
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (datos != null)
        {
            spriteRenderer.sprite = datos.sprite;
            gameObject.name = "Tienda_" + datos.itemName;
        }
    }

    public void MostrarInfo()
    {
        if (globoInfo == null || datos == null) return;

        globoInfo.SetActive(true);

        globoInfo.transform.localScale = escalaBaseGlobo * (enVenta ? escalaGloboTienda : escalaGlobo);

        if (textoInfo != null)
        {
            if (enVenta)
            {
                string desc = string.IsNullOrEmpty(datos.descripcion)
                    ? ""
                    : $"\n<size=60%>{datos.descripcion}</size>";
                string precio = $"\n<size=70%>Precio: {datos.precio} Coins</size>";
                textoInfo.text = $"<b>{datos.itemName}</b>{desc}{precio}";
            }
            else
            {
                textoInfo.text = $"<b>{datos.itemName}</b>";
            }
            textoInfo.ForceMeshUpdate();
        }

        AjustarFondoAlTexto();
    }

    public void OcultarInfo()
    {
        if (globoInfo != null) globoInfo.SetActive(false);
    }

    private void AjustarFondoAlTexto()
    {
        if (fondoGlobo == null || textoInfo == null) return;
        if (fondoGlobo.drawMode == SpriteDrawMode.Simple) return; 

        textoInfo.wordWrappingRatios = 1f;

        float anchoTexto = Mathf.Max(0.1f, anchoGlobo - fondoPadding.x);
        RectTransform rtTexto = textoInfo.rectTransform;
        if (rtTexto != null)
        {
            Vector2 sd = rtTexto.sizeDelta;
            sd.x = anchoTexto;
            sd.y = altoMaximo;
            rtTexto.sizeDelta = sd;
        }
        textoInfo.ForceMeshUpdate();

        float alto = Mathf.Clamp(textoInfo.preferredHeight + fondoPadding.y + margenInferior,
                                 altoMinimo, altoMaximo);
        fondoGlobo.size = new Vector2(anchoGlobo, alto);

        Vector3 nubePos = fondoGlobo.transform.localPosition;
        nubePos.y = -margenInferior / 2f;
        fondoGlobo.transform.localPosition = nubePos;
    }

    private void CrearGloboRuntime()
    {
        GameObject globo = new GameObject("GloboInfoRuntime");
        globo.transform.SetParent(transform, false);
        globo.transform.localPosition = new Vector3(0f, 0.7f, 0f);

        const string capa = "Player and Enemy";

        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(globo.transform, false);
        SpriteRenderer fsr = fondo.AddComponent<SpriteRenderer>();
        fsr.sprite = PanelSprite();
        fsr.drawMode = SpriteDrawMode.Sliced;         
        fsr.size = new Vector2(anchoGlobo, altoMinimo);
        fsr.color = new Color(0f, 0f, 0f, 0.72f);
        fsr.sortingLayerName = capa;
        fsr.sortingOrder = 1000;

        // Texto
        GameObject texto = new GameObject("Texto");
        texto.transform.SetParent(globo.transform, false);
        texto.transform.localPosition = new Vector3(0f, 0f, -0.01f);
        TextMeshPro tmp = texto.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.enableAutoSizing = true;                  
        tmp.fontSizeMin = 1f;
        tmp.fontSizeMax = 6f;
        tmp.color = Color.white;
        tmp.rectTransform.sizeDelta = new Vector2(anchoGlobo, altoMaximo);
        MeshRenderer tmr = texto.GetComponent<MeshRenderer>();
        if (tmr != null)
        {
            tmr.sortingLayerName = capa;
            tmr.sortingOrder = 1001;                   
        }

        globoInfo = globo;
        textoInfo = tmp;
        fondoGlobo = fsr;
    }

    private static Sprite PanelSprite()
    {
        if (_panelSprite != null) return _panelSprite;

        int size = 16;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] px = new Color[size * size];
        for (int i = 0; i < px.Length; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();

        _panelSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f),
                                     16f, 0, SpriteMeshType.FullRect, new Vector4(4, 4, 4, 4));
        _panelSprite.name = "PanelGlobo";
        return _panelSprite;
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
