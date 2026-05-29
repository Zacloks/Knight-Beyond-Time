using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Puntos de spawn")]
    [Tooltip("Vacío = usa los hijos de este GameObject automáticamente")]
    [SerializeField] private List<Transform> spawnPoints = new();

    private void Awake()
    {
        if (spawnPoints.Count == 0)
        {
            foreach (Transform child in transform)
                spawnPoints.Add(child);
        }
    }

    public int SpawnWave(WaveData wave, WaveManager manager)
    {
        int total = 0;

        foreach (var entry in wave.entries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Transform point = GetSpawnPoint(entry.spawnPointIndex);
                GameObject go = Instantiate(entry.enemyPrefab, point.position, point.rotation);

                if (go.TryGetComponent<LifeEnemy>(out var lifeEnemy))
                {
                    lifeEnemy.RegisterWaveManager(manager);
                }
                else
                {
                    Debug.LogWarning($"[EnemySpawner] {go.name} no tiene LifeEnemy. No podrá notificar su muerte.");
                }

                total++;
            }
        }

        return total;
    }

    private Transform GetSpawnPoint(int index)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] Sin puntos de spawn. Usando posición del spawner.");
            return transform;
        }

        if (index < 0 || index >= spawnPoints.Count)
            return spawnPoints[Random.Range(0, spawnPoints.Count)];

        return spawnPoints[index];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform p in spawnPoints)
            if (p != null) Gizmos.DrawWireSphere(p.position, 0.4f);
    }
}