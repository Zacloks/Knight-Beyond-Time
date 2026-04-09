using UnityEngine;

public class WeaponDistance : Weapon
{
    [Header("Configuración de Distancia")]
    public GameObject projectilePrefab;
    public Transform shootPoint; 

    public override void Atacar()
    {
        Disparar();
    }

    private void Disparar()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            
            Projectile script = newProjectile.GetComponent<Projectile>();
            if (script != null)
            {
                script.Setup(damage);
            }
        }
        else
        {
            Debug.LogWarning("Falta el Prefab del proyectil o el ShootPoint en " + gameObject.name);
        }
    }
}