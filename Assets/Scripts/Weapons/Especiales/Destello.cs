using UnityEngine;
public class Destello : MonoBehaviour
{
    
    private const string CAPA = "Player and Enemy";
    private const int ORDEN = 30000; 

    private float duracion = 0.4f;
    private float radioFinal = 1f;
    private Color color = Color.white;
    private float t;

    private SpriteRenderer discoSr;
    private SpriteRenderer anilloSr;
    private Transform anilloT;

    private static Sprite _disco;
    private static Sprite _anillo;

    public static void Crear(Vector3 pos, float radio, Color color)
    {
        GameObject go = new GameObject("DestelloSolar");
        go.transform.position = pos;

        Destello d = go.AddComponent<Destello>();
        d.radioFinal = Mathf.Max(radio, 0.5f);
        d.color = (color.a > 0f) ? color : new Color(color.r, color.g, color.b, 1f);
        d.Construir();
    }

    private void Construir()
    {
        discoSr = gameObject.AddComponent<SpriteRenderer>();
        discoSr.sprite = DiscoSprite();
        discoSr.color = color;
        discoSr.sortingLayerName = CAPA;
        discoSr.sortingOrder = ORDEN;

        GameObject anillo = new GameObject("Anillo");
        anilloT = anillo.transform;
        anilloT.position = transform.position;
        anilloSr = anillo.AddComponent<SpriteRenderer>();
        anilloSr.sprite = AnilloSprite();
        anilloSr.color = color;
        anilloSr.sortingLayerName = CAPA;
        anilloSr.sortingOrder = ORDEN;
    }

    void Update()
    {
        t += Time.deltaTime / Mathf.Max(duracion, 0.0001f);
        float k = Mathf.Clamp01(t);

        float discoEscala = radioFinal * Mathf.Lerp(1.7f, 2.1f, k);
        transform.localScale = new Vector3(discoEscala, discoEscala, 1f);
        SetAlpha(discoSr, (1f - k) * color.a);

        if (anilloT != null)
        {
            float anilloEscala = radioFinal * Mathf.Lerp(0.8f, 2.8f, k);
            anilloT.position = transform.position;
            anilloT.localScale = new Vector3(anilloEscala, anilloEscala, 1f);
            SetAlpha(anilloSr, (1f - k) * color.a);
        }

        if (t >= 1f)
        {
            if (anilloT != null) Destroy(anilloT.gameObject);
            Destroy(gameObject);
        }
    }

    private static void SetAlpha(SpriteRenderer sr, float a)
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }

    private static Sprite DiscoSprite()
    {
        if (_disco != null) return _disco;

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
                float a = Mathf.Clamp01(1.3f * (1f - d)); 
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();

        _disco = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
        _disco.name = "DestelloDisco";
        return _disco;
    }

    private static Sprite AnilloSprite()
    {
        if (_anillo != null) return _anillo;

        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        float r = size * 0.5f;
        Vector2 c = new Vector2(r, r);
        const float centroBanda = 0.82f; 
        const float grosorBanda = 0.16f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), c) / r;
                float a = Mathf.Clamp01(1f - Mathf.Abs(d - centroBanda) / grosorBanda);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();

        _anillo = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
        _anillo.name = "DestelloAnillo";
        return _anillo;
    }
}
