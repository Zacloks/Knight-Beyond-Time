using UnityEngine;

[CreateAssetMenu(fileName = "EspecialLibroFuego", menuName = "Armas Magicas/Especial - Libro de Fuego")]
public class EspecialLibroFuego : AtaqueEspecialMagico
{
    [Header("Bola de Fuego")]
    public GameObject prefabBolaFuego;
    public int daño = 25;
    public float velocidad = 10f;
    public float rango = 12f;

    [Header("Quemadura (daño en el tiempo)")]
    [Tooltip("Daño de cada tick de quemadura tras el impacto.")]
    public int dañoPorTick = 3;
    [Tooltip("Segundos entre tick y tick.")]
    public float intervaloTick = 0.8f;
    [Tooltip("Duración total de la quemadura (segundos). 0 = sin quemadura.")]
    public float duracionQuemadura = 4f;
    [Tooltip("Color de las partículas de fuego.")]
    public Color colorFuego = new Color(1f, 0.55f, 0.1f, 1f);

    public override void EjecutarAtaque(WeaponMagic arma, Transform shootPoint, PlayerScript jugador)
    {
        if (prefabBolaFuego == null || shootPoint == null) return;

        // Mismo flip que el ataque básico de distancia.
        float escalaX = arma.transform.lossyScale.x;
        Quaternion rot = shootPoint.rotation;
        if (escalaX > 0) rot *= Quaternion.Euler(0, 0, 180f);

        GameObject bola = Instantiate(prefabBolaFuego, shootPoint.position, rot);
        Projectile proj = bola.GetComponent<Projectile>();
        if (proj != null)
        {
            int dañoFinal = arma.CalcularDañoCritico(daño);
            proj.Setup(dañoFinal, velocidad, 0, arma);
            if (duracionQuemadura > 0f)
                proj.SetQuemadura(dañoPorTick, intervaloTick, duracionQuemadura, colorFuego);
            Destroy(bola, rango / velocidad);
        }
    }
}
