using UnityEngine;

public class EfectoInmovilizado : MonoBehaviour
{
    private float fin;
    private Color color = new Color(0.45f, 0.75f, 1f, 1f); // azul electrico
    private float proximaChispa;

    private static Sprite _rayoSprite;

    public static void Aplicar(Enemy enemigo, float duracion)
    {
        if (enemigo == null) return;

        EfectoInmovilizado e = enemigo.GetComponent<EfectoInmovilizado>();
        if (e == null) e = enemigo.gameObject.AddComponent<EfectoInmovilizado>();

        e.fin = Time.time + duracion;   
    }

    void Update()
    {
        LifeEnemy vida = GetComponent<LifeEnemy>();
        if (Time.time >= fin || (vida != null && vida.IsDead()))
        {
            Destroy(this);
            return;
        }

        if (Time.time >= proximaChispa)
        {
            proximaChispa = Time.time + Random.Range(0.05f, 0.1f);
            CrearChispa();
        }
    }

    private void CrearChispa()
    {
        GameObject c = new GameObject("ChispaRayo");
        Vector2 offset = new Vector2(Random.Range(-0.35f, 0.35f), Random.Range(-0.3f, 0.45f));
        c.transform.position = (Vector2)transform.position + offset;
        c.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 180f)); // angulo aleatorio

        SpriteRenderer sr = c.AddComponent<SpriteRenderer>();
        sr.sprite = RayoSprite();
        // Varia entre azul y casi blanco para que parezca electricidad.
        sr.color = Color.Lerp(color, Color.white, Random.value * 0.5f);
        sr.sortingLayerName = "Player and Enemy";   // por encima del mapa y del enemigo
        sr.sortingOrder = 500;

        ChispaRayo ch = c.AddComponent<ChispaRayo>();
        ch.duracion = Random.Range(0.12f, 0.25f);
        ch.largo = Random.Range(0.25f, 0.5f);
    }

    private static Sprite RayoSprite()
    {
        if (_rayoSprite != null) return _rayoSprite;

        int w = 16, h = 3;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                // Mas brillante al centro, se apaga hacia las puntas y los bordes.
                float fx = 1f - Mathf.Abs((x + 0.5f) / w - 0.5f) * 2f;
                float fy = 1f - Mathf.Abs((y + 0.5f) / h - 0.5f) * 2f;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(fx * (0.4f + 0.6f * fy))));
            }
        }
        tex.Apply();

        _rayoSprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
        _rayoSprite.name = "ChispaRayo";
        return _rayoSprite;
    }
}

// Una chispa: parpadea y desaparece rapido; se destruye sola.
public class ChispaRayo : MonoBehaviour
{
    public float duracion = 0.2f;
    public float largo = 0.35f;

    private SpriteRenderer sr;
    private float t;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(largo, 1f, 1f);
    }

    void Update()
    {
        t += Time.deltaTime / Mathf.Max(duracion, 0.0001f);
        if (t >= 1f) { Destroy(gameObject); return; }

        if (sr != null)
        {
            // Parpadeo electrico: alterna intensidad mientras se desvanece.
            Color c = sr.color;
            c.a = (1f - t) * (Random.value > 0.3f ? 1f : 0.3f);
            sr.color = c;
        }
    }
}
