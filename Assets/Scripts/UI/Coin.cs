using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class Coin : MonoBehaviour
{
    public void setCoins(int coins)
    {
        var t = GetComponent<TextMeshProUGUI>();;
        t.text = coins.ToString();
    
    }

}
