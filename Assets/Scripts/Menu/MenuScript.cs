using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{
    public void cerrarJuego()
    {
        Application.Quit();
    }
    public void volverMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void abrirOpciones()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void jugar()
    {
        if (GameManager.Instance != null) GameManager.Instance.NuevaPartida();
        SceneManager.LoadSceneAsync("Inicio");
    }

  
}
