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
    public Transform spawnPoint;

    void Awake()
    {
        if (GameManager.Instance == null && gameManagerPrefab != null)
            Instantiate(gameManagerPrefab);
    }

    void Start()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;

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
}
