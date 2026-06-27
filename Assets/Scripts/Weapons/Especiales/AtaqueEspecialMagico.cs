using UnityEngine;

public abstract class AtaqueEspecialMagico : ScriptableObject
{
    [Header("Atributos del Especial")]
    public string nombreAtaque = "Especial";
    public float cooldown = 5f;        // Segundos entre usos
    public int costoDurabilidad = 3;   // Durabilidad (o maná) que consume

    public abstract void EjecutarAtaque(WeaponMagic arma, Transform shootPoint, PlayerScript jugador);
}