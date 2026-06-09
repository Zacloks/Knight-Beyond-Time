using UnityEngine;

/// <summary>
/// Centraliza todas las interacciones con el Animator del jugador.
/// Ningún otro componente debe llamar a anim.SetTrigger() directamente.
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    // Nombres de parámetros del Animator como constantes para evitar typos
    private const string PARAM_SPEED        = "Speed";
    private const string TRIGGER_ATTACK     = "2_Attack";
    private const string TRIGGER_MAGIC      = "attackMagic";
    private const string TRIGGER_DASH       = "7_Dash";
    private const string TRIGGER_DEBUFF     = "5_Debuff";
    private const string STATE_IDLE         = "0_Idle";

    private PlayerMovement movement;
    private PlayerCombat combat;
    [Header("Animador de Sprite")]
    public Animator anim;
    void Awake()
    {
        anim     = GetComponentInChildren<Animator>();
        movement = GetComponent<PlayerMovement>();
        combat   = GetComponent<PlayerCombat>();

        if (anim == null)
            Debug.LogError("PlayerAnimator: no se encontró Animator en los hijos.", this);
    }

    void Update()
    {
        UpdateLocomotion();
        UpdateFacing();
    }

    // ──────────────────────────────────────────────
    // Locomotion blend
    // ──────────────────────────────────────────────

    private void UpdateLocomotion()
    {
        if (anim == null) return;

        // Si está atacando o bebiendo, congela la animación de movimiento
        if (combat != null && combat.isAtacking)
        {
            anim.SetFloat(PARAM_SPEED, 0f);
            return;
        }

        Vector2 input = movement != null ? movement.MoveInput : Vector2.zero;
        anim.SetFloat(PARAM_SPEED, input.magnitude);
    }

    private void UpdateFacing()
    {
        if (movement == null) return;

        Vector2 input = movement.MoveInput;
        if (input.x > 0.1f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (input.x < -0.1f)
            transform.localScale = new Vector3(1f, 1f, 1f);
    }

    // ──────────────────────────────────────────────
    // Triggers públicos (llamados por otros componentes)
    // ──────────────────────────────────────────────

    public void TriggerMeleeAttack() => SafeTrigger(TRIGGER_ATTACK);
    public void TriggerMagicAttack() => SafeTrigger(TRIGGER_MAGIC);
    public void TriggerDash()        => SafeTrigger(TRIGGER_DASH);
    public void TriggerDebuff()      => SafeTrigger(TRIGGER_DEBUFF);

    /// <summary>Trigger genérico para animaciones de ítems/pociones.</summary>
    public void Trigger(string nombreAnimacion) => SafeTrigger(nombreAnimacion);

    public void CrossFadeToIdle(float transitionDuration = 0.05f)
    {
        anim?.CrossFade(STATE_IDLE, transitionDuration);
    }

    // ──────────────────────────────────────────────
    // Privados
    // ──────────────────────────────────────────────

    private void SafeTrigger(string triggerName)
    {
        if (anim == null) return;
#if UNITY_EDITOR
        Debug.Log($"[PlayerAnimator] Trigger: {triggerName}");
#endif
        anim.SetTrigger(triggerName);
    }
}
