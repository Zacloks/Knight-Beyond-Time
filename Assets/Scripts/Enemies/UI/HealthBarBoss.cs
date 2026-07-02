using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
