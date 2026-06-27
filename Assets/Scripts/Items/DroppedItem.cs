using UnityEngine;

// Hace que CUALQUIER item soltado en el suelo (arma, pocion, etc.) se integre con el mapa:

[RequireComponent(typeof(SpriteRenderer))]
public class DroppedItem : MonoBehaviour
{
    [Header("Flotar")]
    public float bobAmplitude = 0.08f;
    public float bobSpeed = 2.5f;

    [Header("Sombra")]
    [Tooltip("Tamano (ancho) de la sombra en unidades del mundo. Independiente de la escala del item.")]
    public float shadowSize = 0.3f;
    public float shadowAlpha = 0.35f;
    [Tooltip("Ajuste fino de la altura de la sombra respecto a la base del item.")]
    public float shadowYOffset = 0f;

    [Header("Orden")]
    [Tooltip("Cuantos ordenes de sorting por debajo del jugador queda el item.")]
    public int ordenesDebajoDelJugador = 1;

    private SpriteRenderer itemSr;
    private SpriteRenderer shadowSr;
    private Transform shadowT;
    private float baseY;     // altura de reposo del item (sobre el suelo)
    private float shadowY; 
    private float phase;

    private static Sprite _shadowSprite;

    void Start()
    {
        itemSr = GetComponent<SpriteRenderer>();
        baseY = transform.position.y;
        phase = (transform.position.x + transform.position.y) * 3f;

        AjustarOrden();
        CrearSombra();
    }

    void Update()
    {
        phase += Time.deltaTime * bobSpeed;
        float h = (Mathf.Sin(phase) + 1f) * 0.5f * bobAmplitude; // 0..amplitude

        Vector3 p = transform.position;
        p.y = baseY + h;
        transform.position = p;

        if (shadowT == null) return;

        shadowT.position = new Vector3(transform.position.x, shadowY, transform.position.z);

        float t = h / Mathf.Max(bobAmplitude, 0.0001f);          // 0 abajo, 1 arriba
        float ancho = Mathf.Lerp(shadowSize, shadowSize * 0.75f, t);
        SetShadowWorldScale(ancho, ancho * 0.4f);               
        if (shadowSr != null)
            shadowSr.color = new Color(0f, 0f, 0f, Mathf.Lerp(shadowAlpha, shadowAlpha * 0.6f, t));
    }

    private void AjustarOrden()
    {
        if (itemSr == null) return;

        PlayerInventory jugador = FindObjectOfType<PlayerInventory>();
        if (jugador == null) return;

        int min = int.MaxValue;
        foreach (SpriteRenderer r in jugador.GetComponentsInChildren<SpriteRenderer>())
            if (r.sortingOrder < min) min = r.sortingOrder;

        if (min != int.MaxValue)
            itemSr.sortingOrder = min - ordenesDebajoDelJugador;
    }

    private void CrearSombra()
    {
        float baseSprite = (itemSr != null) ? itemSr.bounds.min.y : baseY;
        shadowY = baseSprite + shadowYOffset;

        GameObject s = new GameObject("Sombra");
        shadowT = s.transform;
        shadowT.SetParent(transform);
        shadowT.position = new Vector3(transform.position.x, shadowY, transform.position.z);

        shadowSr = s.AddComponent<SpriteRenderer>();
        shadowSr.sprite = ShadowSprite();
        shadowSr.color = new Color(0f, 0f, 0f, shadowAlpha);

        if (itemSr != null)
        {
            shadowSr.sortingLayerID = itemSr.sortingLayerID;
            shadowSr.sortingOrder = itemSr.sortingOrder - 1; 
        }

        SetShadowWorldScale(shadowSize, shadowSize * 0.4f);
    }

    private void SetShadowWorldScale(float anchoMundo, float altoMundo)
    {
        if (shadowT == null) return;

        Vector3 ls = transform.lossyScale;
        float ix = (Mathf.Abs(ls.x) > 0.0001f) ? 1f / ls.x : 1f;
        float iy = (Mathf.Abs(ls.y) > 0.0001f) ? 1f / ls.y : 1f;
        shadowT.localScale = new Vector3(anchoMundo * ix, altoMundo * iy, 1f);
    }

    private static Sprite ShadowSprite()
    {
        if (_shadowSprite != null) return _shadowSprite;

        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        float r = size * 0.5f;
        Vector2 c = new Vector2(r, r);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), c) / r;
                float a = Mathf.Clamp01(1f - d);
                a *= a; // borde más suave
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();

        _shadowSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
        _shadowSprite.name = "SombraCircular";
        return _shadowSprite;
    }
}
