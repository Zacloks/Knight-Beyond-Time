using UnityEngine;

public class ZonaCambioEscena : MonoBehaviour
{
    public enum Modo { SiguienteNivel, EscenaPorNombre, EscenaPorIndice }

    [Header("Destino")]
    public Modo modo = Modo.SiguienteNivel;

    [Tooltip("Solo si modo = EscenaPorNombre")]
    public string nombreEscena;

    [Tooltip("Solo si modo = EscenaPorIndice (Build Settings)")]
    public int indiceEscena;

    [Header("Filtro")]
    public string tagJugador = "Player";

    public string spawnIdDestino;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagJugador)) return;

        if (GameManager.Instance != null)
            GameManager.Instance.nextSpawnId = spawnIdDestino;

        switch (modo)
        {
            case Modo.SiguienteNivel:
                LevelLoader.LoadNextLevel();
                break;

            case Modo.EscenaPorNombre:
                if (string.IsNullOrEmpty(nombreEscena))
                {
                    Debug.LogError($"[ZonaCambioEscena] {gameObject.name}: falta el nombre de la escena.");
                    return;
                }
                LevelLoader.LoadScene(nombreEscena);
                break;

            case Modo.EscenaPorIndice:
                LevelLoader.LoadScene(indiceEscena);
                break;
        }
    }
}
