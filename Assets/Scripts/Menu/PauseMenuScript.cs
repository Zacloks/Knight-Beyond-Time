using UnityEngine;
using UnityEngine.InputSystem;
public class PauseMenuScript : MonoBehaviour
{
    public GameObject menuPausa;
    public static bool pausado;
    public InputActionReference pausar;
    void Start()
    {
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
    }
    void despausarJuego()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
        pausado=false;
    }
}
