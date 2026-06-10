using UnityEngine;
public class WeaponMagic : WeaponDistance
{
    [Header("Configuración del Ataque Especial (Tecla K)")]
    public float specialCooldown = 5f;
    public int specialDurabilityCost = 3;
    
    public GameObject specialSpellPrefab; 
    public Transform specialSpawnPoint;
    public int specialDamage = 25;

    private float nextSpecialTime;
    public override bool AtaqueEspecial()
    {
        if (durabilidadActual < specialDurabilityCost)
        {
            Debug.LogWarning("El arma mágica está sin energía/durabilidad.");
            return false;
        }

        if (Time.time >= nextSpecialTime)
        {
            nextSpecialTime = Time.time + specialCooldown;
            GastarDurabilidad(specialDurabilityCost);

            EjecutarEfectoEspecial(); 
            
            return true; 
        }
        
        return false; 
    }

    protected virtual void EjecutarEfectoEspecial()
    {
        Debug.Log($"¡Lanzando ataque especial de {weaponName}!");

        if (specialSpellPrefab != null && specialSpawnPoint != null)
        {
            Vector2 dir = FacingDir();
            Quaternion rotacion = (dir.x >= 0f) ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);

            GameObject hechizo = Instantiate(specialSpellPrefab, specialSpawnPoint.position, rotacion);
            
            Projectile script = hechizo.GetComponent<Projectile>();
            if (script != null)
            {
                int dañoFinal = CalcularDañoCritico(specialDamage);
                
                script.Setup(dañoFinal, 2f, 0, this); 
            }
        }
    }
}
