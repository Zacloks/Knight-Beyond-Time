using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{

    public GameObject objetoTienda; 
    public Transform[] puntosDeAparicion; 

    private List<ItemData> catalogoCompleto;
    
    void Start()
    {
        cargarCatalogo();
        generarTienda();
    }

    
    void cargarCatalogo()
    {
        ItemData[] itemsCargados = Resources.LoadAll<ItemData>("Items");
        catalogoCompleto = new List<ItemData>();
        foreach (ItemData item in itemsCargados)
            if (item != null && item.EsValido())
                catalogoCompleto.Add(item);
    }

    void generarTienda()
    {
        List<ItemData> itemsDisponibles = new List<ItemData>(catalogoCompleto);

        int total = Mathf.Min(3, puntosDeAparicion.Length);
        for (int i = 0; i < total; i++)
        {
            if (itemsDisponibles.Count == 0) break;

            int indiceAleatorio = Random.Range(0, itemsDisponibles.Count);
            ItemData itemElegido = itemsDisponibles[indiceAleatorio];

            itemsDisponibles.RemoveAt(indiceAleatorio);

            GameObject nuevoItemFisico = Instantiate(objetoTienda, puntosDeAparicion[i].position, Quaternion.identity);

            Item itemComponente = nuevoItemFisico.GetComponent<Item>();
            itemComponente.datos = itemElegido;
            itemComponente.enVenta = true; // Marca el item como comprable: descuenta monedas al recogerlo.
        }
    }
}
