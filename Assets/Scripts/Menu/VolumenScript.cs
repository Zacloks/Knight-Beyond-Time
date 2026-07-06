using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumenScript : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Image imagenMute;

    private float valorGuardado;

    void Start()
    {
        valorGuardado = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        slider.value = valorGuardado;
        sliderValue = valorGuardado;
        AudioListener.volume = valorGuardado;
        RevisarSiEstoyMute();
    }

    // Preview en vivo, sin guardar
    public void ChangeSlider(float valor)
    {
        sliderValue = valor;
        AudioListener.volume = valor;
        RevisarSiEstoyMute();
    }

    public void Aplicar()
    {
        valorGuardado = sliderValue;
        PlayerPrefs.SetFloat("volumenAudio", valorGuardado);
    }

    public void Cancelar()
    {
        slider.value = valorGuardado;
        sliderValue = valorGuardado;
        AudioListener.volume = valorGuardado;
        RevisarSiEstoyMute();
    }

    private void RevisarSiEstoyMute()
    {
        if (imagenMute == null) return;
        imagenMute.enabled = (sliderValue == 0);
    }
}