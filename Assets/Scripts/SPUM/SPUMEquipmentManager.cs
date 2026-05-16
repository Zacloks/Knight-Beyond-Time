using UnityEngine;

public class SPUMEquipmentManager : MonoBehaviour
{
    [Header("SPUM Reference")]
    public SPUM_Prefabs spumPrefabs;
    
    [Header("Equipment Part Transforms")]
    // These will be auto-found based on naming conventions
    public Transform helmetTransform;
    public Transform armorTransform;
    public Transform leftWeaponTransform;
    public Transform rightWeaponTransform;
    public Transform shieldTransform;
    public Transform backTransform;
    
    [Header("Default Equipment")]
    public Sprite defaultHelmet;
    public Sprite defaultArmor;
    public Sprite defaultWeapon;
    
   // private Dictionary<EquipSlot, Sprite> currentEquipment = new Dictionary<EquipSlot, Sprite>();
   // private Dictionary<string, SpriteRenderer> partRenderers = new Dictionary<string, SpriteRenderer>();
    

    private SpriteRenderer helmetRenderer;
    private SpriteRenderer armorRenderer;
    private SpriteRenderer leftWeaponRenderer;
    private SpriteRenderer rightWeaponRenderer;
    private SpriteRenderer shieldRenderer;
    private SpriteRenderer backRenderer;

    public void EquipWeapon(Sprite weaponSprite)
    {
        if (weaponSprite == null)
        {
            UnequipWeapon();
            return;
        }
        
        if (rightWeaponRenderer != null)
        {
            rightWeaponRenderer.sprite = weaponSprite;
            rightWeaponRenderer.enabled = true;
        }
        else if (rightWeaponTransform != null)
        {
            SpriteRenderer sr = rightWeaponTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = weaponSprite;
                sr.enabled = true;
            }
        }
        
        Debug.Log($"Equipped weapon: {weaponSprite.name}");
    }

    public void UnequipWeapon()
    {
        if (rightWeaponRenderer != null)
        {
            rightWeaponRenderer.sprite = null;
            rightWeaponRenderer.enabled = false;
        }
        else if (rightWeaponTransform != null)
        {
            SpriteRenderer sr = rightWeaponTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = null;
                sr.enabled = false;
            }
        }
        
        if (leftWeaponRenderer != null)
        {
            leftWeaponRenderer.enabled = false;
        }
        else if (leftWeaponTransform != null)
        {
            SpriteRenderer sr = leftWeaponTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }
        }
    }
}
