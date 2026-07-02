using UnityEngine;
using UnityEngine.SceneManagement;

public class ZonaCambioEscena : MonoBehaviour
{
    public enum Modo { SiguienteNivel = 0, EscenaPorNombre = 1, VolverAEscenaAnterior = 3 }

    [Header("Destino")]
    public Modo modo = Modo.SiguienteNivel;

    [Tooltip("Solo si modo = EscenaPorNombre")]
    public string nombreEscena;

    [Header("Filtro")]
    public string tagJugador = "Player";

    public string spawnIdDestino;

    [Header("Entrada única")]
    [Tooltip("Id único de esta zona. Si se deja vacío, la zona se puede usar siempre.")]
    public string id;

    [Header("Retorno (zona de ENTRADA a la tienda)")]
    [Tooltip("Marcar en la zona que ENTRA a la tienda: recuerda esta escena para volver luego a su PuntoSalidaTienda.")]
    public bool recordarEscenaActual = false;

    private bool usada;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (usada || !collision.CompareTag(tagJugador)) return;

        GameManager gm = GameManager.Instance;
        if (gm != null && gm.ZonaUsada(id)) return;

        usada = true;

        if (gm != null)
        {
            gm.MarcarZonaUsada(id);

            if (recordarEscenaActual)
            {
                gm.escenaRetorno = SceneManager.GetActiveScene().name;
                gm.tablaTienda = DropsDelNivel.Instancia != null ? DropsDelNivel.Instancia.tabla : null;
            }
        }

        switch (modo)
        {
            case Modo.SiguienteNivel:
                if (gm != null) gm.nextSpawnId = spawnIdDestino;
                LevelLoader.LoadNextLevel();
                break;

            case Modo.EscenaPorNombre:
                if (string.IsNullOrEmpty(nombreEscena))
                {
                    Debug.LogError($"[ZonaCambioEscena] {gameObject.name}: falta el nombre de la escena.");
                    return;
                }
                if (gm != null) gm.nextSpawnId = spawnIdDestino;
                LevelLoader.LoadScene(nombreEscena);
                break;

            case Modo.VolverAEscenaAnterior:
                if (gm == null || string.IsNullOrEmpty(gm.escenaRetorno))
                {
                    Debug.LogError($"[ZonaCambioEscena] {gameObject.name}: no hay escena de retorno guardada.");
                    return;
                }
                gm.volviendoDeTienda = true;
                LevelLoader.LoadScene(gm.escenaRetorno);
                break;
        }
    }
}
