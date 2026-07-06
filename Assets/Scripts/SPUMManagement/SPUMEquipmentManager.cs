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

    public SpriteRenderer GetRightItemRenderer()
    {
        if (rightItemRenderer != null) return rightItemRenderer;
        if (rightItemTransform != null) return rightItemTransform.GetComponent<SpriteRenderer>();
        return null;
    }
    //enderezar agarre para armas a distancia 
    private Quaternion rightItemBaseRotation;
    private bool rightItemBaseRotationSaved;

    public void SetRightItemRotation(float zAngle)
    {
        SpriteRenderer r = GetRightItemRenderer();
        if (r == null) return;
        if (!rightItemBaseRotationSaved)
        {
            rightItemBaseRotation = r.transform.localRotation;
            rightItemBaseRotationSaved = true;
        }
        r.transform.localRotation = Quaternion.Euler(0f, 0f, zAngle);
    }

    public void ResetRightItemRotation()
    {
        SpriteRenderer r = GetRightItemRenderer();
        if (r == null) return;
        if (rightItemBaseRotationSaved)
        {
            r.transform.localRotation = rightItemBaseRotation;
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
