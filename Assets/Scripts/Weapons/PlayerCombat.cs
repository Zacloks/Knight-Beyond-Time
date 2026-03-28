using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Weapon equippedWeapon; 
    public Transform shootPoint;  

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Z)) 
        {
            Attack();
        }
    }

    void Attack()
    {
        if (equippedWeapon == null) return;

        if (equippedWeapon.isMelee)
        {
            equippedWeapon.Swing();
            Debug.Log("Atacando con " + equippedWeapon.weaponName);
            Invoke("DeactivateWeapon", 0.2f); 
        }
        else
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (equippedWeapon.projectilePrefab != null && shootPoint != null)
        {
            Instantiate(equippedWeapon.projectilePrefab, shootPoint.position, shootPoint.rotation);
            Debug.Log("¡Flecha lanzada!");
        }
    }
}