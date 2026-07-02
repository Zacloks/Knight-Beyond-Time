using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverScript : MonoBehaviour
{
    public static GameOverScript Instance;
    public GameObject panelGameOver;

    void Awake()
    {
        Instance = this;
        panelGameOver.SetActive(false);
    }

    public void MostrarGameOver()
    {
        panelGameOver.SetActive(true);
        Time.timeScale = 0;
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.ResetState();
        LevelLoader.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}