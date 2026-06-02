using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Weapon equippedWeapon; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Z)) 
        {
            Attack();
        }
    }

    void Attack()
    {
        if (equippedWeapon == null) 
        {
            Debug.LogWarning("¡No tienes un arma equipada!");
            return;
        }

        equippedWeapon.Atacar();
        
        Debug.Log("Atacando con " + equippedWeapon.weaponName);
    }
}