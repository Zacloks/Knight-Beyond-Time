using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponDistance : Weapon
{
    [Header("Configuración de Carga")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float maxChargeTime = 1.5f;
    public float minProjectileSpeed = 8f;
    public float maxProjectileSpeed = 25f;
    public float maxRange = 15f; // Nueva: Distancia máxima a plena carga

    private float chargeTimer;
    private bool isCharging;
    private float nextAttackTime; // Nueva: Control de cooldown

    [Header("Referencia del Teclado")]
    public InputActionReference attackActionRef;

    public override bool Atacar()
    {
        if (!isCharging && Time.time >= nextAttackTime)
        {
            isCharging = true;
            chargeTimer = 0;
            Debug.Log("Cargando arco...");
            return true; 
        }
        return false; 
    }

    void Update()
    {
        if (isCharging && attackActionRef != null)
        {
            if (attackActionRef.action.IsPressed())
            {
                chargeTimer += Time.deltaTime;
                chargeTimer = Mathf.Min(chargeTimer, maxChargeTime);
            }
            else 
            {
                Disparar();
            }
        }
    }

    private void Disparar()
    {
        isCharging = false;
        nextAttackTime = Time.time + attackRate; 

        float chargePercent = Mathf.Clamp01(chargeTimer / maxChargeTime);
        int finalDamage = Mathf.RoundToInt(damage * Mathf.Lerp(0.5f, 1.5f, chargePercent));
        float finalSpeed = Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, chargePercent);

        if (projectilePrefab != null && shootPoint != null)
        {
            float escalaX = transform.lossyScale.x;
            Quaternion rotacionFlecha = shootPoint.rotation;

            if (escalaX > 0) 
            {
                rotacionFlecha *= Quaternion.Euler(0, 0, 180f);
            }

            GameObject newProjectile = Instantiate(projectilePrefab, shootPoint.position, rotacionFlecha);
            
            Projectile script = newProjectile.GetComponent<Projectile>();
            if (script != null)
            {
                script.Setup(finalDamage, finalSpeed, 0, this);
                float actualRange = maxRange * Mathf.Lerp(0.2f, 1f, chargePercent);
                float lifeTime = actualRange / finalSpeed; 
                Destroy(newProjectile, lifeTime); 
            }
        }
    }
}