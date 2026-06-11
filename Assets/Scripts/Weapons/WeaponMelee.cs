using UnityEngine;
using System.Collections;

public class WeaponMelee : Weapon
{
    [Header("Configuración Melee")]
    public float attackRadius = 1.5f;
    [Range(0, 360)] public float attackAngle = 90f;
    public float coneOffset = 0f;
    public float damageDelay = 0.15f;
    public LayerMask enemyLayer;

    private bool isSwinging = false;   // Hay un golpe en curso (durante damageDelay)
    private float nextAttackTime = 0f; // Momento a partir del cual se puede volver a atacar
    private Quaternion originalRotation;

    void Start() {
        originalRotation = transform.localRotation;
    }

    // Dirección del cono = hacia donde mira el jugador (FacingDir, en world space)
    // con un offset opcional. NO depende del transform del arma.
    private Vector2 GetFacingDir()
    {
        return Quaternion.AngleAxis(coneOffset, Vector3.forward) * FacingDir();
    }

    // Origen del golpe = posición del JUGADOR (raíz), no la del arma, que puede
    // estar desplazada/espejada por el rig de SPUM al voltear el personaje.
    private Vector3 AttackOrigin()
    {
        return ownerMovement != null ? ownerMovement.transform.position : transform.position;
    }

    public override bool Atacar()
    {
        if (durabilidadActual <= 0) {
            Debug.LogWarning("Esta arma está rota.");
            return false;
        }

        // Cooldown de attack rate: mientras no pase el tiempo (o haya un golpe en
        // curso) NO se puede atacar ni hacer daño. Bloquea el spam de la tecla.
        if (isSwinging || Time.time < nextAttackTime) return false;

        nextAttackTime = Time.time + attackRate;
        StartCoroutine(SwingRoutine());
        return true;
    }

    IEnumerator SwingRoutine()
    {
        isSwinging = true;
        GastarDurabilidad(1);

        yield return new WaitForSeconds(damageDelay);

        // Se calculan en el momento del impacto (el jugador pudo moverse/girar).
        Vector3 origen = AttackOrigin();
        Vector2 facingDir = GetFacingDir();

        Collider2D[] enemigosEnRadio = Physics2D.OverlapCircleAll(origen, attackRadius, enemyLayer);

        foreach (Collider2D col in enemigosEnRadio)
        {
            Vector2 dirToEnemy = ((Vector2)col.transform.position - (Vector2)origen).normalized;

            float angleToEnemy = Vector2.Angle(facingDir, dirToEnemy);

            if (angleToEnemy <= attackAngle / 2f)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int dañoFinal = CalcularDañoCritico(damage);
                    enemy.TakeDamage(dañoFinal, origen);
                    SpriteRenderer enemySprite = col.GetComponent<SpriteRenderer>();
                    if (enemySprite != null) StartCoroutine(FlashRed(enemySprite));
                }
            }
        }

        isSwinging = false;
    }

    private void OnDrawGizmosSelected()
    {
        // En editor (sin play) ownerMovement es null: usa el transform como respaldo.
        Vector3 origen = AttackOrigin();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origen, attackRadius);

        Vector3 facingDir = GetFacingDir();
        Vector3 leftDir = Quaternion.AngleAxis(-attackAngle / 2, Vector3.forward) * facingDir;
        Vector3 rightDir = Quaternion.AngleAxis(attackAngle / 2, Vector3.forward) * facingDir;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origen, leftDir * attackRadius);
        Gizmos.DrawRay(origen, rightDir * attackRadius);
    }
}
