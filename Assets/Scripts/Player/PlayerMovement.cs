using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Gestiona el movimiento físico del jugador: desplazamiento, dash y clamping de posición.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference mover;
    public InputActionReference dash;

    [Header("Velocidad")]
    public float velocidadMov = 7f;
    public float dashSpeed = 12f;
    public float extraVelocidad = 1f;

    [Header("Dash")]
    private const float DASH_DURATION = 0.5f;
    private const int   DASH_ENERGY_COST = 60;
    public bool dashInfinito = false;

    [Header("Límites del mapa")]
    public float minX = -100000f;
    public float maxX =  10000f;
    public float minY = -100000f;
    public float maxY =  46.49283f;

    // Estado
    public bool IsDashing { get; private set; }
    public Vector2 CurrentDirection { get; private set; } = Vector2.right;
    public Vector2 MoveInput { get; private set; }

    // Referencias
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

        MoveInput = mover.action.ReadValue<Vector2>();

        if (dash != null && dash.action.triggered)
            TryDash();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ClampPosition();
    }

    // ──────────────────────────────────────────────
    // Movimiento
    // ──────────────────────────────────────────────

    private void ApplyMovement()
    {
        // Si está bebiendo poción u otro estado bloqueante, el coordinador pone
        // MoveInput en cero externamente; aquí solo aplicamos física.
        if (MoveInput.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = IsDashing
                ? CurrentDirection * velocidadMov * extraVelocidad
                : Vector2.zero;
        }
        else
        {
            rb.linearVelocity = MoveInput * velocidadMov * extraVelocidad;

            if (!IsDashing)
                CurrentDirection = MoveInput.x < 0 ? Vector2.left : Vector2.right;
        }
    }

    private void ClampPosition()
    {
        float cx = Mathf.Clamp(rb.position.x, minX, maxX);
        float cy = Mathf.Clamp(rb.position.y, minY, maxY);
        rb.position = new Vector2(cx, cy);
    }

    // ──────────────────────────────────────────────
    // Dash
    // ──────────────────────────────────────────────

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

        Debug.Log("Dash terminado.");
    }

    // ──────────────────────────────────────────────
    // API pública para el coordinador
    // ──────────────────────────────────────────────

    /// <summary>Fuerza la velocidad a cero (ej: al beber pociones).</summary>
    public void CongelarMovimiento()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
