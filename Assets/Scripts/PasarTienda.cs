using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PasarTienda : MonoBehaviour
{
public string nombreEscenaTienda;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (string.IsNullOrEmpty(nombreEscenaTienda))
            {
                Debug.LogError($"[PasarTienda] ¡Error en {gameObject.name}! No has asignado el nombre de la escena de la tienda en el Inspector.");
                return;
            }

            Debug.Log($"¡{collision.gameObject.name} entró a la tienda! Cargando escena: {nombreEscenaTienda}");
            
            SceneManager.LoadScene(nombreEscenaTienda);
        }
    }
}
