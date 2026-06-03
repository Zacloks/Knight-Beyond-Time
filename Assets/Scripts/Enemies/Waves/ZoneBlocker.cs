using System.Collections;
using UnityEngine;

public class ZoneBlocker : MonoBehaviour
{
    [Header("Colisión")]
    [SerializeField] private Collider2D blockCollider;

    [Header("Visual")]
    [SerializeField] private GameObject visualBlocker;
    [SerializeField] private SpriteRenderer visualRenderer;

    [Header("Animación de desbloqueo")]
    [SerializeField] private float fadeOutDuration = 0.8f;

    public void Block()
    {
        if (blockCollider) blockCollider.enabled = true;
        if (visualBlocker) visualBlocker.SetActive(true);
    }

    public void Unblock()
    {
        if (blockCollider) blockCollider.enabled = false;

        if (visualRenderer != null)
            StartCoroutine(FadeOut());
        else if (visualBlocker)
            visualBlocker.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = visualRenderer.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            visualRenderer.color = c;
            yield return null;
        }

        visualBlocker?.SetActive(false);
    }
}