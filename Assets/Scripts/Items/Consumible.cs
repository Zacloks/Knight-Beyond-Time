using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Consumible : Item
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Pick(PlayerScript player)
    {
        Destroy(gameObject);
    }
}