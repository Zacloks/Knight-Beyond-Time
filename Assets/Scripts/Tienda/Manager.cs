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
        catalogoCompleto = new List<ItemData>(itemsCargados);
    }

    void generarTienda()
    {
        List<ItemData> itemsDisponibles = new List<ItemData>(catalogoCompleto);

        for (int i = 0; i < 3; i++)
        {
            int indiceAleatorio = Random.Range(0, itemsDisponibles.Count);
            ItemData itemElegido = itemsDisponibles[indiceAleatorio];

            itemsDisponibles.RemoveAt(indiceAleatorio);

            GameObject nuevoItemFisico = Instantiate(objetoTienda, puntosDeAparicion[i].position, Quaternion.identity);
            
            nuevoItemFisico.GetComponent<Item>().datos = itemElegido;
        }
    }
}
