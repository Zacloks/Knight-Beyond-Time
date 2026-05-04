using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Consumible : MonoBehaviour
{
    [Header("Atributos")]
    public string name;
    
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Pick(PlayerScript player)
    {
        Destroy(gameObject);
        player.testVida(10);
    }
}