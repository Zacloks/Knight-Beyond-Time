using System;
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

    public event Action OnStateChanged;

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
    NotifyChanged();
}
}
