using UnityEngine;
using System.Collections.Generic;

public class CofreTestPociones : MonoBehaviour
{
    [Header("Item")]
    public Item itemPrefab;
    public float escalaItem = 0.6f;

    [Header("Disposición")]
    public float separacion = 0.8f;
    public int columnas = 4;

    [Header("Cuándo soltar")]
    public bool soltarAlIniciar = true;

    void Start()
    {
        if (itemPrefab == null)
        {
            Cofre cofre = GetComponent<Cofre>();
            if (cofre != null) itemPrefab = cofre.itemPrefab;
        }

        if (soltarAlIniciar) SoltarTodas();
    }

    public void SoltarTodas()
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning($"{name}: no hay itemPrefab asignado para soltar las pociones.");
            return;
        }

        ItemData[] catalogo = Resources.LoadAll<ItemData>("Items");

        for (int i = 0; i < catalogo.Length; i++)
        {
            int fila = i / columnas;
            int columna = i % columnas;
            Vector3 offset = new Vector3(columna * separacion, fila * separacion, 0f);
            Vector3 pos = transform.position + offset;

            Item nuevo = Instantiate(itemPrefab, pos, Quaternion.identity);
            nuevo.datos = catalogo[i];
            nuevo.enVenta = false;
            nuevo.gameObject.tag = "Consumible";

            nuevo.transform.localScale *= escalaItem;

            nuevo.gameObject.AddComponent<DroppedItem>();
        }

        Debug.Log($"CofreTestPociones: soltadas {catalogo.Length} pociones.");
    }
}
