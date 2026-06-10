using UnityEngine;

public class SPUMPlayerBridge : MonoBehaviour
{

    [Header("SPUM Components")]
    public SPUM_Prefabs spumPrefabs;
    public Animator spumAnimator;

    [Header("Animation State Indices")]
    public int idleAnimationIndex = 0;
    public int moveAnimationIndex = 0;
    public int attackAnimationIndex = 0;
    public int damagedAnimationIndex = 0;
    public int debuffAnimationIndex = 0;
    public int deathAnimationIndex = 0;
    
    [Header("Skill Animation Indices")]
    public int[] skillAnimationIndices = new int[4] { 1, 1, 1, 1 }; // Default to skill animation

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
