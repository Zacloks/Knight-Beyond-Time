using UnityEngine;
using System.Collections;

public class WeaponDistance : Weapon
{
    [Header("Configuración de Carga")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float maxChargeTime = 2f;    
    public float minProjectileSpeed = 5f; 
    public float maxProjectileSpeed = 20f;
    public float maxKnockback = 8f;  

    private float chargeTimer;
    private bool isCharging;
    private float lastAttackTime;
    private KeyCode currentKey;

    public override void Atacar()
    {
        if (Input.GetKeyDown(KeyCode.J)) currentKey = KeyCode.J;
        else if (Input.GetKeyDown(KeyCode.Z)) currentKey = KeyCode.Z;
        else return;
        if (Time.time >= lastAttackTime + attackRate && !isCharging)
        {
            isCharging = true;
            chargeTimer = 0;
            Debug.Log("Cargando arco...");
        }
    }

    void Update()
    {
     if (isCharging)
        {
            if (Input.GetKey(currentKey))
            {
                chargeTimer += Time.deltaTime;
                chargeTimer = Mathf.Min(chargeTimer, maxChargeTime);
            }
            
            if (Input.GetKeyUp(currentKey))
            {
                Disparar();
            }
        }
    }

    private void Disparar()
    {
        isCharging = false;
        lastAttackTime = Time.time;

        float chargePercent = Mathf.Clamp01(chargeTimer / maxChargeTime);
        
        int finalDamage = Mathf.RoundToInt(damage * Mathf.Lerp(0.2f, 1f, chargePercent));
        float finalSpeed = Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, chargePercent);
        float finalKnockback = chargePercent * maxKnockback;

        if (projectilePrefab != null && shootPoint != null)
        {
            Vector3 spawnPosition = shootPoint.transform.position;
            Quaternion spawnRotation = shootPoint.transform.rotation;
            GameObject newProjectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
            Projectile script = newProjectile.GetComponent<Projectile>();
            
            if (script != null)
            {
                script.Setup(finalDamage, finalSpeed, finalKnockback, this);
                
                float life = Mathf.Lerp(0.5f, 3f, chargePercent); 
                Destroy(newProjectile, life);
            }
        }
        Debug.Log($"¡Fuego! Carga: {Mathf.Round(chargePercent * 100)}% - Daño: {finalDamage}");
    }
}