using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public Slider slider;

    public void setMaxEnergy(int energy)
    {
        slider.maxValue = energy;
        slider.value = energy;
    }
    public void setEnergy(int energy)
    {
        slider.value = energy; 
    }
}
