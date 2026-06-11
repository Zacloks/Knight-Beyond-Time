using UnityEngine;

[CreateAssetMenu(fileName = "EspecialBaculoInmovilizar", menuName = "Armas Magicas/Especial - Baculo Inmovilizador")]
public class EspecialBaculoInmovilizar : AtaqueEspecialMagico
{
    [Header("Proyectil Inmovilizador")]
    public GameObject prefabProyectil;
    public int daño = 15;
    public float velocidad = 9f;
    public float rango = 12f;

    [Header("Inmovilización")]
    public float duracionInmovilizar = 2f;

    public override void EjecutarAtaque(WeaponMagic arma, Transform shootPoint, PlayerScript jugador)
    {
        if (prefabProyectil == null || shootPoint == null) return;

        // Mismo flip que el ataque básico de distancia.
        float escalaX = arma.transform.lossyScale.x;
        Quaternion rot = shootPoint.rotation;
        if (escalaX > 0) rot *= Quaternion.Euler(0, 0, 180f);

        GameObject go = Instantiate(prefabProyectil, shootPoint.position, rot);
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
        {
            int dañoFinal = arma.CalcularDañoCritico(daño);
            proj.Setup(dañoFinal, velocidad, 0, arma);
            proj.SetInmovilizacion(duracionInmovilizar); // Congela al enemigo al impactar
            Destroy(go, rango / velocidad);
        }
    }
}
