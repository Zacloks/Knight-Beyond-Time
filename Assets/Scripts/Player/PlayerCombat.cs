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
    [SerializeField] private float attackLockTime = 1f;

    [Header("Ataque sin arma (manos vacías)")]
    public int unarmedDamage = 3;
    public float unarmedRadius = 1.1f;
    [Range(0, 360)] public float unarmedAngle = 110f;
    public float unarmedCloseRange = 0.6f;
    public float unarmedDamageDelay = 0.15f;

    //Referencias
    private PlayerAnimator playerAnimator;
    private PlayerInventory playerInventory;
    private PlayerStats playerStats;
    private Coroutine attackLockRoutine;
    void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
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
            StartCoroutine(GolpeSinArma());
        }
    }

    private IEnumerator GolpeSinArma()
    {
        yield return new WaitForSeconds(unarmedDamageDelay);

        PlayerMovement movement = GetComponent<PlayerMovement>();
        Vector3 origen = transform.position;
        Vector2 facingDir = (movement != null)
            ? (movement.LastDirection.x >= 0f ? Vector2.right : Vector2.left)
            : (transform.lossyScale.x < 0f ? Vector2.right : Vector2.left);

        float alcance = playerStats != null ? playerStats.multiplicadorAlcance : 1f;

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(origen, unarmedRadius * alcance);
        foreach (Collider2D col in enemigos)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null) continue;

            Vector2 toEnemy = (Vector2)col.transform.position - (Vector2)origen;
            float dist = toEnemy.magnitude;

            bool enRango = dist <= unarmedCloseRange
                ? Vector2.Dot(facingDir, toEnemy) > 0f
                : Vector2.Angle(facingDir, toEnemy / Mathf.Max(dist, 0.0001f)) <= unarmedAngle / 2f;

            if (enRango)
            {
                enemy.TakeDamage(CalcularDañoSinArma(), origen);

                SpriteRenderer enemySprite = col.GetComponent<SpriteRenderer>();
                if (enemySprite == null) enemySprite = col.GetComponentInChildren<SpriteRenderer>();
                if (enemySprite != null) StartCoroutine(FlashRed(enemySprite));
            }
        }
    }

    private int CalcularDañoSinArma()
    {
        float mult = playerStats != null ? playerStats.multiplicadorDaño : 1f;
        float critExtra = playerStats != null ? playerStats.bonusCritico : 0f;
        int dañoConBuff = Mathf.RoundToInt(unarmedDamage * mult);

        if (Random.Range(0f, 100f) <= critExtra)
            return dañoConBuff * 2;
        return dañoConBuff;
    }

    private IEnumerator FlashRed(SpriteRenderer spr)
    {
        Color originalColor = spr.color;
        spr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (spr != null) spr.color = originalColor;
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

        float mult = playerStats != null ? playerStats.multiplicadorVelocidadAtaque : 1f;
        if (mult <= 0f) mult = 1f;

        yield return new WaitForSeconds(attackLockTime / mult);

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