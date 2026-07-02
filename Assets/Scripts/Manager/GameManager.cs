using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("Energía")]
    public int maxEnergy = 100;
    public int currentEnergy = 100;

    [Header("Monedas")]
    public int coins = 0;

    [Header("Inventario")]
    public ItemData[] inventorySlots = new ItemData[5];
    public int selectedSlot = 0;

    [Header("Estado interno")]
    [Tooltip("Se pone a true tras sembrar el estado en la primera escena.")]
    public bool initialized = false;

    [System.NonSerialized] public string nextSpawnId;

    [System.NonSerialized] public string escenaRetorno;
    [System.NonSerialized] public bool volviendoDeTienda;

    [System.NonSerialized] public HashSet<string> zonasUsadas = new HashSet<string>();

    public event Action OnStateChanged;

    public bool ZonaUsada(string id) => !string.IsNullOrEmpty(id) && zonasUsadas.Contains(id);
    public void MarcarZonaUsada(string id) { if (!string.IsNullOrEmpty(id)) zonasUsadas.Add(id); }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (inventorySlots == null || inventorySlots.Length == 0)
            inventorySlots = new ItemData[5];
    }

    public void NotifyChanged() => OnStateChanged?.Invoke();

    public void ResetState()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        zonasUsadas.Clear();
        nextSpawnId = null;
        volviendoDeTienda = false;
        escenaRetorno = null;
        NotifyChanged();
    }

    public void NuevaPartida()
    {
        coins = 0;
        inventorySlots = new ItemData[5];
        selectedSlot = 0;
        zonasUsadas.Clear();
        nextSpawnId = null;
        volviendoDeTienda = false;
        escenaRetorno = null;
        initialized = false;
        NotifyChanged();
    }
}
