using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicaBrillo : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Image panelBrillo;

    private float valorGuardado;

    void Start()
    {
        valorGuardado = PlayerPrefs.GetFloat("brillo", 0.5f);
        slider.value = valorGuardado;
        sliderValue = valorGuardado;
        AplicarBrillo(valorGuardado);
    }

    // Preview en vivo, sin guardar
    public void ChangeSlider(float valor)
    {
        sliderValue = valor;
        AplicarBrillo(valor);
    }

    public void Aplicar()
    {
        valorGuardado = sliderValue;
        PlayerPrefs.SetFloat("brillo", valorGuardado);
    }

    public void Cancelar()
    {
        slider.value = valorGuardado;
        sliderValue = valorGuardado;
        AplicarBrillo(valorGuardado);
    }

    private void AplicarBrillo(float valor)
    {
        // Panel local en OptionsMenu
        float alfa = 1f - valor;
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, alfa);

        // Manager global para que afecte todas las escenas
        if (BrilloManager.Instance != null)
            BrilloManager.Instance.AplicarBrillo(valor);
    }
}
