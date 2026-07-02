using UnityEngine;

public class PosicionTienda : MonoBehaviour
{
    [Header("Posiciones posibles para la tienda")]
    public Transform posicionA;
    public Transform posicionB;

    void Awake()
    {
        if (posicionA == null || posicionB == null)
        {
            Debug.LogWarning("[PosicionTienda] Asigná las dos posiciones en el Inspector.");
            return;
        }

        Vector3 posicionElegida = Random.value < 0.5f ? posicionA.position : posicionB.position;
        transform.position = posicionElegida;
    }
}
