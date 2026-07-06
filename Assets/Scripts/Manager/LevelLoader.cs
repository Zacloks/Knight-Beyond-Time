using UnityEngine.SceneManagement;

public static class LevelLoader
{
    public static void LoadScene(string sceneName)
    {
        SaveState();
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadScene(int buildIndex)
    {
        SaveState();
        SceneManager.LoadScene(buildIndex);
    }

    public static void LoadNextLevel()
    {
        SaveState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private static void SaveState()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.NotifyChanged();
    }
}
