using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [Header("Configuración de oleadas")]
    [SerializeField] private List<WaveData> waves = new();
    [SerializeField] private float timeBetweenWaves = 2f;

    [Header("Referencias")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private ZoneBlocker zoneBlocker;

    [Header("Eventos (conectar en Inspector)")]
    public UnityEvent OnAllWavesCleared;
    public UnityEvent<int, int> OnWaveStarted;

    private int currentWaveIndex = 0;
    private readonly List<GameObject> liveEnemies = new();
    private bool zoneActive = false;

    public void ActivateZone()
    {
        if (zoneActive) return;

        if (spawner == null)
        {
            Debug.LogError("[WaveManager] No hay EnemySpawner asignado. No se puede activar la zona.");
            return;
        }
        if (waves.Count == 0)
        {
            Debug.LogWarning("[WaveManager] No hay oleadas configuradas. Se desbloquea la zona directamente.");
            zoneBlocker?.Unblock();
            return;
        }

        zoneActive = true;
        zoneBlocker?.Block();
        StartCoroutine(RunWaves());
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null) liveEnemies.Add(enemy);
    }

    public void NotifyEnemyDied(GameObject enemy)
    {
        liveEnemies.Remove(enemy);
        Debug.Log($"[WaveManager] Enemigos vivos: {AliveCount()}");
    }

    private int AliveCount()
    {
        liveEnemies.RemoveAll(e => e == null);
        return liveEnemies.Count;
    }

    private IEnumerator RunWaves()
    {
        for (currentWaveIndex = 0; currentWaveIndex < waves.Count; currentWaveIndex++)
        {
            if (currentWaveIndex > 0)
                yield return new WaitForSeconds(timeBetweenWaves);

            WaveData wave = waves[currentWaveIndex];

            List<EnemySpawnEntry> queue = BuildSpawnQueue(wave);
            int cap = wave.maxConcurrent <= 0 ? int.MaxValue : wave.maxConcurrent;

            OnWaveStarted?.Invoke(currentWaveIndex + 1, waves.Count);
            Debug.Log($"[WaveManager] Oleada {currentWaveIndex + 1}/{waves.Count} — {queue.Count} enemigos (máx en pantalla: {(cap == int.MaxValue ? "sin límite" : cap.ToString())})");

            int idx = 0;
            while (idx < queue.Count)
            {
                if (AliveCount() < cap)
                {
                    EnemySpawnEntry unit = queue[idx];
                    idx++;

                    GameObject go = spawner.SpawnEnemy(unit.enemyPrefab, unit.spawnPointIndex, this);
                    if (go == null) continue;

                    if (wave.spawnInterval > 0f)
                        yield return new WaitForSeconds(wave.spawnInterval);
                    else
                        yield return null;
                }
                else
                {
                    yield return null;
                }
            }

            yield return new WaitUntil(() => AliveCount() == 0);
        }

        Debug.Log("[WaveManager] ¡Zona completada!");
        zoneBlocker?.Unblock();
        OnAllWavesCleared?.Invoke();
    }

    private List<EnemySpawnEntry> BuildSpawnQueue(WaveData wave)
    {
        List<EnemySpawnEntry> queue = new();

        foreach (EnemySpawnEntry entry in wave.entries)
        {
            if (entry.enemyPrefab == null)
            {
                Debug.LogWarning("[WaveManager] Una entrada de oleada no tiene prefab asignado; se ignora.");
                continue;
            }
            for (int i = 0; i < entry.count; i++)
                queue.Add(entry);
        }

        for (int i = queue.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (queue[i], queue[j]) = (queue[j], queue[i]);
        }

        return queue;
    }
}


[System.Serializable]
public class WaveData
{
    public List<EnemySpawnEntry> entries;

    [Header("Ritmo (estilo Castle Crashers)")]
    [Min(0)] public int maxConcurrent = 5;
    [Min(0f)] public float spawnInterval = 0.5f;
}

[System.Serializable]
public class EnemySpawnEntry
{
    public GameObject enemyPrefab;
    [Min(1)] public int count = 3;
    [Tooltip("-1 = spawn point aleatorio")]
    public int spawnPointIndex = -1;
}
