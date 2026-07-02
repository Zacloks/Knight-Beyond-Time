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

    [Header("Golpe sin arma (puños)")]
    [Tooltip("Daño del golpe con la mano cuando no hay arma equipada.")]
    public int dañoPuño = 5;
    [Tooltip("Radio de alcance del puñetazo.")]
    public float radioPuño = 1f;
    [Range(0, 360)]
    [Tooltip("Ángulo del cono del puñetazo al frente del jugador.")]
    public float anguloPuño = 90f;
    [Tooltip("A esta distancia o menos, basta con que el enemigo esté un poco adelante.")]
    public float closeRangePuño = 0.6f;
    [Tooltip("Retraso antes de aplicar el daño (sincroniza con la animación de golpe).")]
    public float delayPuño = 0.15f;
    [Tooltip("Capa(s) de los enemigos a los que puede pegar el puño. Asignar la layer de los enemigos.")]
    public LayerMask enemyLayer;

    //Referencias
    private PlayerAnimator playerAnimator;
    private PlayerInventory playerInventory;
    private PlayerMovement playerMovement;
    private Coroutine attackLockRoutine;
    void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerInventory = GetComponent<PlayerInventory>();
        playerMovement = GetComponent<PlayerMovement>();
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
            // Sin arma: golpea con la mano (misma animación) y hace daño.
            playerAnimator.TriggerMeleeAttack();
            StartCoroutine(GolpePuñoRoutine());
            StartAttackLock();
        }
    }

    // Golpe cuerpo a cuerpo cuando el jugador NO tiene arma (puñetazo).
    // Replica la detección en cono de WeaponMelee, con sus propios parámetros.
    private IEnumerator GolpePuñoRoutine()
    {
        yield return new WaitForSeconds(delayPuño);

        Vector3 origen = transform.position;
        Vector2 facingDir = Vector2.right;
        if (playerMovement != null)
            facingDir = playerMovement.LastDirection.x >= 0f ? Vector2.right : Vector2.left;

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(origen, radioPuño, enemyLayer);

        foreach (Collider2D col in enemigos)
        {
            Vector2 toEnemy = (Vector2)col.transform.position - (Vector2)origen;
            float dist = toEnemy.magnitude;

            bool enRango;
            if (dist <= closeRangePuño)
                enRango = Vector2.Dot(facingDir, toEnemy) > 0f;       // muy cerca: basta con estar adelante
            else
                enRango = Vector2.Angle(facingDir, toEnemy / dist) <= anguloPuño / 2f; // cono al frente

            if (!enRango) continue;

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(dañoPuño, origen);
                SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
                if (sr != null) StartCoroutine(FlashRed(sr));
            }
        }
    }

    private IEnumerator FlashRed(SpriteRenderer spr)
    {
        Color original = spr.color;
        spr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (spr != null) spr.color = original;
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