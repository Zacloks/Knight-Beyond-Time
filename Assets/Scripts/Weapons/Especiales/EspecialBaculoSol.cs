using UnityEngine;

// Especial del Báculo del Sol: dispara un proyectil de luz que, al golpear a un
// enemigo, genera un destello en esa posición y daña a los enemigos cercanos
// dentro de un radio.
[CreateAssetMenu(fileName = "EspecialBaculoSol", menuName = "Armas Magicas/Especial - Baculo del Sol")]
public class EspecialBaculoSol : AtaqueEspecialMagico
{
    [Header("Proyectil de Luz")]
    public GameObject prefabProyectil;
    public int daño = 15;
    public float velocidad = 10f;
    public float rango = 12f;

    [Header("Destello al Impactar")]
    [Tooltip("Radio del destello: los enemigos dentro reciben el daño de área.")]
    public float radioDestello = 2.5f;
    [Tooltip("Daño a cada enemigo cercano alcanzado por el destello.")]
    public int dañoDestello = 12;
    [Tooltip("Prefab opcional del efecto visual. Si se deja vacío, se genera un destello por código.")]
    public GameObject prefabDestello;
    [Tooltip("Color del destello generado por código (si no se asigna un prefab).")]
    public Color colorDestello = new Color(1f, 0.95f, 0.5f, 1f);

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
            proj.SetExplosion(radioDestello, dañoDestello, prefabDestello, colorDestello);
            Destroy(go, rango / velocidad);
        }
    }
}
