using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossMusic : MonoBehaviour
{
    [Header("Música del jefe")]
    public AudioClip bossTheme;
    [Range(0f, 1f)] public float volume = 0.7f;
    [Tooltip("Segundos de desvanecido (fade out) al morir el jefe.")]
    public float fadeOutOnDefeat = 2f;
    [Tooltip("Segundos de aparición suave (fade in) al empezar la pelea. 0 = arranca a tope.")]
    public float fadeInOnSpawn = 0.5f;

    [Header("Música del nivel (opcional)")]
    [Tooltip("Si la asignas, se pausa al aparecer el jefe y se reanuda al morir.")]
    public AudioSource levelMusic;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f; 
    }

    private void OnEnable()
    {
        BossEvents.OnBossSpawned += HandleSpawned;
        BossEvents.OnBossDefeated += HandleDefeated;
    }

    private void OnDisable()
    {
        BossEvents.OnBossSpawned -= HandleSpawned;
        BossEvents.OnBossDefeated -= HandleDefeated;
    }

    private void HandleSpawned(string nombre, int maxLife)
    {
        if (bossTheme == null)
        {
            Debug.LogWarning($"{name}: BossMusic sin 'bossTheme' asignado.", this);
            return;
        }

        StopAllCoroutines();
        if (levelMusic != null && levelMusic.isPlaying) levelMusic.Pause();

        source.clip = bossTheme;
        source.loop = true;
        source.Play();

        if (fadeInOnSpawn > 0f)
        {
            source.volume = 0f;
            StartCoroutine(FadeTo(volume, fadeInOnSpawn));
        }
        else
        {
            source.volume = volume;
        }
    }

    private void HandleDefeated()
    {
        transform.SetParent(null, true);
        StopAllCoroutines();
        StartCoroutine(FadeOutAndDie());
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        source.volume = target;
    }

    private IEnumerator FadeOutAndDie()
    {
        float start = source.volume;
        float t = 0f;
        while (t < fadeOutOnDefeat && source.volume > 0f)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(start, 0f, t / fadeOutOnDefeat);
            yield return null;
        }

        source.Stop();
        if (levelMusic != null) levelMusic.UnPause();
        Destroy(gameObject);
    }
}
