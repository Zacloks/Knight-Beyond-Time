using UnityEngine;


public class DropsDelNivel : MonoBehaviour
{
    public static DropsDelNivel Instancia { get; private set; }

    [Tooltip("Tabla de drops de este nivel. Asset reutilizable entre escenas del mismo nivel.")]
    public TablaDropsNivel tabla;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Debug.LogWarning($"[DropsDelNivel] Ya había una tabla de drops en la escena ({Instancia.name}). Se ignora la de '{name}'.", this);
            return;
        }
        Instancia = this;
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }
}
