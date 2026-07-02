using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Componentes de Audio")]
    [SerializeField] private AudioSource musicaSource;

    [Header("Ajustes")]
    [SerializeField] private float volumen = 1f;
    [SerializeField] private float fadeDuration = 1f;

    private AudioClip clipDeseado;
    private bool suspendido;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicaSource == null) musicaSource = gameObject.AddComponent<AudioSource>();
        musicaSource.playOnAwake = false;
        musicaSource.loop = true;
        musicaSource.volume = 0f;

        AudioListener.volume = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
    }

    void OnEnable()
    {
        BossEvents.OnBossSpawned += HandleBossSpawned;
        BossEvents.OnBossDefeated += HandleBossDefeated;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        BossEvents.OnBossSpawned -= HandleBossSpawned;
        BossEvents.OnBossDefeated -= HandleBossDefeated;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene escena, LoadSceneMode modo)
    {
        suspendido = false;
    }

    public void PlayMusic(AudioClip clip)
    {
        clipDeseado = clip;
        if (suspendido) return;

        if (clip == null)
        {
            IniciarFade(FadeOut());
            return;
        }
        if (musicaSource.clip == clip && musicaSource.isPlaying) return;

        IniciarFade(FadeSwap(clip));
    }

    private void HandleBossSpawned(string nombre, int maxLife)
    {
        suspendido = true;
        IniciarFade(FadeOut());
    }

    private void HandleBossDefeated()
    {
        suspendido = false;
        if (clipDeseado != null) IniciarFade(FadeSwap(clipDeseado));
    }

    private void IniciarFade(IEnumerator rutina)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(rutina);
    }

    private IEnumerator FadeSwap(AudioClip clip)
    {
        yield return FadeVolumen(0f);
        musicaSource.clip = clip;
        musicaSource.loop = true;
        musicaSource.Play();
        yield return FadeVolumen(volumen);
        fadeRoutine = null;
    }

    private IEnumerator FadeOut()
    {
        yield return FadeVolumen(0f);
        musicaSource.Stop();
        fadeRoutine = null;
    }

    private IEnumerator FadeVolumen(float objetivo)
    {
        if (fadeDuration <= 0f)
        {
            musicaSource.volume = objetivo;
            yield break;
        }

        float inicio = musicaSource.volume;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicaSource.volume = Mathf.Lerp(inicio, objetivo, t / fadeDuration);
            yield return null;
        }
        musicaSource.volume = objetivo;
    }
}
