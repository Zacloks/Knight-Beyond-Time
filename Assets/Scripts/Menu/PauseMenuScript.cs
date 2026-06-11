using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject menuPausa;
    public static bool pausado;
    public InputActionReference pausar;

    void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
        }

        menuPausa.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       if (pausar != null && pausar.action.triggered)
        {
            if (pausado == false)
            {
                pausarJuego();
            } else
            {
                despausarJuego();
            }
        }
    }
    void pausarJuego()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0f;
        pausado=true;
        Debug.Log("TimeScale: " + Time.timeScale);
    }
    public void despausarJuego()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
        pausado=false;
    }
    public void salirJuego()
    {
        Debug.Log("Saliendo del juego");
        Application.Quit();
    }

    void OnEnable()
{
    pausar.action.Enable();
}

void OnDisable()
{
    pausar.action.Disable();
}
}
