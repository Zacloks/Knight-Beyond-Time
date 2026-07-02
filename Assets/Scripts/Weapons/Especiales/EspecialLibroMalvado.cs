using UnityEngine;

// Especial del Libro Malvado: dispara un proyectil que, al golpear a un enemigo,
// cura al jugador una parte del daño realizado (robo de vida). Solo cura si
// realmente impacta a un enemigo.
[CreateAssetMenu(fileName = "EspecialLibroMalvado", menuName = "Armas Magicas/Especial - Libro Malvado")]
public class EspecialLibroMalvado : AtaqueEspecialMagico
{
    [Header("Proyectil Maldito")]
    public GameObject prefabProyectil;
    public int daño = 20;
    public float velocidad = 10f;
    public float rango = 12f;

    [Header("Robo de Vida")]
    [Range(0f, 1f)]
    [Tooltip("Porcentaje del daño que se cura el jugador al golpear (0.5 = 50%).")]
    public float porcentajeRoboVida = 0.5f;

    public override void EjecutarAtaque(WeaponMagic arma, Transform shootPoint, PlayerScript jugador)
    {
        if (prefabProyectil == null || shootPoint == null) return;

        float escalaX = arma.transform.lossyScale.x;
        Quaternion rot = shootPoint.rotation;
        if (escalaX > 0) rot *= Quaternion.Euler(0, 0, 180f);

        GameObject go = Instantiate(prefabProyectil, shootPoint.position, rot);
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
        {
            int dañoFinal = arma.CalcularDañoCritico(daño);
            proj.Setup(dañoFinal, velocidad, 0, arma);
            proj.SetRoboVida(porcentajeRoboVida, jugador);
            Destroy(go, rango / velocidad);
        }
    }
}
