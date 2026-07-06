using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossTrigger : MonoBehaviour
{
    [SerializeField] private EnemyBoss boss;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool disableAfterTrigger = true;

    private bool triggered;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag(playerTag)) return;
        if (boss == null)
        {
            Debug.LogWarning($"{name}: BossTrigger sin 'boss' asignado.", this);
            return;
        }

        triggered = true;
        boss.Activate();

        if (disableAfterTrigger) gameObject.SetActive(false);
    }
}
