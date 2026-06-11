using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Arma mágica genérica: ataque básico a distancia (heredado de WeaponDistance)
// + ataque especial intercambiable vía Patrón Estrategia.
public class WeaponMagic : WeaponDistance
{
    [Header("Ataque Especial (Estrategia)")]
    public AtaqueEspecialMagico ataqueEspecial;

    [Header("Referencia del Teclado (Especial / K) - OPCIONAL")]
    [Tooltip("Si se deja vacío, usa automáticamente la acción de ataque mágico del " +
             "jugador (PlayerScript.attackMagic). Solo asignar para sobrescribirla.")]
    public InputActionReference specialAttackActionRef;

    private float nextSpecialTime;
    private PlayerScript jugador;
    private bool isCastingSpecial = false;

    protected override void Awake()
    {
        base.Awake();
        // El arma se instancia como hijo del jugador (ver PlayerScript.updateItem).
        jugador = GetComponentInParent<PlayerScript>();
    }

    // Bloquea el ataque básico mientras se está casteando el especial,
    // para que su animación de carga no pise la del especial.
    public override bool Atacar()
    {
        if (isCastingSpecial) return false;
        return base.Atacar();
    }

    public override bool AtaqueEspecial()
    {
        if (isCastingSpecial) return false; // Ya hay un especial en curso

        if (ataqueEspecial == null)
        {
            Debug.LogWarning($"{weaponName} no tiene ataque especial asignado.");
            return false;
        }
        if (durabilidadActual < ataqueEspecial.costoDurabilidad)
        {
            Debug.LogWarning("Sin durabilidad para el especial.");
            return false;
        }
        if (Time.time < nextSpecialTime) return false; // En cooldown

        // Fallback perezoso por si el arma no encontró al jugador en Awake.
        if (jugador == null) jugador = GetComponentInParent<PlayerScript>();

        nextSpecialTime = Time.time + ataqueEspecial.cooldown;
        GastarDurabilidad(ataqueEspecial.costoDurabilidad);

        // Mantener pulsado para cargar; lanzar al soltar.
        StartCoroutine(CastSpecialRoutine());
        return true;
    }

    // Devuelve la acción de input que controla el especial: la del arma si se
    // asignó, o si no, la del jugador (que ya está habilitada y funcionando).
    private InputAction GetSpecialAction()
    {
        if (specialAttackActionRef != null) return specialAttackActionRef.action;
        if (jugador != null && jugador.attackMagic != null) return jugador.attackMagic.action;
        return null;
    }

    private IEnumerator CastSpecialRoutine()
    {
        isCastingSpecial = true;

        Sprite original = (weaponRenderer != null) ? weaponRenderer.sprite : null;
        bool hayAnimacion = weaponRenderer != null && spritesDeCarga != null && spritesDeCarga.Length > 0;

        InputAction specialAction = GetSpecialAction();
        float timer = 0f;

        // Mantiene la carga mientras la tecla esté pulsada (aunque supere
        // maxChargeTime: el sprite se queda en el frame máximo). Esperamos al
        // menos un frame para no soltar instantáneamente en el frame de pulsado.
        if (specialAction != null)
        {
            do
            {
                timer += Time.deltaTime;
                ActualizarSpriteCarga(hayAnimacion, timer);
                yield return null;
            }
            while (specialAction.IsPressed());
        }
        else
        {
            // Respaldo: sin acción de input, carga temporizada y lanza.
            while (timer < maxChargeTime)
            {
                timer += Time.deltaTime;
                ActualizarSpriteCarga(hayAnimacion, timer);
                yield return null;
            }
        }

        // Restaura el sprite y lanza el efecto especial (al soltar la tecla).
        if (weaponRenderer != null && original != null) weaponRenderer.sprite = original;

        ataqueEspecial.EjecutarAtaque(this, shootPoint, jugador);
        isCastingSpecial = false;
    }

    private void ActualizarSpriteCarga(bool hayAnimacion, float timer)
    {
        if (!hayAnimacion) return;

        float chargePercent = Mathf.Clamp01(timer / maxChargeTime);
        int indiceSprite = Mathf.FloorToInt(chargePercent * (spritesDeCarga.Length - 1));
        indiceSprite = Mathf.Clamp(indiceSprite, 0, spritesDeCarga.Length - 1);
        weaponRenderer.sprite = spritesDeCarga[indiceSprite];
    }
}
