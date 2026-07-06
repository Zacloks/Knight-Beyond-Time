using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Puntos de spawn")]
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private float spawnScatter = 0.6f;
    [SerializeField] private float minDistanceFromPlayer = 4f;

    [SerializeField] private string playerTag = "Player";
    private Transform player;

    private void Awake()
    {
        if (spawnPoints.Count == 0)
        {
            foreach (Transform child in transform)
                spawnPoints.Add(child);
        }

        if (string.IsNullOrEmpty(playerTag)) playerTag = "Player";
        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;
    }

    public GameObject SpawnEnemy(GameObject prefab, int spawnPointIndex, WaveManager manager)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[EnemySpawner] Se pidió spawnear un prefab nulo.");
            return null;
        }

        Transform point = GetSpawnPoint(spawnPointIndex);
        Vector3 scatter = (Vector3)(Random.insideUnitCircle * spawnScatter);
        GameObject go = Instantiate(prefab, point.position + scatter, point.rotation);

        if (go.TryGetComponent<LifeEnemy>(out var lifeEnemy))
            lifeEnemy.RegisterWaveManager(manager);
        else
            Debug.LogWarning($"[EnemySpawner] {go.name} no tiene LifeEnemy; su muerte no se notificará a la oleada.");

        manager.RegisterEnemy(go);
        return go;
    }

    private Transform GetSpawnPoint(int index)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] Sin puntos de spawn. Usando posición del spawner.");
            return transform;
        }

        if (index >= 0 && index < spawnPoints.Count)
            return spawnPoints[index];

        return GetRandomPointAwayFromPlayer();
    }

    private Transform GetRandomPointAwayFromPlayer()
    {
        if (player == null || minDistanceFromPlayer <= 0f)
            return spawnPoints[Random.Range(0, spawnPoints.Count)];

        List<Transform> faraway = new();
        foreach (Transform p in spawnPoints)
            if (p != null && Vector2.Distance(p.position, player.position) >= minDistanceFromPlayer)
                faraway.Add(p);

        if (faraway.Count > 0)
            return faraway[Random.Range(0, faraway.Count)];

        Transform farthest = null;
        float best = -1f;
        foreach (Transform p in spawnPoints)
        {
            if (p == null) continue;
            float d = Vector2.Distance(p.position, player.position);
            if (d > best) { best = d; farthest = p; }
        }
        return farthest != null ? farthest : transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform p in spawnPoints)
            if (p != null) Gizmos.DrawWireSphere(p.position, 0.4f);
    }
}
