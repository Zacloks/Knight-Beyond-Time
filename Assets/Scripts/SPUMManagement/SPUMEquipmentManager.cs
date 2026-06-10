using UnityEngine;

public class SPUMEquipmentManager : MonoBehaviour
{
    [Header("SPUM Reference")]
    public SPUM_Prefabs spumPrefabs;
    
    [Header("Equipment Part Transforms")]
    // These will be auto-found based on naming conventions
    public Transform helmetTransform;
    public Transform armorTransform;
    public Transform leftItemTransform;
    public Transform rightItemTransform;
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
    private SpriteRenderer leftItemRenderer;
    private SpriteRenderer rightItemRenderer;
    private SpriteRenderer shieldRenderer;
    private SpriteRenderer backRenderer;

    public void EquipItem(Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            UnequipItem();
            return;
        }
        
        if (rightItemRenderer != null)
        {
            rightItemRenderer.sprite = itemSprite;
            rightItemRenderer.enabled = true;
        }
        else if (rightItemTransform != null)
        {
            SpriteRenderer sr = rightItemTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = itemSprite;
                sr.enabled = true;
            }
        }
    }

    public void UnequipItem()
    {
        if (rightItemRenderer != null)
        {
            rightItemRenderer.sprite = null;
            rightItemRenderer.enabled = false;
        }
        else if (rightItemTransform != null)
        {
            SpriteRenderer sr = rightItemTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = null;
                sr.enabled = false;
            }
        }
        
        if (leftItemRenderer != null)
        {
            leftItemRenderer.enabled = false;
        }
        else if (leftItemTransform != null)
        {
            SpriteRenderer sr = leftItemTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }
        }
    }
}
