using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //NOMBRES DE PARÁMETROS.
    private string PARAM_SPEED        = "Speed";
    private string TRIGGER_ATTACK     = "2_Attack";
    private string TRIGGER_MAGIC      = "attackMagic";
    private string TRIGGER_DASH       = "7_Dash";
    private string TRIGGER_DEBUFF     = "5_Debuff";
    private string STATE_IDLE         = "0_Idle";

    private PlayerMovement movement;
    private PlayerCombat combat;
    [Header("Animador de Sprite")]
    public Animator anim;
    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        combat   = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        UpdateLocomotion();
        UpdateFacing();
    }

    private void UpdateLocomotion()
    {
        if (anim == null) return;

        //SI ESTÁ ATACANDO CONGELA ANIMACION MOV.
        if (combat != null && combat.isAtacking)
        {
            anim.SetFloat(PARAM_SPEED, 0);
            return;
        }

        Vector2 input = movement.direccionMov;
        float fuerza = input.magnitude;
        anim.SetFloat(PARAM_SPEED, fuerza);
    }

    private void UpdateFacing()
    {
        if (movement == null) return;

        Vector2 input = movement.direccionMov;
        if (input.x > 0.1f) transform.localScale = new Vector3(-1, 1, 1);
            else if (input.x < -0.1f) transform.localScale = new Vector3(1, 1, 1);
    }

//--------TRIGGERS DE ANIMACIONES-------

    public void TriggerMeleeAttack() {Trigger(TRIGGER_ATTACK);}
    public void TriggerMagicAttack() {Trigger(TRIGGER_MAGIC);}
    public void TriggerDash() {Trigger(TRIGGER_DASH);}
    public void TriggerDebuff() {Trigger(TRIGGER_DEBUFF);}

    public void Trigger(string nombreAnimacion) {anim.SetTrigger(nombreAnimacion);}

    public void CrossFadeToIdle(float transitionDuration = 0.05f)
    {
        anim.CrossFade(STATE_IDLE, transitionDuration);
    }
}
