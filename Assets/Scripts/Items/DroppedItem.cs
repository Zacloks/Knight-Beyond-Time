using UnityEngine;

// Hace que un item soltado en el suelo se vea integrado con el mapa:
// - le pone una sombra redonda (achatada) debajo
// - lo hace flotar suavemente (bobbing)
// El orden de sorting (que quede por debajo del jugador) lo configura quien lo
// instancia (PlayerInventory) antes de añadir este componente.
[RequireComponent(typeof(SpriteRenderer))]
public class DroppedItem : MonoBehaviour
{
    [Header("Flotar")]
    public float bobAmplitude = 0.08f;
    public float bobSpeed = 2.5f;

    [Header("Sombra")]
    public float shadowSize = 0.45f;
    public float shadowAlpha = 0.35f;
    public float shadowYOffset = -0.05f;

    private SpriteRenderer itemSr;
    private SpriteRenderer shadowSr;
    private Transform shadowT;
    private float baseY;
    private float phase;

    private static Sprite _shadowSprite;

    void Start()
    {
        itemSr = GetComponent<SpriteRenderer>();
        baseY = transform.position.y;
        // Fase inicial variada para que varios items no floten sincronizados.
        phase = (transform.position.x + transform.position.y) * 3f;
        CrearSombra();
    }

    void Update()
    {
        phase += Time.deltaTime * bobSpeed;
        float h = (Mathf.Sin(phase) + 1f) * 0.5f * bobAmplitude; // 0..amplitude

        // El item flota por encima de su posición base (el suelo).
        Vector3 p = transform.position;
        p.y = baseY + h;
        transform.position = p;

        if (shadowT == null) return;

        // La sombra se queda fija en el suelo y se achica/aclara al subir el item.
        shadowT.position = new Vector3(transform.position.x, baseY + shadowYOffset, transform.position.z);

        float t = h / Mathf.Max(bobAmplitude, 0.0001f);          // 0 abajo, 1 arriba
        float escala = Mathf.Lerp(shadowSize, shadowSize * 0.75f, t);
        shadowT.localScale = new Vector3(escala, escala * 0.4f, 1f); // elipse achatada
        if (shadowSr != null)
            shadowSr.color = new Color(0f, 0f, 0f, Mathf.Lerp(shadowAlpha, shadowAlpha * 0.6f, t));
    }

    private void CrearSombra()
    {
        GameObject s = new GameObject("Sombra");
        shadowT = s.transform;
        shadowT.SetParent(transform);
        shadowT.position = new Vector3(transform.position.x, baseY + shadowYOffset, transform.position.z);
        shadowT.localScale = new Vector3(shadowSize, shadowSize * 0.4f, 1f);

        shadowSr = s.AddComponent<SpriteRenderer>();
        shadowSr.sprite = ShadowSprite();
        shadowSr.color = new Color(0f, 0f, 0f, shadowAlpha);

        if (itemSr != null)
        {
            shadowSr.sortingLayerID = itemSr.sortingLayerID;
            shadowSr.sortingOrder = itemSr.sortingOrder - 1; // justo detrás del item
        }
    }

    // Genera (una sola vez) un sprite circular suave para usar como sombra.
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
