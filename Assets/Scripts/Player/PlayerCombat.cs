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
    [SerializeField] private float attackLockTime = 0.4f;

    //Referencias
    private PlayerAnimator playerAnimator;
    private PlayerInventory playerInventory; 
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
        ItemData itemData = playerInventory.GetEquippedItemData();
        Item itemEnMano   = playerInventory.GetEquippedItemInstance();

        if (itemData != null)
        {
            if (itemEnMano is Weapon arma)
            {
                bool exito = arma.Atacar();
                if (exito)
                {
                    string tipo = arma.GetType().Name;
                    Debug.Log($"Ataque confirmado. Arma: {tipo}");

                    if (tipo == "WeaponMelee")
                        playerAnimator.TriggerMeleeAttack();

                    StartCoroutine(AtackCoroutine());
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
            StartCoroutine(AtackCoroutine());
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
                StartCoroutine(AtackCoroutine());
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
            StartCoroutine(AtackCoroutine());
        }
    }
    public void IniciarAnimacionMitad(string nombreAnimacion)
    {
        if (playerAnimator != null)
        {
            StartCoroutine(AnimacionMitadCoroutine(nombreAnimacion));
        }
    }

//-----------COROUTINES--------
    private IEnumerator AtackCoroutine()
    {
        isAtacking = true;

        yield return new WaitForSeconds(attackLockTime);

        isAtacking = false;
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