using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    [Header("Prefabs a instanciar")]
    public GameObject playerPrefab;
    public GameObject hudCanvasPrefab;
    public GameObject pauseMenuPrefab;

    [Tooltip("Opcional: prefab del GameManager. Solo se instancia si aún no existe uno.")]
    public GameObject gameManagerPrefab;

    [Header("Punto de aparición del player")]
    [Tooltip("Punto por defecto (primera entrada al nivel).")]
    public Transform spawnPoint;

    [System.Serializable]
    public class NamedSpawn
    {
        public string id;
        public Transform point;
    }

    [Tooltip("Entradas por id. Si el GameManager pide un id (al volver por una puerta), " +
             "el player aparece en el punto con ese id; si no, se usa el spawnPoint por defecto.")]
    public NamedSpawn[] entradas;

    void Awake()
    {
        if (GameManager.Instance == null && gameManagerPrefab != null)
            Instantiate(gameManagerPrefab);
    }

    void Start()
    {
        Transform spawn = ElegirSpawn();
        Vector3 pos = spawn != null ? spawn.position : transform.position;

        if (playerPrefab != null)
            Instantiate(playerPrefab, pos, Quaternion.identity);
        else
            Debug.LogWarning("[SceneSetup] Falta asignar playerPrefab.");

        if (hudCanvasPrefab != null)
            Instantiate(hudCanvasPrefab);
        else
            Debug.LogWarning("[SceneSetup] Falta asignar hudCanvasPrefab.");

        if (pauseMenuPrefab != null)
            Instantiate(pauseMenuPrefab);
        else
            Debug.LogWarning("[SceneSetup] Falta asignar pauseMenuPrefab.");
    }
    private Transform ElegirSpawn()
    {
        string wanted = GameManager.Instance != null ? GameManager.Instance.nextSpawnId : null;

        if (!string.IsNullOrEmpty(wanted) && entradas != null)
        {
            foreach (NamedSpawn e in entradas)
                if (e != null && e.id == wanted && e.point != null)
                {
                    GameManager.Instance.nextSpawnId = null; // consumir
                    return e.point;
                }
        }

        if (GameManager.Instance != null) GameManager.Instance.nextSpawnId = null;
        return spawnPoint;
    }
}
