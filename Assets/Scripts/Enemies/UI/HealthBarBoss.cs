using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Barra de vida del jefe dirigida por eventos. Vive siempre activa en el HUD y
/// se suscribe a BossEvents: al aparecer un jefe se muestra y fija el máximo, con
/// cada golpe se actualiza y al morir se oculta. No necesita referencia al jefe,
/// así funciona igual si el jefe está en la escena o se instancia como prefab.
///
/// IMPORTANTE: este componente debe estar en un objeto que permanezca ACTIVO
/// (si se desactiva, deja de recibir eventos). Para ocultar la barra usa el campo
/// 'container' (el objeto visual de la barra), NO el objeto de este componente.
/// </summary>
public class HealthBarBoss : MonoBehaviour
{
    [Header("Referencias")]
    public Slider slider;
    [Tooltip("Objeto visual que se enciende/apaga. Debe ser DISTINTO al objeto de este script.")]
    public GameObject container;
    [Tooltip("Opcional: nombre del jefe.")]
    public TMP_Text nameLabel;

    private void Awake()
    {
        SetVisible(false);
    }

    private void OnEnable()
    {
        BossEvents.OnBossSpawned += HandleSpawned;
        BossEvents.OnBossHealthChanged += HandleHealthChanged;
        BossEvents.OnBossDefeated += HandleDefeated;
    }

    private void OnDisable()
    {
        BossEvents.OnBossSpawned -= HandleSpawned;
        BossEvents.OnBossHealthChanged -= HandleHealthChanged;
        BossEvents.OnBossDefeated -= HandleDefeated;
    }

    private void HandleSpawned(string nombre, int maxLife)
    {
        SetVisible(true);
        if (slider != null)
        {
            slider.maxValue = maxLife;
            slider.value = maxLife;
        }
        if (nameLabel != null) nameLabel.text = nombre;
    }

    private void HandleHealthChanged(int current)
    {
        if (slider != null) slider.value = current;
    }

    private void HandleDefeated()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        GameObject target = container != null ? container : (slider != null ? slider.gameObject : null);
        if (target == gameObject)
        {
            Debug.LogWarning($"{name}: 'container' apunta al propio objeto del HealthBarBoss; al ocultarse dejaría de recibir eventos. Asigna un objeto hijo distinto.", this);
            return;
        }
        if (target != null) target.SetActive(visible);
    }
}
