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
    public float maxRange = 15f; 


    [Header("Animación Visual de Carga")]
    public SpriteRenderer weaponRenderer;
    public Sprite[] spritesDeCarga;
    private Sprite spriteOriginal;

    [Tooltip("Ángulo (en grados) al que se endereza el arma en la mano de SPUM. 0 = horizontal.")]
    public float anguloVisual = 0f;


    private float chargeTimer;
    private bool isCharging;
    private float nextAttackTime; 

    [Header("Referencia del Teclado")]
    public InputActionReference attackActionRef;

    public override bool Atacar()
    {

        if (durabilidadActual <= 0) {
            Debug.LogWarning("Esta arma está rota.");
            return false; 
        }
        
        if (!isCharging && Time.time >= nextAttackTime)
        {
            isCharging = true;
            chargeTimer = 0;
            if (weaponRenderer != null) spriteOriginal = weaponRenderer.sprite;
            Debug.Log("Cargando ataque...");
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

                if (weaponRenderer != null && spritesDeCarga.Length > 0)
                {
                    float chargePercent = chargeTimer / maxChargeTime;
                    
                    int indiceSprite = Mathf.FloorToInt(chargePercent * (spritesDeCarga.Length - 1));
                    weaponRenderer.sprite = spritesDeCarga[indiceSprite];
                }
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
        GastarDurabilidad(1); 

        if (weaponRenderer != null && spriteOriginal != null)
        {
            weaponRenderer.sprite = spriteOriginal;
        }
        
        float chargePercent = Mathf.Clamp01(chargeTimer / maxChargeTime);
        int dañoCarga = Mathf.RoundToInt(damage * Mathf.Lerp(0.5f, 1f, chargePercent));
        int finalDamage = CalcularDañoCritico(dañoCarga);
        float finalSpeed = Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, chargePercent);

        if (projectilePrefab != null && shootPoint != null)
        {
            Vector2 dir = FacingDir();
            Quaternion rotacionFlecha = (dir.x >= 0f) ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);

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