using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Componentes de Audio")]
    [SerializeField] private AudioSource musicaSource;

    [Header("Lista de Canciones (Opcional)")]
    public AudioClip musicaFondoPrincipal;

    void Awake()
    {
        // Implementación del patrón Singleton para que solo exista UN AudioManager
        if (Instance == null)
        {
            Instance = this;
            // ¡Esto es la clave! Evita que el objeto se destruya al cambiar de escena
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Si ya existe uno en la siguiente escena, destruye el duplicado
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Reproducir la música inicial si está asignada
        if (musicaSource != null && musicaFondoPrincipal != null)
        {
            PlayMusica(musicaFondoPrincipal);
        }
    }

    public void PlayMusica(AudioClip cancion)
    {
        if (musicaSource.clip == cancion) return; // Ya se está reproduciendo

        musicaSource.clip = cancion;
        musicaSource.loop = true; // Para que no se detenga
        musicaSource.Play();
    }

    public void DetenerMusica()
    {
        musicaSource.Stop();
    }
}