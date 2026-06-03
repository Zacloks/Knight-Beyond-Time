using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private string playerTag = "Player";

    private bool triggered = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag(playerTag)) return;
        triggered = true;
        waveManager.ActivateZone();
    }
}