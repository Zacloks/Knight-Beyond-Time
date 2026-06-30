using System.Collections;
using UnityEngine;

/// <summary>
/// Base para un ataque del jefe. Cada ataque es un componente que se agrega al
/// prefab del jefe. El cerebro (EnemyBoss) recolecta todas las habilidades,
/// elige una usable según rango + cooldown + peso, y ejecuta su corutina Run.
/// Estructura recomendada de Run: anticipación (windup) -> daño -> recuperación.
/// </summary>
public abstract class BossAbility : MonoBehaviour
{
    [Header("Selección (común a todas las habilidades)")]
    [Tooltip("Segundos de espera propios antes de poder repetir ESTE ataque.")]
    public float cooldown = 4f;
    [Tooltip("Distancia mínima al jugador para poder usar este ataque.")]
    public float minRange = 0f;
    [Tooltip("Distancia máxima al jugador para poder usar este ataque.")]
    public float maxRange = 99f;
    [Tooltip("Peso para la selección aleatoria (mayor = más probable).")]
    [Min(0f)] public float weight = 1f;

    private float lastUsedTime = -999f;

    public float Weight => weight;
    public bool IsReady => Time.time >= lastUsedTime + cooldown;

    /// <summary>¿Se puede usar ahora mismo? (cooldown propio + rango)</summary>
    public virtual bool CanUse(float distToPlayer)
    {
        return IsReady && distToPlayer >= minRange && distToPlayer <= maxRange;
    }

    public void MarkUsed() => lastUsedTime = Time.time;

    /// <summary>Ejecuta el ataque. El cerebro mantiene al jefe quieto mientras dura.</summary>
    public abstract IEnumerator Run(BossContext ctx);

    /// <summary>Aplica daño (con crítico) a cualquier PlayerScript dentro de un círculo.</summary>
    protected void DealDamage(BossContext ctx, Vector2 center, float radius, int baseDamage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, ctx.hittableLayers);
        foreach (Collider2D h in hits)
        {
            if (h.TryGetComponent(out PlayerScript player))
            {
                int dmg = ctx.enemy != null ? ctx.enemy.CalcularDanoConCritico(baseDamage) : baseDamage;
                player.TakeDamage(dmg, transform.position);
            }
        }
    }
}
