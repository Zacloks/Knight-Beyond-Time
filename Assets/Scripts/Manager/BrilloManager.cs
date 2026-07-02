using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BrilloManager : MonoBehaviour
{
    public static BrilloManager Instance { get; private set; }

    private Canvas canvas;
    private Image panelBrillo;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CrearPanel();
            AplicarBrillo(PlayerPrefs.GetFloat("brillo", 0.5f));
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reaplicar el brillo cada vez que carga una escena nueva
        AplicarBrillo(PlayerPrefs.GetFloat("brillo", 0.5f));
    }

    private void CrearPanel()
    {
        // Canvas propio para que siempre quede encima de todo
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        // Panel negro que cubre toda la pantalla
        GameObject panelObj = new GameObject("PanelBrilloGlobal");
        panelObj.transform.SetParent(canvas.transform, false);

        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        panelBrillo = panelObj.AddComponent<Image>();
        panelBrillo.color = new Color(0, 0, 0, 0);
        panelBrillo.raycastTarget = false; // No bloquea clicks
    }

    public void AplicarBrillo(float valor)
    {
        if (panelBrillo == null) return;
        // Slider al máximo (1) = sin oscurecer (alfa 0). Mínimo (0) = negro total (alfa 1).
        float alfa = 1f - valor;
        panelBrillo.color = new Color(0, 0, 0, alfa);
    }
}
