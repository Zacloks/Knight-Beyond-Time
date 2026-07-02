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
    public float anchoGlobo = 2.2f;
    public float altoMinimo = 0.4f;
    public float altoMaximo = 2.5f;
    public Vector2 fondoPadding = new Vector2(0.1f, 0.15f);
    public float margenInferior = 0.1f;

    void Awake()
    {
        if (globoInfo != null) globoInfo.SetActive(false);
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

        if (textoInfo != null)
        {
            string precio = enVenta ? $"\n<size=70%>Precio: {datos.precio} Coins</size>" : "";
            string desc = string.IsNullOrEmpty(datos.descripcion)
                ? ""
                : $"\n<size=60%>{datos.descripcion}</size>";
            textoInfo.text = $"<b>{datos.itemName}</b>{desc}{precio}";
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
