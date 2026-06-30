using System.Collections;
using UnityEngine;

/// <summary>
/// Cerebro del jefe. Recolecta todas las BossAbility del prefab y, cuando está
/// en rango y terminó el cooldown global, elige una usable (random ponderado por
/// rango + cooldown propio) y la ejecuta manteniéndose quieto mientras dura.
/// Incluye intro al aparecer (invulnerable) y un stun único al bajar de media vida.
/// Avisa de su vida por BossEvents (la barra se suscribe).
/// </summary>
public class EnemyBoss : Enemy
{
    public override bool PuedeSoltarMonedas => true;
    protected override bool RequiresAttackComponent => false;

    [Header("Identidad")]
    public string bossName = "Jefe";

    [Header("IA del Jefe")]
    [Tooltip("Capas a las que el jefe puede golpear (normalmente la del jugador).")]
    public LayerMask hittableLayers;
    [Tooltip("Pausa entre el fin de un ataque y la elección del siguiente.")]
    public float globalCooldown = 1.3f;

    [Header("Intro al aparecer")]
    [Tooltip("Reproduce una animación de entrada al spawnear (invulnerable y quieto).")]
    public bool playIntro = true;
    public string introTrigger = "Entry";
    [Tooltip("Cuánto dura la intro antes de empezar a pelear. Ajústalo a tu clip.")]
    public float introDuration = 1.5f;

    [Header("Stun a media vida")]
    public bool useHalfHealthStun = true;
    [Tooltip("Trigger del Animator que ENTRA al stun (se dispara una vez).")]
    public string stunTrigger = "StunStart";
    [Tooltip("Bool del Animator: se mantiene en true durante el loop; al ponerse false sale del stun.")]
    public string stunBool = "Stunned";
    [Tooltip("Cuánto tiempo queda aturdido (vulnerable).")]
    public float stunDuration = 3.5f;
    [Tooltip("Duración de la animación Stun[Out]: el boss sigue quieto este tiempo " +
             "tras el aturdimiento para que la salida se reproduzca sin que el " +
             "movimiento (Chasing) la corte. Ajústalo a tu clip de Stun[Out].")]
    public float stunOutDuration = 0.5f;
    [Range(0f, 1f)]
    [Tooltip("Fracción de vida a la que se dispara el stun (0.5 = mitad).")]
    public float stunHealthThreshold = 0.5f;

    private BossAbility[] abilities;
    private Rigidbody2D rb;
    private Animator animator;
    private BossContext ctx;
    private bool isActing;
    private bool introPlaying;
    private bool hasStunned;
    private float nextDecisionTime;

    private Transform Player => movementEnemy != null ? movementEnemy.PlayerTransform : null;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        abilities = GetComponents<BossAbility>();

        if (abilities.Length == 0)
            Debug.LogWarning($"{name}: el jefe no tiene ninguna BossAbility. No podrá atacar.", this);

        ctx = new BossContext
        {
            self = transform,
            rb = rb,
            animator = animator,
            enemy = this,
            movement = movementEnemy,
            hittableLayers = hittableLayers
        };

        BossEvents.RaiseSpawned(bossName, lifeEnemy.MaxLife);

        if (playIntro) StartCoroutine(IntroRoutine());
    }

    private void Update()
    {
        if (lifeEnemy == null || lifeEnemy.IsDead()) return;
        if (introPlaying || isActing) return;
        if (Time.time < nextDecisionTime) return;

        ctx.player = Player;
        if (ctx.player == null) return;

        BossAbility choice = ChooseAbility(ctx.DistanceToPlayer);
        if (choice != null) StartCoroutine(RunAbility(choice));
    }

    // INTRO: entra haciendo su animación, quieto e invulnerable, y luego pelea.
    private IEnumerator IntroRoutine()
    {
        introPlaying = true;
        movementEnemy.BeginSustainedAttack();
        if (animator != null && !string.IsNullOrEmpty(introTrigger))
            animator.SetTrigger(introTrigger);

        yield return new WaitForSeconds(introDuration);

        movementEnemy.EndSustainedAttack();
        introPlaying = false;
    }

    private IEnumerator RunAbility(BossAbility ability)
    {
        isActing = true;
        ability.MarkUsed();
        movementEnemy.BeginSustainedAttack();

        yield return ability.Run(ctx);

        movementEnemy.EndSustainedAttack();
        nextDecisionTime = Time.time + globalCooldown;
        isActing = false;
    }

    // STUN: corta lo que esté haciendo, queda aturdido y vulnerable un tiempo.
    private IEnumerator StunRoutine()
    {
        isActing = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        movementEnemy.BeginSustainedAttack();
        if (animator != null)
        {
            // Trigger: entra una sola vez (no se re-dispara aunque siga aturdido).
            if (!string.IsNullOrEmpty(stunTrigger)) animator.SetTrigger(stunTrigger);
            // Bool: mantiene el loop hasta que lo apaguemos para salir.
            if (!string.IsNullOrEmpty(stunBool)) animator.SetBool(stunBool, true);
        }

        yield return new WaitForSeconds(stunDuration);

        // Apaga el bool -> dispara Stun[Out]. Seguimos "quietos" (sustained attack)
        // mientras dura el Out, para que el movimiento (Chasing -> AnyState/Run) no
        // lo interrumpa antes de tiempo.
        if (animator != null && !string.IsNullOrEmpty(stunBool))
            animator.SetBool(stunBool, false);

        if (stunOutDuration > 0f)
            yield return new WaitForSeconds(stunOutDuration);

        movementEnemy.EndSustainedAttack();
        nextDecisionTime = Time.time + globalCooldown;
        isActing = false;
    }

    // Random ponderado entre las habilidades usables (rango + cooldown propio).
    private BossAbility ChooseAbility(float dist)
    {
        float totalWeight = 0f;
        foreach (BossAbility a in abilities)
            if (a.CanUse(dist)) totalWeight += a.Weight;

        if (totalWeight <= 0f) return null;

        float pick = Random.Range(0f, totalWeight);
        foreach (BossAbility a in abilities)
        {
            if (!a.CanUse(dist)) continue;
            pick -= a.Weight;
            if (pick <= 0f) return a;
        }
        return null;
    }

    public override void TakeDamage(int amount, Vector2 sender)
    {
        // Invulnerable mientras hace su entrada.
        if (introPlaying) return;

        base.TakeDamage(amount, sender);

        BossEvents.RaiseHealthChanged(lifeEnemy.currentLife);

        if (lifeEnemy.currentLife <= 0)
        {
            StopAllCoroutines(); // corta cualquier ataque/stun en curso al morir
            isActing = false;
            BossEvents.RaiseDefeated();
            return;
        }

        // Stun único al cruzar el umbral de vida.
        if (useHalfHealthStun && !hasStunned &&
            lifeEnemy.currentLife <= lifeEnemy.MaxLife * stunHealthThreshold)
        {
            hasStunned = true;
            StopAllCoroutines();   // interrumpe el ataque en curso
            StartCoroutine(StunRoutine());
        }
    }
}
