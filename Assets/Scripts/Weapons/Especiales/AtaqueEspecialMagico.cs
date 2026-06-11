using UnityEngine;

// Clase base del Patrón Estrategia para ataques especiales de armas mágicas.
// Cada arma mágica referencia un asset concreto que hereda de esta clase.
public abstract class AtaqueEspecialMagico : ScriptableObject
{
    [Header("Atributos del Especial")]
    public string nombreAtaque = "Especial";
    public float cooldown = 5f;        // Segundos entre usos
    public int costoDurabilidad = 3;   // Durabilidad (o maná) que consume

    // El arma pasa su propio contexto: a sí misma (para críticos/flip),
    // el punto de disparo y el jugador (para curaciones/efectos).
    public abstract void EjecutarAtaque(WeaponMagic arma, Transform shootPoint, PlayerScript jugador);
}
