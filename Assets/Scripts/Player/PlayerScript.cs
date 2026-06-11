using UnityEngine;
using System.Collections;
public class PlayerScript : MonoBehaviour
{
    //REFERENCIAS
    public PlayerMovement   Movement;
    public PlayerCombat     Combat;
    public PlayerStats      Stats;
    public PlayerInventory  Inventory;
    public PlayerInteraction Interaction;
    private PlayerAnimator Animator;

    [Header("Estados")]
    public bool isDrinking = false;

    void Awake()
    {
        Movement    = GetComponent<PlayerMovement>();
        Combat      = GetComponent<PlayerCombat>();
        Stats       = GetComponent<PlayerStats>();
        Inventory   = GetComponent<PlayerInventory>();
        Interaction = GetComponent<PlayerInteraction>();
        Animator    = GetComponent<PlayerAnimator>();
    }

    void Start()
    {
        // Suscribirse a eventos de Stats para reacciones cross-módulo
        if (Stats != null)
        {
            Stats.PlayerMuerto += PlayerMatar;
        }
    }

    void Update()
    {
        //SI EL JUGADOR ESTÁ BEBIENDO, NO PUEDE MOVERSE.
        if (isDrinking && Movement != null)
            Movement.CongelarMovimiento();
    }

//------FUNCIONES PARA ITEMS/POCIONES-----
    public void TakeDamage(int amount) {
        Stats.TakeDamage(amount);
        Animator.TriggerDebuff();
    }
    public void TakeDamage(int amount, Vector2 sourcePosition) {
        Stats.TakeDamage(amount, sourcePosition);
        Animator.TriggerDebuff();
    }
    public void buy(int price) {Stats.TrySpendCoins(price);}

//ACTIVAR ANIMACIONES
    public void IniciarAnimacion(string nombre) {Combat.IniciarAnimacion(nombre);}
    public void IniciarAnimacionMitad(string nombre) {Combat.IniciarAnimacionMitad(nombre);}
//ACTIVAR STATS
    public void ActivarDashInfinito(float duracion) {Stats.ActivarDashInfinito(duracion);}
    public void ActivarVelocidad(float aumento, float dur){Stats.ActivarVelocidad(aumento, dur);}
//SETTERS 
    public void SetDashInfinito(bool valor)
    {
        if (Movement != null) Movement.dashInfinito = valor;
    }
    public void SetExtraVelocidad(float valor)
    {
        if (Movement != null) Movement.extraVelocidad = valor;
    }

    public Animator getPlayerAnimator(){
        return Animator.anim;
    }

//MUERTE DEL PERSONAJE ='(
    void OnDestroy()
    {
        if (Stats != null)
            Stats.PlayerMuerto -= PlayerMatar;
    }

    private void PlayerMatar()
    {
        Debug.Log("El jugador ha muerto.");
        Movement.enabled = false;
        Combat.enabled = false;
        Animator.TriggerDeath(); 
        StartCoroutine(MostrarGameOverConDelay());
    }

    private IEnumerator MostrarGameOverConDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f); // RealTime ignora timeScale
        GameOverScript.Instance.MostrarGameOver();
        Destroy(gameObject);
    }
}
