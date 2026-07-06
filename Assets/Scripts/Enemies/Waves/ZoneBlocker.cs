using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBlocker : MonoBehaviour
{
    [Header("Muros")]
    [Tooltip("Cada muro debe tener un Collider2D. Si se deja vacío, se usan los hijos directos (ej: MuroEntrada, MuroSalida).")]
    [SerializeField] private List<GameObject> muros = new();

    [Header("Animación de desbloqueo")]
    [SerializeField] private float fadeOutDuration = 0.8f;

    private void Awake()
    {
        if (muros.Count == 0)
            foreach (Transform child in transform)
                muros.Add(child.gameObject);

        SetMuros(false);
    }

    public void Block()
    {
        SetMuros(true);
    }
    public bool TryGetHorizontalBounds(out float left, out float right)
    {
        left = float.MaxValue;
        right = float.MinValue;
        bool any = false;

        foreach (GameObject muro in muros)
        {
            if (muro == null) continue;
            float x = muro.transform.position.x;
            left = Mathf.Min(left, x);
            right = Mathf.Max(right, x);
            any = true;
        }
        return any;
    }

    public void Unblock()
    {
        foreach (GameObject muro in muros)
        {
            if (muro == null) continue;

            if (muro.TryGetComponent<Collider2D>(out var col)) col.enabled = false;

            if (muro.TryGetComponent<SpriteRenderer>(out var sr))
                StartCoroutine(FadeOut(muro, sr));
            else
                muro.SetActive(false);
        }
    }

    private void SetMuros(bool cerrado)
    {
        foreach (GameObject muro in muros)
        {
            if (muro == null) continue;

            muro.SetActive(cerrado);
            if (muro.TryGetComponent<Collider2D>(out var col)) col.enabled = cerrado;

            if (cerrado && muro.TryGetComponent<SpriteRenderer>(out var sr))
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }
    }

    private IEnumerator FadeOut(GameObject muro, SpriteRenderer sr)
    {
        float elapsed = 0f;
        Color c = sr.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            sr.color = c;
            yield return null;
        }

        muro.SetActive(false);
    }
}
