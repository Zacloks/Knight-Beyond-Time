using UnityEngine;

// Efecto de QUEMADURA (Libro de Fuego): se pega al enemigo golpeado y le va
// quitando vida por ticks durante un tiempo, emitiendo particulas naranjas.
// Si el enemigo vuelve a ser golpeado, la quemadura se refresca (no se apila).
// Todo el visual se genera por codigo: no necesita prefabs ni sprites.
public class Quemadura : MonoBehaviour
{
    private int dañoPorTick;
    private float intervalo;
    private float finQuemadura;
    private Color color = new Color(1f, 0.55f, 0.1f, 1f); // naranja fuego

    private LifeEnemy vida;
    private float proximoTick;
    private float proximaParticula;

    private static Sprite _particulaSprite;

    // Aplica (o refresca) la quemadura sobre un enemigo.
    public static void Aplicar(Enemy enemigo, int dañoPorTick, float intervalo, float duracion, Color color)
    {
        if (enemigo == null) return;

        Quemadura q = enemigo.GetComponent<Quemadura>();
        if (q == null) q = enemigo.gameObject.AddComponent<Quemadura>();

        q.dañoPorTick = dañoPorTick;
        q.intervalo = Mathf.Max(0.1f, intervalo);
        q.finQuemadura = Time.time + duracion;      // volver a pegar refresca la duracion
        q.color = color;
        q.proximoTick = Time.time + q.intervalo;    // el primer tick espera un intervalo
    }

    void Awake()
    {
        vida = GetComponent<LifeEnemy>();
    }

    void Update()
    {
        // Termina si se acabo el tiempo o el enemigo murio.
        if (Time.time >= finQuemadura || vida == null || vida.IsDead())
        {
            Destroy(this);
            return;
        }

        // Tick de daño (sin knockback: no interrumpe al enemigo cada tick).
        if (Time.time >= proximoTick)
        {
            proximoTick = Time.time + intervalo;
            vida.TakeDamageSinKnockback(dañoPorTick);
        }

        // Particulas naranjas alrededor del enemigo.
        if (Time.time >= proximaParticula)
        {
            proximaParticula = Time.time + 0.08f;
            CrearParticula();
        }
    }

    private void CrearParticula()
    {
        GameObject p = new GameObject("ParticulaFuego");
        Vector2 offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.3f));
        p.transform.position = (Vector2)transform.position + offset;

        SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
        sr.sprite = ParticulaSprite();
        // Varia entre naranja y amarillo para que parezca fuego.
        sr.color = Color.Lerp(color, new Color(1f, 0.9f, 0.3f, 1f), Random.value * 0.6f);
        sr.sortingLayerName = "Player and Enemy";   // por encima del mapa y del enemigo
        sr.sortingOrder = 500;

        ParticulaFuego pf = p.AddComponent<ParticulaFuego>();
        pf.velocidadSubida = Random.Range(0.6f, 1.2f);
        pf.duracion = Random.Range(0.35f, 0.6f);
        pf.escala = Random.Range(0.06f, 0.14f);
    }

    // Sprite circular suave (una sola vez) para las particulas.
    private static Sprite ParticulaSprite()
    {
        if (_particulaSprite != null) return _particulaSprite;

        int size = 16;
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
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
            }
        }
        tex.Apply();

        _particulaSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16f);
        _particulaSprite.name = "ParticulaFuego";
        return _particulaSprite;
    }
}

// Una particula de fuego: sube, se encoge y se desvanece; se destruye sola.
public class ParticulaFuego : MonoBehaviour
{
    public float velocidadSubida = 1f;
    public float duracion = 0.5f;
    public float escala = 0.1f;

    private SpriteRenderer sr;
    private float t;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(escala, escala, 1f);
    }

    void Update()
    {
        t += Time.deltaTime / Mathf.Max(duracion, 0.0001f);
        if (t >= 1f) { Destroy(gameObject); return; }

        transform.position += Vector3.up * velocidadSubida * Time.deltaTime;

        float k = 1f - t;
        transform.localScale = new Vector3(escala * k, escala * k, 1f);
        if (sr != null)
        {
            Color c = sr.color;
            c.a = k;
            sr.color = c;
        }
    }
}
