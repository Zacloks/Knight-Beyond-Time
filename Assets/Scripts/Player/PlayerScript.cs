using UnityEngine;
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
        // Animación de aturdido desactivada temporalmente para pruebas.
        // Animator.TriggerDebuff();
    }
    public void TakeDamage(int amount, Vector2 sourcePosition) {
        Stats.TakeDamage(amount, sourcePosition);
        // Animación de aturdido desactivada temporalmente para pruebas.
        // Animator.TriggerDebuff();
    }
    public void buy(int price) {Stats.TrySpendCoins(price);}
    public void IniciarAnimacion(string nombre) {Combat.IniciarAnimacion(nombre);}
    public void IniciarAnimacionMitad(string nombre) {Combat.IniciarAnimacionMitad(nombre);}
    public void ActivarDashInfinito(float duracion) {Stats.ActivarDashInfinito(duracion);}
    public void ActivarVelocidad(float aumento, float dur){Stats.ActivarVelocidad(aumento, dur);}

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
}
