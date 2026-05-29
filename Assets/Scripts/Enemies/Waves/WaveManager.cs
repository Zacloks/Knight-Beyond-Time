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
    private int enemiesAlive = 0;
    private bool zoneActive = false;

    public void ActivateZone()
    {
        if (zoneActive) return;
        zoneActive = true;
        zoneBlocker.Block();
        StartCoroutine(RunWaves());
    }

    public void NotifyEnemyDied()
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        Debug.Log($"[WaveManager] Enemigos restantes: {enemiesAlive}");
    }

    private IEnumerator RunWaves()
    {
        for (currentWaveIndex = 0; currentWaveIndex < waves.Count; currentWaveIndex++)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            int spawned = spawner.SpawnWave(waves[currentWaveIndex], this);
            enemiesAlive += spawned;

            OnWaveStarted?.Invoke(currentWaveIndex + 1, waves.Count);
            Debug.Log($"[WaveManager] Oleada {currentWaveIndex + 1}/{waves.Count} — {spawned} enemigos");

            yield return new WaitUntil(() => enemiesAlive == 0);
        }

        Debug.Log("[WaveManager] ¡Zona completada!");
        zoneBlocker.Unblock();
        OnAllWavesCleared?.Invoke();
    }
}


[System.Serializable]
public class WaveData
{
    public List<EnemySpawnEntry> entries;
}

[System.Serializable]
public class EnemySpawnEntry
{
    public GameObject enemyPrefab;
    [Min(1)] public int count = 3;
    [Tooltip("-1 = spawn point aleatorio")]
    public int spawnPointIndex = -1;
}