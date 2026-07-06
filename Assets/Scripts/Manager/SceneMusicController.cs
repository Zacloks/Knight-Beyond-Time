using UnityEngine;

public class SceneMusicController : MonoBehaviour
{
    [Header("Música de esta escena")]
    [SerializeField] private AudioClip musicaEscena;
    [Tooltip("Opcional. Si se deja vacío, no cambia la música cuando hay enemigos.")]
    [SerializeField] private AudioClip musicaCombate;

    [Header("Detección de combate")]
    [SerializeField] private float intervaloChequeo = 0.5f;

    private bool enCombate;

    void Start()
    {
        enCombate = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(musicaEscena);

        if (musicaCombate != null)
            InvokeRepeating(nameof(ChequearCombate), intervaloChequeo, intervaloChequeo);
    }

    private void ChequearCombate()
    {
        if (AudioManager.Instance == null) return;

        bool hay = HayEnemigosVivos();

        if (hay && !enCombate)
        {
            enCombate = true;
            AudioManager.Instance.PlayMusic(musicaCombate);
        }
        else if (!hay && enCombate)
        {
            enCombate = false;
            AudioManager.Instance.PlayMusic(musicaEscena);
        }
    }

    private bool HayEnemigosVivos()
    {
        Enemy[] enemigos = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy e in enemigos)
        {
            if (e is EnemyBoss) continue;
            if (!e.isActiveAndEnabled) continue;

            LifeEnemy vida = e.GetComponent<LifeEnemy>();
            if (vida != null && vida.IsDead()) continue;

            return true;
        }
        return false;
    }
}
