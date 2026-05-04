using UnityEngine;

public class Flotador: MonoBehaviour
{
    public float amplitud = 0.05f;
    public float velocidad = 5f;
    
    private Vector2 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.position = new Vector2(posicionInicial.x, nuevaY);
    }
}