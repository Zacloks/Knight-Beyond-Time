using UnityEngine;
using System.Collections.Generic;

public class Cofre : MonoBehaviour
{
    [Header("Visual")]
    public Sprite spriteAbierto;

    [Header("Loot - Monedas")]
    public GameObject monedaPrefab;
    public int monedasMin = 10;
    public int monedasMax = 30;

    [Header("Loot - Item")]
    public Item itemPrefab;
    public bool dropearItem = true;

    [Header("Dispersión")]
    public float radioDispersion = 0.6f;

    [Header("Tamaño del item en el suelo")]
    public float escalaItem = 0.6f;

    private SpriteRenderer sr;
    private bool abierto = false;
    private List<ItemData> catalogo;

    public bool YaAbierto => abierto;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ItemData[] items = Resources.LoadAll<ItemData>("Items");
        catalogo = new List<ItemData>();
        foreach (ItemData item in items)
            if (item != null && item.EsValido())
                catalogo.Add(item);
    }

    public void Abrir()
    {
        if (abierto) return;
        abierto = true;

        if (spriteAbierto != null && sr != null)
            sr.sprite = spriteAbierto;

        SoltarMonedas();
        if (dropearItem) SoltarItem();
    }

    private void SoltarMonedas()
    {
        if (monedaPrefab == null) return;

        int total = Random.Range(monedasMin, monedasMax + 1);
        int restante = total;
        while (restante > 0)
        {
            int val = restante >= 10 ? 10 : restante >= 5 ? 5 : 1;
            SpawnMoneda(val);
            restante -= val;
        }
    }

    private void SpawnMoneda(int valor)
    {
        Vector3 pos = transform.position + (Vector3)(Random.insideUnitCircle * radioDispersion);
        GameObject m = Instantiate(monedaPrefab, pos, Quaternion.identity);
        Moneda comp = m.GetComponent<Moneda>();
        if (comp != null) comp.crearMoneda(valor);
    }

    private void SoltarItem()
    {
        if (itemPrefab == null || catalogo == null || catalogo.Count == 0) return;

        ItemData elegido = catalogo[Random.Range(0, catalogo.Count)];
        Vector3 pos = transform.position + (Vector3)(Random.insideUnitCircle * radioDispersion);

        Item nuevo = Instantiate(itemPrefab, pos, Quaternion.identity);
        nuevo.datos = elegido;
        nuevo.enVenta = false;
        nuevo.gameObject.tag = "Consumible";

        nuevo.transform.localScale *= escalaItem;

        nuevo.gameObject.AddComponent<DroppedItem>();
    }
}
