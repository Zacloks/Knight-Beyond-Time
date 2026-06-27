using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//mejora el aspecto visual de menus y bottones para interactuar 
[RequireComponent(typeof(RectTransform))]
public class HoverBoton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Borde")]
    public Color colorBorde = new Color(1f, 0.85f, 0.3f, 1f); // dorado
    public float grosor = 4f;     // grosor del borde en pixeles
    public float margen = 4f;     // separacion del borde respecto al boton

    [Header("Pop (opcional)")]
    public float escalaHover = 1.05f;
    public float velocidad = 12f;

    private RectTransform contenedorBorde;
    private Vector3 escalaOriginal;
    private Vector3 escalaObjetivo;

    void Awake()
    {
        escalaOriginal = transform.localScale;
        escalaObjetivo = escalaOriginal;

        CrearBorde();
        MostrarBorde(false);
    }

    void OnDisable()
    {
        transform.localScale = escalaOriginal;
        escalaObjetivo = escalaOriginal;
        MostrarBorde(false);
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, escalaObjetivo,
                                            Time.unscaledDeltaTime * velocidad);
    }

    void CrearBorde()
    {
        contenedorBorde = new GameObject("BordeHover", typeof(RectTransform)).GetComponent<RectTransform>();
        contenedorBorde.SetParent(transform, false);
        contenedorBorde.anchorMin = Vector2.zero;
        contenedorBorde.anchorMax = Vector2.one;
        contenedorBorde.offsetMin = new Vector2(-margen, -margen);
        contenedorBorde.offsetMax = new Vector2(margen, margen);

        // 0 arriba, 1 abajo, 2 izquierda, 3 derecha
        CrearLado(new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f), new Vector2(0, grosor)); // arriba
        CrearLado(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0f), new Vector2(0, grosor)); // abajo
        CrearLado(new Vector2(0, 0), new Vector2(0, 1), new Vector2(0f, 0.5f), new Vector2(grosor, 0)); // izquierda
        CrearLado(new Vector2(1, 0), new Vector2(1, 1), new Vector2(1f, 0.5f), new Vector2(grosor, 0)); // derecha
    }

    void CrearLado(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta)
    {
        var img = new GameObject("Lado", typeof(Image)).GetComponent<Image>();
        img.color = colorBorde;
        img.raycastTarget = false;

        var rt = img.rectTransform;
        rt.SetParent(contenedorBorde, false);
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = Vector2.zero;
    }

    void MostrarBorde(bool activo)
    {
        if (contenedorBorde != null) contenedorBorde.gameObject.SetActive(activo);
    }

    void Resaltar(bool activo)
    {
        escalaObjetivo = activo ? escalaOriginal * escalaHover : escalaOriginal;
        MostrarBorde(activo);
    }

    public void OnPointerEnter(PointerEventData eventData) => Resaltar(true);
    public void OnPointerExit(PointerEventData eventData) => Resaltar(false);
    public void OnSelect(BaseEventData eventData) => Resaltar(true);
    public void OnDeselect(BaseEventData eventData) => Resaltar(false);
}
