using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //NOMBRES DE PARÁMETROS.
    public AudioSource audioSource;
    public AudioClip attackClip;
    private string PARAM_SPEED        = "Speed";
    private string TRIGGER_ATTACK     = "2_Attack";
    private string TRIGGER_MAGIC      = "attackMagic";
    private string TRIGGER_DASH       = "7_Dash";
    private string TRIGGER_DEBUFF     = "5_Debuff";
    private string STATE_IDLE         = "0_Idle";
    private string PARAM_DEATH        = "isDeath";
    private PlayerMovement movement;
    private PlayerCombat combat;
    [Header("Animador de Sprite")]
    public Animator anim;
    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        combat   = GetComponent<PlayerCombat>();
        audioSource = GetComponent<AudioSource>();
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
        Vector3 escala = transform.localScale;
        float magnitud = Mathf.Abs(escala.x);
        if (input.x > 0.1f) transform.localScale = new Vector3(-magnitud, escala.y, escala.z);
            else if (input.x < -0.1f) transform.localScale = new Vector3(magnitud, escala.y, escala.z);
    }

//--------TRIGGERS DE ANIMACIONES-------

    public void TriggerMeleeAttack()
    {
        anim.ResetTrigger(TRIGGER_ATTACK);
        anim.SetTrigger(TRIGGER_ATTACK);
        audioSource.PlayOneShot(attackClip);
    }
    public void ResetMeleeTrigger() {anim.ResetTrigger(TRIGGER_ATTACK);}
    public void TriggerMagicAttack() {Trigger(TRIGGER_MAGIC);}
    public void TriggerDash() {Trigger(TRIGGER_DASH);}
    public void TriggerDebuff() {Trigger(TRIGGER_DEBUFF);}
    public void TriggerDeath() {anim.SetBool(PARAM_DEATH, true);}
    public void Trigger(string nombreAnimacion) {anim.SetTrigger(nombreAnimacion);}

    public void CrossFadeToIdle(float transitionDuration = 0.05f)
    {
        anim.CrossFade(STATE_IDLE, transitionDuration);
    }
}
