using System.Collections;
using UnityEngine;

public class BossShockwaveAoE : BossAbility
{
    [Header("Onda de choque")]
    public float radius = 3.5f;
    public int damage = 25;
    [Tooltip("Anticipación generosa: el jugador debe poder salir del área.")]
    public float telegraph = 0.9f;
    public float recovery = 0.6f;
    public string animTrigger = "Shockwave";

    [Header("VFX (opcionales)")]
    [Tooltip("Indicador del área durante la anticipación (se destruye al golpear).")]
    public GameObject telegraphVfx;
    [Tooltip("Efecto del impacto al golpear.")]
    public GameObject shockwaveVfx;
    [Tooltip("Si el indicador es un sprite de 1 unidad, se escala a 2*radius.")]
    public bool scaleTelegraphToRadius = true;

    public override IEnumerator Run(BossContext ctx)
    {
        if (ctx.animator != null && !string.IsNullOrEmpty(animTrigger))
            ctx.animator.SetTrigger(animTrigger);

        GameObject indicator = null;
        if (telegraphVfx != null)
        {
            indicator = Instantiate(telegraphVfx, transform.position, Quaternion.identity);
            if (scaleTelegraphToRadius) indicator.transform.localScale = Vector3.one * (radius * 2f);
        }

        yield return new WaitForSeconds(telegraph);

        if (indicator != null) Destroy(indicator);
        if (shockwaveVfx != null)
        {
            GameObject fx = Instantiate(shockwaveVfx, transform.position, Quaternion.identity);
            if (scaleTelegraphToRadius) fx.transform.localScale = Vector3.one * (radius * 2f);
            Destroy(fx, 1.5f);
        }

        DealDamage(ctx, transform.position, radius, damage);

        yield return new WaitForSeconds(recovery);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
