using UnityEngine;
using UnityEngine.SceneManagement;

public class AplicarOpciones : MonoBehaviour
{
    public VolumenScript volumen;
    public LogicaBrillo brillo;
    public PantallaCompletaScript pantalla;

    public void Aplicar()
    {
        volumen.Aplicar();
        brillo.Aplicar();
        pantalla.Aplicar();
        PlayerPrefs.Save();
        Debug.Log("[Opciones] Configuración guardada.");
    }

    public void Volver()
    {
        // Revertir solo lo que no fue guardado (visual preview)
        volumen.Cancelar();
        brillo.Cancelar();
        pantalla.Cancelar();
        SceneManager.LoadSceneAsync(0);
    }
}
