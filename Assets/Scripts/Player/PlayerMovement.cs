using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference mover;
    public InputActionReference dash;

    [Header("Velocidad")]
    public float velocidadMov = 5.5f;
    public float dashSpeed = 10f;
    public float extraVelocidad = 1;

    [Header("Dash")]
    private const float DASH_DURATION = 0.5f;
    private const int   DASH_ENERGY_COST = 60;
    public bool dashInfinito = false;

    [Header("Knockback (empuje al recibir golpes/choques)")]
    public float knockbackDuration = 0.22f;
    [Range(0f, 1f)] public float knockbackDecay = 0.85f;
    private Vector2 knockbackVel;
    private float knockbackEndTime;

    [Header("Límites del mapa")]
    public float minX = -100000f;
    public float maxX =  10000f;
    public float minY = -100000f;
    public float maxY =  46.49283f;

    //ESTADOS
    public bool IsDashing = false;
    public Vector2 direccionMov;
    public Vector2 LastDirection = Vector2.right;

    //REFERENCIAS
    private Rigidbody2D rb;
    private PlayerStats stats;
    private PlayerAnimator playerAnimator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    void Update()
    {
        if (IsDashing) return;

        direccionMov = mover.action.ReadValue<Vector2>();

        if (dash != null && dash.action.triggered)
            TryDash();
    }

    void FixedUpdate()
    {
        if (Time.time < knockbackEndTime)
        {
            rb.linearVelocity = knockbackVel;
            knockbackVel *= knockbackDecay;
            ClampPosition();
            return;
        }

        ApplyMovement();
        ClampPosition();
    }

//-----------Knockback---------------
    public void AplicarKnockback(Vector2 fromPosition, float fuerza)
    {
        Vector2 dir = (Vector2)transform.position - fromPosition;
        if (dir.sqrMagnitude < 0.0001f) dir = LastDirection;
        dir.Normalize();

        knockbackVel = dir * fuerza;
        knockbackEndTime = Time.time + knockbackDuration;
        IsDashing = false; // cancela el dash si estaba en curso
    }

//-----------Movimiento---------------
    private void ApplyMovement()
    {

        if (direccionMov.sqrMagnitude < 0.01f) {
            rb.linearVelocity = Vector2.zero;
            if (IsDashing) rb.linearVelocity = LastDirection * velocidadMov * extraVelocidad;

        } else {
            rb.linearVelocity = direccionMov * velocidadMov * extraVelocidad;

            if (!IsDashing)
            {
                if (direccionMov.x < -0.1f) LastDirection = Vector2.left;
                else if (direccionMov.x > 0.1f) LastDirection = Vector2.right;
            }
        }
    }

    private void ClampPosition()
    {
        float clampedX = Mathf.Clamp(rb.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(rb.position.y, minY, maxY);
        rb.position = new Vector2(clampedX, clampedY);
    }

//---------Dash-----------

    private void TryDash()
    {
        if (dashInfinito)
        {
            StartCoroutine(DashCoroutine());
            return;
        }

        if (stats != null && stats.TryConsumeEnergy(DASH_ENERGY_COST))
        {
            Debug.Log($"Dash activado. Energía restante: {stats.currentEnergy}");
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        float velocidadOriginal = velocidadMov;
        velocidadMov = dashSpeed;
        IsDashing = true;

        if (playerAnimator != null) playerAnimator.TriggerDash();

        yield return new WaitForSeconds(DASH_DURATION);

        velocidadMov = velocidadOriginal;
        IsDashing = false;

        Debug.LogWarning("Dash terminado.");
    }

    public void CongelarMovimiento()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
