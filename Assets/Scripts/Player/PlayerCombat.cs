using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerCombat : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference atacar;
    public InputActionReference attackMagic;

    [Header("Estado")]
    public bool isAtacking = false;
    [Tooltip("Segundos que el player queda 'atacando' (congela anim. de movimiento).")]
    [SerializeField] private float attackLockTime = 1f;

    //Referencias
    private PlayerAnimator playerAnimator;
    private PlayerInventory playerInventory;
    private Coroutine attackLockRoutine;
    void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (atacar != null && atacar.action.triggered)
            EjecutarAtaque();

        if (attackMagic != null && attackMagic.action.triggered)
            EjecutarAtaqueMagic();
    }

//-----------ATAQUE----------
    private void EjecutarAtaque()
    {
        // No permitir iniciar un nuevo ataque mientras el anterior sigue 
        // en lock/animación
        if (isAtacking) return;

        ItemData itemData = playerInventory.GetEquippedItemData();
        Item itemEnMano   = playerInventory.GetEquippedItemInstance();

        if (itemData != null)
        {
            if (itemEnMano is Weapon arma)
            {
                bool exito = arma.Atacar();
                if (exito)
                {
                    Debug.Log($"Ataque confirmado. Arma: {arma.GetType().Name}");

                    if (arma is WeaponMelee)
                        playerAnimator.TriggerMeleeAttack();

                    StartAttackLock();
                }
                else
                {
                    Debug.Log("Ataque ignorado por Cooldown o falta de durabilidad.");
                }
            }
            else if (itemEnMano != null)
            {
                itemEnMano.Usar(GetComponent<PlayerScript>());
            }
            else
            {
                itemData.Usar(GetComponent<PlayerScript>());
            }
        }
        else
        {
            playerAnimator.TriggerMeleeAttack();
            Debug.Log("¡Ataque SIN ARMA ejecutado con J!");
            StartAttackLock();
        }
    }

    private void EjecutarAtaqueMagic()
    {
        Item itemEnMano = playerInventory.GetEquippedItemInstance();

        if (itemEnMano is Weapon arma)
        {
            bool exito = arma.AtaqueEspecial();
            if (exito)
            {
                playerAnimator.TriggerMagicAttack();
                Debug.Log("¡Ataque Especial / Mágico ejecutado con K!");
                StartAttackLock();
            }
        }
        else
        {
            Debug.LogWarning("No hay arma equipada para usar magia.");
        }
    }
//----------ANIMACIONES ITEMS/POCIONES---------
    public void IniciarAnimacion(string nombreAnimacion)
    {
        if (playerAnimator != null)
        {
            playerAnimator.Trigger(nombreAnimacion);
            StartAttackLock();
        }
    }
    public void IniciarAnimacionMitad(string nombreAnimacion)
    {
        if (playerAnimator != null)
        {
            StartCoroutine(AnimacionMitadCoroutine(nombreAnimacion));
        }
    }

//evitar errores en animacion ataque 
    private void StartAttackLock()
    {
        if (attackLockRoutine != null) StopCoroutine(attackLockRoutine);
        attackLockRoutine = StartCoroutine(AtackCoroutine());
    }

    private IEnumerator AtackCoroutine()
    {
        isAtacking = true;

        yield return new WaitForSeconds(attackLockTime);

        isAtacking = false;
        attackLockRoutine = null;

        if (playerAnimator != null) playerAnimator.ResetMeleeTrigger();
    }
    private IEnumerator AnimacionMitadCoroutine(string nombreAnimacion)
    {
        playerAnimator.Trigger(nombreAnimacion);

        yield return new WaitForEndOfFrame();
        Animator anim = GetComponentInChildren<Animator>();

        if (anim != null)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            float tiempoMitad = stateInfo.length / 2f;
            yield return new WaitForSeconds(tiempoMitad);
            anim.CrossFade("0_Idle", 0.05f); 
        }
    }
}