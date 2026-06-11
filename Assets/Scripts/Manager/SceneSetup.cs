using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    [Header("Prefabs a instanciar")]
    public GameObject playerPrefab;
    public GameObject hudCanvasPrefab;
    public GameObject pauseMenuPrefab;
    public GameObject GameOverMenuPrefab;


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

    [Header("Ajustes visuales de esta escena")]
    [Tooltip("Escala del player en esta escena (1 = normal). Súbelo para verlo más grande, ej. en la tienda.")]
    public float playerScale = 1f;
    [Tooltip("Tamaño ortográfico de la cámara en esta escena (0 = no cambiar). Más grande = la cámara abarca más.")]
    public float cameraSize = 0f;

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
        {
            GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);

            if (playerScale > 0f && !Mathf.Approximately(playerScale, 1f))
            {
                Vector3 s = player.transform.localScale;
                player.transform.localScale = new Vector3(s.x * playerScale, s.y * playerScale, s.z);
            }
        }
        else
            Debug.LogWarning("[SceneSetup] Falta asignar playerPrefab.");

        if (cameraSize > 0f && Camera.main != null)
            Camera.main.orthographicSize = cameraSize;

        if (hudCanvasPrefab != null)
            Instantiate(hudCanvasPrefab);
        else
            Debug.LogWarning("[SceneSetup] Falta asignar hudCanvasPrefab.");

        if (pauseMenuPrefab != null)
            Instantiate(pauseMenuPrefab);
        else
            Debug.LogWarning("[SceneSetup] Falta asignar pauseMenuPrefab.");

        if (GameOverMenuPrefab != null)
            Instantiate(GameOverMenuPrefab);
        else
            Debug.LogWarning("[SceneSetup] Falta asignar GameOverMenuPrefab.");
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
