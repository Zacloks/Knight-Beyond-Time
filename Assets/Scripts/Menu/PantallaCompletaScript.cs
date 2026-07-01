using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class PantallaCompletaScript : MonoBehaviour
{
    public Toggle toggle;    

    public TMP_Dropdown resolucionesDropDown;
    Resolution[] resoluciones;

    private bool fullscreenGuardado;
    private int resolucionGuardada;
    private bool fullscreenPendiente;
    private int resolucionPendiente;

    void Start()
    {
        fullscreenGuardado = Screen.fullScreen;
        resolucionGuardada = PlayerPrefs.GetInt("numeroResolucion", 0);
        fullscreenPendiente = fullscreenGuardado;
        resolucionPendiente = resolucionGuardada;

        StartCoroutine(InicializarToggleSincrono());
        RevisarResolucion();
    }

    private IEnumerator InicializarToggleSincrono()
    {
        yield return new WaitForEndOfFrame();
        toggle.SetIsOnWithoutNotify(fullscreenGuardado);
    }

    // Preview en vivo, sin guardar
    public void ActiveFULLS(bool fullscreen)
    {
        fullscreenPendiente = fullscreen;
    }

    public void RevisarResolucion()
    {
        resoluciones = Screen.resolutions;
        resolucionesDropDown.ClearOptions();

        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);

            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }
        }

        resolucionesDropDown.AddOptions(opciones);
        resolucionesDropDown.value = PlayerPrefs.GetInt("numeroResolucion", resolucionActual);
        resolucionesDropDown.RefreshShownValue();
        resolucionPendiente = resolucionesDropDown.value;
    }

    // Preview en vivo, sin guardar
    public void CambiarResolucion(int indiceResolucion)
    {
        resolucionPendiente = indiceResolucion;
    }

    public void Aplicar()
    {
        fullscreenGuardado = fullscreenPendiente;
        resolucionGuardada = resolucionPendiente;

        PlayerPrefs.SetInt("numeroResolucion", resolucionGuardada);
        Resolution res = resoluciones[resolucionGuardada];
        Screen.SetResolution(res.width, res.height, fullscreenGuardado);
        Debug.Log("[Opciones] Aplicado — " + res.width + "x" + res.height + " fullscreen:" + fullscreenGuardado);
    }

    public void Cancelar()
    {
        fullscreenPendiente = fullscreenGuardado;
        resolucionPendiente = resolucionGuardada;

        toggle.SetIsOnWithoutNotify(fullscreenGuardado);
        resolucionesDropDown.value = resolucionGuardada;
        resolucionesDropDown.RefreshShownValue();
    }
}